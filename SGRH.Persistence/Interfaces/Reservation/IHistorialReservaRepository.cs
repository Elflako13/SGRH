using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Reservation
{
    public interface IHistorialReservaRepository : IBaseRepository<HistorialReserva>
    {
        Task<List<HistorialReserva>> GetByReservaAsync(int reservaId);
    }
}
