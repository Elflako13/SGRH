using SGRH.DAL.Models.Objects.TipoHabitacion;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class TipoHabitacionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string? Nombre { get; set; }

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Range(1, 1_000_000, ErrorMessage = "La tarifa base debe ser mayor que cero.")]
        [Display(Name = "Tarifa base por noche (RD$)")]
        public decimal TarifaBase { get; set; }

        public bool EsNuevo => Id == 0;

        public TipoHabitacionAdd ToAdd(int usuarioId) => new() { Nombre = Nombre, Descripcion = Descripcion, TarifaBase = TarifaBase, UsuarioCreacion = usuarioId };

        public TipoHabitacionUpdate ToUpdate(int usuarioId) => new() { Id = Id, Nombre = Nombre, Descripcion = Descripcion, TarifaBase = TarifaBase, UsuarioModificacion = usuarioId };

        public static TipoHabitacionViewModel From(TipoHabitacionModel m) => new() { Id = m.TipoHabitacionId, Nombre = m.Nombre, Descripcion = m.Descripcion, TarifaBase = m.TarifaBase };
    }
}
