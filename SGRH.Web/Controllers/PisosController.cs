using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Piso;
using SGRH.Web.Models;
using SGRH.Web.Services;

namespace SGRH.Web.Controllers
{
    public class PisosController : AdminController
    {
        private readonly IPisoService pisoService;
        private readonly ICatalogoService catalogo;

        public PisosController(IPisoService pisoService, ICatalogoService catalogo)
        {
            this.pisoService = pisoService;
            this.catalogo = catalogo;
        }

        public async Task<IActionResult> Index(PisoFiltro filtro)
        {
            var resultado = await pisoService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var pisos = resultado.Data ?? [];
            var items = filtro.AplicarEstado(pisos, p => p.Estatus)
                              .Where(p => filtro.HotelId is null || p.HotelId == filtro.HotelId)
                              .Where(p => filtro.BloqueId is null || p.BloqueId == filtro.BloqueId)
                              .Where(p => filtro.Coincide(p.Hotel, p.Bloque, p.Numero.ToString()))
                              .OrderByDescending(p => p.PisoId)
                              .ToList();

            ViewBag.Hoteles = await catalogo.HotelesAsync(filtro.HotelId);
            ViewBag.Bloques = await catalogo.BloquesAsync(filtro.HotelId, filtro.BloqueId);
            return View(new ListadoViewModel<PisoModel, PisoFiltro> { Items = items, Filtro = filtro, Total = pisos.Count });
        }

        public async Task<IActionResult> Create(int? hotelId, int? bloqueId)
        {
            var modelo = new PisoViewModel { HotelId = hotelId ?? 0, BloqueId = bloqueId };
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Create(PisoViewModel modelo) =>
            GuardarAsync(modelo, () => pisoService.AddAsync(modelo.ToAdd(UsuarioActualId)), CargarListasAsync);

        public async Task<IActionResult> Edit(int id)
        {
            var piso = await pisoService.GetByIdAsync(id);
            if (!piso.Success)
                return VolverAlListado(piso.Message);

            var modelo = PisoViewModel.From(piso.Data!);
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Edit(int id, PisoViewModel modelo)
        {
            modelo.Id = id;
            return GuardarAsync(modelo, () => pisoService.UpdateAsync(modelo.ToUpdate(UsuarioActualId)), CargarListasAsync);
        }

        [HttpPost]
        public Task<IActionResult> Inactivar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => pisoService.RemoveAsync(new PisoRemove { Id = id, UsuarioModificacion = UsuarioActualId }), volverA);

        [HttpPost]
        public Task<IActionResult> Activar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => pisoService.ActivarAsync(id, UsuarioActualId), volverA);

        public async Task<IActionResult> PorHotel(int hotelId)
        {
            var lista = await catalogo.PisosAsync(hotelId);
            return Json(lista.Select(i => new { valor = i.Value, texto = i.Text }));
        }

        private async Task CargarListasAsync(PisoViewModel modelo)
        {
            ViewBag.Hoteles = await catalogo.HotelesAsync(modelo.HotelId);
            ViewBag.Bloques = await catalogo.BloquesAsync(modelo.HotelId, modelo.BloqueId);
        }
    }
}
