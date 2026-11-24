using Microsoft.AspNetCore.Mvc;
using SGRH.DAL.Models.Result;
using SGRH.Web.Models;
using System.Security.Claims;

namespace SGRH.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected int UsuarioActualId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 1;

        protected static string Limpiar(string mensaje) => Formato.Limpiar(mensaje);

        protected void Exito(string mensaje) => TempData["Exito"] = Limpiar(mensaje);

        protected void Error(string mensaje) => TempData["Error"] = Limpiar(mensaje);

        protected void Notificar<T>(OperationResult<T> resultado)
        {
            if (resultado.Success)
                Exito(resultado.Message);
            else
                Error(resultado.Message);
        }

        protected IActionResult VolverAlListado(string mensaje)
        {
            Error(mensaje);
            return RedirectToAction("Index");
        }

        protected async Task<IActionResult> GuardarAsync<TViewModel>(TViewModel modelo,
                                                                     Func<Task<OperationResult<bool>>> guardar,
                                                                     Func<TViewModel, Task> cargarListas)
        {
            if (ModelState.IsValid)
            {
                var resultado = await guardar();
                if (resultado.Success)
                {
                    Exito(resultado.Message);
                    TempData["Resaltar"] = RouteData.Values["id"]?.ToString() ?? "nuevo";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, Limpiar(resultado.Message));
            }

            await cargarListas(modelo);
            return View("Form", modelo);
        }

        protected async Task<IActionResult> EjecutarYVolverAsync(Func<Task<OperationResult<bool>>> accion, string? volverA)
        {
            Notificar(await accion());

            return !string.IsNullOrEmpty(volverA) && Url.IsLocalUrl(volverA)
                ? LocalRedirect(volverA)
                : RedirectToAction("Index");
        }
    }
}
