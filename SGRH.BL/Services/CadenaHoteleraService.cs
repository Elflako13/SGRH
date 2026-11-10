using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.CadenaHotelera;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    public class CadenaHoteleraService : ServiceBase, ICadenaHoteleraService
    {
        private readonly ICadenaHoteleraRepository cadenaRepository;

        public CadenaHoteleraService(ICadenaHoteleraRepository cadenaRepository, ILogger<CadenaHoteleraService> logger) : base(logger)
        {
            this.cadenaRepository = cadenaRepository ?? throw new ArgumentNullException(nameof(cadenaRepository));
        }

        public Task<OperationResult<List<CadenaHoteleraModel>>> GetAllAsync() =>
            EjecutarAsync("consultar cadenas hoteleras", () => cadenaRepository.GetAllAsync());

        public Task<OperationResult<CadenaHoteleraModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar cadena hotelera", () => ValidarId(id, "cadena hotelera"), () => cadenaRepository.GetByIdAsync(id));

        public Task<OperationResult<bool>> AddAsync(CadenaHoteleraAdd cadena) =>
            EjecutarAsync("registrar cadena hotelera", () => Requerido(cadena, nameof(CadenaHoteleraAdd)),
                          () => cadenaRepository.AddAsync(cadena), cadena?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(CadenaHoteleraUpdate cadena) =>
            EjecutarAsync("actualizar cadena hotelera",
                          () => Requerido(cadena, nameof(CadenaHoteleraUpdate)) ?? ValidarId(cadena.Id, "cadena hotelera"),
                          () => cadenaRepository.UpdateAsync(cadena), cadena?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(CadenaHoteleraRemove cadena) =>
            EjecutarAsync("inactivar cadena hotelera",
                          () => Requerido(cadena, nameof(CadenaHoteleraRemove)) ?? ValidarId(cadena.Id, "cadena hotelera"),
                          () => cadenaRepository.RemoveAsync(cadena), cadena?.UsuarioModificacion);
    }
}
