using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.ValueObjects;

namespace SGRH.Domain.Services
{
    /// <summary>
    /// Define cómo se calcula el precio de una estadía (RN-013).
    /// Es un punto de extensión (OCP / patrón Strategy): si mañana hay tarifas por temporada,
    /// se crea otra implementación sin modificar la clase Reserva.
    /// </summary>
    public interface ICalculadoraPrecio
    {
        decimal CalcularTotal(Habitacion habitacion, RangoFechas fechas);
    }
}
