namespace SGRH.DAL.Models.Objects.Bloque
{
    public class BloqueUpdate : BloqueBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
