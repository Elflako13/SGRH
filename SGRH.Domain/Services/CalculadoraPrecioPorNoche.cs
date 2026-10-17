using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.ValueObjects;

namespace SGRH.Domain.Services
{
    /// <summary>
    /// Implementación inicial de RN-013: precio total = noches x tarifa por noche.
    /// </summary>
    public sealed class CalculadoraPrecioPorNoche : ICalculadoraPrecio
    {
        public decimal CalcularTotal(Habitacion habitacion, RangoFechas fechas)
        {
            return fechas.Noches * habitacion.TarifaPorNoche;
        }
    }
}
