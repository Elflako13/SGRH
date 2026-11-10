using Microsoft.AspNetCore.Mvc.Rendering;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.Web.Models;

namespace SGRH.Web.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IHotelService hotelService;
        private readonly IBloqueService bloqueService;
        private readonly IPisoService pisoService;
        private readonly ITipoHabitacionService tipoService;

        public CatalogoService(IHotelService hotelService, IBloqueService bloqueService,
                               IPisoService pisoService, ITipoHabitacionService tipoService)
        {
            this.hotelService = hotelService;
            this.bloqueService = bloqueService;
            this.pisoService = pisoService;
            this.tipoService = tipoService;
        }

        public async Task<SelectList> HotelesAsync(int? seleccionado = null)
        {
            var r = await hotelService.GetAllAsync();
            var hoteles = r.Success ? r.Data!.Where(h => h.Estatus || h.HotelId == seleccionado) : [];
            return new SelectList(hoteles, "HotelId", "Nombre", seleccionado);
        }

        public async Task<SelectList> BloquesAsync(int? hotelId, int? seleccionado = null)
        {
            if (hotelId is null or <= 0)
                return Vacia();

            var r = await bloqueService.GetByHotelAsync(hotelId.Value);
            return new SelectList(r.Success ? r.Data! : [], "BloqueId", "Nombre", seleccionado);
        }

        public async Task<SelectList> PisosAsync(int? hotelId, int? seleccionado = null)
        {
            if (hotelId is null or <= 0)
                return Vacia();

            var r = await pisoService.GetByHotelAsync(hotelId.Value);
            var pisos = r.Success
                ? r.Data!.Select(p => new { p.PisoId, Texto = p.Bloque is null ? $"Piso {p.Numero}" : $"Piso {p.Numero} · {p.Bloque}" })
                : [];
            return new SelectList(pisos, "PisoId", "Texto", seleccionado);
        }

        public async Task<List<TipoHabitacionModel>> TiposActivosAsync()
        {
            var r = await tipoService.GetAllAsync();
            return r.Success ? r.Data!.Where(t => t.Estatus).ToList() : [];
        }

        public async Task<SelectList> TiposAsync(int? seleccionado = null) =>
            new(await TiposActivosAsync(), "TipoHabitacionId", "Nombre", seleccionado);

        public async Task<List<string>> CiudadesAsync()
        {
            var r = await hotelService.GetAllAsync();
            return r.Success ? r.Data!.Select(h => h.Ciudad).Distinct().OrderBy(c => c).ToList() : [];
        }

        public SelectList EstadosHabitacion(EstadoHabitacion? seleccionado = null) =>
            new(Enum.GetValues<EstadoHabitacion>().Select(e => new { Valor = e, Texto = Formato.Estado(e) }), "Valor", "Texto", seleccionado);

        private static SelectList Vacia() => new(Array.Empty<object>());
    }
}
