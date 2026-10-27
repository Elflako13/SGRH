namespace SGRH.DAL.Models.Objects.Usuario
{
    public class UsuarioBase
    {
        public string? NombreUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public Rol Rol { get; set; }
    }
}
