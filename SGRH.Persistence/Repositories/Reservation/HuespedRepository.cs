using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Reservation;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Reservation;

namespace SGRH.Persistence.Repositories.Reservation
{
    public class HuespedRepository : BaseRepository<Huesped>, IHuespedRepository
    {
        public HuespedRepository(SGRHContext context, ILogger<HuespedRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Huesped?> GetByDocumentoAsync(string documento)
        {
            return await _context.Huespedes.FirstOrDefaultAsync(h => h.Documento == documento);
        }

        public override async Task<OperationResult> SaveEntityAsync(Huesped entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            // RN-017
            if (await _context.Huespedes.AnyAsync(h => h.Documento == entity.Documento))
                return OperationResult.Fail($"Ya existe un huésped con el documento {entity.Documento} (RN-017).");

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Huesped entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            if (await _context.Huespedes.AnyAsync(h => h.Documento == entity.Documento && h.Id != entity.Id))
                return OperationResult.Fail($"Ya existe un huésped con el documento {entity.Documento} (RN-017).");

            return await base.UpdateEntityAsync(entity);
        }

        public override async Task<List<Huesped>> GetAllAsync()
        {
            return await _context.Huespedes
                                 .Where(h => h.Estatus)
                                 .OrderBy(h => h.Apellido).ThenBy(h => h.Nombre)
                                 .ToListAsync();
        }

        private static OperationResult Validar(Huesped? entity)
        {
            if (entity is null)
                return OperationResult.Fail("El huésped es requerido.");

            if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Apellido))
                return OperationResult.Fail("El nombre y el apellido del huésped son obligatorios.");

            if (string.IsNullOrWhiteSpace(entity.Documento))
                return OperationResult.Fail("El documento del huésped es obligatorio (RN-017).");

            if (!entity.TieneContactoValido())
                return OperationResult.Fail("El huésped debe tener un correo o teléfono válido para poder notificarle.");

            return OperationResult.Ok();
        }
    }
}
