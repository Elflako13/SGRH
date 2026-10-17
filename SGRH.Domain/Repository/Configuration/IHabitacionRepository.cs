using SGRH.Domain.Entities.Configuration;

namespace SGRH.Domain.Repository.Configuration
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetByHotelAsync(int hotelId);

        /// <summary>RN-005.</summary>
        Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero);

        /// <summary>RN-007 y RN-011: habitaciones activas y con capacidad suficiente.</summary>
        Task<List<Habitacion>> GetReservablesAsync(int hotelId, int cantidadHuespedes);
    }
}
