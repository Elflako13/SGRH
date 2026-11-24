using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class RangoFechas
    {
        private RangoFechas()
        {
        }

        public RangoFechas(DateOnly entrada, DateOnly salida)
        {
            if (salida <= entrada)
                throw new ReglaNegocioException("La fecha de salida debe ser posterior a la fecha de entrada (RN-008).");

            Entrada = entrada;
            Salida = salida;
        }

        public DateOnly Entrada { get; private set; }
        public DateOnly Salida { get; private set; }

        public int Noches => Salida.DayNumber - Entrada.DayNumber;

        public bool SeSolapaCon(RangoFechas otro)
        {
            return Entrada < otro.Salida && otro.Entrada < Salida;
        }

        public bool EmpiezaAntesDe(DateOnly fecha)
        {
            return Entrada < fecha;
        }

        public override string ToString() => $"{Entrada:dd/MM/yyyy} - {Salida:dd/MM/yyyy}";
    }
}
