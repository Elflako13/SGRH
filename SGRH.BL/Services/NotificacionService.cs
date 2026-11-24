using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    public class NotificacionService : ServiceBase, INotificacionService
    {
        private readonly INotificacionRepository notificacionRepository;
        private readonly NotificacionOptions notificacionOptions;

        public NotificacionService(INotificacionRepository notificacionRepository, IOptions<NotificacionOptions> notificacionOptions,
                                   ILogger<NotificacionService> logger) : base(logger)
        {
            this.notificacionRepository = notificacionRepository ?? throw new ArgumentNullException(nameof(notificacionRepository));
            this.notificacionOptions = notificacionOptions?.Value ?? throw new ArgumentNullException(nameof(notificacionOptions));
        }

        public Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId) =>
            EjecutarAsync("consultar notificaciones de la reserva", () => ValidarId(reservaId, "reserva"), () => notificacionRepository.GetByReservaAsync(reservaId));

        public Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync() =>
            EjecutarAsync("consultar notificaciones pendientes", () => notificacionRepository.GetPendientesDeReintentoAsync());

        public Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion) =>
            EjecutarAsync("registrar notificación",
                          () => Requerido(notificacion, nameof(NotificacionAdd))
                                ?? (string.IsNullOrWhiteSpace(notificacion.Mensaje) ? "La notificación debe tener un mensaje." : null),
                          () =>
                          {
                              if (!Enum.IsDefined(notificacion.Canal))
                                  notificacion.Canal = notificacionOptions.CanalPorDefecto;

                              return notificacionRepository.AddAsync(notificacion);
                          }, notificacion?.UsuarioCreacion);

        public Task<OperationResult<bool>> RegistrarResultadoEnvioAsync(int notificacionId, bool enviada, string? error) =>
            EjecutarAsync("registrar resultado de envío", () => ValidarId(notificacionId, "notificación"), () =>
            {
                if (enviada)
                    return notificacionRepository.MarcarEnviadaAsync(notificacionId);

                logger.LogWarning("La notificación {NotificacionId} no se pudo enviar: {Error}", notificacionId, error);
                return notificacionRepository.MarcarFallidaAsync(notificacionId,
                    string.IsNullOrWhiteSpace(error) ? "Error no especificado." : error);
            });

        public Task<OperationResult<int>> ReintentarFallidasAsync(Func<NotificacionModel, Task<bool>> enviar) =>
            EjecutarAsync("reintentar notificaciones fallidas",
                          () => enviar is null ? "Debe indicar cómo se envían las notificaciones." : null,
                          () => ReintentarAsync(enviar));

        private async Task<OperationResult<int>> ReintentarAsync(Func<NotificacionModel, Task<bool>> enviar)
        {
            var pendientes = await notificacionRepository.GetPendientesDeReintentoAsync();
            if (!pendientes.Success)
                return Failure<int>(pendientes.Message);

            int enviadas = 0;
            foreach (var notificacion in pendientes.Data!)
            {
                bool exito;
                string? error = null;
                try
                {
                    exito = await enviar(notificacion);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al reenviar la notificación {NotificacionId}. Tipo: {ExceptionType}.", notificacion.NotificacionId, ex.GetType().Name);
                    exito = false;
                    error = ex.Message;
                }

                await RegistrarResultadoEnvioAsync(notificacion.NotificacionId, exito, error ?? "El servicio no confirmó el envío.");
                if (exito)
                    enviadas++;
            }

            return Success(enviadas, $"Se reintentaron {pendientes.Data!.Count} notificación(es); {enviadas} enviada(s).");
        }
    }
}
