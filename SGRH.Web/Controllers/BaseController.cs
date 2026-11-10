using Microsoft.AspNetCore.Mvc;
using SGRH.DAL.Models.Result;

namespace SGRH.Web.Controllers
{
    /// <summary>
    /// Base de los controladores de mantenimiento. Agrupa lo que todos hacen igual (DRY):
    /// mostrar el mensaje de resultado, guardar un formulario y activar o inactivar un registro.
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Usuario que registra las operaciones. Mientras no exista el inicio de sesión
        /// (próxima fase), se usa el administrador creado por el script (Id = 1).
        /// </summary>
        protected const int UsuarioActualId = 1;

        protected void Exito(string mensaje) => TempData["Exito"] = mensaje;

        protected void Error(string mensaje) => TempData["Error"] = mensaje;

        protected void Notificar<T>(OperationResult<T> resultado)
        {
            if (resultado.Success)
                Exito(resultado.Message);
            else
                Error(resultado.Message);
        }

        /// <summary>Se usa cuando no se encuentra el registro pedido: vuelve al listado con el mensaje.</summary>
        protected IActionResult VolverAlListado(string mensaje)
        {
            Error(mensaje);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Flujo común de Crear y Editar: valida el formulario, llama al servicio y, si algo falla,
        /// vuelve a mostrar el formulario con el mensaje de la regla de negocio que no se cumplió.
        /// </summary>
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
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, resultado.Message);
            }

            await cargarListas(modelo);
            return View("Form", modelo);
        }

        /// <summary>Activar, inactivar o cambiar estado: ejecuta la acción y regresa a la página anterior.</summary>
        protected async Task<IActionResult> EjecutarYVolverAsync(Func<Task<OperationResult<bool>>> accion, string? volverA)
        {
            Notificar(await accion());

            return !string.IsNullOrEmpty(volverA) && Url.IsLocalUrl(volverA)
                ? LocalRedirect(volverA)
                : RedirectToAction("Index");
        }
    }
}
