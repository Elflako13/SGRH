using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Configuration
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        Task<Hotel?> GetConEstructuraAsync(int hotelId);
    }
}
