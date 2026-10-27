using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface IHuespedService
    {
        Task<OperationResult<List<HuespedModel>>> GetAllAsync();
        Task<OperationResult<HuespedModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HuespedAdd huesped);
        Task<OperationResult<bool>> UpdateAsync(HuespedUpdate huesped);
        Task<OperationResult<bool>> RemoveAsync(HuespedRemove huesped);
        Task<OperationResult<HuespedModel>> GetByDocumentoAsync(string documento);
    }
}
