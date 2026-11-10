using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class UsuarioServiceTests
    {
        private static async Task<UsuarioService> CrearServicioAsync(TestDatabase db, Rol rol)
        {
            var context = db.CrearContexto();
            var service = new UsuarioService(new UsuarioRepository(context), Log.De<UsuarioService>());
            await service.AddAsync(new UsuarioAdd { NombreUsuario = "usuario", NombreCompleto = "Usuario Prueba", Clave = "Clave123*", Rol = rol });
            return service;
        }

        [Fact]
        public async Task Login_ClaveCorrecta_DevuelveElUsuario()
        {
            using var db = new TestDatabase();
            var service = await CrearServicioAsync(db, Rol.Recepcionista);

            var resultado = await service.LoginAsync("usuario", "Clave123*");

            Assert.True(resultado.Success);
            Assert.Equal(Rol.Recepcionista, resultado.Data!.Rol);
        }

        [Fact]
        public async Task Login_ClaveIncorrecta_Falla()
        {
            using var db = new TestDatabase();
            var service = await CrearServicioAsync(db, Rol.Recepcionista);

            var resultado = await service.LoginAsync("usuario", "otra");

            Assert.False(resultado.Success);
            Assert.Equal("Usuario o clave incorrectos.", resultado.Message);
        }

        [Fact]
        public async Task TienePermiso_RecepcionistaNoGestionaEstructura_RNF001()
        {
            using var db = new TestDatabase();
            var service = await CrearServicioAsync(db, Rol.Recepcionista);
            var usuario = await service.GetByNombreUsuarioAsync("usuario");

            var reservas = await service.TienePermisoAsync(usuario.Data!.UsuarioId, Permisos.GestionarReservas);
            var estructura = await service.TienePermisoAsync(usuario.Data.UsuarioId, Permisos.GestionarEstructura);

            Assert.True(reservas.Success);
            Assert.False(estructura.Success);
            Assert.Contains("RNF-001", estructura.Message);
        }
    }
}
