namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionDisponibleModel
    {
        public int HabitacionId { get; set; }
        public string Hotel { get; set; } = string.Empty;
        public int Piso { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public int Noches { get; set; }
        public decimal CostoEstimado { get; set; }
    }
}
