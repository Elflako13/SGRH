using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>Gestión de pisos (CU-002, RF-003, RF-004).</summary>
    public class PisoService : ServiceBase, IPisoService
    {
        private readonly IPisoRepository pisoRepository;

        public PisoService(IPisoRepository pisoRepository, ILogger<PisoService> logger) : base(logger)
        {
            this.pisoRepository = pisoRepository ?? throw new ArgumentNullException(nameof(pisoRepository));
        }

        public Task<OperationResult<List<PisoModel>>> GetAllAsync() =>
            EjecutarAsync("consultar pisos", () => pisoRepository.GetAllAsync());

        public Task<OperationResult<PisoModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar piso", () => ValidarId(id, "piso"), () => pisoRepository.GetByIdAsync(id));

        public Task<OperationResult<List<PisoModel>>> GetByHotelAsync(int hotelId) =>
            EjecutarAsync("consultar pisos del hotel", () => ValidarId(hotelId, "hotel"), () => pisoRepository.GetByHotelAsync(hotelId));

        public Task<OperationResult<bool>> AddAsync(PisoAdd piso) =>
            EjecutarAsync("registrar piso",
                          () => Requerido(piso, nameof(PisoAdd)) ?? PrimerError(ValidarPiso(piso)),
                          () => pisoRepository.AddAsync(piso), piso?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(PisoUpdate piso) =>
            EjecutarAsync("actualizar piso",
                          () => Requerido(piso, nameof(PisoUpdate)) ?? PrimerError(ValidarId(piso.Id, "piso"), ValidarPiso(piso)),
                          () => pisoRepository.UpdateAsync(piso), piso?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(PisoRemove piso) =>
            EjecutarAsync("inactivar piso",
                          () => Requerido(piso, nameof(PisoRemove)) ?? PrimerError(ValidarId(piso.Id, "piso")),
                          () => pisoRepository.RemoveAsync(piso), piso?.UsuarioModificacion);

        public Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion) =>
            EjecutarAsync("activar piso", () => ValidarId(id, "piso"),
                          () => pisoRepository.ActivarAsync(id, usuarioModificacion), usuarioModificacion);

        private static string? ValidarPiso(PisoBase? piso)
        {
            if (piso is null)
                return null;

            if (piso.Numero <= 0)
                return "El número de piso debe ser mayor que cero.";

            return ValidarId(piso.HotelId, "hotel");
        }
    }
}
