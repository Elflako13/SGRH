using Microsoft.AspNetCore.Mvc.Rendering;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.TipoHabitacion;

namespace SGRH.Web.Services
{
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
