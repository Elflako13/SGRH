using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class TipoHabitacionRepository : ITipoHabitacionRepository
    {
        private readonly SGRHDbContext context;

        public TipoHabitacionRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync()
        {
            try
            {
                var tipos = await Consultar().OrderBy(t => t.Nombre).ToListAsync();
                return Success(tipos);
            }
            catch (Exception)
            {
                return Failure<List<TipoHabitacionModel>>("Ocurrió un error obteniendo los tipos de habitación.");
            }
        }

        public async Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id)
        {
            var tipo = await Consultar().FirstOrDefaultAsync(t => t.TipoHabitacionId == id);
            if (tipo is null)
                return Failure<TipoHabitacionModel>("El tipo de habitación no existe.");

            return Success(tipo);
        }

        public async Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipo)
        {
            try
            {
                if (tipo is null)
                    return Failure<bool>("El objeto TipoHabitacionAdd es requerido.");

                var validation = await ValidateTipoAsync(tipo, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.TiposHabitacion.AddAsync(new TipoHabitacion
                {
                    Nombre = tipo.Nombre!.Trim(),
                    Descripcion = tipo.Descripcion?.Trim(),
                    TarifaBase = tipo.TarifaBase,
                    UsuarioCreacion = tipo.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Tipo de habitación guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipo)
        {
            try
            {
                if (tipo is null)
                    return Failure<bool>("El objeto TipoHabitacionUpdate es requerido.");

                var entity = await context.TiposHabitacion.FindAsync(tipo.Id);
                if (entity is null)
                    return Failure<bool>("El tipo de habitación no existe.");

                var validation = await ValidateTipoAsync(tipo, tipo.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Nombre = tipo.Nombre!.Trim();
                entity.Descripcion = tipo.Descripcion?.Trim();
                entity.TarifaBase = tipo.TarifaBase;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = tipo.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Tipo de habitación actualizado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(TipoHabitacionRemove tipo)
        {
            try
            {
                if (tipo is null)
                    return Failure<bool>("El objeto TipoHabitacionRemove es requerido.");

                var entity = await context.TiposHabitacion.FindAsync(tipo.Id);
                if (entity is null)
                    return Failure<bool>("El tipo de habitación no existe.");

                if (await context.Habitaciones.AnyAsync(h => h.TipoHabitacionId == tipo.Id && h.Estatus))
                    return Failure<bool>("No se puede inactivar un tipo que tiene habitaciones activas.");

                entity.Inactivar();
                entity.UsuarioModificacion = tipo.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Tipo de habitación inactivado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error inactivando el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.TiposHabitacion.FindAsync(id);
                if (entity is null)
                    return Failure<bool>("El tipo de habitación no existe.");

                if (entity.Estatus)
                    return Failure<bool>("El tipo de habitación ya está activo.");

                entity.Activar();
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Tipo de habitación activado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error activando el tipo de habitación.");
            }
        }

        private async Task<string?> ValidateTipoAsync(TipoHabitacionBase tipo, int? tipoId)
        {
            if (string.IsNullOrWhiteSpace(tipo.Nombre))
                return "El nombre del tipo de habitación es obligatorio.";

            if (tipo.TarifaBase <= 0)
                return "La tarifa base debe ser mayor que cero.";

            string nombre = tipo.Nombre.Trim();
            if (await context.TiposHabitacion.AnyAsync(t => t.Nombre == nombre && t.Id != tipoId))
                return $"Ya existe un tipo de habitación llamado {nombre}.";

            return null;
        }

        private IQueryable<TipoHabitacionModel> Consultar() =>
            context.TiposHabitacion.Select(t => new TipoHabitacionModel
            {
                TipoHabitacionId = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                TarifaBase = t.TarifaBase,
                CantidadHabitaciones = t.Habitaciones.Count(h => h.Estatus),
                Estatus = t.Estatus
            });
    }
}
