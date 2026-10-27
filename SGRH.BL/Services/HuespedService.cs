using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class HuespedService : IHuespedService
    {
        private readonly IHuespedRepository huespedRepository;

        public HuespedService(IHuespedRepository huespedRepository)
        {
            this.huespedRepository = huespedRepository;
        }

        public Task<OperationResult<List<HuespedModel>>> GetAllAsync() => huespedRepository.GetAllAsync();

        public Task<OperationResult<HuespedModel>> GetByIdAsync(int id) => huespedRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(HuespedAdd huesped) => huespedRepository.AddAsync(huesped);

        public Task<OperationResult<bool>> UpdateAsync(HuespedUpdate huesped) => huespedRepository.UpdateAsync(huesped);

        public Task<OperationResult<bool>> RemoveAsync(HuespedRemove huesped) => huespedRepository.RemoveAsync(huesped);

        public Task<OperationResult<HuespedModel>> GetByDocumentoAsync(string documento) => huespedRepository.GetByDocumentoAsync(documento);
    }
}
