using Microsoft.Extensions.Options;
using SGRH.BL.Configuration;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.BL.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository habitacionRepository;
        private readonly ReservaOptions reservaOptions;

        public HabitacionService(IHabitacionRepository habitacionRepository, IOptions<ReservaOptions> reservaOptions)
        {
            this.habitacionRepository = habitacionRepository;
            this.reservaOptions = reservaOptions.Value;
        }

        public Task<OperationResult<List<HabitacionModel>>> GetAllAsync() => habitacionRepository.GetAllAsync();

        public Task<OperationResult<HabitacionModel>> GetByIdAsync(int id) => habitacionRepository.GetByIdAsync(id);

        public Task<OperationResult<List<HabitacionModel>>> GetByHotelAsync(int hotelId) => habitacionRepository.GetByHotelAsync(hotelId);

        public Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion) => habitacionRepository.AddAsync(habitacion);

        public Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion) => habitacionRepository.UpdateAsync(habitacion);

        public Task<OperationResult<bool>> RemoveAsync(HabitacionRemove habitacion) => habitacionRepository.RemoveAsync(habitacion);

        public Task<OperationResult<bool>> CambiarEstadoAsync(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion)
            => habitacionRepository.CambiarEstadoAsync(habitacionId, estado, usuarioModificacion);

        /// <summary>
        /// CU-009: consulta de disponibilidad. Antes de ir a la base de datos aplica las reglas
        /// de la solicitud (RN-009, RN-011 y los límites de ReservaOptions) y devuelve las
        /// habitaciones ordenadas de la más económica a la más cara.
        /// </summary>
        public async Task<OperationResult<List<HabitacionDisponibleModel>>> GetDisponiblesAsync(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes)
        {
            var validation = ValidarSolicitud(entrada, salida, cantidadHuespedes);
            if (validation is not null)
                return Failure<List<HabitacionDisponibleModel>>(validation);

            var resultado = await habitacionRepository.GetDisponiblesAsync(hotelId, entrada, salida, cantidadHuespedes);
            if (!resultado.Success)
                return resultado;

            var ordenadas = resultado.Data!.OrderBy(h => h.CostoEstimado).ThenBy(h => h.Numero).ToList();

            return ordenadas.Count == 0
                ? Success(ordenadas, "No hay habitaciones disponibles para esas fechas. Pruebe con otras fechas o con menos huéspedes.")
                : Success(ordenadas, $"Se encontraron {ordenadas.Count} habitación(es) disponible(s).");
        }

        private string? ValidarSolicitud(DateOnly entrada, DateOnly salida, int cantidadHuespedes)
        {
            if (cantidadHuespedes < 1)
                return "La cantidad de huéspedes debe ser al menos 1 (RN-011).";

            if (salida <= entrada)
                return "La fecha de salida debe ser posterior a la fecha de entrada (RN-008).";

            var hoy = DateOnly.FromDateTime(DateTime.Today);
            if (entrada < hoy)
                return "No se puede consultar disponibilidad para fechas pasadas (RN-009).";

            int noches = salida.DayNumber - entrada.DayNumber;
            if (noches > reservaOptions.MaximoNoches)
                return $"Una reserva no puede pasar de {reservaOptions.MaximoNoches} noches.";

            if (entrada > hoy.AddDays(reservaOptions.DiasMaximoAnticipacion))
                return $"Solo se puede reservar con un máximo de {reservaOptions.DiasMaximoAnticipacion} días de anticipación.";

            return null;
        }
    }
}
