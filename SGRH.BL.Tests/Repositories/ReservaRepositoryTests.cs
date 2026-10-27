using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class ReservaRepositoryTests
    {
        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        private static ReservaAdd Nueva(DatosDePrueba datos, int desde, int hasta, int cantidad = 2) => new()
        {
            HuespedId = datos.HuespedId,
            HabitacionId = datos.Habitacion101Id,
            FechaEntrada = Hoy.AddDays(desde),
            FechaSalida = Hoy.AddDays(hasta),
            CantidadHuespedes = cantidad
        };

        [Fact]
        public async Task Add_ReservaValida_CalculaPrecioYQuedaPendiente_RN013()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new ReservaRepository(context).AddAsync(Nueva(datos, 10, 13));

            Assert.True(resultado.Success, resultado.Message);
            Assert.Equal(7500m, resultado.Data!.PrecioTotal);
            Assert.Equal(EstadoReserva.Pendiente, resultado.Data.Estado);
            Assert.StartsWith("RES-", resultado.Data.Codigo);
        }

        [Fact]
        public async Task Add_FechasQueSeSolapan_Falla_RN010()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new ReservaRepository(context);
            await repo.AddAsync(Nueva(datos, 10, 13));

            var resultado = await repo.AddAsync(Nueva(datos, 12, 15, 1));

            Assert.False(resultado.Success);
            Assert.Contains("RN-010", resultado.Message);
        }

        [Fact]
        public async Task Add_EntradaElDiaQueOtraSale_SePermite_RN010()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new ReservaRepository(context);
            await repo.AddAsync(Nueva(datos, 10, 13));

            var resultado = await repo.AddAsync(Nueva(datos, 13, 15));

            Assert.True(resultado.Success, resultado.Message);
        }

        [Fact]
        public async Task Add_EntradaEnElPasado_Falla_RN009()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new ReservaRepository(context).AddAsync(Nueva(datos, -2, 1, 1));

            Assert.False(resultado.Success);
            Assert.Contains("RN-009", resultado.Message);
        }

        [Fact]
        public async Task Add_MasHuespedesQueCapacidad_Falla_RN011()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new ReservaRepository(context).AddAsync(Nueva(datos, 5, 7, 3));

            Assert.False(resultado.Success);
            Assert.Contains("RN-011", resultado.Message);
        }

        [Fact]
        public async Task Remove_ReservaCanceladaLiberaLasFechas_RN012()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new ReservaRepository(context);
            var primera = await repo.AddAsync(Nueva(datos, 10, 13));

            var cancelada = await repo.RemoveAsync(new ReservaRemove { Id = primera.Data!.ReservaId, Motivo = "El huésped canceló el viaje" });
            var otra = await repo.AddAsync(Nueva(datos, 10, 13));

            Assert.True(cancelada.Success);
            Assert.True(otra.Success, otra.Message);
        }

        [Fact]
        public async Task Confirmar_ReservaCancelada_Falla_RN016()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new ReservaRepository(context);
            var reserva = await repo.AddAsync(Nueva(datos, 10, 13));
            await repo.RemoveAsync(new ReservaRemove { Id = reserva.Data!.ReservaId, Motivo = "Prueba" });

            var resultado = await repo.ConfirmarAsync(reserva.Data.ReservaId, null);

            Assert.False(resultado.Success);
            Assert.Contains("RN-016", resultado.Message);
        }

        [Fact]
        public async Task HabitacionFueraDeServicioConReservasFuturas_Falla_RN018()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            await new ReservaRepository(context).AddAsync(Nueva(datos, 5, 7));

            var resultado = await new HabitacionRepository(context).CambiarEstadoAsync(datos.Habitacion101Id, EstadoHabitacion.FueraDeServicio, null);

            Assert.False(resultado.Success);
            Assert.Contains("RN-018", resultado.Message);
        }
    }
}
