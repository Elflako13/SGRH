using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class RegistroClienteTests
    {
        private static UsuarioService CrearServicio(TestDatabase db)
        {
            var context = db.CrearContexto();
            return new UsuarioService(new UsuarioRepository(context), new HuespedRepository(context), Log.De<UsuarioService>());
        }

        private static ClienteRegistro Registro(string documento = "402-1111111-1", string correo = "cliente@correo.com") => new()
        {
            Nombre = "Luis",
            Apellido = "Gómez",
            Documento = documento,
            Correo = correo,
            Telefono = "809-555-1111",
            Clave = "Clave123*"
        };

        [Fact]
        public async Task Registrar_CreaClienteYHuespedYPermiteIniciarSesion()
        {
            using var db = new TestDatabase();
            var service = CrearServicio(db);

            var resultado = await service.RegistrarClienteAsync(Registro());
            var login = await service.LoginAsync("cliente@correo.com", "Clave123*");

            Assert.True(resultado.Success);
            Assert.Equal(Rol.Cliente, resultado.Data!.Rol);
            Assert.NotNull(resultado.Data.HuespedId);
            Assert.True(login.Success);
        }

        [Fact]
        public async Task Registrar_HuespedExistente_ReutilizaElHuesped()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            var service = CrearServicio(db);

            var resultado = await service.RegistrarClienteAsync(Registro(documento: "001-0000000-1"));

            Assert.True(resultado.Success);
            Assert.Equal(datos.HuespedId, resultado.Data!.HuespedId);
        }

        [Fact]
        public async Task Registrar_DocumentoConCuenta_Falla()
        {
            using var db = new TestDatabase();
            var service = CrearServicio(db);
            await service.RegistrarClienteAsync(Registro());

            var resultado = await service.RegistrarClienteAsync(Registro(correo: "otro@correo.com"));

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task Registrar_ClaveCorta_Falla()
        {
            using var db = new TestDatabase();
            var registro = Registro();
            registro.Clave = "123";

            var resultado = await CrearServicio(db).RegistrarClienteAsync(registro);

            Assert.False(resultado.Success);
        }

        [Fact]
        public void Cliente_SoloPuedeReservarEnLinea()
        {
            Assert.True(Usuario.PuedeRealizar(Rol.Cliente, Permisos.ReservarEnLinea));
            Assert.False(Usuario.PuedeRealizar(Rol.Cliente, Permisos.GestionarEstructura));
        }
    }
}
