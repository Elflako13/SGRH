using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.CadenaHotelera;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class CadenaHoteleraRepository : ICadenaHoteleraRepository
    {
        private readonly SGRHDbContext context;

        public CadenaHoteleraRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<CadenaHoteleraModel>>> GetAllAsync()
        {
            try
            {
                var cadenas = await context.CadenasHoteleras
                                           .Where(c => c.Estatus)
                                           .OrderBy(c => c.Nombre)
                                           .ToListAsync();
                return Success(cadenas.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<CadenaHoteleraModel>>("Ocurrió un error obteniendo las cadenas hoteleras.");
            }
        }

        public async Task<OperationResult<CadenaHoteleraModel>> GetByIdAsync(int id)
        {
            var cadena = await context.CadenasHoteleras.FindAsync(id);
            if (cadena is null)
                return Failure<CadenaHoteleraModel>("La cadena hotelera no existe.");

            return Success(ToModel(cadena));
        }

        public async Task<OperationResult<bool>> AddAsync(CadenaHoteleraAdd cadena)
        {
            try
            {
                if (cadena is null)
                    return Failure<bool>("El objeto CadenaHoteleraAdd es requerido.");

                var validation = ValidateCadena(cadena);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.CadenasHoteleras.AddAsync(new CadenaHotelera
                {
                    Nombre = cadena.Nombre!.Trim(),
                    RNC = cadena.RNC?.Trim(),
                    UsuarioCreacion = cadena.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Cadena hotelera guardada correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando la cadena hotelera.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(CadenaHoteleraUpdate cadena)
        {
            try
            {
                if (cadena is null)
                    return Failure<bool>("El objeto CadenaHoteleraUpdate es requerido.");

                var validation = ValidateCadena(cadena);
                if (validation is not null)
                    return Failure<bool>(validation);

                var entity = await context.CadenasHoteleras.FindAsync(cadena.Id);
                if (entity is null)
                    return Failure<bool>("La cadena hotelera no existe.");

                entity.Nombre = cadena.Nombre!.Trim();
                entity.RNC = cadena.RNC?.Trim();
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = cadena.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Cadena hotelera actualizada correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando la cadena hotelera.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(CadenaHoteleraRemove cadena)
        {
            try
            {
                if (cadena is null)
                    return Failure<bool>("El objeto CadenaHoteleraRemove es requerido.");

                var entity = await context.CadenasHoteleras.FindAsync(cadena.Id);
                if (entity is null)
                    return Failure<bool>("La cadena hotelera no existe.");

                entity.Inactivar();
                entity.UsuarioModificacion = cadena.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Cadena hotelera eliminada correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error eliminando la cadena hotelera.");
            }
        }

        private static string? ValidateCadena(CadenaHoteleraBase cadena)
        {
            if (string.IsNullOrWhiteSpace(cadena.Nombre))
                return "El nombre de la cadena hotelera es obligatorio.";

            if (cadena.Nombre.Length > 150)
                return "El nombre de la cadena hotelera no puede pasar de 150 caracteres.";

            return null;
        }

        private static CadenaHoteleraModel ToModel(CadenaHotelera c) => new()
        {
            CadenaHoteleraId = c.Id,
            Nombre = c.Nombre,
            RNC = c.RNC
        };
    }
}
