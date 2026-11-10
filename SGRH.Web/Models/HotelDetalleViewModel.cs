using SGRH.DAL.Models.Objects.Bloque;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Objects.Piso;

namespace SGRH.Web.Models
{
    /// <summary>Estructura completa de un hotel (CU-006): bloques, pisos y habitaciones.</summary>
    public class HotelDetalleViewModel
    {
        public HotelModel Hotel { get; set; } = default!;
        public List<BloqueModel> Bloques { get; set; } = new();
        public List<PisoModel> Pisos { get; set; } = new();
        public List<HabitacionModel> Habitaciones { get; set; } = new();

        public List<HabitacionModel> HabitacionesDe(int pisoId) =>
            Habitaciones.Where(h => h.PisoId == pisoId).OrderBy(h => h.Numero).ToList();
    }
}
