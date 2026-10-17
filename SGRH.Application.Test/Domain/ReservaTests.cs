using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Domain.Exceptions;
using SGRH.Domain.Services;
using SGRH.Domain.ValueObjects;

namespace SGRH.Application.Test.Domain
{
    public class ReservaTests
    {
        private readonly ICalculadoraPrecio _calculadora = new CalculadoraPrecioPorNoche();
        private readonly Huesped _huesped = new Huesped { Nombre = "Ana", Apellido = "Pérez", Documento = "001-0000000-1", Correo = "ana@correo.com" };
        private readonly RangoFechas _tresNoches = new RangoFechas(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 13));

        [Fact]
        public void Crear_CalculaPrecioTotal_NochesPorTarifa_RN013()
        {
            var habitacion = new Habitacion("101", TipoHabitacion.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2, _calculadora);

            Assert.Equal(7500m, reserva.PrecioTotal);
            Assert.Equal(EstadoReserva.Pendiente, reserva.Estado);
        }

        [Fact]
        public void Crear_HabitacionEnMantenimiento_LanzaExcepcion_RN007()
        {
            var habitacion = new Habitacion("102", TipoHabitacion.Doble, 2, 2500m);
            habitacion.CambiarEstado(EstadoHabitacion.Mantenimiento);

            Assert.Throws<ReglaNegocioException>(() => new Reserva(_huesped, habitacion, _tresNoches, 2, _calculadora));
        }

        [Fact]
        public void Crear_MasHuespedesQueCapacidad_LanzaExcepcion_RN011()
        {
            var habitacion = new Habitacion("103", TipoHabitacion.Sencilla, 1, 1800m);
            Assert.Throws<ReglaNegocioException>(() => new Reserva(_huesped, habitacion, _tresNoches, 2, _calculadora));
        }

        [Fact]
        public void Cancelar_ReservaCanceladaNoBloqueaFechas_RN012()
        {
            var habitacion = new Habitacion("104", TipoHabitacion.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2, _calculadora);

            reserva.Cancelar("El huésped cambió sus planes");

            Assert.False(reserva.SeSolapaCon(_tresNoches));
        }

        [Fact]
        public void Confirmar_ReservaCancelada_LanzaExcepcion_RN016()
        {
            var habitacion = new Habitacion("105", TipoHabitacion.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2, _calculadora);
            reserva.Cancelar("Prueba");

            Assert.Throws<ReglaNegocioException>(() => reserva.Confirmar());
        }
    }
}
