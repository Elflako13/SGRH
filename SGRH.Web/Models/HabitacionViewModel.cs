using SGRH.DAL.Models.Objects.Habitacion;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class HabitacionViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el hotel.")]
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el piso.")]
        [Display(Name = "Piso")]
        public int PisoId { get; set; }

        [Required(ErrorMessage = "El número de habitación es obligatorio.")]
        [StringLength(10)]
        [Display(Name = "Número")]
        public string? Numero { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el tipo de habitación.")]
        [Display(Name = "Tipo de habitación")]
        public int TipoHabitacionId { get; set; }

        [Range(1, 20, ErrorMessage = "La capacidad debe estar entre 1 y 20 personas.")]
        [Display(Name = "Capacidad (personas)")]
        public int Capacidad { get; set; } = 2;

        [Range(1, 1_000_000, ErrorMessage = "La tarifa por noche debe ser mayor que cero.")]
        [Display(Name = "Tarifa por noche (RD$)")]
        public decimal TarifaPorNoche { get; set; }

        public bool EsNuevo => Id == 0;

        public HabitacionAdd ToAdd(int usuarioId) => new()
        {
            PisoId = PisoId, Numero = Numero, TipoHabitacionId = TipoHabitacionId,
            Capacidad = Capacidad, TarifaPorNoche = TarifaPorNoche, UsuarioCreacion = usuarioId
        };

        public HabitacionUpdate ToUpdate(int usuarioId) => new()
        {
            Id = Id, TipoHabitacionId = TipoHabitacionId, Capacidad = Capacidad,
            TarifaPorNoche = TarifaPorNoche, UsuarioModificacion = usuarioId
        };

        public static HabitacionViewModel From(HabitacionModel m) => new()
        {
            Id = m.HabitacionId, HotelId = m.HotelId, PisoId = m.PisoId, Numero = m.Numero,
            TipoHabitacionId = m.TipoHabitacionId, Capacidad = m.Capacidad, TarifaPorNoche = m.TarifaPorNoche
        };
    }
}
