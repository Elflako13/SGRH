using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Usuario;
using SGRH.Web.Models;
using System.Security.Claims;

namespace SGRH.Web.Controllers
{
    public class CuentaController : BaseController
    {
        private readonly IUsuarioService usuarioService;

        public CuentaController(IUsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        public IActionResult Ingresar(string? returnUrl)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await usuarioService.LoginAsync(modelo.Usuario!, modelo.Clave!);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, Limpiar(resultado.Message));
                return View(modelo);
            }

            await IniciarSesionAsync(resultado.Data!);
            Exito(resultado.Message);
            return Redirigir(modelo.ReturnUrl, resultado.Data!.Rol);
        }

        public IActionResult Registro(string? returnUrl) => View(new RegistroViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await usuarioService.RegistrarClienteAsync(modelo.ToRegistro());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, Limpiar(resultado.Message));
                return View(modelo);
            }

            await IniciarSesionAsync(resultado.Data!);
            Exito(resultado.Message);
            return Redirigir(modelo.ReturnUrl, Rol.Cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccesoDenegado() => View();

        private async Task IniciarSesionAsync(UsuarioModel usuario)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new(ClaimTypes.Name, usuario.NombreCompleto),
                new(ClaimTypes.Role, usuario.Rol.ToString())
            };

            if (usuario.Correo is not null)
                claims.Add(new(ClaimTypes.Email, usuario.Correo));

            if (usuario.HuespedId is not null)
                claims.Add(new("HuespedId", usuario.HuespedId.Value.ToString()));

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
        }

        private IActionResult Redirigir(string? returnUrl, Rol rol)
        {
            bool volverAlInicio = string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || !Url.IsLocalUrl(returnUrl);

            if (!volverAlInicio)
                return LocalRedirect(returnUrl!);

            return rol == Rol.Administrador
                ? RedirectToAction("Index", "Panel")
                : RedirectToAction("Index", "Home");
        }
    }
}
