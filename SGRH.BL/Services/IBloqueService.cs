using SGRH.DAL.Models.Objects.Bloque;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface IBloqueService
    {
        Task<OperationResult<List<BloqueModel>>> GetAllAsync();
        Task<OperationResult<BloqueModel>> GetByIdAsync(int id);
        Task<OperationResult<List<BloqueModel>>> GetByHotelAsync(int hotelId);
        Task<OperationResult<bool>> AddAsync(BloqueAdd bloque);
        Task<OperationResult<bool>> UpdateAsync(BloqueUpdate bloque);
        Task<OperationResult<bool>> RemoveAsync(BloqueRemove bloque);
        Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion);
    }
}
