using Microsoft.AspNetCore.Mvc;
using SGRH.BL.Services;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.Web.Models;
using SGRH.Web.Services;

namespace SGRH.Web.Controllers
{
    public class ReservasController : AdminController
    {
        private readonly IReservaService reservaService;
        private readonly IHuespedService huespedService;
        private readonly IHabitacionService habitacionService;
        private readonly IDisponibilidadService disponibilidad;

        public ReservasController(IReservaService reservaService, IHuespedService huespedService,
                                  IHabitacionService habitacionService, IDisponibilidadService disponibilidad)
        {
            this.reservaService = reservaService;
            this.huespedService = huespedService;
            this.habitacionService = habitacionService;
            this.disponibilidad = disponibilidad;
        }

        public async Task<IActionResult> Index(EstadoReserva? estado, string? buscar)
        {
            var resultado = await reservaService.GetAllAsync();
            if (!resultado.Success)
                Error(resultado.Message);

            var reservas = (resultado.Data ?? [])
                .Where(r => estado is null || r.Estado == estado)
                .Where(r => string.IsNullOrWhiteSpace(buscar)
                            || r.Codigo.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                            || r.Huesped.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.ReservaId)
                .ToList();

            ViewBag.Estado = estado;
            ViewBag.Buscar = buscar;
            ViewBag.Total = resultado.Data?.Count ?? 0;
            return View(reservas);
        }

        public async Task<IActionResult> Create(ReservaViewModel modelo)
        {
            ModelState.Clear();
            await CargarDisponiblesAsync(modelo);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ReservaViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var huesped = await huespedService.GetByDocumentoAsync(modelo.Documento!.Trim());
                if (!huesped.Success)
                {
                    var registro = await huespedService.AddAsync(modelo.ToHuesped(UsuarioActualId));
                    huesped = registro.Success
                        ? await huespedService.GetByDocumentoAsync(modelo.Documento.Trim())
                        : new() { Success = false, Message = registro.Message };
                }

                if (huesped.Success)
                {
                    var reserva = await reservaService.AddAsync(modelo.ToReserva(huesped.Data!.HuespedId, UsuarioActualId));
                    if (reserva.Success)
                    {
                        Exito($"Reserva {reserva.Data!.Codigo} creada para {reserva.Data.Huesped}. Total: {Formato.Moneda(reserva.Data.PrecioTotal)}. Queda pendiente de confirmar.");
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, Limpiar(reserva.Message));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Limpiar(huesped.Message));
                }
            }

            await CargarDisponiblesAsync(modelo);
            return View("Create", modelo);
        }

        [HttpPost]
        public Task<IActionResult> Confirmar(int id) =>
            EjecutarYVolverAsync(() => reservaService.ConfirmarAsync(id, UsuarioActualId), null);

        [HttpPost]
        public Task<IActionResult> Cancelar(int id, string? motivo) =>
            EjecutarYVolverAsync(() => reservaService.RemoveAsync(new ReservaRemove
            {
                Id = id,
                Motivo = string.IsNullOrWhiteSpace(motivo) ? "Cancelada a solicitud del huésped." : motivo,
                UsuarioModificacion = UsuarioActualId
            }), null);

        private async Task CargarDisponiblesAsync(ReservaViewModel modelo)
        {
            ViewBag.Disponibles = null;

            var (hoteles, error) = await disponibilidad.PorHotelAsync(modelo.FechaEntrada, modelo.FechaSalida, modelo.CantidadHuespedes);
            ViewBag.HotelesDisponibles = hoteles;
            if (error is not null)
            {
                ViewBag.ErrorDisponibilidad = error;
                return;
            }

            if (modelo.HotelId <= 0)
                return;

            var disponibles = await habitacionService.GetDisponiblesAsync(modelo.HotelId, modelo.FechaEntrada, modelo.FechaSalida, modelo.CantidadHuespedes);
            if (disponibles.Success)
            {
                ViewBag.Disponibles = disponibles.Data;
                ViewBag.MensajeDisponibilidad = disponibles.Message;
                ViewBag.HotelSeleccionado = hoteles.FirstOrDefault(h => h.Hotel.HotelId == modelo.HotelId)?.Hotel.Nombre;
            }
            else
            {
                ViewBag.ErrorDisponibilidad = Limpiar(disponibles.Message);
            }
        }
    }
}
