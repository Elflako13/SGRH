using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface INotificacionService
    {
        Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId);
        Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync();
        Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion);
        Task<OperationResult<bool>> MarcarEnviadaAsync(int notificacionId);
        Task<OperationResult<bool>> MarcarFallidaAsync(int notificacionId, string error);
    }
}
