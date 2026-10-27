namespace SGRH.DAL.Models.Objects.Habitacion
{
    /// <summary>Resultado de la consulta de disponibilidad (CU-009).</summary>
    public class HabitacionDisponibleModel
    {
        public int HabitacionId { get; set; }
        public string Hotel { get; set; } = string.Empty;
        public int Piso { get; set; }
        public string Numero { get; set; } = string.Empty;
        public TipoHabitacion Tipo { get; set; }
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public int Noches { get; set; }
        public decimal CostoEstimado { get; set; }
    }
}
