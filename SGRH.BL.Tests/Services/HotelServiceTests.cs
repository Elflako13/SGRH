using SGRH.BL.Services;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Models.Objects.Hotel;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Services
{
    public class HotelServiceTests
    {
        [Fact]
        public async Task Add_SinNombre_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var service = new HotelService(new HotelRepository(context));

            var resultado = await service.AddAsync(new HotelAdd { Direccion = "Av. Las Américas" });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task Add_YGetAll_DevuelveElHotel()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var service = new HotelService(new HotelRepository(context));

            await service.AddAsync(new HotelAdd { Nombre = "Hotel Caribe", Direccion = "Santo Domingo" });
            var hoteles = await service.GetAllAsync();

            Assert.True(hoteles.Success);
            Assert.Single(hoteles.Data!);
        }

        [Fact]
        public async Task Remove_HotelConPisosActivos_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var service = new HotelService(new HotelRepository(context));

            var resultado = await service.RemoveAsync(new HotelRemove { Id = datos.HotelId });

            Assert.False(resultado.Success);
        }
    }
}
