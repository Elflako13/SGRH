using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class HabitacionRepositoryTests
    {
        [Fact]
        public async Task Add_NumeroRepetidoEnElHotel_Falla_RN005()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context);

            var resultado = await repo.AddAsync(new HabitacionAdd { Numero = "101", PisoId = datos.Piso2Id, Tipo = TipoHabitacion.Sencilla, Capacidad = 1, TarifaPorNoche = 1500m });

            Assert.False(resultado.Success);
            Assert.Contains("RN-005", resultado.Message);
        }

        [Fact]
        public async Task Add_PisoInactivo_Falla_RN006()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            context.Pisos.Find(datos.Piso2Id)!.Inactivar();
            context.SaveChanges();

            var resultado = await new HabitacionRepository(context).AddAsync(new HabitacionAdd { Numero = "202", PisoId = datos.Piso2Id, Tipo = TipoHabitacion.Doble, Capacidad = 2, TarifaPorNoche = 2500m });

            Assert.False(resultado.Success);
            Assert.Contains("RN-006", resultado.Message);
        }

        [Fact]
        public async Task Add_CapacidadCero_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new HabitacionRepository(context).AddAsync(new HabitacionAdd { Numero = "203", PisoId = datos.Piso2Id, Tipo = TipoHabitacion.Doble, Capacidad = 0, TarifaPorNoche = 2500m });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task Add_HabitacionValida_SeGuardaConElHotelDelPiso()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context);

            var resultado = await repo.AddAsync(new HabitacionAdd { Numero = "202", PisoId = datos.Piso2Id, Tipo = TipoHabitacion.Doble, Capacidad = 2, TarifaPorNoche = 2700m });
            var habitaciones = await repo.GetByHotelAsync(datos.HotelId);

            Assert.True(resultado.Success);
            Assert.Contains(habitaciones.Data!, h => h.Numero == "202" && h.HotelId == datos.HotelId);
        }

        [Fact]
        public async Task GetDisponibles_ExcluyeMantenimientoYCapacidadInsuficiente_RN007_RN011()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context);
            await repo.CambiarEstadoAsync(datos.Habitacion201Id, EstadoHabitacion.Mantenimiento, null);

            var resultado = await repo.GetDisponiblesAsync(datos.HotelId, new DateOnly(2030, 1, 10), new DateOnly(2030, 1, 13), 2);

            // 101 (doble) sí; 102 (sencilla) no por capacidad; 201 no por mantenimiento.
            Assert.True(resultado.Success);
            Assert.Single(resultado.Data!);
            Assert.Equal("101", resultado.Data![0].Numero);
            Assert.Equal(7500m, resultado.Data[0].CostoEstimado);
        }

        [Fact]
        public async Task GetDisponibles_FechasInvalidas_Falla_RN008()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new HabitacionRepository(context).GetDisponiblesAsync(datos.HotelId, new DateOnly(2030, 1, 10), new DateOnly(2030, 1, 10), 1);

            Assert.False(resultado.Success);
            Assert.Contains("RN-008", resultado.Message);
        }
    }
}
