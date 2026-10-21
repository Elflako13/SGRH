using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGRH.Domain.Base;
using SGRH.Domain.Entities.Security;
using SGRH.Persistence.Base;
using SGRH.Persistence.Context;
using SGRH.Persistence.Interfaces.Security;

namespace SGRH.Persistence.Repositories.Security
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SGRHContext context, ILogger<UsuarioRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                                 .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Estatus);
        }

        public override async Task<OperationResult> SaveEntityAsync(Usuario entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == entity.NombreUsuario))
                return OperationResult.Fail($"El nombre de usuario {entity.NombreUsuario} ya existe.");

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Usuario entity)
        {
            var validacion = Validar(entity);
            if (!validacion.Success)
                return validacion;

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == entity.NombreUsuario && u.Id != entity.Id))
                return OperationResult.Fail($"El nombre de usuario {entity.NombreUsuario} ya existe.");

            return await base.UpdateEntityAsync(entity);
        }

        public override async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                                 .Where(u => u.Estatus)
                                 .OrderBy(u => u.NombreUsuario)
                                 .ToListAsync();
        }

        private static OperationResult Validar(Usuario? entity)
        {
            if (entity is null)
                return OperationResult.Fail("El usuario es requerido.");

            if (string.IsNullOrWhiteSpace(entity.NombreUsuario))
                return OperationResult.Fail("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(entity.ClaveHash))
                return OperationResult.Fail("La clave del usuario es obligatoria.");

            return OperationResult.Ok();
        }
    }
}
