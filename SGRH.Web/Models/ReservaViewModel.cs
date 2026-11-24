using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Objects.Reserva;
using System.ComponentModel.DataAnnotations;

namespace SGRH.Web.Models
{
    public class ReservaViewModel
    {
        [Display(Name = "Hotel")]
        public int HotelId { get; set; }

        [Display(Name = "Entrada")]
        public DateOnly FechaEntrada { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        [Display(Name = "Salida")]
        public DateOnly FechaSalida { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

        [Range(1, 20, ErrorMessage = "La cantidad de huéspedes debe estar entre 1 y 20.")]
        [Display(Name = "Huéspedes")]
        public int CantidadHuespedes { get; set; } = 2;

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una habitación disponible.")]
        public int HabitacionId { get; set; }

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "Documento (cédula o pasaporte)")]
        public string? Documento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string? Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        public string? Correo { get; set; }

        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        public HuespedAdd ToHuesped(int usuarioId) => new()
        {
            Documento = Documento, Nombre = Nombre, Apellido = Apellido,
            Correo = Correo, Telefono = Telefono, UsuarioCreacion = usuarioId
        };

        public ReservaAdd ToReserva(int huespedId, int usuarioId) => new()
        {
            HuespedId = huespedId, HabitacionId = HabitacionId, FechaEntrada = FechaEntrada,
            FechaSalida = FechaSalida, CantidadHuespedes = CantidadHuespedes, UsuarioCreacion = usuarioId
        };
    }
}
