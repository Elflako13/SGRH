using SGRH.Domain.Entities.Configuration;

namespace SGRH.Domain.Repository.Configuration
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        Task<List<Piso>> GetByHotelAsync(int hotelId);

        /// <summary>RN-003.</summary>
        Task<bool> ExistePisoActivoAsync(int hotelId, int numero);
    }
}
