using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Models;

namespace SGRH.BL.Services
{
    public interface IHabitacionService
    {
        Task<OperationResult<List<HabitacionModel>>> GetAllAsync();
        Task<OperationResult<HabitacionModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion);
        Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion);
        Task<OperationResult<bool>> RemoveAsync(HabitacionRemove habitacion);
        Task<OperationResult<List<HabitacionModel>>> GetByHotelAsync(int hotelId);
        Task<OperationResult<List<HabitacionDisponibleModel>>> GetDisponiblesAsync(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes);
        Task<OperationResult<bool>> CambiarEstadoAsync(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion);
    }
}
