namespace SGRH.DAL.Models.Objects.Reserva
{
    public class ReservaBase
    {
        public int HabitacionId { get; set; }
        public DateOnly FechaEntrada { get; set; }
        public DateOnly FechaSalida { get; set; }
        public int CantidadHuespedes { get; set; }
    }
}
