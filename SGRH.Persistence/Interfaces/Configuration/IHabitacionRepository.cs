using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Repository;
using SGRH.Domain.ValueObjects;

namespace SGRH.Persistence.Interfaces.Configuration
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetByHotelAsync(int hotelId);

        /// <summary>RN-005.</summary>
        Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero, int? excluirHabitacionId = null);

        /// <summary>RN-007 y RN-011: habitaciones activas y con capacidad suficiente.</summary>
        Task<List<Habitacion>> GetReservablesAsync(int hotelId, int cantidadHuespedes);

        /// <summary>
        /// CU-009: habitaciones que se pueden reservar en esas fechas (RN-007, RN-010, RN-011, RN-012).
        /// Data = List&lt;HabitacionDisponibleModel&gt;.
        /// </summary>
        Task<OperationResult> GetDisponiblesAsync(int hotelId, RangoFechas fechas, int cantidadHuespedes);
    }
}
