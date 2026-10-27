using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface IHotelService
    {
        Task<OperationResult<List<HotelModel>>> GetAllAsync();
        Task<OperationResult<HotelModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HotelAdd hotel);
        Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel);
        Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel);
    }
}
