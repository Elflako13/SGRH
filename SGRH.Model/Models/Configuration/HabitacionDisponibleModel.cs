namespace SGRH.Model.Models.Configuration
{
    /// <summary>Datos que se muestran en la consulta de disponibilidad (CU-009).</summary>
    public class HabitacionDisponibleModel
    {
        public int HabitacionId { get; set; }
        public string Hotel { get; set; } = string.Empty;
        public int Piso { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
        public decimal CostoEstimado { get; set; }
    }
}
