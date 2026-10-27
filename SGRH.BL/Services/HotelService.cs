using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            this.hotelRepository = hotelRepository;
        }

        public Task<OperationResult<List<HotelModel>>> GetAllAsync() => hotelRepository.GetAllAsync();

        public Task<OperationResult<HotelModel>> GetByIdAsync(int id) => hotelRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(HotelAdd hotel) => hotelRepository.AddAsync(hotel);

        public Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel) => hotelRepository.UpdateAsync(hotel);

        public Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel) => hotelRepository.RemoveAsync(hotel);
    }
}
