namespace SGRH.DAL.Models.Objects.Reserva
{
    public class ReservaUpdate : ReservaBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
