namespace SGRH.DAL.Models.Objects.Reserva
{
    public class ReservaModel
    {
        public int ReservaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public int HuespedId { get; set; }
        public string Huesped { get; set; } = string.Empty;
        public int HabitacionId { get; set; }
        public string Habitacion { get; set; } = string.Empty;
        public DateOnly FechaEntrada { get; set; }
        public DateOnly FechaSalida { get; set; }
        public int Noches => FechaSalida.DayNumber - FechaEntrada.DayNumber;
        public int CantidadHuespedes { get; set; }
        public decimal PrecioTotal { get; set; }
        public EstadoReserva Estado { get; set; }
        public string? MotivoCancelacion { get; set; }
    }
}
