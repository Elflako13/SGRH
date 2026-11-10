using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Base;
using SGRH.DAL.Models.Objects.Habitacion;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class HabitacionRepository : IHabitacionRepository
    {
        private readonly SGRHDbContext context;

        public HabitacionRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<HabitacionModel>>> GetAllAsync()
        {
            try
            {
                var habitaciones = await Consultar().OrderBy(h => h.Hotel).ThenBy(h => h.Numero).ToListAsync();
                return Success(habitaciones);
            }
            catch (Exception)
            {
                return Failure<List<HabitacionModel>>("Ocurrió un error obteniendo las habitaciones.");
            }
        }

        public async Task<OperationResult<List<HabitacionModel>>> GetByHotelAsync(int hotelId)
        {
            try
            {
                var habitaciones = await Consultar().Where(h => h.HotelId == hotelId)
                                                    .OrderBy(h => h.Numero)
                                                    .ToListAsync();
                return Success(habitaciones);
            }
            catch (Exception)
            {
                return Failure<List<HabitacionModel>>("Ocurrió un error obteniendo las habitaciones del hotel.");
            }
        }

        public async Task<OperationResult<HabitacionModel>> GetByIdAsync(int id)
        {
            var habitacion = await Consultar().FirstOrDefaultAsync(h => h.HabitacionId == id);
            if (habitacion is null)
                return Failure<HabitacionModel>("La habitación no existe.");

            return Success(habitacion);
        }

        /// <summary>CU-009: habitaciones libres para esas fechas (RN-007, RN-010, RN-011, RN-012, RN-013).</summary>
        public async Task<OperationResult<List<HabitacionDisponibleModel>>> GetDisponiblesAsync(int hotelId, DateOnly entrada, DateOnly salida, int cantidadHuespedes)
        {
            try
            {
                var fechas = new RangoFechas(entrada, salida); // RN-008

                var datos = await (from habitacion in context.Habitaciones
                                   join piso in context.Pisos on habitacion.PisoId equals piso.Id
                                   join hotel in context.Hoteles on habitacion.HotelId equals hotel.Id
                                   where habitacion.HotelId == hotelId
                                      && habitacion.Estatus
                                      && (habitacion.Estado == EstadoHabitacion.Disponible || habitacion.Estado == EstadoHabitacion.Limpieza) // RN-007
                                      && habitacion.Capacidad >= cantidadHuespedes                                                            // RN-011
                                      && !context.Reservas.Any(r => r.HabitacionId == habitacion.Id
                                                                 && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada) // RN-012
                                                                 && r.Fechas.Entrada < salida && entrada < r.Fechas.Salida)                      // RN-010
                                   orderby piso.Numero, habitacion.Numero
                                   select new HabitacionDisponibleModel
                                   {
                                       HabitacionId = habitacion.Id,
                                       Hotel = hotel.Nombre,
                                       Piso = piso.Numero,
                                       Numero = habitacion.Numero,
                                       Tipo = habitacion.TipoHabitacion!.Nombre,
                                       Capacidad = habitacion.Capacidad,
                                       TarifaPorNoche = habitacion.TarifaPorNoche
                                   }).ToListAsync();

                foreach (var d in datos)
                {
                    d.Noches = fechas.Noches;
                    d.CostoEstimado = d.TarifaPorNoche * fechas.Noches; // RN-013
                }

                return Success(datos);
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<List<HabitacionDisponibleModel>>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<List<HabitacionDisponibleModel>>("Ocurrió un error consultando la disponibilidad.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion)
        {
            try
            {
                if (habitacion is null)
                    return Failure<bool>("El objeto HabitacionAdd es requerido.");

                if (string.IsNullOrWhiteSpace(habitacion.Numero))
                    return Failure<bool>("El número de habitación es obligatorio.");

                // RN-004 y RN-006: debe ir en un piso existente y activo.
                var piso = await context.Pisos.FindAsync(habitacion.PisoId);
                if (piso is null)
                    return Failure<bool>("El piso indicado no existe.");

                if (!piso.Estatus)
                    return Failure<bool>($"El piso {piso.Numero} está inactivo y no puede recibir habitaciones (RN-006).");

                // RN-005
                string numero = habitacion.Numero.Trim();
                if (await context.Habitaciones.AnyAsync(h => h.HotelId == piso.HotelId && h.Numero == numero))
                    return Failure<bool>($"Ya existe una habitación con el número {numero} en este hotel (RN-005).");

                var tipoValidation = await ValidateTipoAsync(habitacion.TipoHabitacionId);
                if (tipoValidation is not null)
                    return Failure<bool>(tipoValidation);

                // El constructor valida capacidad y tarifa mayores que cero.
                var entity = new Habitacion(numero, habitacion.TipoHabitacionId, habitacion.Capacidad, habitacion.TarifaPorNoche)
                {
                    PisoId = piso.Id,
                    HotelId = piso.HotelId,
                    UsuarioCreacion = habitacion.UsuarioCreacion
                };

                await context.Habitaciones.AddAsync(entity);
                await context.SaveChangesAsync();
                return Success(true, "Habitación guardada correctamente.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error guardando la habitación.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion)
        {
            try
            {
                if (habitacion is null)
                    return Failure<bool>("El objeto HabitacionUpdate es requerido.");

                var entity = await context.Habitaciones.FindAsync(habitacion.Id);
                if (entity is null)
                    return Failure<bool>("La habitación no existe.");

                var tipoValidation = await ValidateTipoAsync(habitacion.TipoHabitacionId);
                if (tipoValidation is not null)
                    return Failure<bool>(tipoValidation);

                entity.TipoHabitacionId = habitacion.TipoHabitacionId;
                entity.CambiarCapacidad(habitacion.Capacidad);
                entity.ActualizarTarifa(habitacion.TarifaPorNoche);
                entity.UsuarioModificacion = habitacion.UsuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, "Habitación actualizada correctamente.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error actualizando la habitación.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(HabitacionRemove habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionRemove es requerido.");

            return await CambiarEstadoAsync(habitacion.Id, EstadoHabitacion.Inactiva, habitacion.UsuarioModificacion);
        }

        /// <summary>CU-005: cambia el estado operativo validando RN-018.</summary>
        public async Task<OperationResult<bool>> CambiarEstadoAsync(int habitacionId, EstadoHabitacion estado, int? usuarioModificacion)
        {
            try
            {
                var entity = await context.Habitaciones.FindAsync(habitacionId);
                if (entity is null)
                    return Failure<bool>("La habitación no existe.");

                bool dejaDeEstarDisponible = estado == EstadoHabitacion.Mantenimiento
                                             || estado == EstadoHabitacion.FueraDeServicio
                                             || estado == EstadoHabitacion.Inactiva;

                if (dejaDeEstarDisponible)
                {
                    var hoy = DateOnly.FromDateTime(DateTime.Today);
                    bool tieneReservas = await context.Reservas.AnyAsync(r => r.HabitacionId == habitacionId
                                                                           && (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada)
                                                                           && r.Fechas.Salida > hoy);
                    if (tieneReservas)
                        return Failure<bool>($"La habitación {entity.Numero} tiene reservas activas futuras; reubíquelas o cancélelas primero (RN-018).");
                }

                entity.CambiarEstado(estado);
                entity.UsuarioModificacion = usuarioModificacion;
                await context.SaveChangesAsync();
                return Success(true, $"La habitación {entity.Numero} ahora está en estado {estado}.");
            }
            catch (ReglaNegocioException ex)
            {
                return Failure<bool>(ex.Message);
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error cambiando el estado de la habitación.");
            }
        }

        private async Task<string?> ValidateTipoAsync(int tipoHabitacionId)
        {
            if (!await context.TiposHabitacion.AnyAsync(t => t.Id == tipoHabitacionId && t.Estatus))
                return "El tipo de habitación no existe o está inactivo.";

            return null;
        }

        private IQueryable<HabitacionModel> Consultar() =>
            context.Habitaciones.Select(h => new HabitacionModel
            {
                HabitacionId = h.Id,
                Numero = h.Numero,
                TipoHabitacionId = h.TipoHabitacionId,
                Tipo = h.TipoHabitacion!.Nombre,
                Capacidad = h.Capacidad,
                TarifaPorNoche = h.TarifaPorNoche,
                Estado = h.Estado,
                PisoId = h.PisoId,
                NumeroPiso = h.Piso!.Numero,
                Bloque = h.Piso.Bloque != null ? h.Piso.Bloque.Nombre : null,
                HotelId = h.HotelId,
                Hotel = h.Hotel!.Nombre,
                Estatus = h.Estatus
            });
    }
}
