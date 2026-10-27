namespace SGRH.DAL.Models.Objects.HistorialReserva
{
    public class HistorialReservaAdd
    {
        public int ReservaId { get; set; }
        public int UsuarioId { get; set; }
        public string? Accion { get; set; }
        public string? Detalle { get; set; }
    }
}
