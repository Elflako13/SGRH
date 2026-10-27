using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SGRHDbContext context;

        public NotificacionRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<NotificacionModel>>> GetByReservaAsync(int reservaId)
        {
            try
            {
                var notificaciones = await context.Notificaciones
                                                  .Where(n => n.ReservaId == reservaId)
                                                  .OrderByDescending(n => n.FechaCreacion)
                                                  .ToListAsync();
                return Success(notificaciones.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<NotificacionModel>>("Ocurrió un error obteniendo las notificaciones.");
            }
        }

        /// <summary>CU-016: fallidas que todavía no llegan al máximo de intentos.</summary>
        public async Task<OperationResult<List<NotificacionModel>>> GetPendientesDeReintentoAsync()
        {
            try
            {
                var notificaciones = await context.Notificaciones
                                                  .Where(n => n.Estado == EstadoNotificacion.Fallida
                                                           && n.Intentos < Notificacion.MaximoIntentos)
                                                  .OrderBy(n => n.FechaCreacion)
                                                  .ToListAsync();
                return Success(notificaciones.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<NotificacionModel>>("Ocurrió un error obteniendo las notificaciones pendientes.");
            }
        }

        public async Task<OperationResult<NotificacionModel>> AddAsync(NotificacionAdd notificacion)
        {
            try
            {
                if (notificacion is null)
                    return Failure<NotificacionModel>("El objeto NotificacionAdd es requerido.");

                // RN-014: toda notificación pertenece a una reserva existente.
                if (!await context.Reservas.AnyAsync(r => r.Id == notificacion.ReservaId))
                    return Failure<NotificacionModel>("La reserva de la notificación no existe.");

                var entity = new Notificacion(notificacion.ReservaId, notificacion.Tipo, notificacion.Canal, notificacion.Mensaje ?? string.Empty)
                {
                    UsuarioCreacion = notificacion.UsuarioCreacion
                };

                await context.Notificaciones.AddAsync(entity);
                await context.SaveChangesAsync();
                return Success(ToModel(entity), "Notificación registrada.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<NotificacionModel>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<NotificacionModel>("Ocurrió un error guardando la notificación.");
            }
        }

        public async Task<OperationResult<bool>> MarcarEnviadaAsync(int notificacionId)
        {
            var entity = await context.Notificaciones.FindAsync(notificacionId);
            if (entity is null)
                return Failure<bool>("La notificación no existe.");

            entity.MarcarEnviada();
            await context.SaveChangesAsync();
            return Success(true, "Notificación marcada como enviada.");
        }

        /// <summary>RN-015: el fallo se registra, pero la reserva no se toca.</summary>
        public async Task<OperationResult<bool>> MarcarFallidaAsync(int notificacionId, string error)
        {
            var entity = await context.Notificaciones.FindAsync(notificacionId);
            if (entity is null)
                return Failure<bool>("La notificación no existe.");

            entity.MarcarFallida(error);
            await context.SaveChangesAsync();
            return Success(true, "Notificación marcada como fallida.");
        }

        private static NotificacionModel ToModel(Notificacion n) => new()
        {
            NotificacionId = n.Id,
            ReservaId = n.ReservaId,
            Tipo = n.Tipo,
            Canal = n.Canal,
            Mensaje = n.Mensaje,
            Estado = n.Estado,
            Intentos = n.Intentos,
            FechaEnvio = n.FechaEnvio,
            Error = n.Error
        };
    }
}
