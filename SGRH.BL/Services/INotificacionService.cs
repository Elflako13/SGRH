using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface INotificacionService
    {
        Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId);
        Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync();
        Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion);

        /// <summary>Registra el resultado de un intento de envío (enviada o fallida).</summary>
        Task<OperationResult<bool>> RegistrarResultadoEnvioAsync(int notificacionId, bool enviada, string? error);

        /// <summary>CU-016: vuelve a intentar las notificaciones fallidas con el método de envío indicado.</summary>
        Task<OperationResult<int>> ReintentarFallidasAsync(Func<NotificacionModel, Task<bool>> enviar);
    }
}
