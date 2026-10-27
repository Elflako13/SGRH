using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;
using SGRH.DAL.Models.Objects.HistorialReserva;
using SGRH.DAL.Models.Result;
using static SGRH.DAL.Models.Result.OperationResult;

namespace SGRH.DAL.Repositories
{
    public class HistorialReservaRepository : IHistorialReservaRepository
    {
        private readonly SGRHDbContext context;

        public HistorialReservaRepository(SGRHDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<bool>> AddAsync(HistorialReservaAdd historial)
        {
            try
            {
                if (historial is null || string.IsNullOrWhiteSpace(historial.Accion))
                    return Failure<bool>("El registro de historial debe indicar la acción realizada.");

                await context.HistorialReservas.AddAsync(new HistorialReserva
                {
                    ReservaId = historial.ReservaId,
                    UsuarioId = historial.UsuarioId,
                    Accion = historial.Accion.Trim(),
                    Detalle = historial.Detalle
                });
                await context.SaveChangesAsync();
                return Success(true, "Historial registrado.");
            }
            catch (Exception)
            {
                return Failure<bool>("Ocurrió un error registrando el historial.");
            }
        }

        public async Task<OperationResult<List<HistorialReservaModel>>> GetByReservaAsync(int reservaId)
        {
            try
            {
                var registros = await context.HistorialReservas
                                             .Include(h => h.Usuario)
                                             .Where(h => h.ReservaId == reservaId)
                                             .OrderBy(h => h.Fecha)
                                             .ToListAsync();

                return Success(registros.Select(h => new HistorialReservaModel
                {
                    HistorialReservaId = h.Id,
                    ReservaId = h.ReservaId,
                    Fecha = h.Fecha,
                    Usuario = h.Usuario?.NombreUsuario ?? string.Empty,
                    Accion = h.Accion,
                    Detalle = h.Detalle
                }).ToList());
            }
            catch (Exception)
            {
                return Failure<List<HistorialReservaModel>>("Ocurrió un error obteniendo el historial.");
            }
        }
    }
}
