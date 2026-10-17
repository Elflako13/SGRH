namespace SGRH.Model.Models.Reservation
{
    /// <summary>Vista de una reserva con los datos de huésped, hotel y habitación (CU-014).</summary>
    public class ReservaDetalleModel
    {
        public int ReservaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Huesped { get; set; } = string.Empty;
        public string Hotel { get; set; } = string.Empty;
        public string Habitacion { get; set; } = string.Empty;
        public DateOnly Entrada { get; set; }
        public DateOnly Salida { get; set; }
        public int Noches { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
