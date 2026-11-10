using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.Huesped;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>Registro y búsqueda de huéspedes (CU-007, CU-008).</summary>
    public class HuespedService : ServiceBase, IHuespedService
    {
        private readonly IHuespedRepository huespedRepository;

        public HuespedService(IHuespedRepository huespedRepository, ILogger<HuespedService> logger) : base(logger)
        {
            this.huespedRepository = huespedRepository ?? throw new ArgumentNullException(nameof(huespedRepository));
        }

        public Task<OperationResult<List<HuespedModel>>> GetAllAsync() =>
            EjecutarAsync("consultar huéspedes", () => huespedRepository.GetAllAsync());

        public Task<OperationResult<HuespedModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar huésped", () => ValidarId(id, "huésped"), () => huespedRepository.GetByIdAsync(id));

        public Task<OperationResult<HuespedModel>> GetByDocumentoAsync(string documento) =>
            EjecutarAsync("buscar huésped por documento",
                          () => string.IsNullOrWhiteSpace(documento) ? "Debe indicar el documento del huésped." : null,
                          () => huespedRepository.GetByDocumentoAsync(documento));

        public Task<OperationResult<bool>> AddAsync(HuespedAdd huesped) =>
            EjecutarAsync("registrar huésped", () => Requerido(huesped, nameof(HuespedAdd)),
                          () => huespedRepository.AddAsync(huesped), huesped?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(HuespedUpdate huesped) =>
            EjecutarAsync("actualizar huésped",
                          () => Requerido(huesped, nameof(HuespedUpdate)) ?? ValidarId(huesped.Id, "huésped"),
                          () => huespedRepository.UpdateAsync(huesped), huesped?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(HuespedRemove huesped) =>
            EjecutarAsync("inactivar huésped",
                          () => Requerido(huesped, nameof(HuespedRemove)) ?? ValidarId(huesped.Id, "huésped"),
                          () => huespedRepository.RemoveAsync(huesped), huesped?.UsuarioModificacion);
    }
}
