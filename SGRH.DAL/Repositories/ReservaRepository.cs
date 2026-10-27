using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Reserva;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly SGRHDbContext context;

        public ReservaRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<ReservaModel>>> GetAllAsync()
        {
            try
            {
                var reservas = await Consulta().OrderByDescending(r => r.Fechas.Entrada).ToListAsync();
                return Success(reservas.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<ReservaModel>>("Ocurrió un error obteniendo las reservas.");
            }
        }

        public async Task<OperationResult<ReservaModel>> GetByIdAsync(int id)
        {
            var reserva = await Consulta().FirstOrDefaultAsync(r => r.Id == id);
            if (reserva is null)
                return Failure<ReservaModel>("La reserva no existe.");

            return Success(ToModel(reserva));
        }

        /// <summary>CU-014: historial de reservas de un huésped.</summary>
        public async Task<OperationResult<List<ReservaModel>>> GetByHuespedAsync(int huespedId)
        {
            try
            {
                var reservas = await Consulta().Where(r => r.HuespedId == huespedId)
                                               .OrderByDescending(r => r.Fechas.Entrada)
                                               .ToListAsync();
                return Success(reservas.Select(ToModel).ToList());
            }
            catch (Exception)
            {
                return Failure<List<ReservaModel>>("Ocurrió un error obteniendo las reservas del huésped.");
            }
        }

        public async Task<OperationResult<ReservaModel>> AddAsync(ReservaAdd reserva)
        {
            try
            {
                if (reserva is null)
                    return Failure<ReservaModel>("El objeto ReservaAdd es requerido.");

                // RN-017: la reserva debe tener un huésped registrado.
                var huesped = await context.Huespedes.FindAsync(reserva.HuespedId);
                if (huesped is null || !huesped.Estatus)
                    return Failure<ReservaModel>("El huésped no está registrado (RN-017).");

                var habitacion = await context.Habitaciones.FindAsync(reserva.HabitacionId);
                if (habitacion is null)
                    return Failure<ReservaModel>("La habitación no existe.");

                var fechas = new RangoFechas(reserva.FechaEntrada, reserva.FechaSalida); // RN-008

                var validation = await ValidateFechasAsync(reserva.HabitacionId, fechas, null);
                if (validation is not null)
                    return Failure<ReservaModel>(validation);

                // El constructor valida RN-007 y RN-011 y calcula el precio (RN-013).
                var entity = new Reserva(huesped, habitacion, fechas, reserva.CantidadHuespedes)
                {
                    UsuarioCreacion = reserva.UsuarioCreacion
                };

                await context.Reservas.AddAsync(entity);
                await context.SaveChangesAsync();
                return Success(ToModel(entity), $"Reserva {entity.Codigo} creada correctamente.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<ReservaModel>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<ReservaModel>("Ocurrió un error guardando la reserva.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva)
        {
            try
            {
                if (reserva is null)
                    return Failure<bool>("El objeto ReservaUpdate es requerido.");

                var entity = await context.Reservas.FindAsync(reserva.Id);
                if (entity is null)
                    return Failure<bool>("La reserva no existe.");

                var habitacion = await context.Habitaciones.FindAsync(reserva.HabitacionId);
                if (habitacion is null)
                    return Failure<bool>("La habitación no existe.");

                var fechas = new RangoFechas(reserva.FechaEntrada, reserva.FechaSalida);

                var validation = await ValidateFechasAsync(reserva.HabitacionId, fechas, reserva.Id);
                if (validation is not null)
                    return Failure<bool>(validation);

                // Valida RN-016 (solo reservas activas), RN-007, RN-011 y recalcula el precio.
                entity.Modificar(habitacion, fechas, reserva.CantidadHuespedes);
                entity.UsuarioModificacion = reserva.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Reserva modificada correctamente.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Failure<bool>("Otro usuario modificó esta reserva. Vuelva a cargarla e intente de nuevo.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error modificando la reserva.");
            }
        }

        public async Task<OperationResult<bool>> ConfirmarAsync(int reservaId, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.Reservas.FindAsync(reservaId);
                if (entity is null)
                    return Failure<bool>("La reserva no existe.");

                entity.Confirmar(); // RN-016
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, $"Reserva {entity.Codigo} confirmada.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error confirmando la reserva.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(ReservaRemove reserva)
        {
            try
            {
                if (reserva is null)
                    return Failure<bool>("El objeto ReservaRemove es requerido.");

                var entity = await context.Reservas.FindAsync(reserva.Id);
                if (entity is null)
                    return Failure<bool>("La reserva no existe.");

                entity.Cancelar(reserva.Motivo ?? string.Empty); // RN-012 y RN-016
                entity.UsuarioModificacion = reserva.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, $"Reserva {entity.Codigo} cancelada.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error cancelando la reserva.");
            }
        }

        /// <summary>RN-009 y RN-010.</summary>
        private async Task<string?> ValidateFechasAsync(int habitacionId, RangoFechas fechas, int? reservaId)
        {
            if (fechas.EmpiezaAntesDe(DateOnly.FromDateTime(DateTime.Today)))
                return "No se aceptan reservas con fecha de entrada anterior a hoy (RN-009).";

            var entrada = fechas.Entrada;
            var salida = fechas.Salida;

            bool seSolapa = await context.Reservas.AnyAsync(r => r.HabitacionId == habitacionId
                                                              && r.Id != reservaId
                                                              && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada) // RN-012
                                                              && r.Fechas.Entrada < salida && entrada < r.Fechas.Salida);                      // RN-010
            if (seSolapa)
                return $"La habitación ya está reservada entre {fechas} (RN-010).";

            return null;
        }

        private IQueryable<Reserva> Consulta()
        {
            return context.Reservas.Include(r => r.Huesped).Include(r => r.Habitacion);
        }

        private static ReservaModel ToModel(Reserva r) => new()
        {
            ReservaId = r.Id,
            Codigo = r.Codigo,
            HuespedId = r.HuespedId,
            Huesped = r.Huesped?.NombreCompleto ?? string.Empty,
            HabitacionId = r.HabitacionId,
            Habitacion = r.Habitacion?.Numero ?? string.Empty,
            FechaEntrada = r.Fechas.Entrada,
            FechaSalida = r.Fechas.Salida,
            CantidadHuespedes = r.CantidadHuespedes,
            PrecioTotal = r.PrecioTotal,
            Estado = r.Estado,
            MotivoCancelacion = r.MotivoCancelacion
        };
    }
}
