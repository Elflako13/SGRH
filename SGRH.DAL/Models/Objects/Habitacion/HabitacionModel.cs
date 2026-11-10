namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionModel
    {
        public int HabitacionId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public int TipoHabitacionId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public EstadoHabitacion Estado { get; set; }
        public int PisoId { get; set; }
        public int NumeroPiso { get; set; }
        public string? Bloque { get; set; }
        public int HotelId { get; set; }
        public string Hotel { get; set; } = string.Empty;
        public bool Estatus { get; set; }
    }
}
