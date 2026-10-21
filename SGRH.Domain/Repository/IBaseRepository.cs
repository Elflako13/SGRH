using SGRH.Domain.Base;
using System.Linq.Expressions;

namespace SGRH.Domain.Repository
{
    /// <summary>
    /// Operaciones comunes a todos los repositorios. Las interfaces de cada entidad
    /// (en SGRH.Persistence/Interfaces) heredan de esta y solo agregan lo que necesitan (ISP + DRY).
    /// La eliminación es lógica: se inactiva la entidad (Estatus = false) y se llama a UpdateEntityAsync.
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task<OperationResult> SaveEntityAsync(TEntity entity);
        Task<OperationResult> UpdateEntityAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
