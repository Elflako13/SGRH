using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Domain.ValueObjects;
using SGRH.Model.Models.Reservation;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Reservation;

namespace SGRH.Persistence.Repositories.Reservation
{
    public class ReservaRepository : BaseRepository<Reserva>, IReservaRepository
    {
        public ReservaRepository(SGRHContext context, ILogger<ReservaRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Reserva?> GetByCodigoAsync(string codigo)
        {
            return await _context.Reservas
                                 .Include(r => r.Huesped)
                                 .Include(r => r.Habitacion)
                                 .FirstOrDefaultAsync(r => r.Codigo == codigo);
        }

        public async Task<bool> ExisteSolapamientoAsync(int habitacionId, RangoFechas fechas, int? excluirReservaId = null)
        {
            var entrada = fechas.Entrada;
            var salida = fechas.Salida;

            return await _context.Reservas.AnyAsync(r => r.HabitacionId == habitacionId
                                                      && r.Id != excluirReservaId
                                                      && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada) // RN-012
                                                      && r.Fechas.Entrada < salida && entrada < r.Fechas.Salida);                      // RN-010
        }

        public async Task<List<Reserva>> GetActivasFuturasPorHabitacionAsync(int habitacionId, DateOnly desde)
        {
            return await _context.Reservas
                                 .Include(r => r.Huesped)
                                 .Where(r => r.HabitacionId == habitacionId
                                          && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
                                          && r.Fechas.Salida > desde)
                                 .OrderBy(r => r.Fechas.Entrada)
                                 .ToListAsync();
        }

        public async Task<List<Reserva>> GetByHuespedAsync(int huespedId)
        {
            return await _context.Reservas
                                 .Include(r => r.Habitacion)
                                 .Where(r => r.HuespedId == huespedId)
                                 .OrderByDescending(r => r.Fechas.Entrada)
                                 .ToListAsync();
        }

        public async Task<List<Reserva>> GetByHabitacionAsync(int habitacionId)
        {
            return await _context.Reservas
                                 .Include(r => r.Huesped)
                                 .Where(r => r.HabitacionId == habitacionId)
                                 .OrderByDescending(r => r.Fechas.Entrada)
                                 .ToListAsync();
        }

        public async Task<List<Reserva>> GetByRangoAsync(RangoFechas fechas)
        {
            var entrada = fechas.Entrada;
            var salida = fechas.Salida;

            return await _context.Reservas
                                 .Include(r => r.Huesped)
                                 .Include(r => r.Habitacion)
                                 .Where(r => r.Fechas.Entrada < salida && entrada < r.Fechas.Salida)
                                 .OrderBy(r => r.Fechas.Entrada)
                                 .ToListAsync();
        }

        public async Task<OperationResult> GetDetalleByHuespedAsync(int huespedId)
        {
            try
            {
                var datos = await (from reserva in _context.Reservas
                                   join huesped in _context.Huespedes on reserva.HuespedId equals huesped.Id
                                   join habitacion in _context.Habitaciones on reserva.HabitacionId equals habitacion.Id
                                   join hotel in _context.Hoteles on habitacion.HotelId equals hotel.Id
                                   where reserva.HuespedId == huespedId
                                   orderby reserva.Fechas.Entrada descending
                                   select new
                                   {
                                       reserva.Id,
                                       reserva.Codigo,
                                       Huesped = huesped.Nombre + " " + huesped.Apellido,
                                       Hotel = hotel.Nombre,
                                       Habitacion = habitacion.Numero,
                                       reserva.Fechas.Entrada,
                                       reserva.Fechas.Salida,
                                       reserva.PrecioTotal,
                                       reserva.Estado
                                   }).ToListAsync();

                var detalle = datos.Select(d => new ReservaDetalleModel
                {
                    ReservaId = d.Id,
                    Codigo = d.Codigo,
                    Huesped = d.Huesped,
                    Hotel = d.Hotel,
                    Habitacion = d.Habitacion,
                    Entrada = d.Entrada,
                    Salida = d.Salida,
                    Noches = d.Salida.DayNumber - d.Entrada.DayNumber,
                    PrecioTotal = d.PrecioTotal,
                    Estado = d.Estado.ToString()
                }).ToList();

                return OperationResult.Ok(detalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo las reservas del huésped {HuespedId}.", huespedId);
                return OperationResult.Fail("Ocurrió un error obteniendo las reservas del huésped.");
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Reserva entity)
        {
            if (entity is null)
                return OperationResult.Fail("La reserva es requerida.");

            // RN-009: no se aceptan reservas para fechas pasadas.
            if (entity.Fechas.EmpiezaAntesDe(DateOnly.FromDateTime(DateTime.Today)))
                return OperationResult.Fail("No se aceptan reservas con fecha de entrada anterior a hoy (RN-009).");

            // RN-010: la habitación no puede tener otra reserva activa en esas fechas.
            if (await ExisteSolapamientoAsync(entity.HabitacionId, entity.Fechas))
                return OperationResult.Fail($"La habitación ya está reservada entre {entity.Fechas} (RN-010).");

            // El huésped y la habitación ya existen en la base de datos: se marcan como "sin cambios"
            // para que EF Core no intente insertarlos de nuevo junto con la reserva.
            AdjuntarSiEsNecesario(entity.Huesped);
            AdjuntarSiEsNecesario(entity.Habitacion);

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Reserva entity)
        {
            if (entity is null)
                return OperationResult.Fail("La reserva es requerida.");

            // RN-010 al modificar: se excluye la propia reserva.
            if (entity.EstaActiva() && await ExisteSolapamientoAsync(entity.HabitacionId, entity.Fechas, entity.Id))
                return OperationResult.Fail($"La habitación ya está reservada entre {entity.Fechas} (RN-010).");

            return await base.UpdateEntityAsync(entity);
        }

        private void AdjuntarSiEsNecesario(object? entidad)
        {
            if (entidad != null && _context.Entry(entidad).State == EntityState.Detached)
                _context.Attach(entidad);
        }
    }
}
