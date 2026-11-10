using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class PisoRepository : IPisoRepository
    {
        private readonly SGRHDbContext context;

        public PisoRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<PisoModel>>> GetAllAsync()
        {
            try
            {
                var pisos = await Consultar().OrderBy(p => p.Hotel).ThenBy(p => p.Numero).ToListAsync();
                return Success(pisos);
            }
            catch (Exception)
            {
                return Failure<List<PisoModel>>("Ocurrió un error obteniendo los pisos.");
            }
        }

        public async Task<OperationResult<List<PisoModel>>> GetByHotelAsync(int hotelId)
        {
            try
            {
                var pisos = await Consultar().Where(p => p.HotelId == hotelId && p.Estatus)
                                             .OrderBy(p => p.Numero)
                                             .ToListAsync();
                return Success(pisos);
            }
            catch (Exception)
            {
                return Failure<List<PisoModel>>("Ocurrió un error obteniendo los pisos del hotel.");
            }
        }

        public async Task<OperationResult<PisoModel>> GetByIdAsync(int id)
        {
            var piso = await Consultar().FirstOrDefaultAsync(p => p.PisoId == id);
            if (piso is null)
                return Failure<PisoModel>("El piso no existe.");

            return Success(piso);
        }

        public async Task<OperationResult<bool>> AddAsync(PisoAdd piso)
        {
            try
            {
                if (piso is null)
                    return Failure<bool>("El objeto PisoAdd es requerido.");

                var validation = await ValidatePisoAsync(piso, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.Pisos.AddAsync(new Piso
                {
                    Numero = piso.Numero,
                    HotelId = piso.HotelId,
                    BloqueId = piso.BloqueId,
                    UsuarioCreacion = piso.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Piso guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el piso.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(PisoUpdate piso)
        {
            try
            {
                if (piso is null)
                    return Failure<bool>("El objeto PisoUpdate es requerido.");

                var entity = await context.Pisos.FindAsync(piso.Id);
                if (entity is null)
                    return Failure<bool>("El piso no existe.");

                // RN-002: el piso no se puede trasladar a otro hotel.
                if (entity.HotelId != piso.HotelId)
                    return Failure<bool>("Un piso pertenece a un solo hotel y no puede trasladarse a otro (RN-002).");

                var validation = await ValidatePisoAsync(piso, piso.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Numero = piso.Numero;
                entity.BloqueId = piso.BloqueId;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = piso.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Piso actualizado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el piso.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(PisoRemove piso)
        {
            try
            {
                if (piso is null)
                    return Failure<bool>("El objeto PisoRemove es requerido.");

                var entity = await context.Pisos.FindAsync(piso.Id);
                if (entity is null)
                    return Failure<bool>("El piso no existe.");

                // RN-018: no se inactiva un piso con reservas activas futuras.
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                bool tieneReservas = await context.Reservas.AnyAsync(r => r.Habitacion!.PisoId == piso.Id
                                                                       && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
                                                                       && r.Fechas.Salida > hoy);
                if (tieneReservas)
                    return Failure<bool>("No se puede inactivar el piso porque tiene reservas activas futuras (RN-018).");

                entity.Inactivar();
                entity.UsuarioModificacion = piso.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Piso inactivado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error inactivando el piso.");
            }
        }

        /// <summary>RF-004: vuelve a activar un piso validando que no exista otro activo con el mismo número (RN-003).</summary>
        public async Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.Pisos.Include(p => p.Hotel).FirstOrDefaultAsync(p => p.Id == id);
                if (entity is null)
                    return Failure<bool>("El piso no existe.");

                if (entity.Estatus)
                    return Failure<bool>("El piso ya está activo.");

                if (entity.Hotel is { Estatus: false })
                    return Failure<bool>("No se puede activar el piso porque su hotel está inactivo.");

                if (await context.Pisos.AnyAsync(p => p.HotelId == entity.HotelId && p.Numero == entity.Numero && p.Estatus))
                    return Failure<bool>($"Ya existe un piso activo con el número {entity.Numero} en este hotel (RN-003).");

                entity.Activar();
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Piso activado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error activando el piso.");
            }
        }

        private async Task<string?> ValidatePisoAsync(PisoBase piso, int? pisoId)
        {
            if (piso.Numero <= 0)
                return "El número de piso debe ser mayor que cero.";

            // RN-001 / RN-002
            if (!await context.Hoteles.AnyAsync(h => h.Id == piso.HotelId && h.Estatus))
                return "El hotel no existe o está inactivo.";

            // RN-003
            if (await context.Pisos.AnyAsync(p => p.HotelId == piso.HotelId && p.Numero == piso.Numero && p.Estatus && p.Id != pisoId))
                return $"Ya existe un piso activo con el número {piso.Numero} en este hotel (RN-003).";

            // El bloque es opcional, pero si se indica debe estar activo y ser del mismo hotel.
            if (piso.BloqueId.HasValue
                && !await context.Bloques.AnyAsync(b => b.Id == piso.BloqueId && b.HotelId == piso.HotelId && b.Estatus))
                return "El bloque indicado no existe, está inactivo o pertenece a otro hotel.";

            return null;
        }

        private IQueryable<PisoModel> Consultar() =>
            context.Pisos.Select(p => new PisoModel
            {
                PisoId = p.Id,
                Numero = p.Numero,
                HotelId = p.HotelId,
                Hotel = p.Hotel!.Nombre,
                BloqueId = p.BloqueId,
                Bloque = p.Bloque != null ? p.Bloque.Nombre : null,
                CantidadHabitaciones = p.Habitaciones.Count(h => h.Estatus),
                Estatus = p.Estatus
            });
    }
}
