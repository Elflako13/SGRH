using SGRH.DAL.Models.Objects.Piso;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class PisoViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el hotel.")]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [Display(Name = "Bloque (opcional)")]
        public int? BloqueId { get; set; }

        [Range(1, 200, ErrorMessage = "El número de piso debe estar entre 1 y 200.")]
        [Display(Name = "Número de piso")]
        public int Numero { get; set; } = 1;

        public bool EsNuevo => Id == 0;

        public PisoAdd ToAdd(int usuarioId) => new() { HotelId = HotelId, BloqueId = BloqueId, Numero = Numero, UsuarioCreacion = usuarioId };

        public PisoUpdate ToUpdate(int usuarioId) => new() { Id = Id, HotelId = HotelId, BloqueId = BloqueId, Numero = Numero, UsuarioModificacion = usuarioId };

        public static PisoViewModel From(PisoModel m) => new() { Id = m.PisoId, HotelId = m.HotelId, BloqueId = m.BloqueId, Numero = m.Numero };
    }
}
