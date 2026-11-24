namespace SGRH.DAL.Models
{
    public sealed class HistorialReserva
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public Reserva? Reserva { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string? Detalle { get; set; }
    }
}
