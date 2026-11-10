using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.Web.Models;

namespace SGRH.Web.Controllers
{
    /// <summary>Mantenimiento de tipos de habitación.</summary>
    public class TiposHabitacionController : BaseController
    {
        private readonly ITipoHabitacionService tipoService;

        public TiposHabitacionController(ITipoHabitacionService tipoService)
        {
            this.tipoService = tipoService;
        }

        // GET: TiposHabitacion
        public async Task<IActionResult> Index(FiltroBase filtro)
        {
            var resultado = await tipoService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var tipos = resultado.Data ?? [];
            var items = filtro.AplicarEstado(tipos, t => t.Estatus)
                              .Where(t => filtro.Coincide(t.Nombre, t.Descripcion))
                              .ToList();

            return View(new ListadoViewModel<TipoHabitacionModel, FiltroBase> { Items = items, Filtro = filtro, Total = tipos.Count });
        }

        public IActionResult Create() => View("Form", new TipoHabitacionViewModel());

        [HttpPost]
        public Task<IActionResult> Create(TipoHabitacionViewModel modelo) =>
            GuardarAsync(modelo, () => tipoService.AddAsync(modelo.ToAdd(UsuarioActualId)), _ => Task.CompletedTask);

        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await tipoService.GetByIdAsync(id);
            return tipo.Success ? View("Form", TipoHabitacionViewModel.From(tipo.Data!)) : VolverAlListado(tipo.Message);
        }

        [HttpPost]
        public Task<IActionResult> Edit(int id, TipoHabitacionViewModel modelo)
        {
            modelo.Id = id;
            return GuardarAsync(modelo, () => tipoService.UpdateAsync(modelo.ToUpdate(UsuarioActualId)), _ => Task.CompletedTask);
        }

        [HttpPost]
        public Task<IActionResult> Inactivar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => tipoService.RemoveAsync(new TipoHabitacionRemove { Id = id, UsuarioModificacion = UsuarioActualId }), volverA);

        [HttpPost]
        public Task<IActionResult> Activar(int id, string? volverA) =>
            EjecutarYVolverAsync(() => tipoService.ActivarAsync(id, UsuarioActualId), volverA);
    }
}
