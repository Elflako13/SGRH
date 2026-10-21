using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Enums;
using SGRH.Domain.ValueObjects;
using SGRH.Model.Models.Configuration;
using SGRH.Persistence.Repositories.Configuration;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Configuration
{
    public class HabitacionRepositoryTests
    {
        [Fact]
        public async Task Save_NumeroRepetidoEnElHotel_Falla_RN005()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context, TestDatabase.Logger<HabitacionRepository>());

            // La 101 ya existe en el piso 1; se intenta crear otra 101 en el piso 2.
            var repetida = new Habitacion("101", TipoHabitacion.Sencilla, 1, 1500m) { PisoId = datos.Piso2Id };
            var resultado = await repo.SaveEntityAsync(repetida);

            Assert.False(resultado.Success);
            Assert.Contains("RN-005", resultado.Message);
        }

        [Fact]
        public async Task Save_PisoInactivo_Falla_RN006()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var piso = context.Pisos.Find(datos.Piso2Id)!;
            piso.Inactivar();
            context.SaveChanges();

            var repo = new HabitacionRepository(context, TestDatabase.Logger<HabitacionRepository>());
            var resultado = await repo.SaveEntityAsync(new Habitacion("202", TipoHabitacion.Doble, 2, 2500m) { PisoId = datos.Piso2Id });

            Assert.False(resultado.Success);
            Assert.Contains("RN-006", resultado.Message);
        }

        [Fact]
        public async Task Save_HabitacionValida_SeGuardaConElHotelDelPiso()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context, TestDatabase.Logger<HabitacionRepository>());

            var nueva = new Habitacion("202", TipoHabitacion.Doble, 2, 2700m) { PisoId = datos.Piso2Id };
            var resultado = await repo.SaveEntityAsync(nueva);

            Assert.True(resultado.Success);
            Assert.Equal(datos.HotelId, nueva.HotelId);
        }

        [Fact]
        public async Task GetDisponibles_ExcluyeMantenimientoYCapacidadInsuficiente_RN007_RN011()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            context.Habitaciones.Find(datos.Habitacion201Id)!.CambiarEstado(EstadoHabitacion.Mantenimiento);
            context.SaveChanges();

            var repo = new HabitacionRepository(context, TestDatabase.Logger<HabitacionRepository>());
            var fechas = new RangoFechas(new DateOnly(2030, 1, 10), new DateOnly(2030, 1, 13));

            var resultado = await repo.GetDisponiblesAsync(datos.HotelId, fechas, 2);
            var lista = (List<HabitacionDisponibleModel>)resultado.Data!;

            // 101 (doble) sí; 102 (sencilla) no por capacidad; 201 no por mantenimiento.
            Assert.True(resultado.Success);
            Assert.Single(lista);
            Assert.Equal("101", lista[0].Numero);
            Assert.Equal(7500m, lista[0].CostoEstimado);
        }
    }
}
