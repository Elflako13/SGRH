using Microsoft.Extensions.Logging;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Services
{
    /// <summary>Gestión de hoteles (CU-001, RF-001, RF-002).</summary>
    public class HotelService : ServiceBase, IHotelService
    {
        private readonly IHotelRepository hotelRepository;

        public HotelService(IHotelRepository hotelRepository, ILogger<HotelService> logger) : base(logger)
        {
            this.hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
        }

        public Task<OperationResult<List<HotelModel>>> GetAllAsync() =>
            EjecutarAsync("consultar hoteles", () => hotelRepository.GetAllAsync());

        public Task<OperationResult<HotelModel>> GetByIdAsync(int id) =>
            EjecutarAsync("consultar hotel", () => ValidarId(id, "hotel"), () => hotelRepository.GetByIdAsync(id));

        public Task<OperationResult<bool>> AddAsync(HotelAdd hotel) =>
            EjecutarAsync("registrar hotel",
                          () => Requerido(hotel, nameof(HotelAdd)) ?? PrimerError(ValidarHotel(hotel)),
                          () => hotelRepository.AddAsync(hotel), hotel?.UsuarioCreacion);

        public Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel) =>
            EjecutarAsync("actualizar hotel",
                          () => Requerido(hotel, nameof(HotelUpdate)) ?? PrimerError(ValidarId(hotel.Id, "hotel"), ValidarHotel(hotel)),
                          () => hotelRepository.UpdateAsync(hotel), hotel?.UsuarioModificacion);

        public Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel) =>
            EjecutarAsync("inactivar hotel",
                          () => Requerido(hotel, nameof(HotelRemove)) ?? PrimerError(ValidarId(hotel.Id, "hotel")),
                          () => hotelRepository.RemoveAsync(hotel), hotel?.UsuarioModificacion);

        public Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion) =>
            EjecutarAsync("activar hotel", () => ValidarId(id, "hotel"),
                          () => hotelRepository.ActivarAsync(id, usuarioModificacion), usuarioModificacion);

        private static string? ValidarHotel(HotelBase? hotel)
        {
            if (hotel is null)
                return null;

            if (string.IsNullOrWhiteSpace(hotel.Nombre))
                return "El nombre del hotel es obligatorio.";

            if (hotel.Estrellas < 1 || hotel.Estrellas > 5)
                return "La categoría del hotel debe estar entre 1 y 5 estrellas.";

            return null;
        }
    }
}
