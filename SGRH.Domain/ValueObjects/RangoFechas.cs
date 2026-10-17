using SGRH.Domain.Exceptions;

namespace SGRH.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de valor con la fecha de entrada y salida de una estadía.
    /// Concentra en un solo lugar las reglas de fechas (RN-008) y de solapamiento (RN-010).
    /// </summary>
    public sealed class RangoFechas
    {
        // Constructor vacío requerido por Entity Framework Core.
        private RangoFechas()
        {
        }

        public RangoFechas(DateOnly entrada, DateOnly salida)
        {
            // RN-008: la salida debe ser posterior a la entrada (mínimo una noche).
            if (salida <= entrada)
                throw new ReglaNegocioException("La fecha de salida debe ser posterior a la fecha de entrada (RN-008).");

            Entrada = entrada;
            Salida = salida;
        }

        public DateOnly Entrada { get; private set; }
        public DateOnly Salida { get; private set; }

        public int Noches => Salida.DayNumber - Entrada.DayNumber;

        /// <summary>
        /// RN-010: dos rangos se solapan si cada uno empieza antes de que termine el otro.
        /// El día de salida de una reserva puede ser el día de entrada de otra.
        /// </summary>
        public bool SeSolapaCon(RangoFechas otro)
        {
            return Entrada < otro.Salida && otro.Entrada < Salida;
        }

        /// <summary>RN-009: indica si la entrada es anterior a la fecha dada (normalmente hoy).</summary>
        public bool EmpiezaAntesDe(DateOnly fecha)
        {
            return Entrada < fecha;
        }

        public override string ToString() => $"{Entrada:dd/MM/yyyy} - {Salida:dd/MM/yyyy}";
    }
}
