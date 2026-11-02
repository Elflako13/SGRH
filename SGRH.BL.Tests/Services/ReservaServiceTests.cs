using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class ReservaServiceTests
    {
        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        private static ReservaService CrearServicio(SGRHDbContext context, int maximoNoches = 30)
            => new ReservaService(new ReservaRepository(context),
                                  new HistorialReservaRepository(context),
                                  new NotificacionRepository(context),
                                  Opciones.Reserva(maximoNoches),
                                  Opciones.Notificacion());

        private static ReservaAdd Nueva(DatosDePrueba datos, int desde, int hasta, int? usuarioId = null) => new()
        {
            HuespedId = datos.HuespedId,
            HabitacionId = datos.Habitacion101Id,
            FechaEntrada = Hoy.AddDays(desde),
            FechaSalida = Hoy.AddDays(hasta),
            CantidadHuespedes = 2,
            UsuarioCreacion = usuarioId
        };

        private static int CrearUsuario(SGRHDbContext context)
        {
            context.Usuarios.Add(new Usuario { NombreUsuario = "recepcion", NombreCompleto = "Recepción", ClaveHash = "hash", Rol = Rol.Recepcionista });
            context.SaveChanges();
            return context.Usuarios.Single().Id;
        }

        [Fact]
        public async Task Add_MasNochesQueElMaximo_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await CrearServicio(context, maximoNoches: 5).AddAsync(Nueva(datos, 3, 10));

            Assert.False(resultado.Success);
            Assert.Contains("5 noches", resultado.Message);
        }

        [Fact]
        public async Task CrearYConfirmar_ConUsuario_RegistraElHistorial_RF021()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            int usuarioId = CrearUsuario(context);
            var service = CrearServicio(context);

            var creada = await service.AddAsync(Nueva(datos, 3, 5, usuarioId));
            await service.ConfirmarAsync(creada.Data!.ReservaId, usuarioId);

            var reserva = await service.GetByIdAsync(creada.Data.ReservaId);
            var historial = await service.GetHistorialAsync(creada.Data.ReservaId);

            Assert.Equal(EstadoReserva.Confirmada, reserva.Data!.Estado);
            Assert.Equal(new[] { "Creación", "Confirmación" }, historial.Data!.Select(h => h.Accion));
        }

        [Fact]
        public async Task Confirmar_GeneraNotificacionPendiente_RN014()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = CrearServicio(context);
            var creada = await service.AddAsync(Nueva(datos, 3, 5));

            await service.ConfirmarAsync(creada.Data!.ReservaId, null);

            var notificaciones = await new NotificacionRepository(context).GetByReservaAsync(creada.Data.ReservaId);
            var notificacion = Assert.Single(notificaciones.Data!);
            Assert.Equal(TipoNotificacion.Confirmacion, notificacion.Tipo);
            Assert.Equal(EstadoNotificacion.Pendiente, notificacion.Estado);
            Assert.Contains(creada.Data.Codigo, notificacion.Mensaje);
        }

        [Fact]
        public async Task Cancelar_GeneraNotificacionDeCancelacion_RN014()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = CrearServicio(context);
            var creada = await service.AddAsync(Nueva(datos, 3, 5));

            var cancelada = await service.RemoveAsync(new ReservaRemove { Id = creada.Data!.ReservaId, Motivo = "Cambio de planes" });

            var notificaciones = await new NotificacionRepository(context).GetByReservaAsync(creada.Data.ReservaId);
            Assert.True(cancelada.Success);
            Assert.Equal(TipoNotificacion.Cancelacion, Assert.Single(notificaciones.Data!).Tipo);
        }

        [Fact]
        public async Task ConfirmarDosVeces_SegundaFallaYNoGeneraOtraNotificacion_RN016()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = CrearServicio(context);
            var creada = await service.AddAsync(Nueva(datos, 3, 5));
            await service.ConfirmarAsync(creada.Data!.ReservaId, null);

            var segunda = await service.ConfirmarAsync(creada.Data.ReservaId, null);

            var notificaciones = await new NotificacionRepository(context).GetByReservaAsync(creada.Data.ReservaId);
            Assert.False(segunda.Success);
            Assert.Single(notificaciones.Data!);
        }
    }
}
