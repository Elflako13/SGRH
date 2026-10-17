using SGRH.Domain.Entities.Notification;

namespace SGRH.Domain.Repository.Notification
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        Task<List<Notificacion>> GetByReservaAsync(int reservaId);

        /// <summary>CU-016: notificaciones fallidas que todavía se pueden reintentar.</summary>
        Task<List<Notificacion>> GetPendientesDeReintentoAsync();
    }
}
