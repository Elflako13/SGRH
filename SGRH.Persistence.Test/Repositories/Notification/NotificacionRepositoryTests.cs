using SGRH.Domain.Entities.Notification;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Domain.Services;
using SGRH.Domain.ValueObjects;
using SGRH.Persistence.Repositories.Notification;
using SGRH.Persistence.Repositories.Reservation;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Notification
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
            var reserva = new Reserva((await context.Huespedes.FindAsync(datos.HuespedId))!,
                                      (await context.Habitaciones.FindAsync(datos.Habitacion101Id))!,
                                      new RangoFechas(hoy.AddDays(3), hoy.AddDays(5)), 1, new CalculadoraPrecioPorNoche());
            await new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>()).SaveEntityAsync(reserva);

            var repo = new NotificacionRepository(context, TestDatabase.Logger<NotificacionRepository>());

            var enviada = new Notificacion(reserva.Id, TipoNotificacion.Confirmacion, CanalNotificacion.Correo, "Reserva confirmada");
            enviada.MarcarEnviada();

            var fallida = new Notificacion(reserva.Id, TipoNotificacion.Confirmacion, CanalNotificacion.Correo, "Reserva confirmada");
            fallida.MarcarFallida("Servidor de correo no disponible");

            var agotada = new Notificacion(reserva.Id, TipoNotificacion.Cancelacion, CanalNotificacion.Correo, "Reserva cancelada");
            for (int i = 0; i < Notificacion.MaximoIntentos; i++) agotada.MarcarFallida("Sin respuesta");

            await repo.SaveEntityAsync(enviada);
            await repo.SaveEntityAsync(fallida);
            await repo.SaveEntityAsync(agotada);

            var pendientes = await repo.GetPendientesDeReintentoAsync();

            Assert.Single(pendientes);
            Assert.Equal(fallida.Id, pendientes[0].Id);
        }

        [Fact]
        public async Task Save_ReservaInexistente_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new NotificacionRepository(context, TestDatabase.Logger<NotificacionRepository>());

            var resultado = await repo.SaveEntityAsync(new Notificacion(999, TipoNotificacion.Confirmacion, CanalNotificacion.Correo, "Hola"));

            Assert.False(resultado.Success);
        }
    }
}
