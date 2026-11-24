using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface INotificacionService
    {
        Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId);
        Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync();
        Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion);

        Task<OperationResult<bool>> RegistrarResultadoEnvioAsync(int notificacionId, bool enviada, string? error);

        Task<OperationResult<int>> ReintentarFallidasAsync(Func<NotificacionModel, Task<bool>> enviar);
    }
}
