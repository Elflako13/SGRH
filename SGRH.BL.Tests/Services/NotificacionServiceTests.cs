using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class NotificacionServiceTests
    {
        private static async Task<int> CrearReservaAsync(SGRHDbContext context, DatosDePrueba datos)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var reserva = await new ReservaRepository(context).AddAsync(new ReservaAdd
            {
                HuespedId = datos.HuespedId, HabitacionId = datos.Habitacion101Id,
                FechaEntrada = hoy.AddDays(3), FechaSalida = hoy.AddDays(5), CantidadHuespedes = 1
            });
            return reserva.Data!.ReservaId;
        }

        [Fact]
        public async Task ReintentarFallidas_EnviaLasPendientesYCuentaLasExitosas_CU016()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            int reservaId = await CrearReservaAsync(context, datos);
            var service = new NotificacionService(new NotificacionRepository(context), Opciones.Notificacion(), Log.De<NotificacionService>());

            var n1 = await service.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Confirmacion, Canal = CanalNotificacion.Correo, Mensaje = "Uno" });
            var n2 = await service.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Modificacion, Canal = CanalNotificacion.Correo, Mensaje = "Dos" });
            await service.RegistrarResultadoEnvioAsync(n1.Data!.NotificacionId, false, "Servidor caído");
            await service.RegistrarResultadoEnvioAsync(n2.Data!.NotificacionId, false, "Servidor caído");

            // Simula un envío que funciona para la primera y falla para la segunda.
            var resultado = await service.ReintentarFallidasAsync(n => Task.FromResult(n.Mensaje == "Uno"));

            var pendientes = await service.GetPendientesDeReintentoAsync();
            Assert.True(resultado.Success);
            Assert.Equal(1, resultado.Data);
            Assert.Equal(n2.Data.NotificacionId, Assert.Single(pendientes.Data!).NotificacionId);
        }

        [Fact]
        public async Task ReintentarFallidas_ErrorAlEnviar_NoDetieneElProceso_RN015()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            int reservaId = await CrearReservaAsync(context, datos);
            var service = new NotificacionService(new NotificacionRepository(context), Opciones.Notificacion(), Log.De<NotificacionService>());
            var n = await service.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Confirmacion, Canal = CanalNotificacion.Correo, Mensaje = "Hola" });
            await service.RegistrarResultadoEnvioAsync(n.Data!.NotificacionId, false, "Sin conexión");

            var resultado = await service.ReintentarFallidasAsync(_ => throw new InvalidOperationException("SMTP no disponible"));

            var guardada = Assert.Single((await service.GetByReservaAsync(reservaId)).Data!);
            Assert.True(resultado.Success);
            Assert.Equal(0, resultado.Data);
            Assert.Equal(2, guardada.Intentos);
            Assert.Equal("SMTP no disponible", guardada.Error);
        }

        [Fact]
        public async Task Add_SinMensaje_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var service = new NotificacionService(new NotificacionRepository(context), Opciones.Notificacion(), Log.De<NotificacionService>());

            var resultado = await service.AddAsync(new NotificacionAdd { ReservaId = 1, Mensaje = " " });

            Assert.False(resultado.Success);
        }
    }
}
