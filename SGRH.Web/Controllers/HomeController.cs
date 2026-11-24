using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.Web.Models;
using SGRH.Web.Services;
using System.Diagnostics;

namespace SGRH.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHotelService hotelService;
        private readonly IHabitacionService habitacionService;
        private readonly ITipoHabitacionService tipoService;
        private readonly ICatalogoService catalogo;
        private readonly IDisponibilidadService disponibilidad;
        private readonly ILogger<HomeController> logger;

        public HomeController(IHotelService hotelService, IHabitacionService habitacionService,
                              ITipoHabitacionService tipoService, ICatalogoService catalogo,
                              IDisponibilidadService disponibilidad, ILogger<HomeController> logger)
        {
            this.hotelService = hotelService;
            this.habitacionService = habitacionService;
            this.tipoService = tipoService;
            this.catalogo = catalogo;
            this.disponibilidad = disponibilidad;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var hoteles = (await hotelService.GetAllAsync()).Data ?? [];
            var tipos = await catalogo.TiposActivosAsync();

            return View(new SitioInicioViewModel
            {
                Busqueda = new BusquedaViewModel { Ciudades = await catalogo.CiudadesAsync() },
                Hoteles = hoteles.Where(h => h.Estatus).OrderByDescending(h => h.Estrellas).ToList(),
                Tipos = tipos.OrderBy(t => t.TarifaBase).ToList()
            });
        }

        public async Task<IActionResult> Buscar(BusquedaViewModel busqueda)
        {
            busqueda.Ciudades = await catalogo.CiudadesAsync();
            var modelo = new BusquedaResultadoViewModel { Busqueda = busqueda };

            var (resultados, error) = await disponibilidad.PorHotelAsync(busqueda.Entrada, busqueda.Salida, busqueda.Huespedes, busqueda.Ciudad);
            modelo.Resultados = resultados;
            modelo.Error = error;
            return View(modelo);
        }

        public async Task<IActionResult> Hotel(int id, BusquedaViewModel busqueda, bool verDisponibilidad = false)
        {
            var hotel = await hotelService.GetByIdAsync(id);
            if (!hotel.Success || !hotel.Data!.Estatus)
            {
                Error("El hotel no está disponible.");
                return RedirectToAction(nameof(Index));
            }

            var habitaciones = ((await habitacionService.GetByHotelAsync(id)).Data ?? []).Where(h => h.Estatus).ToList();
            var tipos = await catalogo.TiposActivosAsync();

            var modelo = new HotelPublicoViewModel
            {
                Hotel = hotel.Data,
                Busqueda = busqueda,
                Tipos = habitaciones.GroupBy(h => h.Tipo)
                                    .Select(g => new TipoOferta
                                    {
                                        Nombre = g.Key,
                                        Descripcion = tipos.FirstOrDefault(t => t.Nombre == g.Key)?.Descripcion,
                                        Desde = g.Min(h => h.TarifaPorNoche),
                                        CapacidadMaxima = g.Max(h => h.Capacidad)
                                    })
                                    .OrderBy(t => t.Desde)
                                    .ToList()
            };

            if (verDisponibilidad)
            {
                var disponibles = await habitacionService.GetDisponiblesAsync(id, busqueda.Entrada, busqueda.Salida, busqueda.Huespedes);
                if (disponibles.Success)
                    modelo.Disponibles = disponibles.Data;
                else
                    modelo.Error = Limpiar(disponibles.Message);
            }

            return View(modelo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (error is not null)
                logger.LogError(error.Error, "Error no controlado en {Ruta}.", error.Path);

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
