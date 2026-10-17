using SGRH.Domain.Entities.Configuration;

namespace SGRH.Domain.Repository.Configuration
{
    public interface IHotelRepository : IBaseRepository<Hotel>
    {
        /// <summary>Trae el hotel con sus pisos y habitaciones (CU-006).</summary>
        Task<Hotel?> GetConEstructuraAsync(int hotelId);
    }
}
