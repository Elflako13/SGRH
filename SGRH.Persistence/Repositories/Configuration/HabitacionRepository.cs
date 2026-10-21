using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Enums;
using SGRH.Domain.ValueObjects;
using SGRH.Model.Models.Configuration;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Configuration;

namespace SGRH.Persistence.Repositories.Configuration
{
    public class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        public HabitacionRepository(SGRHContext context, ILogger<HabitacionRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<List<Habitacion>> GetByHotelAsync(int hotelId)
        {
            return await _context.Habitaciones
                                 .Include(h => h.Piso)
                                 .Where(h => h.HotelId == hotelId)
                                 .OrderBy(h => h.Numero)
                                 .ToListAsync();
        }

        public async Task<bool> ExisteNumeroEnHotelAsync(int hotelId, string numero, int? excluirHabitacionId = null)
        {
            return await _context.Habitaciones.AnyAsync(h => h.HotelId == hotelId
                                                          && h.Numero == numero
                                                          && h.Id != excluirHabitacionId);
        }

        public async Task<List<Habitacion>> GetReservablesAsync(int hotelId, int cantidadHuespedes)
        {
            return await _context.Habitaciones
                                 .Where(h => h.HotelId == hotelId
                                          && h.Estatus
                                          && (h.Estado == EstadoHabitacion.Disponible || h.Estado == EstadoHabitacion.Limpieza)
                                          && h.Capacidad >= cantidadHuespedes)
                                 .OrderBy(h => h.Numero)
                                 .ToListAsync();
        }

        public async Task<OperationResult> GetDisponiblesAsync(int hotelId, RangoFechas fechas, int cantidadHuespedes)
        {
            try
            {
                var entrada = fechas.Entrada;
                var salida = fechas.Salida;

                var datos = await (from habitacion in _context.Habitaciones
                                   join piso in _context.Pisos on habitacion.PisoId equals piso.Id
                                   join hotel in _context.Hoteles on habitacion.HotelId equals hotel.Id
                                   where habitacion.HotelId == hotelId
                                      && habitacion.Estatus
                                      && (habitacion.Estado == EstadoHabitacion.Disponible || habitacion.Estado == EstadoHabitacion.Limpieza) // RN-007
                                      && habitacion.Capacidad >= cantidadHuespedes                                                            // RN-011
                                      && !_context.Reservas.Any(r => r.HabitacionId == habitacion.Id
                                                                  && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada) // RN-012
                                                                  && r.Fechas.Entrada < salida && entrada < r.Fechas.Salida)                      // RN-010
                                   orderby piso.Numero, habitacion.Numero
                                   select new
                                   {
                                       habitacion.Id,
                                       Hotel = hotel.Nombre,
                                       Piso = piso.Numero,
                                       habitacion.Numero,
                                       habitacion.Tipo,
                                       habitacion.Capacidad,
                                       habitacion.TarifaPorNoche
                                   }).ToListAsync();

                var disponibles = datos.Select(d => new HabitacionDisponibleModel
                {
                    HabitacionId = d.Id,
                    Hotel = d.Hotel,
                    Piso = d.Piso,
                    Numero = d.Numero,
                    Tipo = d.Tipo.ToString(),
                    Capacidad = d.Capacidad,
                    TarifaPorNoche = d.TarifaPorNoche,
                    CostoEstimado = d.TarifaPorNoche * fechas.Noches // RN-013
                }).ToList();

                return OperationResult.Ok(disponibles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando la disponibilidad del hotel {HotelId}.", hotelId);
                return OperationResult.Fail("Ocurrió un error consultando la disponibilidad.");
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Habitacion entity)
        {
            if (entity is null)
                return OperationResult.Fail("La habitación es requerida.");

            // RN-004 y RN-006: debe ir en un piso existente y activo.
            var piso = await _context.Pisos.FirstOrDefaultAsync(p => p.Id == entity.PisoId);
            if (piso is null)
                return OperationResult.Fail("El piso indicado no existe.");

            if (!piso.Estatus)
                return OperationResult.Fail($"El piso {piso.Numero} está inactivo y no puede recibir habitaciones (RN-006).");

            entity.HotelId = piso.HotelId;

            // RN-005
            if (await ExisteNumeroEnHotelAsync(entity.HotelId, entity.Numero))
                return OperationResult.Fail($"Ya existe una habitación con el número {entity.Numero} en este hotel (RN-005).");

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion entity)
        {
            if (entity is null)
                return OperationResult.Fail("La habitación es requerida.");

            if (await ExisteNumeroEnHotelAsync(entity.HotelId, entity.Numero, entity.Id))
                return OperationResult.Fail($"Ya existe una habitación con el número {entity.Numero} en este hotel (RN-005).");

            // RN-018: no se saca de servicio una habitación con reservas activas futuras.
            bool dejaDeEstarDisponible = !entity.Estatus
                                         || entity.Estado == EstadoHabitacion.Mantenimiento
                                         || entity.Estado == EstadoHabitacion.FueraDeServicio
                                         || entity.Estado == EstadoHabitacion.Inactiva;

            if (dejaDeEstarDisponible && await TieneReservasActivasFuturasAsync(entity.Id))
                return OperationResult.Fail($"La habitación {entity.Numero} tiene reservas activas futuras; reubíquelas o cancélelas primero (RN-018).");

            return await base.UpdateEntityAsync(entity);
        }

        private async Task<bool> TieneReservasActivasFuturasAsync(int habitacionId)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Reservas.AnyAsync(r => r.HabitacionId == habitacionId
                                                      && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
                                                      && r.Fechas.Salida > hoy);
        }
    }
}
