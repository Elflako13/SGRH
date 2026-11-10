using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.Web.Models;
using System.Diagnostics;

namespace SGRH.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHotelService hotelService;
        private readonly IBloqueService bloqueService;
        private readonly IPisoService pisoService;
        private readonly IHabitacionService habitacionService;
        private readonly ITipoHabitacionService tipoService;
        private readonly ILogger<HomeController> logger;

        public HomeController(IHotelService hotelService, IBloqueService bloqueService, IPisoService pisoService,
                              IHabitacionService habitacionService, ITipoHabitacionService tipoService, ILogger<HomeController> logger)
        {
            this.hotelService = hotelService;
            this.bloqueService = bloqueService;
            this.pisoService = pisoService;
            this.habitacionService = habitacionService;
            this.tipoService = tipoService;
            this.logger = logger;
        }

        // GET: / -> panel con el resumen de la estructura hotelera
        public async Task<IActionResult> Index()
        {
            var hoteles = (await hotelService.GetAllAsync()).Data ?? [];
            var bloques = (await bloqueService.GetAllAsync()).Data ?? [];
            var pisos = (await pisoService.GetAllAsync()).Data ?? [];
            var habitaciones = ((await habitacionService.GetAllAsync()).Data ?? []).Where(h => h.Estatus).ToList();
            var tipos = (await tipoService.GetAllAsync()).Data ?? [];

            return View(new InicioViewModel
            {
                Hoteles = hoteles.Count(h => h.Estatus),
                Bloques = bloques.Count(b => b.Estatus),
                Pisos = pisos.Count(p => p.Estatus),
                Habitaciones = habitaciones.Count,
                TiposHabitacion = tipos.Count(t => t.Estatus),
                HabitacionesPorEstado = habitaciones.GroupBy(h => h.Estado)
                                                    .OrderBy(g => g.Key)
                                                    .ToDictionary(g => Formato.Estado(g.Key), g => g.Count()),
                HotelesDestacados = hoteles.Where(h => h.Estatus).OrderByDescending(h => h.Estrellas).Take(3).ToList(),
                HabitacionesNoDisponibles = habitaciones.Where(h => h.Estado != EstadoHabitacion.Disponible).Take(6).ToList()
            });
        }

        // Página de error: el detalle técnico queda en el log; al usuario se le muestra un mensaje claro.
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
