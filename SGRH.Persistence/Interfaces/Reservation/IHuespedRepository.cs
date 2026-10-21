using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Reservation
{
    public interface IHuespedRepository : IBaseRepository<Huesped>
    {
        /// <summary>RN-017: el documento es único.</summary>
        Task<Huesped?> GetByDocumentoAsync(string documento);
    }
}
