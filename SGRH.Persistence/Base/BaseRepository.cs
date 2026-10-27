using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Repository;
using SGRH.Persistence.Context;
using System.Linq.Expressions;

namespace SGRH.Persistence.Base
{
  
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SGRHContext _context;
        protected readonly ILogger _logger;
        protected DbSet<TEntity> Entity { get; }

        protected BaseRepository(SGRHContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            Entity = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetEntityByIdAsync(int id)
        {
            return await Entity.FindAsync(id);
        }

        public virtual async Task<OperationResult> SaveEntityAsync(TEntity entity)
        {
            try
            {
                Entity.Add(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok(entity, "Datos guardados correctamente.");
            }
            catch (Exception ex)
            {
                _context.Entry(entity).State = EntityState.Detached;
                _logger.LogError(ex, "Error guardando {Entidad}.", typeof(TEntity).Name);
                return OperationResult.Fail("Ocurrió un error guardando los datos.");
            }
        }

        public virtual async Task<OperationResult> UpdateEntityAsync(TEntity entity)
        {
            try
            {
                Entity.Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Ok(entity, "Datos actualizados correctamente.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _context.Entry(entity).State = EntityState.Detached;
                _logger.LogWarning(ex, "Conflicto de concurrencia actualizando {Entidad}.", typeof(TEntity).Name);
                return OperationResult.Fail("Otro usuario modificó estos datos. Vuelva a cargarlos e intente de nuevo.");
            }
            catch (Exception ex)
            {
                _context.Entry(entity).State = EntityState.Detached;
                _logger.LogError(ex, "Error actualizando {Entidad}.", typeof(TEntity).Name);
                return OperationResult.Fail("Ocurrió un error actualizando los datos.");
            }
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Entity.ToListAsync();
        }

        public virtual async Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                var datos = await Entity.Where(filter).ToListAsync();
                return OperationResult.Ok(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo {Entidad}.", typeof(TEntity).Name);
                return OperationResult.Fail("Ocurrió un error obteniendo los datos.");
            }
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await Entity.AnyAsync(filter);
        }
    }
}
