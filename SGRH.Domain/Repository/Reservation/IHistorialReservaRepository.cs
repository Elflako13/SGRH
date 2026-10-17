using SGRH.Domain.Entities.Reservation;

namespace SGRH.Domain.Repository.Reservation
{
    public interface IHistorialReservaRepository : IBaseRepository<HistorialReserva>
    {
        Task<List<HistorialReserva>> GetByReservaAsync(int reservaId);
    }
}
