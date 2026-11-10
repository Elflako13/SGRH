using Microsoft.AspNetCore.Mvc.Rendering;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.TipoHabitacion;

namespace SGRH.Web.Services
{
    /// <summary>
    /// Arma las listas desplegables que usan varios formularios y filtros
    /// (hoteles, bloques, pisos, tipos y estados). Así no se repite ese código en cada controlador (DRY).
    /// </summary>
    public interface ICatalogoService
    {
        Task<SelectList> HotelesAsync(int? seleccionado = null);
        Task<SelectList> BloquesAsync(int? hotelId, int? seleccionado = null);
        Task<SelectList> PisosAsync(int? hotelId, int? seleccionado = null);
        Task<List<TipoHabitacionModel>> TiposActivosAsync();
        Task<SelectList> TiposAsync(int? seleccionado = null);
        Task<List<string>> CiudadesAsync();
        SelectList EstadosHabitacion(EstadoHabitacion? seleccionado = null);
    }
}
