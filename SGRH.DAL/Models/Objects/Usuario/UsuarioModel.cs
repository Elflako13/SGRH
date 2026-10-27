namespace SGRH.DAL.Models.Objects.Usuario
{
    public class UsuarioModel
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public Rol Rol { get; set; }
    }
}
