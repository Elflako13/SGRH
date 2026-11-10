namespace SGRH.DAL.Models.Objects.TipoHabitacion
{
    public class TipoHabitacionModel
    {
        public int TipoHabitacionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal TarifaBase { get; set; }
        public int CantidadHabitaciones { get; set; }
        public bool Estatus { get; set; }
    }
}
