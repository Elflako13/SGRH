using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class HabitacionServiceTests
    {
        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        [Fact]
        public async Task GetDisponibles_CeroHuespedes_Falla_RN011()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HabitacionService(new HabitacionRepository(context), Opciones.Reserva(), Log.De<HabitacionService>());

            var resultado = await service.GetDisponiblesAsync(datos.HotelId, Hoy.AddDays(1), Hoy.AddDays(3), 0);

            Assert.False(resultado.Success);
            Assert.Contains("RN-011", resultado.Message);
        }

        [Fact]
        public async Task GetDisponibles_FechaPasada_Falla_RN009()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HabitacionService(new HabitacionRepository(context), Opciones.Reserva(), Log.De<HabitacionService>());

            var resultado = await service.GetDisponiblesAsync(datos.HotelId, Hoy.AddDays(-3), Hoy.AddDays(1), 1);

            Assert.False(resultado.Success);
            Assert.Contains("RN-009", resultado.Message);
        }

        [Fact]
        public async Task GetDisponibles_MasNochesQueElMaximo_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HabitacionService(new HabitacionRepository(context), Opciones.Reserva(maximoNoches: 7), Log.De<HabitacionService>());

            var resultado = await service.GetDisponiblesAsync(datos.HotelId, Hoy.AddDays(1), Hoy.AddDays(10), 1);

            Assert.False(resultado.Success);
            Assert.Contains("7 noches", resultado.Message);
        }

        [Fact]
        public async Task GetDisponibles_DemasiadaAnticipacion_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HabitacionService(new HabitacionRepository(context), Opciones.Reserva(diasAnticipacion: 30), Log.De<HabitacionService>());

            var resultado = await service.GetDisponiblesAsync(datos.HotelId, Hoy.AddDays(60), Hoy.AddDays(62), 1);

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task GetDisponibles_OrdenaDeMasEconomicaAMasCara()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HabitacionService(new HabitacionRepository(context), Opciones.Reserva(), Log.De<HabitacionService>());

            var resultado = await service.GetDisponiblesAsync(datos.HotelId, Hoy.AddDays(5), Hoy.AddDays(7), 1);

            // 102 (1,800), 101 (2,500) y 201 (6,000)
            Assert.True(resultado.Success);
            Assert.Equal(new[] { "102", "101", "201" }, resultado.Data!.Select(h => h.Numero));
        }
    }
}
