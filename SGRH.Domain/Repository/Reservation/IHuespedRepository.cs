using SGRH.Domain.Entities.Reservation;

namespace SGRH.Domain.Repository.Reservation
{
    public interface IHuespedRepository : IBaseRepository<Huesped>
    {
        /// <summary>RN-017: el documento es único.</summary>
        Task<Huesped?> GetByDocumentoAsync(string documento);
    }
}
