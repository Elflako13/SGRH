using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
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

        /// <summary>Devuelve todos los hoteles (activos e inactivos) con la cantidad de bloques, pisos y habitaciones.</summary>
        public async Task<OperationResult<List<HotelModel>>> GetAllAsync()
        {
            try
            {
                var hoteles = await Consultar().OrderBy(h => h.Nombre).ToListAsync();
                return Success(hoteles);
            }
            catch (Exception)
            {
                return Failure<List<HotelModel>>("Ocurrió un error obteniendo los hoteles.");
            }
        }

        public async Task<OperationResult<HotelModel>> GetByIdAsync(int id)
        {
            var hotel = await Consultar().FirstOrDefaultAsync(h => h.HotelId == id);
            if (hotel is null)
                return Failure<HotelModel>("El hotel no existe.");

            return Success(hotel);
        }

        public async Task<OperationResult<bool>> AddAsync(HotelAdd hotel)
        {
            try
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelAdd es requerido.");

                var validation = await ValidateHotelAsync(hotel, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.Hoteles.AddAsync(new Hotel
                {
                    Nombre = hotel.Nombre!.Trim(),
                    Direccion = hotel.Direccion!.Trim(),
                    Ciudad = hotel.Ciudad!.Trim(),
                    Estrellas = hotel.Estrellas,
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

                var entity = await context.Hoteles.FindAsync(hotel.Id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                var validation = await ValidateHotelAsync(hotel, hotel.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Nombre = hotel.Nombre!.Trim();
                entity.Direccion = hotel.Direccion!.Trim();
                entity.Ciudad = hotel.Ciudad!.Trim();
                entity.Estrellas = hotel.Estrellas;
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

        /// <summary>Inactiva el hotel (RF-002). No se permite si todavía tiene pisos activos.</summary>
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
                    return Failure<bool>("No se puede inactivar un hotel que tiene pisos activos. Inactive primero sus pisos.");

                entity.Inactivar();
                entity.UsuarioModificacion = hotel.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Hotel inactivado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error inactivando el hotel.");
            }
        }

        /// <summary>Vuelve a activar un hotel inactivo (RF-002).</summary>
        public async Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.Hoteles.FindAsync(id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                if (entity.Estatus)
                    return Failure<bool>("El hotel ya está activo.");

                entity.Activar();
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Hotel activado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error activando el hotel.");
            }
        }

        private async Task<string?> ValidateHotelAsync(HotelBase hotel, int? hotelId)
        {
            if (string.IsNullOrWhiteSpace(hotel.Nombre))
                return "El nombre del hotel es obligatorio.";

            if (string.IsNullOrWhiteSpace(hotel.Direccion))
                return "La dirección del hotel es obligatoria.";

            if (string.IsNullOrWhiteSpace(hotel.Ciudad))
                return "La ciudad del hotel es obligatoria.";

            if (hotel.Estrellas < 1 || hotel.Estrellas > 5)
                return "La categoría del hotel debe estar entre 1 y 5 estrellas.";

            string nombre = hotel.Nombre.Trim();
            if (await context.Hoteles.AnyAsync(h => h.Nombre == nombre && h.Id != hotelId))
                return $"Ya existe un hotel con el nombre {nombre}.";

            if (hotel.CadenaHoteleraId.HasValue && !await context.CadenasHoteleras.AnyAsync(c => c.Id == hotel.CadenaHoteleraId && c.Estatus))
                return "La cadena hotelera indicada no existe.";

            return null;
        }

        private IQueryable<HotelModel> Consultar() =>
            context.Hoteles.Select(h => new HotelModel
            {
                HotelId = h.Id,
                Nombre = h.Nombre,
                Direccion = h.Direccion,
                Ciudad = h.Ciudad,
                Estrellas = h.Estrellas,
                Telefono = h.Telefono,
                Correo = h.Correo,
                CadenaHoteleraId = h.CadenaHoteleraId,
                CantidadBloques = h.Bloques.Count(b => b.Estatus),
                CantidadPisos = h.Pisos.Count(p => p.Estatus),
                CantidadHabitaciones = context.Habitaciones.Count(x => x.HotelId == h.Id && x.Estatus),
                Estatus = h.Estatus
            });
    }
}
