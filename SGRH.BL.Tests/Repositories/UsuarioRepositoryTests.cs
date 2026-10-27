using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class UsuarioRepositoryTests
    {
        [Fact]
        public async Task Add_NombreUsuarioRepetido_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new UsuarioRepository(context);

            await repo.AddAsync(new UsuarioAdd { NombreUsuario = "recepcion", NombreCompleto = "Recepción 1", Clave = "Clave123", Rol = Rol.Recepcionista });
            var resultado = await repo.AddAsync(new UsuarioAdd { NombreUsuario = "recepcion", NombreCompleto = "Recepción 2", Clave = "Clave123", Rol = Rol.Recepcionista });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task Add_YGetByNombreUsuario_GuardaLaClaveCifrada()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new UsuarioRepository(context);

            await repo.AddAsync(new UsuarioAdd { NombreUsuario = "admin", NombreCompleto = "Administrador", Clave = "Admin123*", Rol = Rol.Administrador });
            var usuario = await repo.GetByNombreUsuarioAsync("admin");

            Assert.True(usuario.Success);
            Assert.Equal(Rol.Administrador, usuario.Data!.Rol);
            Assert.NotEqual("Admin123*", context.Usuarios.Single().ClaveHash);
        }
    }
}
