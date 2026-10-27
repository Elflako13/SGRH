using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Repository;
using SGRH.Domain.ValueObjects;

namespace SGRH.Persistence.Interfaces.Configuration
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetByHotelAsync(int hotelId);

        Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero, int? excluirHabitacionId = null);

        Task<List<Habitacion>> GetReservablesAsync(int hotelId, int cantidadHuespedes);

        
        Task<OperationResult> GetDisponiblesAsync(int hotelId, RangoFechas fechas, int cantidadHuespedes);
    }
}
