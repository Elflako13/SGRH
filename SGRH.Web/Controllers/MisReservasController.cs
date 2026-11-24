using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.Web.Models;
using System.Security.Claims;

namespace SGRH.Web.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class MisReservasController : BaseController
    {
        private readonly IReservaService reservaService;
        private readonly IHabitacionService habitacionService;

        public MisReservasController(IReservaService reservaService, IHabitacionService habitacionService)
        {
            this.reservaService = reservaService;
            this.habitacionService = habitacionService;
        }

        private int HuespedId => int.Parse(User.FindFirstValue("HuespedId") ?? "0");

        public async Task<IActionResult> Index()
        {
            var resultado = await reservaService.GetByHuespedAsync(HuespedId);
            if (!resultado.Success)
                Error(resultado.Message);

            return View((resultado.Data ?? []).OrderByDescending(r => r.ReservaId).ToList());
        }

        public async Task<IActionResult> Reservar(ReservarViewModel modelo)
        {
            var habitacion = await habitacionService.GetByIdAsync(modelo.HabitacionId);
            if (!habitacion.Success || !habitacion.Data!.Estatus)
            {
                Error("La habitación seleccionada no está disponible.");
                return RedirectToAction("Index", "Home");
            }

            modelo.Habitacion = habitacion.Data;
            return View(modelo);
        }

        [HttpPost]
        [ActionName("Reservar")]
        public async Task<IActionResult> ConfirmarReserva(ReservarViewModel modelo)
        {
            var resultado = await reservaService.AddAsync(new ReservaAdd
            {
                HuespedId = HuespedId,
                HabitacionId = modelo.HabitacionId,
                FechaEntrada = modelo.Entrada,
                FechaSalida = modelo.Salida,
                CantidadHuespedes = modelo.Huespedes,
                UsuarioCreacion = UsuarioActualId
            });

            if (resultado.Success)
            {
                Exito($"¡Listo! Su reserva {resultado.Data!.Codigo} fue registrada por {Formato.Moneda(resultado.Data.PrecioTotal)}. El hotel la confirmará en breve.");
                return RedirectToAction(nameof(Index));
            }

            Error(resultado.Message);
            var habitacion = await habitacionService.GetByIdAsync(modelo.HabitacionId);
            modelo.Habitacion = habitacion.Data;
            return View("Reservar", modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await reservaService.GetByIdAsync(id);
            if (!reserva.Success || reserva.Data!.HuespedId != HuespedId)
            {
                Error("No se encontró la reserva.");
                return RedirectToAction(nameof(Index));
            }

            Notificar(await reservaService.RemoveAsync(new ReservaRemove
            {
                Id = id,
                Motivo = "Cancelada por el huésped desde el sitio web.",
                UsuarioModificacion = UsuarioActualId
            }));
            return RedirectToAction(nameof(Index));
        }
    }
}
