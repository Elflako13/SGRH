using SGRH.Domain.Base;
using System.Linq.Expressions;

namespace SGRH.Domain.Repository
{
    /// <summary>
    /// Operaciones comunes a todos los repositorios. Cada repositorio específico hereda de esta
    /// interfaz y solo agrega lo que necesita (ISP + DRY). Se implementa en SGRH.Persistence.
    /// </summary>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task UpdateEntityAsync(TEntity entity);
        Task DeleteEntityAsync(TEntity entity);
        Task SaveEntityAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
        Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
