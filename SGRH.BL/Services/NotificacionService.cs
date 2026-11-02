using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    /// <summary>
    /// Notificaciones al huésped (RN-014, RN-015, CU-015, CU-016).
    /// El envío real (correo, SMS) se conectará en la capa de presentación o infraestructura;
    /// aquí está la lógica: canal por defecto, registro de resultados y reintentos.
    /// </summary>
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository notificacionRepository;
        private readonly NotificacionOptions notificacionOptions;

        public NotificacionService(INotificacionRepository notificacionRepository, IOptions<NotificacionOptions> notificacionOptions)
        {
            this.notificacionRepository = notificacionRepository;
            this.notificacionOptions = notificacionOptions.Value;
        }

        public Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId) => notificacionRepository.GetByReservaAsync(reservaId);

        public Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync() => notificacionRepository.GetPendientesDeReintentoAsync();

        public Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion)
        {
            if (notificacion is null)
                return Task.FromResult(Failure<NotificacionModel>("El objeto NotificacionAdd es requerido."));

            if (string.IsNullOrWhiteSpace(notificacion.Mensaje))
                return Task.FromResult(Failure<NotificacionModel>("La notificación debe tener un mensaje."));

            // Si no se indica un canal válido, se usa el configurado en NotificacionOptions.
            if (!Enum.IsDefined(notificacion.Canal))
                notificacion.Canal = notificacionOptions.CanalPorDefecto;

            return notificacionRepository.AddAsync(notificacion);
        }

        public Task<OperationResult<bool>> RegistrarResultadoEnvioAsync(int notificacionId, bool enviada, string? error)
        {
            if (enviada)
                return notificacionRepository.MarcarEnviadaAsync(notificacionId);

            return notificacionRepository.MarcarFallidaAsync(notificacionId,
                string.IsNullOrWhiteSpace(error) ? "Error no especificado." : error);
        }

        /// <summary>
        /// CU-016: recorre las fallidas que aún tienen intentos disponibles y vuelve a enviarlas.
        /// Devuelve cuántas se enviaron con éxito. Un error en una no detiene a las demás (RN-015).
        /// </summary>
        public async Task<OperationResult<int>> ReintentarFallidasAsync(Func<NotificacionModel, Task<bool>> enviar)
        {
            if (enviar is null)
                return Failure<int>("Debe indicar cómo se envían las notificaciones.");

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
