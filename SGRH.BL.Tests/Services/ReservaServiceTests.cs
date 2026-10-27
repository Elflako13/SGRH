using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class ReservaServiceTests
    {
        [Fact]
        public async Task CrearYConfirmar_ConUsuario_RegistraElHistorial()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            context.Usuarios.Add(new Usuario { NombreUsuario = "recepcion", NombreCompleto = "Recepción", ClaveHash = "hash", Rol = Rol.Recepcionista });
            context.SaveChanges();
            int usuarioId = context.Usuarios.Single().Id;

            var service = new ReservaService(new ReservaRepository(context), new HistorialReservaRepository(context));
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            var creada = await service.AddAsync(new ReservaAdd
            {
                HuespedId = datos.HuespedId,
                HabitacionId = datos.Habitacion101Id,
                FechaEntrada = hoy.AddDays(3),
                FechaSalida = hoy.AddDays(5),
                CantidadHuespedes = 2,
                UsuarioCreacion = usuarioId
            });
            await service.ConfirmarAsync(creada.Data!.ReservaId, usuarioId);

            var reserva = await service.GetByIdAsync(creada.Data.ReservaId);
            var historial = await service.GetHistorialAsync(creada.Data.ReservaId);

            Assert.Equal(EstadoReserva.Confirmada, reserva.Data!.Estado);
            Assert.Equal(new[] { "Creación", "Confirmación" }, historial.Data!.Select(h => h.Accion));
        }
    }
}
