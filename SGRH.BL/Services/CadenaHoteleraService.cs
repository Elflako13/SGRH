using SGRH.DAL.Models.Objects.CadenaHotelera;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class CadenaHoteleraService : ICadenaHoteleraService
    {
        private readonly ICadenaHoteleraRepository cadenaRepository;

        public CadenaHoteleraService(ICadenaHoteleraRepository cadenaRepository)
        {
            this.cadenaRepository = cadenaRepository;
        }

        public Task<OperationResult<List<CadenaHoteleraModel>>> GetAllAsync() => cadenaRepository.GetAllAsync();

        public Task<OperationResult<CadenaHoteleraModel>> GetByIdAsync(int id) => cadenaRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(CadenaHoteleraAdd cadena) => cadenaRepository.AddAsync(cadena);

        public Task<OperationResult<bool>> UpdateAsync(CadenaHoteleraUpdate cadena) => cadenaRepository.UpdateAsync(cadena);

        public Task<OperationResult<bool>> RemoveAsync(CadenaHoteleraRemove cadena) => cadenaRepository.RemoveAsync(cadena);
    }
}
