using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.HistorialReserva;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    /// <summary>
    /// Casos de uso de la reserva (CU-010 a CU-013). Además de guardar a través del repositorio:
    /// - aplica los límites de negocio de ReservaOptions,
    /// - deja constancia de cada cambio en el historial (RF-021),
    /// - genera la notificación al huésped al confirmar, modificar o cancelar (RN-014),
    ///   sin que un fallo de la notificación afecte a la reserva (RN-015).
    /// </summary>
    public class ReservaService : ServiceBase, IReservaService
    {
        private readonly IReservaRepository reservaRepository;
        private readonly IHistorialReservaRepository historialRepository;
        private readonly INotificacionRepository notificacionRepository;
        private readonly ReservaOptions reservaOptions;
        private readonly NotificacionOptions notificacionOptions;

        public ReservaService(IReservaRepository reservaRepository,
                              IHistorialReservaRepository historialRepository,
                              INotificacionRepository notificacionRepository,
                              IOptions<ReservaOptions> reservaOptions,
                              IOptions<NotificacionOptions> notificacionOptions,
                              ILogger<ReservaService> logger) : base(logger)
        {
            this.reservaRepository = reservaRepository ?? throw new ArgumentNullException(nameof(reservaRepository));
            this.historialRepository = historialRepository ?? throw new ArgumentNullException(nameof(historialRepository));
            this.notificacionRepository = notificacionRepository ?? throw new ArgumentNullException(nameof(notificacionRepository));
            this.reservaOptions = reservaOptions?.Value ?? throw new ArgumentNullException(nameof(reservaOptions));
            this.notificacionOptions = notificacionOptions?.Value ?? throw new ArgumentNullException(nameof(notificacionOptions));
        }

        public Task<OperationResult<List<ReservaModel>>> GetAllAsync() =>
            EjecutarAsync("consultar reservas", () => reservaRepository.GetAllAsync());

        public Task<OperationResult<ReservaModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar reserva", () => ValidarId(id, "reserva"), () => reservaRepository.GetByIdAsync(id));

        public Task<OperationResult<List<ReservaModel>>> GetByHuespedAsync(int huespedId) =>
            EjecutarAsync("consultar reservas del huésped", () => ValidarId(huespedId, "huésped"), () => reservaRepository.GetByHuespedAsync(huespedId));

        public Task<OperationResult<List<HistorialReservaModel>>> GetHistorialAsync(int reservaId) =>
            EjecutarAsync("consultar historial de reserva", () => ValidarId(reservaId, "reserva"), () => historialRepository.GetByReservaAsync(reservaId));

        /// <summary>CU-010: crear reserva (queda Pendiente).</summary>
        public Task<OperationResult<ReservaModel>> AddAsync(ReservaAdd reserva) =>
            EjecutarAsync("crear reserva",
                          () => Requerido(reserva, nameof(ReservaAdd)) ?? ValidarLimites(reserva.FechaEntrada, reserva.FechaSalida),
                          async () =>
                          {
                              var resultado = await reservaRepository.AddAsync(reserva);

                              if (resultado.Success)
                                  await RegistrarHistorialAsync(resultado.Data!.ReservaId, reserva.UsuarioCreacion, "Creación", $"Reserva {resultado.Data.Codigo} creada.");

                              return resultado;
                          }, reserva?.UsuarioCreacion);

        /// <summary>CU-012: modificar reserva.</summary>
        public Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva) =>
            EjecutarAsync("modificar reserva",
                          () => Requerido(reserva, nameof(ReservaUpdate)) ?? PrimerError(ValidarId(reserva.Id, "reserva"), ValidarLimites(reserva.FechaEntrada, reserva.FechaSalida)),
                          async () =>
                          {
                              var resultado = await reservaRepository.UpdateAsync(reserva);

                              if (resultado.Success)
                              {
                                  await RegistrarHistorialAsync(reserva.Id, reserva.UsuarioModificacion, "Modificación",
                                                                $"Habitación {reserva.HabitacionId}, del {reserva.FechaEntrada:dd/MM/yyyy} al {reserva.FechaSalida:dd/MM/yyyy}.");
                                  await NotificarAsync(reserva.Id, TipoNotificacion.Modificacion, reserva.UsuarioModificacion);
                              }

                              return resultado;
                          }, reserva?.UsuarioModificacion);

        /// <summary>CU-011: confirmar reserva.</summary>
        public Task<OperationResult<bool>> ConfirmarAsync(int reservaId, int? usuarioModificacion) =>
            EjecutarAsync("confirmar reserva", () => ValidarId(reservaId, "reserva"), async () =>
            {
                var resultado = await reservaRepository.ConfirmarAsync(reservaId, usuarioModificacion);

                if (resultado.Success)
                {
                    await RegistrarHistorialAsync(reservaId, usuarioModificacion, "Confirmación", null);
                    await NotificarAsync(reservaId, TipoNotificacion.Confirmacion, usuarioModificacion);
                }

                return resultado;
            }, usuarioModificacion);

        /// <summary>CU-013: cancelar reserva.</summary>
        public Task<OperationResult<bool>> RemoveAsync(ReservaRemove reserva) =>
            EjecutarAsync("cancelar reserva",
                          () => Requerido(reserva, nameof(ReservaRemove)) ?? ValidarId(reserva.Id, "reserva"),
                          async () =>
                          {
                              var resultado = await reservaRepository.RemoveAsync(reserva);

                              if (resultado.Success)
                              {
                                  await RegistrarHistorialAsync(reserva.Id, reserva.UsuarioModificacion, "Cancelación", reserva.Motivo);
                                  await NotificarAsync(reserva.Id, TipoNotificacion.Cancelacion, reserva.UsuarioModificacion);
                              }

                              return resultado;
                          }, reserva?.UsuarioModificacion);

        /// <summary>Límites de negocio configurables (ReservaOptions).</summary>
        private string? ValidarLimites(DateOnly entrada, DateOnly salida)
        {
            int noches = salida.DayNumber - entrada.DayNumber;
            if (noches > reservaOptions.MaximoNoches)
                return $"Una reserva no puede pasar de {reservaOptions.MaximoNoches} noches.";

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (entrada > hoy.AddDays(reservaOptions.DiasMaximoAnticipacion))
                return $"Solo se puede reservar con un máximo de {reservaOptions.DiasMaximoAnticipacion} días de anticipación.";

            return null;
        }

        private async Task RegistrarHistorialAsync(int reservaId, int? usuarioId, string accion, string? detalle)
        {
            if (usuarioId is null)
                return;

            var resultado = await historialRepository.AddAsync(new HistorialReservaAdd
            {
                ReservaId = reservaId,
                UsuarioId = usuarioId.Value,
                Accion = accion,
                Detalle = detalle
            });

            if (!resultado.Success)
                logger.LogWarning("No se pudo registrar el historial ({Accion}) de la reserva {ReservaId}: {Mensaje}", accion, reservaId, resultado.Message);
        }

        /// <summary>
        /// RN-014: genera la notificación (queda Pendiente de envío).
        /// RN-015: si algo falla aquí, la reserva ya está guardada y no se revierte.
        /// </summary>
        private async Task NotificarAsync(int reservaId, TipoNotificacion tipo, int? usuarioId)
        {
            try
            {
                var reserva = await reservaRepository.GetByIdAsync(reservaId);
                if (!reserva.Success)
                    return;

                var resultado = await notificacionRepository.AddAsync(new NotificacionAdd
                {
                    ReservaId = reservaId,
                    Tipo = tipo,
                    Canal = notificacionOptions.CanalPorDefecto,
                    Mensaje = CrearMensaje(reserva.Data!, tipo),
                    UsuarioCreacion = usuarioId
                });

                if (resultado.Success)
                    logger.LogInformation("Notificación de {Tipo} generada para la reserva {ReservaId}.", tipo, reservaId);
                else
                    logger.LogWarning("No se generó la notificación de {Tipo} de la reserva {ReservaId}: {Mensaje}", tipo, reservaId, resultado.Message);
            }
            catch (Exception ex)
            {
                // RN-015: el fallo de la notificación no afecta a la reserva, pero queda registrado.
                logger.LogError(ex, "Error al generar la notificación de {Tipo} de la reserva {ReservaId}. Tipo: {ExceptionType}.", tipo, reservaId, ex.GetType().Name);
            }
        }

        private string CrearMensaje(ReservaModel reserva, TipoNotificacion tipo)
        {
            string accion = tipo switch
            {
                TipoNotificacion.Confirmacion => "ha sido confirmada",
                TipoNotificacion.Modificacion => "ha sido modificada",
                TipoNotificacion.Cancelacion => "ha sido cancelada",
                _ => "ha cambiado"
            };

            return $"Hola {reserva.Huesped}, su reserva {reserva.Codigo} (habitación {reserva.Habitacion}, " +
                   $"del {reserva.FechaEntrada:dd/MM/yyyy} al {reserva.FechaSalida:dd/MM/yyyy}, total RD${reserva.PrecioTotal:N2}) {accion}. " +
                   $"- {notificacionOptions.Remitente}";
        }
    }
}
