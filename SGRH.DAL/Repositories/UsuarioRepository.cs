using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Models.Result;
using System.Security.Cryptography;
using System.Text;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SGRHDbContext context;

        public UsuarioRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<UsuarioModel>>> GetAllAsync()
        {
            try
            {
                var usuarios = await context.Usuarios
                                            .Where(u => u.Estatus)
                                            .OrderBy(u => u.NombreUsuario)
                                            .ToListAsync();
                return Success(usuarios.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<UsuarioModel>>("Ocurrió un error obteniendo los usuarios.");
            }
        }

        public async Task<OperationResult<UsuarioModel>> GetByIdAsync(int id)
        {
            var usuario = await context.Usuarios.FindAsync(id);
            if (usuario is null)
                return Failure<UsuarioModel>("El usuario no existe.");

            return Success(ToModel(usuario));
        }

        public async Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Estatus);
            if (usuario is null)
                return Failure<UsuarioModel>("El usuario no existe.");

            return Success(ToModel(usuario));
        }

        public async Task<OperationResult<UsuarioModel>> ValidarCredencialesAsync(string nombreUsuario, string clave)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Estatus);

            // Mismo mensaje en ambos casos para no revelar si el usuario existe.
            if (usuario is null || usuario.ClaveHash != HashClave(clave))
                return Failure<UsuarioModel>("Usuario o clave incorrectos.");

            return Success(ToModel(usuario));
        }

        public async Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario)
        {
            try
            {
                if (usuario is null)
                    return Failure<bool>("El objeto UsuarioAdd es requerido.");

                var validation = await ValidateUsuarioAsync(usuario, null);
                if (validation is not null)
                    return Failure<bool>(validation);

                if (string.IsNullOrWhiteSpace(usuario.Clave) || usuario.Clave.Length < 6)
                    return Failure<bool>("La clave debe tener al menos 6 caracteres.");

                await context.Usuarios.AddAsync(new Usuario
                {
                    NombreUsuario = usuario.NombreUsuario!.Trim(),
                    NombreCompleto = usuario.NombreCompleto!.Trim(),
                    Correo = usuario.Correo?.Trim(),
                    Rol = usuario.Rol,
                    ClaveHash = HashClave(usuario.Clave),
                    UsuarioCreacion = usuario.UsuarioCreacion
                });
                await context.SaveChangesAsync();
                return Success(true, "Usuario guardado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando el usuario.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario)
        {
            try
            {
                if (usuario is null)
                    return Failure<bool>("El objeto UsuarioUpdate es requerido.");

                var entity = await context.Usuarios.FindAsync(usuario.Id);
                if (entity is null)
                    return Failure<bool>("El usuario no existe.");

                var validation = await ValidateUsuarioAsync(usuario, usuario.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                entity.NombreUsuario = usuario.NombreUsuario!.Trim();
                entity.NombreCompleto = usuario.NombreCompleto!.Trim();
                entity.Correo = usuario.Correo?.Trim();
                entity.Rol = usuario.Rol;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificacion = usuario.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Usuario actualizado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando el usuario.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(UsuarioRemove usuario)
        {
            try
            {
                if (usuario is null)
                    return Failure<bool>("El objeto UsuarioRemove es requerido.");

                var entity = await context.Usuarios.FindAsync(usuario.Id);
                if (entity is null)
                    return Failure<bool>("El usuario no existe.");

                entity.Inactivar();
                entity.UsuarioModificacion = usuario.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Usuario eliminado correctamente.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error eliminando el usuario.");
            }
        }

        private async Task<string?> ValidateUsuarioAsync(UsuarioBase usuario, int? usuarioId)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                return "El nombre de usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto))
                return "El nombre completo es obligatorio.";

            string nombre = usuario.NombreUsuario.Trim();
            if (await context.Usuarios.AnyAsync(u => u.NombreUsuario == nombre && u.Id != usuarioId))
                return $"El nombre de usuario {nombre} ya existe.";

            return null;
        }

        private static string HashClave(string clave)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(clave)));
        }

        private static UsuarioModel ToModel(Usuario u) => new()
        {
            UsuarioId = u.Id,
            NombreUsuario = u.NombreUsuario,
            NombreCompleto = u.NombreCompleto,
            Correo = u.Correo,
            Rol = u.Rol
        };
    }
}
