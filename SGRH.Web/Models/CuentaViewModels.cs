using SGRH.DAL.Models.Objects.Usuario;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Escriba su correo o usuario.")]
        [Display(Name = "Correo o usuario")]
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "Escriba su contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Clave { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string? Apellido { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Cédula o pasaporte")]
        public string? Documento { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [StringLength(150)]
        public string? Correo { get; set; }

        [Phone(ErrorMessage = "El teléfono no es válido.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Clave { get; set; }

        [Compare(nameof(Clave), ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        public string? ConfirmarClave { get; set; }

        public string? ReturnUrl { get; set; }

        public ClienteRegistro ToRegistro() => new()
        {
            Nombre = Nombre, Apellido = Apellido, Documento = Documento,
            Correo = Correo, Telefono = Telefono, Clave = Clave
        };
    }
}
