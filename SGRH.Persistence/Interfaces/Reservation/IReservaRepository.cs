using SGRH.Domain.Base;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Repository;
using SGRH.Domain.ValueObjects;

namespace SGRH.Persistence.Interfaces.Reservation
{
    public interface IReservaRepository : IBaseRepository<Reserva>
    {
        Task<Reserva?> GetByCodigoAsync(string codigo);

        /// <summary>RN-010 y RN-012: ¿hay otra reserva activa que choque con estas fechas?</summary>
        Task<bool> ExisteSolapamientoAsync(int habitacionId, RangoFechas fechas, int? excluirReservaId = null);

        /// <summary>RN-018: reservas activas que todavía no han terminado.</summary>
        Task<List<Reserva>> GetActivasFuturasPorHabitacionAsync(int habitacionId, DateOnly desde);

        /// <summary>CU-014: historial por huésped, habitación o rango de fechas.</summary>
        Task<List<Reserva>> GetByHuespedAsync(int huespedId);
        Task<List<Reserva>> GetByHabitacionAsync(int habitacionId);
        Task<List<Reserva>> GetByRangoAsync(RangoFechas fechas);

        /// <summary>CU-014: detalle de las reservas de un huésped. Data = List&lt;ReservaDetalleModel&gt;.</summary>
        Task<OperationResult> GetDetalleByHuespedAsync(int huespedId);
    }
}
