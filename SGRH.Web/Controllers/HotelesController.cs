using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.Web.Models;
using SGRH.Web.Services;

namespace SGRH.Web.Controllers
{
    /// <summary>Gestión de hoteles (CU-001) y consulta de la estructura del hotel (CU-006).</summary>
    public class HotelesController : BaseController
    {
        private readonly IHotelService hotelService;
        private readonly IBloqueService bloqueService;
        private readonly IPisoService pisoService;
        private readonly IHabitacionService habitacionService;
        private readonly ICatalogoService catalogo;

        public HotelesController(IHotelService hotelService, IBloqueService bloqueService, IPisoService pisoService,
                                 IHabitacionService habitacionService, ICatalogoService catalogo)
        {
            this.hotelService = hotelService;
            this.bloqueService = bloqueService;
            this.pisoService = pisoService;
            this.habitacionService = habitacionService;
            this.catalogo = catalogo;
        }

        // GET: Hoteles
        public async Task<IActionResult> Index(HotelFiltro filtro)
        {
            var resultado = await hotelService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var hoteles = resultado.Data ?? [];
            var items = filtro.AplicarEstado(hoteles, h => h.Estatus)
                              .Where(h => filtro.Coincide(h.Nombre, h.Ciudad, h.Direccion))
                              .Where(h => string.IsNullOrEmpty(filtro.Ciudad) || h.Ciudad == filtro.Ciudad)
                              .Where(h => filtro.Estrellas is null || h.Estrellas == filtro.Estrellas)
                              .ToList();

            ViewBag.Ciudades = await catalogo.CiudadesAsync();
            return View(new ListadoViewModel<HotelModel, HotelFiltro> { Items = items, Filtro = filtro, Total = hoteles.Count });
        }

        // GET: Hoteles/Details/5  -> estructura del hotel: bloques, pisos y habitaciones con su estado (RF-008)
        public async Task<IActionResult> Details(int id)
        {
            var hotel = await hotelService.GetByIdAsync(id);
            if (!hotel.Success)
                return VolverAlListado(hotel.Message);

            var bloques = await bloqueService.GetByHotelAsync(id);
            var pisos = await pisoService.GetByHotelAsync(id);
            var habitaciones = await habitacionService.GetByHotelAsync(id);

            return View(new HotelDetalleViewModel
            {
                Hotel = hotel.Data!,
                Bloques = bloques.Data ?? [],
                Pisos = pisos.Data ?? [],
                Habitaciones = (habitaciones.Data ?? []).Where(h => h.Estatus).ToList()
            });
        }

        // GET: Hoteles/Create
        public IActionResult Create() => View("Form", new HotelViewModel());

        // POST: Hoteles/Create
        [HttpPost]
        public Task<IActionResult> Create(HotelViewModel modelo) =>
            GuardarAsync(modelo, () => hotelService.AddAsync(modelo.ToAdd(UsuarioActualId)), _ => Task.CompletedTask);

        // GET: Hoteles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var hotel = await hotelService.GetByIdAsync(id);
            return hotel.Success ? View("Form", HotelViewModel.From(hotel.Data!)) : VolverAlListado(hotel.Message);
        }

        // POST: Hoteles/Edit/5
        [HttpPost]
        public Task<IActionResult> Edit(int id, HotelViewModel modelo)
        {
            modelo.Id = id;
            return GuardarAsync(modelo, () => hotelService.UpdateAsync(modelo.ToUpdate(UsuarioActualId)), _ => Task.CompletedTask);
        }

        // POST: Hoteles/Inactivar/5
        [HttpPost]
        public Task<IActionResult> Inactivar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => hotelService.RemoveAsync(new HotelRemove { Id = id, UsuarioModificacion = UsuarioActualId }), volverA);

        // POST: Hoteles/Activar/5
        [HttpPost]
        public Task<IActionResult> Activar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => hotelService.ActivarAsync(id, UsuarioActualId), volverA);
    }
}
