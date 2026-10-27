using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    /// <summary>
    /// Las notificaciones no se editan ni se borran: se crean y luego se marcan
    /// como enviadas o fallidas (RN-014, RN-015).
    /// </summary>
    public interface INotificacionRepository
    {
        Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId);
        Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync();
        Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion);
        Task<OperationResult<bool>> MarcarEnviadaAsync(int notificacionId);
        Task<OperationResult<bool>> MarcarFallidaAsync(int notificacionId, string error);
    }
}
