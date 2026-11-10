using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    public interface IHotelRepository
    {
        Task<OperationResult<List<HotelModel>>> GetAllAsync();
        Task<OperationResult<HotelModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HotelAdd hotel);
        Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel);
        Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel);
        Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion);
    }
}
