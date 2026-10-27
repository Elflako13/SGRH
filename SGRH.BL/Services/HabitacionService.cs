using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using SGRH.DAL.Models;

namespace SGRH.BL.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository habitacionRepository;

        public HabitacionService(IHabitacionRepository habitacionRepository)
        {
            this.habitacionRepository = habitacionRepository;
        }

        public Task<OperationResult<List<HabitacionModel>>> GetAllAsync() => habitacionRepository.GetAllAsync();

        public Task<OperationResult<HabitacionModel>> GetByIdAsync(int id) => habitacionRepository.GetByIdAsync(id);

        public Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion) => habitacionRepository.AddAsync(habitacion);

        public Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion) => habitacionRepository.UpdateAsync(habitacion);

        public Task<OperationResult<bool>> RemoveAsync(HabitacionRemove habitacion) => habitacionRepository.RemoveAsync(habitacion);

        public Task<OperationResult<List<HabitacionModel>>> GetByHotelAsync(int hotelId) => habitacionRepository.GetByHotelAsync(hotelId);

        public Task<OperationResult<List<HabitacionDisponibleModel>>> GetDisponiblesAsync(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes)
        {
            if (cantidadHuespedes < 1)
                return Task.FromResult(OperationResult.Failure<List<HabitacionDisponibleModel>>("La cantidad de huéspedes debe ser al menos 1 (RN-011)."));

            return habitacionRepository.GetDisponiblesAsync(hotelId, entrada, salida, cantidadHuespedes);
        }

        public Task<OperationResult<bool>> CambiarEstadoAsync(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion)
            => habitacionRepository.CambiarEstadoAsync(habitacionId, estado, usuarioModificacion);
    }
}
