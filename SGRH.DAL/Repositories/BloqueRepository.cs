using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Bloque;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class BloqueRepository : IBloqueRepository
    {
        private readonly SGRHDbContext context;

        public BloqueRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<BloqueModel>>> GetAllAsync()
        {
            try
            {
                var bloques = await Consultar().OrderBy(b => b.Hotel).ThenBy(b => b.Nombre).ToListAsync();
                return Success(bloques);
            }
            catch (Exception)
            {
                return Failure<List<BloqueModel>>("Ocurrió un error obteniendo los bloques.");
            }
        }

        public async Task<OperationResult<List<BloqueModel>>> GetByHotelAsync(int hotelId)
        {
            try
            {
                var bloques = await Consultar().Where(b => b.HotelId == hotelId && b.Estatus)
                                               .OrderBy(b => b.Nombre)
                                               .ToListAsync();
                return Success(bloques);
            }
            catch (Exception)
            {
                return Failure<List<BloqueModel>>("Ocurrió un error obteniendo los bloques del hotel.");
            }
        }

        public async Task<OperationResult<BloqueModel>> GetByIdAsync(int id)
        {
            var bloque = await Consultar().FirstOrDefaultAsync(b => b.BloqueId == id);
            if (bloque is null)
                return Failure<BloqueModel>("El bloque no existe.");

            return Success(bloque);
        }

        public async Task<OperationResult<bool>> AddAsync(BloqueAdd bloque)
        {
            try
            {
                if (bloque is null)
                    return Failure<bool>("El objeto BloqueAdd es requerido.");

                var validation = await ValidateBloqueAsync(bloque, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.Bloques.AddAsync(new Bloque
                {
                    Nombre = bloque.Nombre!.Trim(),
                    Descripcion = bloque.Descripcion?.Trim(),
                    HotelId = bloque.HotelId,
                    UsuarioCreacion = bloque.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Bloque guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el bloque.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(BloqueUpdate bloque)
        {
            try
            {
                if (bloque is null)
                    return Failure<bool>("El objeto BloqueUpdate es requerido.");

                var entity = await context.Bloques.FindAsync(bloque.Id);
                if (entity is null)
                    return Failure<bool>("El bloque no existe.");

                // El bloque no cambia de hotel, igual que el piso (RN-002).
                if (entity.HotelId != bloque.HotelId)
                    return Failure<bool>("Un bloque no puede trasladarse a otro hotel.");

                var validation = await ValidateBloqueAsync(bloque, bloque.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Nombre = bloque.Nombre!.Trim();
                entity.Descripcion = bloque.Descripcion?.Trim();
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = bloque.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Bloque actualizado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el bloque.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(BloqueRemove bloque)
        {
            try
            {
                if (bloque is null)
                    return Failure<bool>("El objeto BloqueRemove es requerido.");

                var entity = await context.Bloques.FindAsync(bloque.Id);
                if (entity is null)
                    return Failure<bool>("El bloque no existe.");

                if (await context.Pisos.AnyAsync(p => p.BloqueId == bloque.Id && p.Estatus))
                    return Failure<bool>("No se puede inactivar un bloque que tiene pisos activos. Muévalos a otro bloque o inactívelos primero.");

                entity.Inactivar();
                entity.UsuarioModificacion = bloque.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Bloque inactivado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error inactivando el bloque.");
            }
        }

        public async Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.Bloques.Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == id);
                if (entity is null)
                    return Failure<bool>("El bloque no existe.");

                if (entity.Estatus)
                    return Failure<bool>("El bloque ya está activo.");

                if (entity.Hotel is { Estatus: false })
                    return Failure<bool>("No se puede activar el bloque porque su hotel está inactivo.");

                entity.Activar();
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Bloque activado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error activando el bloque.");
            }
        }

        private async Task<string?> ValidateBloqueAsync(BloqueBase bloque, int? bloqueId)
        {
            if (string.IsNullOrWhiteSpace(bloque.Nombre))
                return "El nombre del bloque es obligatorio.";

            if (!await context.Hoteles.AnyAsync(h => h.Id == bloque.HotelId && h.Estatus))
                return "El hotel no existe o está inactivo.";

            string nombre = bloque.Nombre.Trim();
            if (await context.Bloques.AnyAsync(b => b.HotelId == bloque.HotelId && b.Nombre == nombre && b.Id != bloqueId))
                return $"Ya existe un bloque llamado {nombre} en este hotel.";

            return null;
        }

        private IQueryable<BloqueModel> Consultar() =>
            context.Bloques.Select(b => new BloqueModel
            {
                BloqueId = b.Id,
                Nombre = b.Nombre,
                Descripcion = b.Descripcion,
                HotelId = b.HotelId,
                Hotel = b.Hotel!.Nombre,
                CantidadPisos = b.Pisos.Count(p => p.Estatus),
                Estatus = b.Estatus
            });
    }
}
