using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Configuration;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Configuration;

namespace SGRH.Persistence.Repositories.Configuration
{
    public class CadenaHoteleraRepository : BaseRepository<CadenaHotelera>, ICadenaHoteleraRepository
    {
        public CadenaHoteleraRepository(SGRHContext context, ILogger<CadenaHoteleraRepository> logger)
            : base(context, logger)
        {
        }

        public override async Task<OperationResult> SaveEntityAsync(CadenaHotelera entity)
        {
            if (entity is null)
                return OperationResult.Fail("La cadena hotelera es requerida.");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult.Fail("El nombre de la cadena hotelera es obligatorio.");

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<List<CadenaHotelera>> GetAllAsync()
        {
            return await _context.CadenasHoteleras
                                 .Where(c => c.Estatus)
                                 .OrderBy(c => c.Nombre)
                                 .ToListAsync();
        }
    }
}
