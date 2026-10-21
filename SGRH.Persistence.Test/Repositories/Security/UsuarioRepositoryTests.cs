using SGRH.Domain.Entities.Security;
using SGRH.Domain.Enums;
using SGRH.Persistence.Repositories.Security;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Security
{
    public class UsuarioRepositoryTests
    {
        [Fact]
        public async Task Save_NombreUsuarioRepetido_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new UsuarioRepository(context, TestDatabase.Logger<UsuarioRepository>());

            await repo.SaveEntityAsync(new Usuario { NombreUsuario = "recepcion", NombreCompleto = "Recepción 1", ClaveHash = "hash", Rol = Rol.Recepcionista });
            var resultado = await repo.SaveEntityAsync(new Usuario { NombreUsuario = "recepcion", NombreCompleto = "Recepción 2", ClaveHash = "hash", Rol = Rol.Recepcionista });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task GetByNombreUsuario_DevuelveElUsuarioActivo()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new UsuarioRepository(context, TestDatabase.Logger<UsuarioRepository>());
            await repo.SaveEntityAsync(new Usuario { NombreUsuario = "admin", NombreCompleto = "Administrador", ClaveHash = "hash", Rol = Rol.Administrador });

            var usuario = await repo.GetByNombreUsuarioAsync("admin");

            Assert.NotNull(usuario);
            Assert.True(usuario!.TienePermiso(Permisos.GestionarEstructura));
        }
    }
}
