using SGRH.DAL.Models.Objects.Hotel;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    /// <summary>Formulario de hotel (CU-001). Las anotaciones validan en el navegador y en el servidor.</summary>
    public class HotelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede pasar de 150 caracteres.")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(250)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [StringLength(100)]
        public string? Ciudad { get; set; }

        [Range(1, 5, ErrorMessage = "La categoría debe estar entre 1 y 5 estrellas.")]
        [Display(Name = "Categoría (estrellas)")]
        public int Estrellas { get; set; } = 3;

        [Phone(ErrorMessage = "El teléfono no es válido.")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [StringLength(150)]
        public string? Correo { get; set; }

        public bool EsNuevo => Id == 0;

        public HotelAdd ToAdd(int usuarioId) => new()
        {
            Nombre = Nombre, Direccion = Direccion, Ciudad = Ciudad, Estrellas = Estrellas,
            Telefono = Telefono, Correo = Correo, UsuarioCreacion = usuarioId
        };

        public HotelUpdate ToUpdate(int usuarioId) => new()
        {
            Id = Id, Nombre = Nombre, Direccion = Direccion, Ciudad = Ciudad, Estrellas = Estrellas,
            Telefono = Telefono, Correo = Correo, UsuarioModificacion = usuarioId
        };

        public static HotelViewModel From(HotelModel m) => new()
        {
            Id = m.HotelId, Nombre = m.Nombre, Direccion = m.Direccion, Ciudad = m.Ciudad,
            Estrellas = m.Estrellas, Telefono = m.Telefono, Correo = m.Correo
        };
    }
}
