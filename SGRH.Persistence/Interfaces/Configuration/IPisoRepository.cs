using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Repository;

namespace SGRH.Persistence.Interfaces.Configuration
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        Task<List<Piso>> GetByHotelAsync(int hotelId);

        /// <summary>RN-003.</summary>
        Task<bool> ExistePisoActivoAsync(int hotelId, int numero, int? excluirPisoId = null);
    }
}
