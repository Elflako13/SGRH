using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Bloque;
using SGRH.Web.Models;
using SGRH.Web.Services;

namespace SGRH.Web.Controllers
{
    /// <summary>Gestión de bloques (torres o edificios) de cada hotel.</summary>
    public class BloquesController : BaseController
    {
        private readonly IBloqueService bloqueService;
        private readonly ICatalogoService catalogo;

        public BloquesController(IBloqueService bloqueService, ICatalogoService catalogo)
        {
            this.bloqueService = bloqueService;
            this.catalogo = catalogo;
        }

        // GET: Bloques
        public async Task<IActionResult> Index(HotelHijoFiltro filtro)
        {
            var resultado = await bloqueService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var bloques = resultado.Data ?? [];
            var items = filtro.AplicarEstado(bloques, b => b.Estatus)
                              .Where(b => filtro.HotelId is null || b.HotelId == filtro.HotelId)
                              .Where(b => filtro.Coincide(b.Nombre, b.Descripcion, b.Hotel))
                              .ToList();

            ViewBag.Hoteles = await catalogo.HotelesAsync(filtro.HotelId);
            return View(new ListadoViewModel<BloqueModel, HotelHijoFiltro> { Items = items, Filtro = filtro, Total = bloques.Count });
        }

        // GET: Bloques/Create?hotelId=1
        public async Task<IActionResult> Create(int? hotelId)
        {
            var modelo = new BloqueViewModel { HotelId = hotelId ?? 0 };
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Create(BloqueViewModel modelo) =>
            GuardarAsync(modelo, () => bloqueService.AddAsync(modelo.ToAdd(UsuarioActualId)), CargarListasAsync);

        // GET: Bloques/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var bloque = await bloqueService.GetByIdAsync(id);
            if (!bloque.Success)
                return VolverAlListado(bloque.Message);

            var modelo = BloqueViewModel.From(bloque.Data!);
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Edit(int id, BloqueViewModel modelo)
        {
            modelo.Id = id;
            return GuardarAsync(modelo, () => bloqueService.UpdateAsync(modelo.ToUpdate(UsuarioActualId)), CargarListasAsync);
        }

        [HttpPost]
        public Task<IActionResult> Inactivar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => bloqueService.RemoveAsync(new BloqueRemove { Id = id, UsuarioModificacion = UsuarioActualId }), volverA);

        [HttpPost]
        public Task<IActionResult> Activar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => bloqueService.ActivarAsync(id, UsuarioActualId), volverA);

        // GET: Bloques/PorHotel?hotelId=1  -> lo usa el formulario de pisos para llenar la lista de bloques
        public async Task<IActionResult> PorHotel(int hotelId)
        {
            var lista = await catalogo.BloquesAsync(hotelId);
            return Json(lista.Select(i => new { valor = i.Value, texto = i.Text }));
        }

        private async Task CargarListasAsync(BloqueViewModel modelo) =>
            ViewBag.Hoteles = await catalogo.HotelesAsync(modelo.HotelId);
    }
}
