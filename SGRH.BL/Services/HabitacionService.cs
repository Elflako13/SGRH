using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    /// <summary>Gestión de habitaciones (CU-003 a CU-005) y consulta de disponibilidad (CU-009).</summary>
    public class HabitacionService : ServiceBase, IHabitacionService
    {
        private readonly IHabitacionRepository habitacionRepository;
        private readonly ReservaOptions reservaOptions;

        public HabitacionService(IHabitacionRepository habitacionRepository, IOptions<ReservaOptions> reservaOptions, ILogger<HabitacionService> logger)
            : base(logger)
        {
            this.habitacionRepository = habitacionRepository ?? throw new ArgumentNullException(nameof(habitacionRepository));
            this.reservaOptions = reservaOptions?.Value ?? throw new ArgumentNullException(nameof(reservaOptions));
        }

        public Task<OperationResult<List<HabitacionModel>>> GetAllAsync() =>
            EjecutarAsync("consultar habitaciones", () => habitacionRepository.GetAllAsync());

        public Task<OperationResult<HabitacionModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar habitación", () => ValidarId(id, "habitación"), () => habitacionRepository.GetByIdAsync(id));

        public Task<OperationResult<List<HabitacionModel>>> GetByHotelAsync(int hotelId) =>
            EjecutarAsync("consultar habitaciones del hotel", () => ValidarId(hotelId, "hotel"), () => habitacionRepository.GetByHotelAsync(hotelId));

        public Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion) =>
            EjecutarAsync("registrar habitación",
                          () => Requerido(habitacion, nameof(HabitacionAdd)) ?? PrimerError(
                                    string.IsNullOrWhiteSpace(habitacion.Numero) ? "El número de habitación es obligatorio." : null,
                                    ValidarId(habitacion.PisoId, "piso"),
                                    ValidarHabitacion(habitacion)),
                          () => habitacionRepository.AddAsync(habitacion), habitacion?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion) =>
            EjecutarAsync("actualizar habitación",
                          () => Requerido(habitacion, nameof(HabitacionUpdate)) ?? PrimerError(ValidarId(habitacion.Id, "habitación"), ValidarHabitacion(habitacion)),
                          () => habitacionRepository.UpdateAsync(habitacion), habitacion?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(HabitacionRemove habitacion) =>
            EjecutarAsync("inactivar habitación",
                          () => Requerido(habitacion, nameof(HabitacionRemove)) ?? ValidarId(habitacion.Id, "habitación"),
                          () => habitacionRepository.RemoveAsync(habitacion), habitacion?.UsuarioModificacion);

        /// <summary>CU-005: cambia el estado operativo (RF-007, RN-007, RN-018).</summary>
        public Task<OperationResult<bool>> CambiarEstadoAsync(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion) =>
            EjecutarAsync("cambiar estado de habitación",
                          () => ValidarId(habitacionId, "habitación") ?? (Enum.IsDefined(estado) ? null : "El estado indicado no es válido."),
                          () => habitacionRepository.CambiarEstadoAsync(habitacionId, estado, usuarioModificacion), usuarioModificacion);

        /// <summary>
        /// CU-009: consulta de disponibilidad. Antes de ir a la base de datos aplica las reglas
        /// de la solicitud (RN-009, RN-011 y los límites de ReservaOptions) y devuelve las
        /// habitaciones ordenadas de la más económica a la más cara.
        /// </summary>
        public Task<OperationResult<List<HabitacionDisponibleModel>>> GetDisponiblesAsync(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes) =>
            EjecutarAsync("consultar disponibilidad", () => ValidarSolicitud(entrada, salida, cantidadHuespedes), async () =>
            {
                var resultado = await habitacionRepository.GetDisponiblesAsync(hotelId, entrada, salida, cantidadHuespedes);
                if (!resultado.Success)
                    return resultado;

                var ordenadas = resultado.Data!.OrderBy(h => h.CostoEstimado).ThenBy(h => h.Numero).ToList();

                return ordenadas.Count == 0
                    ? Success(ordenadas, "No hay habitaciones disponibles para esas fechas. Pruebe con otras fechas o con menos huéspedes.")
                    : Success(ordenadas, $"Se encontraron {ordenadas.Count} habitación(es) disponible(s).");
            });

        private static string? ValidarHabitacion(HabitacionBase habitacion)
        {
            if (habitacion.TipoHabitacionId <= 0)
                return "Debe seleccionar el tipo de habitación.";

            if (habitacion.Capacidad <= 0)
                return "La capacidad debe ser mayor que cero.";

            if (habitacion.TarifaPorNoche <= 0)
                return "La tarifa por noche debe ser mayor que cero.";

            return null;
        }

        private string? ValidarSolicitud(DateOnly entrada, DateOnly salida, int cantidadHuespedes)
        {
            if (cantidadHuespedes < 1)
                return "La cantidad de huéspedes debe ser al menos 1 (RN-011).";

            if (salida <= entrada)
                return "La fecha de salida debe ser posterior a la fecha de entrada (RN-008).";

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (entrada < hoy)
                return "No se puede consultar disponibilidad para fechas pasadas (RN-009).";

            int noches = salida.DayNumber - entrada.DayNumber;
            if (noches > reservaOptions.MaximoNoches)
                return $"Una reserva no puede pasar de {reservaOptions.MaximoNoches} noches.";

            if (entrada > hoy.AddDays(reservaOptions.DiasMaximoAnticipacion))
                return $"Solo se puede reservar con un máximo de {reservaOptions.DiasMaximoAnticipacion} días de anticipación.";

            return null;
        }
    }
}
