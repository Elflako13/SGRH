using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.Web.Models;
using SGRH.Web.Services;

namespace SGRH.Web.Controllers
{
    /// <summary>Gestión de habitaciones: registrar (CU-003), actualizar (CU-004) y cambiar estado (CU-005).</summary>
    public class HabitacionesController : BaseController
    {
        private readonly IHabitacionService habitacionService;
        private readonly ICatalogoService catalogo;

        public HabitacionesController(IHabitacionService habitacionService, ICatalogoService catalogo)
        {
            this.habitacionService = habitacionService;
            this.catalogo = catalogo;
        }

        // GET: Habitaciones
        public async Task<IActionResult> Index(HabitacionFiltro filtro)
        {
            var resultado = await habitacionService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var habitaciones = resultado.Data ?? [];
            var items = filtro.AplicarEstado(habitaciones, h => h.Estatus)
                              .Where(h => filtro.HotelId is null || h.HotelId == filtro.HotelId)
                              .Where(h => filtro.PisoId is null || h.PisoId == filtro.PisoId)
                              .Where(h => filtro.TipoHabitacionId is null || h.TipoHabitacionId == filtro.TipoHabitacionId)
                              .Where(h => filtro.EstadoOperativo is null || h.Estado == filtro.EstadoOperativo)
                              .Where(h => filtro.Coincide(h.Numero, h.Hotel, h.Tipo, h.Bloque))
                              .ToList();

            ViewBag.Hoteles = await catalogo.HotelesAsync(filtro.HotelId);
            ViewBag.Pisos = await catalogo.PisosAsync(filtro.HotelId, filtro.PisoId);
            ViewBag.Tipos = await catalogo.TiposAsync(filtro.TipoHabitacionId);
            ViewBag.Estados = catalogo.EstadosHabitacion(filtro.EstadoOperativo);
            return View(new ListadoViewModel<HabitacionModel, HabitacionFiltro> { Items = items, Filtro = filtro, Total = habitaciones.Count });
        }

        // GET: Habitaciones/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var habitacion = await habitacionService.GetByIdAsync(id);
            if (!habitacion.Success)
                return VolverAlListado(habitacion.Message);

            ViewBag.Estados = catalogo.EstadosHabitacion(habitacion.Data!.Estado);
            return View(habitacion.Data);
        }

        // GET: Habitaciones/Create?hotelId=1
        public async Task<IActionResult> Create(int? hotelId)
        {
            var modelo = new HabitacionViewModel { HotelId = hotelId ?? 0 };
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Create(HabitacionViewModel modelo) =>
            GuardarAsync(modelo, () => habitacionService.AddAsync(modelo.ToAdd(UsuarioActualId)), CargarListasAsync);

        // GET: Habitaciones/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var habitacion = await habitacionService.GetByIdAsync(id);
            if (!habitacion.Success)
                return VolverAlListado(habitacion.Message);

            var modelo = HabitacionViewModel.From(habitacion.Data!);
            await CargarListasAsync(modelo);
            return View("Form", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Edit(int id, HabitacionViewModel modelo)
        {
            modelo.Id = id;
            return GuardarAsync(modelo, () => habitacionService.UpdateAsync(modelo.ToUpdate(UsuarioActualId)), CargarListasAsync);
        }

        // POST: Habitaciones/CambiarEstado/5  (CU-005, RN-007, RN-018)
        [HttpPost]
        public Task<IActionResult> CambiarEstado(int id, EstadoHabitacion estado, string? volverA) =>
            EjecutarYVolverAsync(() => habitacionService.CambiarEstadoAsync(id, estado, UsuarioActualId), volverA);

        [HttpPost]
        public Task<IActionResult> Inactivar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => habitacionService.RemoveAsync(new HabitacionRemove { Id = id, UsuarioModificacion = UsuarioActualId }), volverA);

        [HttpPost]
        public Task<IActionResult> Activar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => habitacionService.CambiarEstadoAsync(id, EstadoHabitacion.Disponible, UsuarioActualId), volverA);

        private async Task CargarListasAsync(HabitacionViewModel modelo)
        {
            ViewBag.Hoteles = await catalogo.HotelesAsync(modelo.HotelId);
            ViewBag.Pisos = await catalogo.PisosAsync(modelo.HotelId, modelo.PisoId);
            ViewBag.Tipos = await catalogo.TiposActivosAsync();
        }
    }
}
