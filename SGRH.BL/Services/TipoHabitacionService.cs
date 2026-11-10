using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>Mantenimiento del catálogo de tipos de habitación.</summary>
    public class TipoHabitacionService : ServiceBase, ITipoHabitacionService
    {
        private readonly ITipoHabitacionRepository tipoRepository;

        public TipoHabitacionService(ITipoHabitacionRepository tipoRepository, ILogger<TipoHabitacionService> logger) : base(logger)
        {
            this.tipoRepository = tipoRepository ?? throw new ArgumentNullException(nameof(tipoRepository));
        }

        public Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync() =>
            EjecutarAsync("consultar tipos de habitación", () => tipoRepository.GetAllAsync());

        public Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar tipo de habitación", () => ValidarId(id, "tipo de habitación"), () => tipoRepository.GetByIdAsync(id));

        public Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipo) =>
            EjecutarAsync("registrar tipo de habitación",
                          () => Requerido(tipo, nameof(TipoHabitacionAdd)) ?? PrimerError(ValidarTipo(tipo)),
                          () => tipoRepository.AddAsync(tipo), tipo?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipo) =>
            EjecutarAsync("actualizar tipo de habitación",
                          () => Requerido(tipo, nameof(TipoHabitacionUpdate)) ?? PrimerError(ValidarId(tipo.Id, "tipo de habitación"), ValidarTipo(tipo)),
                          () => tipoRepository.UpdateAsync(tipo), tipo?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(TipoHabitacionRemove tipo) =>
            EjecutarAsync("inactivar tipo de habitación",
                          () => Requerido(tipo, nameof(TipoHabitacionRemove)) ?? PrimerError(ValidarId(tipo.Id, "tipo de habitación")),
                          () => tipoRepository.RemoveAsync(tipo), tipo?.UsuarioModificacion);

        public Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion) =>
            EjecutarAsync("activar tipo de habitación", () => ValidarId(id, "tipo de habitación"),
                          () => tipoRepository.ActivarAsync(id, usuarioModificacion), usuarioModificacion);

        private static string? ValidarTipo(TipoHabitacionBase? tipo)
        {
            if (tipo is null)
                return null;

            if (string.IsNullOrWhiteSpace(tipo.Nombre))
                return "El nombre del tipo de habitación es obligatorio.";

            if (tipo.TarifaBase <= 0)
                return "La tarifa base debe ser mayor que cero.";

            return null;
        }
    }
}
