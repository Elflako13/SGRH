using SGRH.DAL.Models.Objects.CadenaHotelera;
using SGRH.DAL.Models.Result;

namespace SGRH.BL.Services
{
    public interface ICadenaHoteleraService
    {
        Task<OperationResult<List<CadenaHoteleraModel>>> GetAllAsync();
        Task<OperationResult<CadenaHoteleraModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(CadenaHoteleraAdd cadena);
        Task<OperationResult<bool>> UpdateAsync(CadenaHoteleraUpdate cadena);
        Task<OperationResult<bool>> RemoveAsync(CadenaHoteleraRemove cadena);
    }
}
