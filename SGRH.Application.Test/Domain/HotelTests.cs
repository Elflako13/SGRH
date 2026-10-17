using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Enums;
using SGRH.Domain.Exceptions;

namespace SGRH.Application.Test.Domain
{
    public class HotelTests
    {
        [Fact]
        public void AgregarPiso_NumeroActivoRepetido_LanzaExcepcion_RN003()
        {
            var hotel = new Hotel { Nombre = "Hotel Prueba" };
            hotel.AgregarPiso(1);
            Assert.Throws<ReglaNegocioException>(() => hotel.AgregarPiso(1));
        }

        [Fact]
        public void AgregarHabitacion_NumeroRepetidoEnOtroPiso_LanzaExcepcion_RN005()
        {
            var hotel = new Hotel { Nombre = "Hotel Prueba" };
            var piso1 = hotel.AgregarPiso(1);
            var piso2 = hotel.AgregarPiso(2);
            piso1.AgregarHabitacion(new Habitacion("101", TipoHabitacion.Doble, 2, 2500m));

            Assert.Throws<ReglaNegocioException>(() =>
                piso2.AgregarHabitacion(new Habitacion("101", TipoHabitacion.Sencilla, 1, 1800m)));
        }

        [Fact]
        public void AgregarHabitacion_PisoInactivo_LanzaExcepcion_RN006()
        {
            var hotel = new Hotel { Nombre = "Hotel Prueba" };
            var piso = hotel.AgregarPiso(3);
            piso.Inactivar();

            Assert.Throws<ReglaNegocioException>(() =>
                piso.AgregarHabitacion(new Habitacion("301", TipoHabitacion.Suite, 4, 6000m)));
        }
    }
}
