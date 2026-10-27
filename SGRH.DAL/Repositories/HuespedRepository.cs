using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class HuespedRepository : IHuespedRepository
    {
        private readonly SGRHDbContext context;

        public HuespedRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<HuespedModel>>> GetAllAsync()
        {
            try
            {
                var huespedes = await context.Huespedes
                                             .Where(h => h.Estatus)
                                             .OrderBy(h => h.Apellido).ThenBy(h => h.Nombre)
                                             .ToListAsync();
                return Success(huespedes.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<HuespedModel>>("Ocurrió un error obteniendo los huéspedes.");
            }
        }

        public async Task<OperationResult<HuespedModel>> GetByIdAsync(int id)
        {
            var huesped = await context.Huespedes.FindAsync(id);
            if (huesped is null)
                return Failure<HuespedModel>("El huésped no existe.");

            return Success(ToModel(huesped));
        }

        public async Task<OperationResult<HuespedModel>> GetByDocumentoAsync(string documento)
        {
            var huesped = await context.Huespedes.FirstOrDefaultAsync(h => h.Documento == documento);
            if (huesped is null)
                return Failure<HuespedModel>("No existe un huésped con ese documento.");

            return Success(ToModel(huesped));
        }

        public async Task<OperationResult<bool>> AddAsync(HuespedAdd huesped)
        {
            try
            {
                if (huesped is null)
                    return Failure<bool>("El objeto HuespedAdd es requerido.");

                var validation = await ValidateHuespedAsync(huesped, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                await context.Huespedes.AddAsync(new Huesped
                {
                    Nombre = huesped.Nombre!.Trim(),
                    Apellido = huesped.Apellido!.Trim(),
                    Documento = huesped.Documento!.Trim(),
                    Correo = huesped.Correo?.Trim(),
                    Telefono = huesped.Telefono?.Trim(),
                    UsuarioCreacion = huesped.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Huésped guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el huésped.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(HuespedUpdate huesped)
        {
            try
            {
                if (huesped is null)
                    return Failure<bool>("El objeto HuespedUpdate es requerido.");

                var entity = await context.Huespedes.FindAsync(huesped.Id);
                if (entity is null)
                    return Failure<bool>("El huésped no existe.");

                var validation = await ValidateHuespedAsync(huesped, huesped.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.Nombre = huesped.Nombre!.Trim();
                entity.Apellido = huesped.Apellido!.Trim();
                entity.Documento = huesped.Documento!.Trim();
                entity.ActualizarContacto(huesped.Correo, huesped.Telefono);
                entity.UsuarioModificacion = huesped.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Huésped actualizado correctamente.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el huésped.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(HuespedRemove huesped)
        {
            try
            {
                if (huesped is null)
                    return Failure<bool>("El objeto HuespedRemove es requerido.");

                var entity = await context.Huespedes.FindAsync(huesped.Id);
                if (entity is null)
                    return Failure<bool>("El huésped no existe.");

                entity.Inactivar();
                entity.UsuarioModificacion = huesped.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Huésped eliminado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error eliminando el huésped.");
            }
        }

        private async Task<string?> ValidateHuespedAsync(HuespedBase huesped, int? huespedId)
        {
            if (string.IsNullOrWhiteSpace(huesped.Nombre) || string.IsNullOrWhiteSpace(huesped.Apellido))
                return "El nombre y el apellido del huésped son obligatorios.";

            if (string.IsNullOrWhiteSpace(huesped.Documento))
                return "El documento del huésped es obligatorio (RN-017).";

            bool correoValido = !string.IsNullOrWhiteSpace(huesped.Correo) && huesped.Correo.Contains('@');
            if (!correoValido && string.IsNullOrWhiteSpace(huesped.Telefono))
                return "El huésped debe tener un correo o teléfono válido para poder notificarle.";

            // RN-017
            string documento = huesped.Documento.Trim();
            if (await context.Huespedes.AnyAsync(h => h.Documento == documento && h.Id != huespedId))
                return $"Ya existe un huésped con el documento {documento} (RN-017).";

            return null;
        }

        private static HuespedModel ToModel(Huesped h) => new()
        {
            HuespedId = h.Id,
            Nombre = h.Nombre,
            Apellido = h.Apellido,
            Documento = h.Documento,
            Correo = h.Correo,
            Telefono = h.Telefono
        };
    }
}
