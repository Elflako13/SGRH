using Microsoft.Extensions.Logging;
using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Models.Result;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    /// <summary>
    /// Comprueba el manejo de excepciones y el logging de la capa de negocio:
    /// si el repositorio falla, el servicio no se cae, registra el error y responde con un mensaje claro.
    /// </summary>
    public class ManejoDeErroresTests
    {
        [Fact]
        public async Task RepositorioLanzaExcepcion_ElServicioDevuelveFalloYRegistraElError()
        {
            var logger = new LoggerDePrueba<HotelService>();
            var service = new HotelService(new HotelRepositoryQueFalla(), logger);

            var resultado = await service.GetAllAsync();

            Assert.False(resultado.Success);
            Assert.Contains("error inesperado", resultado.Message);
            Assert.Contains(logger.Entradas, e => e.Nivel == LogLevel.Error && e.Excepcion is TimeoutException);
        }

        [Fact]
        public async Task OperacionCorrecta_RegistraInicioYFin()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var logger = new LoggerDePrueba<HotelService>();
            var service = new HotelService(new HotelRepository(context), logger);

            await service.AddAsync(new HotelAdd { Nombre = "Hotel Norte", Direccion = "Calle 1", Ciudad = "Santiago", Estrellas = 3, UsuarioCreacion = 1 });

            Assert.Contains(logger.Entradas, e => e.Nivel == LogLevel.Information && e.Mensaje.Contains("Iniciando operación de registrar hotel"));
            Assert.Contains(logger.Entradas, e => e.Nivel == LogLevel.Information && e.Mensaje.Contains("completada"));
        }

        [Fact]
        public async Task ObjetoNulo_NoLlegaAlRepositorioYQuedaEnElLog()
        {
            var logger = new LoggerDePrueba<HotelService>();
            var service = new HotelService(new HotelRepositoryQueFalla(), logger);

            var resultado = await service.AddAsync(null!);

            Assert.False(resultado.Success);
            Assert.Equal("El objeto HotelAdd es requerido.", resultado.Message);
            Assert.Contains(logger.Entradas, e => e.Nivel == LogLevel.Warning);
        }

        [Fact]
        public void Constructor_SinLogger_LanzaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HotelService(new HotelRepositoryQueFalla(), null!));
        }

        /// <summary>Repositorio falso que simula una caída de la base de datos.</summary>
        private sealed class HotelRepositoryQueFalla : IHotelRepository
        {
            private static Task<T> Falla<T>() => throw new TimeoutException("La base de datos no responde.");
            public Task<OperationResult<List<HotelModel>>> GetAllAsync() => Falla<OperationResult<List<HotelModel>>>();
            public Task<OperationResult<HotelModel>> GetByIdAsync(int id) => Falla<OperationResult<HotelModel>>();
            public Task<OperationResult<bool>> AddAsync(HotelAdd hotel) => Falla<OperationResult<bool>>();
            public Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel) => Falla<OperationResult<bool>>();
            public Task<OperationResult<bool>> RemoveAsync(HotelRemove hotel) => Falla<OperationResult<bool>>();
            public Task<OperationResult<bool>> ActivarAsync(int id, int? usuarioModificacion) => Falla<OperationResult<bool>>();
        }
    }
}
