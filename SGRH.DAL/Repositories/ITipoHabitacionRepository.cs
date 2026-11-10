using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.DAL.Models.Result;

namespace SGRH.DAL.Repositories
{
    public interface ITipoHabitacionRepository
    {
        Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync();
        Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipo);
        Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipo);
        Task<OperationResult<bool>> RemoveAsync(TipoHabitacionRemove tipo);
        Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion);
    }
}
