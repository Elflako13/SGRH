using SGRH.Domain.Entities.Security;

namespace SGRH.Domain.Entities.Reservation
{
    /// <summary>
    /// Bitácora de cambios de una reserva (RF-021, RNF-007). Solo guarda datos:
    /// no hereda de AuditEntity porque un registro de auditoría no se modifica ni se inactiva.
    /// </summary>
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
