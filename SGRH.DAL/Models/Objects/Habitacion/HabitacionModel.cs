namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionModel
    {
        public int HabitacionId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public TipoHabitacion Tipo { get; set; }
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public EstadoHabitacion Estado { get; set; }
        public int PisoId { get; set; }
        public int NumeroPiso { get; set; }
        public int HotelId { get; set; }
    }
}
