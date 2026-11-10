using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.Bloque;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>Gestión de bloques (torres o edificios) de un hotel.</summary>
    public class BloqueService : ServiceBase, IBloqueService
    {
        private readonly IBloqueRepository bloqueRepository;

        public BloqueService(IBloqueRepository bloqueRepository, ILogger<BloqueService> logger) : base(logger)
        {
            this.bloqueRepository = bloqueRepository ?? throw new ArgumentNullException(nameof(bloqueRepository));
        }

        public Task<OperationResult<List<BloqueModel>>> GetAllAsync() =>
            EjecutarAsync("consultar bloques", () => bloqueRepository.GetAllAsync());

        public Task<OperationResult<BloqueModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar bloque", () => ValidarId(id, "bloque"), () => bloqueRepository.GetByIdAsync(id));

        public Task<OperationResult<List<BloqueModel>>> GetByHotelAsync(int hotelId) =>
            EjecutarAsync("consultar bloques del hotel", () => ValidarId(hotelId, "hotel"), () => bloqueRepository.GetByHotelAsync(hotelId));

        public Task<OperationResult<bool>> AddAsync(BloqueAdd bloque) =>
            EjecutarAsync("registrar bloque",
                          () => Requerido(bloque, nameof(BloqueAdd)) ?? PrimerError(ValidarBloque(bloque)),
                          () => bloqueRepository.AddAsync(bloque), bloque?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(BloqueUpdate bloque) =>
            EjecutarAsync("actualizar bloque",
                          () => Requerido(bloque, nameof(BloqueUpdate)) ?? PrimerError(ValidarId(bloque.Id, "bloque"), ValidarBloque(bloque)),
                          () => bloqueRepository.UpdateAsync(bloque), bloque?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(BloqueRemove bloque) =>
            EjecutarAsync("inactivar bloque",
                          () => Requerido(bloque, nameof(BloqueRemove)) ?? PrimerError(ValidarId(bloque.Id, "bloque")),
                          () => bloqueRepository.RemoveAsync(bloque), bloque?.UsuarioModificacion);

        public Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion) =>
            EjecutarAsync("activar bloque", () => ValidarId(id, "bloque"),
                          () => bloqueRepository.ActivarAsync(id, usuarioModificacion), usuarioModificacion);

        private static string? ValidarBloque(BloqueBase? bloque)
        {
            if (bloque is null)
                return null;

            if (string.IsNullOrWhiteSpace(bloque.Nombre))
                return "El nombre del bloque es obligatorio.";

            return ValidarId(bloque.HotelId, "hotel");
        }
    }
}
