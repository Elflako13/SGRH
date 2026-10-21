using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Configuration;

namespace SGRH.Persistence.Repositories.Configuration
{
    public class HotelRepository : BaseRepository<Hotel>, IHotelRepository
    {
        public HotelRepository(SGRHContext context, ILogger<HotelRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Hotel?> GetConEstructuraAsync(int hotelId)
        {
            return await _context.Hoteles
                                 .Include(h => h.Pisos.Where(p => p.Estatus))
                                 .ThenInclude(p => p.Habitaciones)
                                 .FirstOrDefaultAsync(h => h.Id == hotelId);
        }

        public override async Task<OperationResult> SaveEntityAsync(Hotel entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Hotel entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.UpdateEntityAsync(entity);
        }

        public override async Task<List<Hotel>> GetAllAsync()
        {
            return await _context.Hoteles
                                 .Where(h => h.Estatus)
                                 .OrderBy(h => h.Nombre)
                                 .ToListAsync();
        }

        private static OperationResult Validar(Hotel? entity)
        {
            if (entity is null)
                return OperationResult.Fail("El hotel es requerido.");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult.Fail("El nombre del hotel es obligatorio.");

            if (string.IsNullOrWhiteSpace(entity.Direccion))
                return OperationResult.Fail("La dirección del hotel es obligatoria.");

            return OperationResult.Ok();
        }
    }
}
