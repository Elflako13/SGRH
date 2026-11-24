using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.Web.Models;

namespace SGRH.Web.Controllers
{
    public class PanelController : AdminController
    {
        private readonly IHotelService hotelService;
        private readonly IBloqueService bloqueService;
        private readonly IPisoService pisoService;
        private readonly IHabitacionService habitacionService;
        private readonly ITipoHabitacionService tipoService;

        public PanelController(IHotelService hotelService, IBloqueService bloqueService, IPisoService pisoService,
                              IHabitacionService habitacionService, ITipoHabitacionService tipoService)
        {
            this.hotelService = hotelService;
            this.bloqueService = bloqueService;
            this.pisoService = pisoService;
            this.habitacionService = habitacionService;
            this.tipoService = tipoService;
        }

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
    }
}
