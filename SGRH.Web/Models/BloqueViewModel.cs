using SGRH.DAL.Models.Objects.Bloque;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class BloqueViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el hotel.")]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        public bool EsNuevo => Id == 0;

        public BloqueAdd ToAdd(int usuarioId) => new() { HotelId = HotelId, Nombre = Nombre, Descripcion = Descripcion, UsuarioCreacion = usuarioId };

        public BloqueUpdate ToUpdate(int usuarioId) => new() { Id = Id, HotelId = HotelId, Nombre = Nombre, Descripcion = Descripcion, UsuarioModificacion = usuarioId };

        public static BloqueViewModel From(BloqueModel m) => new() { Id = m.BloqueId, HotelId = m.HotelId, Nombre = m.Nombre, Descripcion = m.Descripcion };
    }
}
