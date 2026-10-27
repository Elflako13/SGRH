using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Reservation
{
    public interface IHuespedRepository : IBaseRepository<Huesped>
    {
        Task<Huesped?> GetByDocumentoAsync(string documento);
    }
}
