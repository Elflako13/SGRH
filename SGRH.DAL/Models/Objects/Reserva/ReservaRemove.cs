namespace SGRH.DAL.Models.Objects.Reserva
{
    /// <summary>Cancelación de la reserva (RN-012, RN-016).</summary>
    public class ReservaRemove
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
        public string? Motivo { get; set; }
    }
}
