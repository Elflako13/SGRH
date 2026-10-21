using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Notification;
using SGRH.Domain.Enums;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Notification;

namespace SGRH.Persistence.Repositories.Notification
{
    public class NotificacionRepository : BaseRepository<Notificacion>, INotificacionRepository
    {
        public NotificacionRepository(SGRHContext context, ILogger<NotificacionRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<List<Notificacion>> GetByReservaAsync(int reservaId)
        {
            return await _context.Notificaciones
                                 .Where(n => n.ReservaId == reservaId)
                                 .OrderByDescending(n => n.FechaCreacion)
                                 .ToListAsync();
        }

        public async Task<List<Notificacion>> GetPendientesDeReintentoAsync()
        {
            return await _context.Notificaciones
                                 .Where(n => n.Estado == EstadoNotificacion.Fallida
                                          && n.Intentos < Notificacion.MaximoIntentos)
                                 .OrderBy(n => n.FechaCreacion)
                                 .ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(Notificacion entity)
        {
            if (entity is null)
                return OperationResult.Fail("La notificación es requerida.");

            // RN-014: toda notificación pertenece a una reserva existente.
            if (!await _context.Reservas.AnyAsync(r => r.Id == entity.ReservaId))
                return OperationResult.Fail("La reserva de la notificación no existe.");

            return await base.SaveEntityAsync(entity);
        }
    }
}
