using SGRH.BL.Tests.Base;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;

namespace SGRH.BL.Tests.Models
{
    public class ReservaTests
    {
        private readonly Huesped _huesped = new Huesped { Nombre = "Ana", Apellido = "Pérez", Documento = "001-0000000-1", Correo = "ana@correo.com" };
        private readonly RangoFechas _tresNoches = new RangoFechas(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 13));

        [Fact]
        public void Crear_CalculaPrecioTotal_NochesPorTarifa_RN013()
        {
            var habitacion = new Habitacion("101", Tipos.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2);

            Assert.Equal(7500m, reserva.PrecioTotal);
            Assert.Equal(EstadoReserva.Pendiente, reserva.Estado);
        }

        [Fact]
        public void Crear_HabitacionEnMantenimiento_LanzaExcepcion_RN007()
        {
            var habitacion = new Habitacion("102", Tipos.Doble, 2, 2500m);
            habitacion.CambiarEstado(EstadoHabitacion.Mantenimiento);

            Assert.Throws<ReglaNegocioException>(() => new Reserva(_huesped, habitacion, _tresNoches, 2));
        }

        [Fact]
        public void Crear_MasHuespedesQueCapacidad_LanzaExcepcion_RN011()
        {
            var habitacion = new Habitacion("103", Tipos.Sencilla, 1, 1800m);
            Assert.Throws<ReglaNegocioException>(() => new Reserva(_huesped, habitacion, _tresNoches, 2));
        }

        [Fact]
        public void Cancelar_ReservaCanceladaNoBloqueaFechas_RN012()
        {
            var habitacion = new Habitacion("104", Tipos.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2);

            reserva.Cancelar("El huésped cambió sus planes");

            Assert.False(reserva.SeSolapaCon(_tresNoches));
        }

        [Fact]
        public void Confirmar_ReservaCancelada_LanzaExcepcion_RN016()
        {
            var habitacion = new Habitacion("105", Tipos.Doble, 2, 2500m);
            var reserva = new Reserva(_huesped, habitacion, _tresNoches, 2);
            reserva.Cancelar("Prueba");

            Assert.Throws<ReglaNegocioException>(() => reserva.Confirmar());
        }
    }
}
