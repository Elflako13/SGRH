using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    public interface IReservaRepository
    {
        Task<OperationResult<List<ReservaModel>>> GetAllAsync();
        Task<OperationResult<ReservaModel>> GetByIdAsync(int id);
        Task<OperationResult<List<ReservaModel>>> GetByHuespedAsync(int huespedId);

        Task<OperationResult<ReservaModel>> AddAsync(ReservaAdd reserva);
        Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva);
        Task<OperationResult<bool>> ConfirmarAsync(int reservaId, int? usuarioModificacion);

        Task<OperationResult<bool>> RemoveAsync(ReservaRemove reserva);
    }
}
