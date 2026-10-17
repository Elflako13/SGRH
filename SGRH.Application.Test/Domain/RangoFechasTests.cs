using SGRH.Domain.Exceptions;
using SGRH.Domain.ValueObjects;

namespace SGRH.Application.Test.Domain
{
    public class RangoFechasTests
    {
        [Fact]
        public void Crear_SalidaIgualALaEntrada_LanzaExcepcion_RN008()
        {
            var fecha = new DateOnly(2026, 11, 10);
            Assert.Throws<ReglaNegocioException>(() => new RangoFechas(fecha, fecha));
        }

        [Fact]
        public void Noches_DelDiezAlTrece_SonTres()
        {
            var rango = new RangoFechas(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 13));
            Assert.Equal(3, rango.Noches);
        }

        [Fact]
        public void SeSolapaCon_EntraElDiaQueLaOtraSale_NoSeSolapa_RN010()
        {
            var primera = new RangoFechas(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 12));
            var segunda = new RangoFechas(new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 14));
            Assert.False(primera.SeSolapaCon(segunda));
        }

        [Fact]
        public void SeSolapaCon_RangosQueSeCruzan_SeSolapa_RN010()
        {
            var primera = new RangoFechas(new DateOnly(2026, 11, 10), new DateOnly(2026, 11, 13));
            var segunda = new RangoFechas(new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 15));
            Assert.True(primera.SeSolapaCon(segunda));
        }
    }
}
