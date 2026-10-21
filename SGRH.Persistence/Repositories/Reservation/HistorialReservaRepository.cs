using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Reservation;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Reservation;

namespace SGRH.Persistence.Repositories.Reservation
{
    /// <summary>Bitácora de reservas (RF-021, RNF-007). Los registros no se modifican.</summary>
    public class HistorialReservaRepository : BaseRepository<HistorialReserva>, IHistorialReservaRepository
    {
        public HistorialReservaRepository(SGRHContext context, ILogger<HistorialReservaRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<List<HistorialReserva>> GetByReservaAsync(int reservaId)
        {
            return await _context.HistorialReservas
                                 .Include(h => h.Usuario)
                                 .Where(h => h.ReservaId == reservaId)
                                 .OrderBy(h => h.Fecha)
                                 .ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(HistorialReserva entity)
        {
            if (entity is null || string.IsNullOrWhiteSpace(entity.Accion))
                return OperationResult.Fail("El registro de historial debe indicar la acción realizada.");

            return await base.SaveEntityAsync(entity);
        }

        /// <summary>RNF-007: los registros de auditoría no se pueden editar.</summary>
        public override Task<OperationResult> UpdateEntityAsync(HistorialReserva entity)
        {
            return Task.FromResult(OperationResult.Fail("Los registros del historial no se pueden modificar (RNF-007)."));
        }
    }
}
