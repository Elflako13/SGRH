using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Objects.TipoHabitacion;

namespace SGRH.Web.Models
{
    public class BusquedaViewModel
    {
        public int? HotelId { get; set; }
        public string? Ciudad { get; set; }
        public DateOnly Entrada { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        public DateOnly Salida { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        public int Huespedes { get; set; } = 2;
        public List<string> Ciudades { get; set; } = new();

        public int Noches => Math.Max(0, Salida.DayNumber - Entrada.DayNumber);
    }

    public class SitioInicioViewModel
    {
        public BusquedaViewModel Busqueda { get; set; } = new();
        public List<HotelModel> Hoteles { get; set; } = new();
        public List<TipoHabitacionModel> Tipos { get; set; } = new();
    }

    public class HotelResultado
    {
        public HotelModel Hotel { get; set; } = default!;
        public int Disponibles { get; set; }
        public decimal? DesdePorNoche { get; set; }
        public decimal? TotalDesde { get; set; }
    }

    public class BusquedaResultadoViewModel
    {
        public BusquedaViewModel Busqueda { get; set; } = new();
        public List<HotelResultado> Resultados { get; set; } = new();
        public string? Error { get; set; }
    }

    public class TipoOferta
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Desde { get; set; }
        public int CapacidadMaxima { get; set; }
    }

    public class HotelPublicoViewModel
    {
        public HotelModel Hotel { get; set; } = default!;
        public BusquedaViewModel Busqueda { get; set; } = new();
        public List<TipoOferta> Tipos { get; set; } = new();
        public List<HabitacionDisponibleModel>? Disponibles { get; set; }
        public string? Error { get; set; }
    }

    public class ReservarViewModel
    {
        public int HabitacionId { get; set; }
        public DateOnly Entrada { get; set; }
        public DateOnly Salida { get; set; }
        public int Huespedes { get; set; }
        public HabitacionModel? Habitacion { get; set; }

        public int Noches => Math.Max(0, Salida.DayNumber - Entrada.DayNumber);
        public decimal Total => (Habitacion?.TarifaPorNoche ?? 0) * Noches;
    }
}
