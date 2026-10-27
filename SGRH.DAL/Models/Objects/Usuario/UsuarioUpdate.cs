namespace SGRH.DAL.Models.Objects.Usuario
{
    public class UsuarioUpdate : UsuarioBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
