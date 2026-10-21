using SGRH.Domain.Entities.Notification;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Notification
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        Task<List<Notificacion>> GetByReservaAsync(int reservaId);

        /// <summary>CU-016: notificaciones fallidas que todavía se pueden reintentar.</summary>
        Task<List<Notificacion>> GetPendientesDeReintentoAsync();
    }
}
