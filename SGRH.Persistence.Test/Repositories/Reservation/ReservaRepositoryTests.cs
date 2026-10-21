using Microsoft.EntityFrameworkCore;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Domain.Services;
using SGRH.Domain.ValueObjects;
using SGRH.Model.Models.Reservation;
using SGRH.Persistence.Repositories.Configuration;
using SGRH.Persistence.Repositories.Reservation;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Reservation
{
    public class ReservaRepositoryTests
    {
        private static readonly ICalculadoraPrecio Calculadora = new CalculadoraPrecioPorNoche();
        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        private static async Task<Reserva> CrearReservaAsync(TestDatabase db, DatosDePrueba datos, RangoFechas fechas)
        {
            using var context = db.CrearContexto();
            var huesped = await context.Huespedes.FindAsync(datos.HuespedId);
            var habitacion = await context.Habitaciones.FindAsync(datos.Habitacion101Id);
            var reserva = new Reserva(huesped!, habitacion!, fechas, 2, Calculadora);

            var resultado = await new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>()).SaveEntityAsync(reserva);
            Assert.True(resultado.Success, resultado.Message);
            return reserva;
        }

        [Fact]
        public async Task Save_ReservaValida_GuardaConPrecioYFechas()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            var fechas = new RangoFechas(Hoy.AddDays(10), Hoy.AddDays(13));

            var reserva = await CrearReservaAsync(db, datos, fechas);

            using var context = db.CrearContexto();
            var guardada = await context.Reservas.SingleAsync(r => r.Id == reserva.Id);
            Assert.Equal(7500m, guardada.PrecioTotal);
            Assert.Equal(fechas.Entrada, guardada.Fechas.Entrada);
            Assert.Equal(1, await context.Huespedes.CountAsync()); // no se duplicó el huésped
        }

        [Fact]
        public async Task Save_FechasQueSeSolapan_Falla_RN010()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            await CrearReservaAsync(db, datos, new RangoFechas(Hoy.AddDays(10), Hoy.AddDays(13)));

            using var context = db.CrearContexto();
            var huesped = await context.Huespedes.FindAsync(datos.HuespedId);
            var habitacion = await context.Habitaciones.FindAsync(datos.Habitacion101Id);
            var otra = new Reserva(huesped!, habitacion!, new RangoFechas(Hoy.AddDays(12), Hoy.AddDays(15)), 1, Calculadora);

            var resultado = await new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>()).SaveEntityAsync(otra);

            Assert.False(resultado.Success);
            Assert.Contains("RN-010", resultado.Message);
        }

        [Fact]
        public async Task Save_EntradaElDiaQueOtraSale_SePermite_RN010()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            await CrearReservaAsync(db, datos, new RangoFechas(Hoy.AddDays(10), Hoy.AddDays(13)));

            var segunda = await CrearReservaAsync(db, datos, new RangoFechas(Hoy.AddDays(13), Hoy.AddDays(15)));

            Assert.True(segunda.Id > 0);
        }

        [Fact]
        public async Task Save_EntradaEnElPasado_Falla_RN009()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var huesped = await context.Huespedes.FindAsync(datos.HuespedId);
            var habitacion = await context.Habitaciones.FindAsync(datos.Habitacion101Id);
            var reserva = new Reserva(huesped!, habitacion!, new RangoFechas(Hoy.AddDays(-2), Hoy.AddDays(1)), 1, Calculadora);

            var resultado = await new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>()).SaveEntityAsync(reserva);

            Assert.False(resultado.Success);
            Assert.Contains("RN-009", resultado.Message);
        }

        [Fact]
        public async Task ExisteSolapamiento_IgnoraReservasCanceladas_RN012()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            var fechas = new RangoFechas(Hoy.AddDays(10), Hoy.AddDays(13));
            var reserva = await CrearReservaAsync(db, datos, fechas);

            using var context = db.CrearContexto();
            var repo = new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>());
            var guardada = await repo.GetEntityByIdAsync(reserva.Id);
            guardada!.Cancelar("El huésped canceló el viaje");
            Assert.True((await repo.UpdateEntityAsync(guardada)).Success);

            Assert.False(await repo.ExisteSolapamientoAsync(datos.Habitacion101Id, fechas));
        }

        [Fact]
        public async Task HabitacionUpdate_FueraDeServicioConReservasFuturas_Falla_RN018()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            await CrearReservaAsync(db, datos, new RangoFechas(Hoy.AddDays(5), Hoy.AddDays(7)));

            using var context = db.CrearContexto();
            var repo = new HabitacionRepository(context, TestDatabase.Logger<HabitacionRepository>());
            var habitacion = await repo.GetEntityByIdAsync(datos.Habitacion101Id);
            habitacion!.CambiarEstado(EstadoHabitacion.FueraDeServicio);

            var resultado = await repo.UpdateEntityAsync(habitacion);

            Assert.False(resultado.Success);
            Assert.Contains("RN-018", resultado.Message);
        }

        [Fact]
        public async Task GetDetalleByHuesped_DevuelveHotelHabitacionYNoches()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            await CrearReservaAsync(db, datos, new RangoFechas(Hoy.AddDays(10), Hoy.AddDays(13)));

            using var context = db.CrearContexto();
            var resultado = await new ReservaRepository(context, TestDatabase.Logger<ReservaRepository>()).GetDetalleByHuespedAsync(datos.HuespedId);
            var detalle = (List<ReservaDetalleModel>)resultado.Data!;

            Assert.True(resultado.Success);
            Assert.Single(detalle);
            Assert.Equal("Hotel Prueba", detalle[0].Hotel);
            Assert.Equal("101", detalle[0].Habitacion);
            Assert.Equal(3, detalle[0].Noches);
        }
    }
}
