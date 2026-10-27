using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class PisoService : IPisoService
    {
        private readonly IPisoRepository pisoRepository;

        public PisoService(IPisoRepository pisoRepository)
        {
            this.pisoRepository = pisoRepository;
        }

        public Task<OperationResult<List<PisoModel>>> GetAllAsync() => pisoRepository.GetAllAsync();

        public Task<OperationResult<PisoModel>> GetByIdAsync(int id) => pisoRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(PisoAdd piso) => pisoRepository.AddAsync(piso);

        public Task<OperationResult<bool>> UpdateAsync(PisoUpdate piso) => pisoRepository.UpdateAsync(piso);

        public Task<OperationResult<bool>> RemoveAsync(PisoRemove piso) => pisoRepository.RemoveAsync(piso);

        public Task<OperationResult<List<PisoModel>>> GetByHotelAsync(int hotelId) => pisoRepository.GetByHotelAsync(hotelId);
    }
}
