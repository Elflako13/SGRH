namespace SGRH.Web.Models
{
    /// <summary>Filtros comunes de todos los listados: texto a buscar y estado (activos, inactivos o todos).</summary>
    public class FiltroBase
    {
        public string? Buscar { get; set; }
        public string Estado { get; set; } = "activos";

        /// <summary>Aplica el filtro de estado a cualquier listado (DRY).</summary>
        public IEnumerable<T> AplicarEstado<T>(IEnumerable<T> items, Func<T, bool> estatus) => Estado switch
        {
            "inactivos" => items.Where(i => !estatus(i)),
            "todos" => items,
            _ => items.Where(estatus)
        };

        /// <summary>true si el texto contiene lo que el usuario escribió en "Buscar" (sin importar mayúsculas).</summary>
        public bool Coincide(params string?[] textos) =>
            string.IsNullOrWhiteSpace(Buscar)
            || textos.Any(t => t is not null && t.Contains(Buscar.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public class HotelFiltro : FiltroBase
    {
        public string? Ciudad { get; set; }
        public int? Estrellas { get; set; }
    }

    public class HotelHijoFiltro : FiltroBase
    {
        public int? HotelId { get; set; }
    }

    public class PisoFiltro : HotelHijoFiltro
    {
        public int? BloqueId { get; set; }
    }

    public class HabitacionFiltro : HotelHijoFiltro
    {
        public int? PisoId { get; set; }
        public int? TipoHabitacionId { get; set; }
        public SGRH.DAL.Models.EstadoHabitacion? EstadoOperativo { get; set; }
    }

    /// <summary>Modelo genérico de las pantallas de listado: los registros y los filtros aplicados.</summary>
    public class ListadoViewModel<TItem, TFiltro> where TFiltro : FiltroBase
    {
        public List<TItem> Items { get; set; } = new();
        public TFiltro Filtro { get; set; } = default!;
        public int Total { get; set; }
    }
}
