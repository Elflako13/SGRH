using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Notificacion;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class NotificacionRepositoryTests
    {
        [Fact]
        public async Task GetPendientesDeReintento_SoloFallidasConIntentosDisponibles_RN015()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var reserva = await new ReservaRepository(context).AddAsync(new ReservaAdd
            {
                HuespedId = datos.HuespedId, HabitacionId = datos.Habitacion101Id,
                FechaEntrada = hoy.AddDays(3), FechaSalida = hoy.AddDays(5), CantidadHuespedes = 1
            });
            int reservaId = reserva.Data!.ReservaId;
            var repo = new NotificacionRepository(context);

            var enviada = await repo.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Confirmacion, Canal = CanalNotificacion.Correo, Mensaje = "Reserva confirmada" });
            var fallida = await repo.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Confirmacion, Canal = CanalNotificacion.Correo, Mensaje = "Reserva confirmada" });
            var agotada = await repo.AddAsync(new NotificacionAdd { ReservaId = reservaId, Tipo = TipoNotificacion.Cancelacion, Canal = CanalNotificacion.Correo, Mensaje = "Reserva cancelada" });

            await repo.MarcarEnviadaAsync(enviada.Data!.NotificacionId);
            await repo.MarcarFallidaAsync(fallida.Data!.NotificacionId, "Servidor de correo no disponible");
            for (int i = 0; i < Notificacion.MaximoIntentos; i++)
                await repo.MarcarFallidaAsync(agotada.Data!.NotificacionId, "Sin respuesta");

            var pendientes = await repo.GetPendientesDeReintentoAsync();

            Assert.Single(pendientes.Data!);
            Assert.Equal(fallida.Data.NotificacionId, pendientes.Data![0].NotificacionId);
        }

        [Fact]
        public async Task Add_ReservaInexistente_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();

            var resultado = await new NotificacionRepository(context).AddAsync(new NotificacionAdd { ReservaId = 999, Tipo = TipoNotificacion.Confirmacion, Canal = CanalNotificacion.Correo, Mensaje = "Hola" });

            Assert.False(resultado.Success);
        }
    }
}
