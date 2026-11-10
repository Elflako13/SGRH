using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Objects.Hotel;

namespace SGRH.Web.Models
{
    /// <summary>Datos del panel de inicio.</summary>
    public class InicioViewModel
    {
        public int Hoteles { get; set; }
        public int Bloques { get; set; }
        public int Pisos { get; set; }
        public int Habitaciones { get; set; }
        public int TiposHabitacion { get; set; }
        public Dictionary<string, int> HabitacionesPorEstado { get; set; } = new();
        public List<HotelModel> HotelesDestacados { get; set; } = new();
        public List<HabitacionModel> HabitacionesNoDisponibles { get; set; } = new();
    }
}
