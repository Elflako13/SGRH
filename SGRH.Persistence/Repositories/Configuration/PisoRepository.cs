using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Enums;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Configuration;

namespace SGRH.Persistence.Repositories.Configuration
{
    public class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        public PisoRepository(SGRHContext context, ILogger<PisoRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<List<Piso>> GetByHotelAsync(int hotelId)
        {
            return await _context.Pisos
                                 .Where(p => p.HotelId == hotelId && p.Estatus)
                                 .OrderBy(p => p.Numero)
                                 .ToListAsync();
        }

        public async Task<bool> ExistePisoActivoAsync(int hotelId, int numero, int? excluirPisoId = null)
        {
            return await _context.Pisos.AnyAsync(p => p.HotelId == hotelId
                                                   && p.Numero == numero
                                                   && p.Estatus
                                                   && p.Id != excluirPisoId);
        }

        public override async Task<OperationResult> SaveEntityAsync(Piso entity)
        {
            if (entity is null)
                return OperationResult.Fail("El piso es requerido.");

            if (entity.Numero <= 0)
                return OperationResult.Fail("El número de piso debe ser mayor que cero.");

            // RN-001 / RN-002: el piso debe pertenecer a un hotel existente y activo.
            if (!await _context.Hoteles.AnyAsync(h => h.Id == entity.HotelId && h.Estatus))
                return OperationResult.Fail("El hotel no existe o está inactivo.");

            // RN-003
            if (await ExistePisoActivoAsync(entity.HotelId, entity.Numero))
                return OperationResult.Fail($"Ya existe un piso activo con el número {entity.Numero} en este hotel (RN-003).");

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Piso entity)
        {
            if (entity is null)
                return OperationResult.Fail("El piso es requerido.");

            if (entity.Estatus && await ExistePisoActivoAsync(entity.HotelId, entity.Numero, entity.Id))
                return OperationResult.Fail($"Ya existe un piso activo con el número {entity.Numero} en este hotel (RN-003).");

            // RN-018: no se inactiva un piso que tenga reservas activas futuras.
            if (!entity.Estatus && await TieneReservasActivasFuturasAsync(entity.Id))
                return OperationResult.Fail("No se puede inactivar el piso porque tiene reservas activas futuras (RN-018).");

            return await base.UpdateEntityAsync(entity);
        }

        private async Task<bool> TieneReservasActivasFuturasAsync(int pisoId)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Reservas.AnyAsync(r => r.Habitacion!.PisoId == pisoId
                                                      && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
                                                      && r.Fechas.Salida > hoy);
        }
    }
}
