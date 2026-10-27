using SGRH.DAL.Models.Objects.HistorialReserva;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>
    /// Casos de uso de la reserva. Además de llamar al repositorio, deja constancia
    /// de cada cambio en el historial cuando se conoce el usuario (RF-021).
    /// </summary>
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository reservaRepository;
        private readonly IHistorialReservaRepository historialRepository;

        public ReservaService(IReservaRepository reservaRepository, IHistorialReservaRepository historialRepository)
        {
            this.reservaRepository = reservaRepository;
            this.historialRepository = historialRepository;
        }

        public Task<OperationResult<List<ReservaModel>>> GetAllAsync() => reservaRepository.GetAllAsync();

        public Task<OperationResult<ReservaModel>> GetByIdAsync(int id) => reservaRepository.GetByIdAsync(id);

        public Task<OperationResult<List<ReservaModel>>> GetByHuespedAsync(int huespedId) => reservaRepository.GetByHuespedAsync(huespedId);

        public Task<OperationResult<List<HistorialReservaModel>>> GetHistorialAsync(int reservaId) => historialRepository.GetByReservaAsync(reservaId);

        public async Task<OperationResult<ReservaModel>> AddAsync(ReservaAdd reserva)
        {
            var resultado = await reservaRepository.AddAsync(reserva);

            if (resultado.Success)
                await RegistrarAsync(resultado.Data!.ReservaId, reserva.UsuarioCreacion, "Creación", $"Reserva {resultado.Data.Codigo} creada.");

            return resultado;
        }

        public async Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva)
        {
            var resultado = await reservaRepository.UpdateAsync(reserva);

            if (resultado.Success)
                await RegistrarAsync(reserva.Id, reserva.UsuarioModificacion, "Modificación",
                                     $"Habitación {reserva.HabitacionId}, del {reserva.FechaEntrada:dd/MM/yyyy} al {reserva.FechaSalida:dd/MM/yyyy}.");

            return resultado;
        }

        public async Task<OperationResult<bool>> ConfirmarAsync(int reservaId, int? usuarioModificacion)
        {
            var resultado = await reservaRepository.ConfirmarAsync(reservaId, usuarioModificacion);

            if (resultado.Success)
                await RegistrarAsync(reservaId, usuarioModificacion, "Confirmación", null);

            return resultado;
        }

        public async Task<OperationResult<bool>> RemoveAsync(ReservaRemove reserva)
        {
            var resultado = await reservaRepository.RemoveAsync(reserva);

            if (resultado.Success)
                await RegistrarAsync(reserva.Id, reserva.UsuarioModificacion, "Cancelación", reserva.Motivo);

            return resultado;
        }

        private async Task RegistrarAsync(int reservaId, int? usuarioId, string accion, string? detalle)
        {
            if (usuarioId is null)
                return;

            await historialRepository.AddAsync(new HistorialReservaAdd
            {
                ReservaId = reservaId,
                UsuarioId = usuarioId.Value,
                Accion = accion,
                Detalle = detalle
            });
        }
    }
}
