using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface IPisoService
    {
        Task<OperationResult<List<PisoModel>>> GetAllAsync();
        Task<OperationResult<PisoModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(PisoAdd piso);
        Task<OperationResult<bool>> UpdateAsync(PisoUpdate piso);
        Task<OperationResult<bool>> RemoveAsync(PisoRemove piso);
        Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion);
        Task<OperationResult<List<PisoModel>>> GetByHotelAsync(int hotelId);
    }
}
