using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly SGRHDbContext context;

        public HotelRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<HotelModel>>> GetAllAsync()
        {
            try
            {
                var hoteles = await context.Hoteles
                                           .Where(h => h.Estatus)
                                           .OrderBy(h => h.Nombre)
                                           .ToListAsync();
                return Success(hoteles.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<HotelModel>>("Ocurrió un error obteniendo los hoteles.");
            }
        }

        public async Task<OperationResult<HotelModel>> GetByIdAsync(int id)
        {
            var hotel = await context.Hoteles.FindAsync(id);
            if (hotel is null)
                return Failure<HotelModel>("El hotel no existe.");

            return Success(ToModel(hotel));
        }

        public async Task<OperationResult<bool>> AddAsync(HotelAdd hotel)
        {
            try
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelAdd es requerido.");

                var validation = await ValidateHotelAsync(hotel);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.Hoteles.AddAsync(new Hotel
                {
                    Nombre = hotel.Nombre!.Trim(),
                    Direccion = hotel.Direccion!.Trim(),
                    Telefono = hotel.Telefono?.Trim(),
                    Correo = hotel.Correo?.Trim(),
                    CadenaHoteleraId = hotel.CadenaHoteleraId,
                    UsuarioCreacion = hotel.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Hotel guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el hotel.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel)
        {
            try
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelUpdate es requerido.");

                var validation = await ValidateHotelAsync(hotel);
                if (validation is not null)
                    return Failure<bool>(validation);

                var entity = await context.Hoteles.FindAsync(hotel.Id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                entity.Nombre = hotel.Nombre!.Trim();
                entity.Direccion = hotel.Direccion!.Trim();
                entity.Telefono = hotel.Telefono?.Trim();
                entity.Correo = hotel.Correo?.Trim();
                entity.CadenaHoteleraId = hotel.CadenaHoteleraId;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = hotel.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Hotel actualizado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el hotel.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel)
        {
            try
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelRemove es requerido.");

                var entity = await context.Hoteles.FindAsync(hotel.Id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                if (await context.Pisos.AnyAsync(p => p.HotelId == hotel.Id && p.Estatus))
                    return Failure<bool>("No se puede eliminar un hotel que tiene pisos activos.");

                entity.Inactivar();
                entity.UsuarioModificacion = hotel.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Hotel eliminado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error eliminando el hotel.");
            }
        }

        private async Task<string?> ValidateHotelAsync(HotelBase hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Nombre))
                return "El nombre del hotel es obligatorio.";

            if (string.IsNullOrWhiteSpace(hotel.Direccion))
                return "La dirección del hotel es obligatoria.";

            if (hotel.CadenaHoteleraId.HasValue && !await context.CadenasHoteleras.AnyAsync(c => c.Id == hotel.CadenaHoteleraId && c.Estatus))
                return "La cadena hotelera indicada no existe.";

            return null;
        }

        private static HotelModel ToModel(Hotel h) => new()
        {
            HotelId = h.Id,
            Nombre = h.Nombre,
            Direccion = h.Direccion,
            Telefono = h.Telefono,
            Correo = h.Correo,
            CadenaHoteleraId = h.CadenaHoteleraId
        };
    }
}
