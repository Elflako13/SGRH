namespace SGRH.DAL.Models.Objects.HistorialReserva
{
    public class HistorialReservaModel
    {
        public int HistorialReservaId { get; set; }
        public int ReservaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string? Detalle { get; set; }
    }
}
