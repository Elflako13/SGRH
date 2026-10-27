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
                var pisos = await context.Pisos
                                         .Include(p => p.Hotel)
                                         .Where(p => p.Estatus)
                                         .OrderBy(p => p.HotelId).ThenBy(p => p.Numero)
                                         .ToListAsync();
                return Success(pisos.Select(ToModel).ToList());
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
                var pisos = await context.Pisos
                                         .Include(p => p.Hotel)
                                         .Where(p => p.HotelId == hotelId && p.Estatus)
                                         .OrderBy(p => p.Numero)
                                         .ToListAsync();
                return Success(pisos.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<PisoModel>>("Ocurrió un error obteniendo los pisos del hotel.");
            }
        }

        public async Task<OperationResult<PisoModel>> GetByIdAsync(int id)
        {
            var piso = await context.Pisos.Include(p => p.Hotel).FirstOrDefaultAsync(p => p.Id == id);
            if (piso is null)
                return Failure<PisoModel>("El piso no existe.");

            return Success(ToModel(piso));
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

                var validation = await ValidatePisoAsync(piso, piso.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Numero = piso.Numero;
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
                    return Failure<bool>("No se puede eliminar el piso porque tiene reservas activas futuras (RN-018).");

                entity.Inactivar();
                entity.UsuarioModificacion = piso.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Piso eliminado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error eliminando el piso.");
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

            return null;
        }

        private static PisoModel ToModel(Piso p) => new()
        {
            PisoId = p.Id,
            Numero = p.Numero,
            HotelId = p.HotelId,
            Hotel = p.Hotel?.Nombre ?? string.Empty
        };
    }
}
