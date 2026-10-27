using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    public interface IReservaRepository
    {
        Task<OperationResult<List<ReservaModel>>> GetAllAsync();
        Task<OperationResult<ReservaModel>> GetByIdAsync(int id);
        Task<OperationResult<List<ReservaModel>>> GetByHuespedAsync(int huespedId);

        /// <summary>Crea la reserva en estado Pendiente y devuelve sus datos (incluido el código).</summary>
        Task<OperationResult<ReservaModel>> AddAsync(ReservaAdd reserva);
        Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva);
        Task<OperationResult<bool>> ConfirmarAsync(int reservaId, int? usuarioModificacion);

        /// <summary>Cancela la reserva (RN-012, RN-016).</summary>
        Task<OperationResult<bool>> RemoveAsync(ReservaRemove reserva);
    }
}
