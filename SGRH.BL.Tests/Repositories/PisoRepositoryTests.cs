using SGRH.BL.Tests.Base;
using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class PisoRepositoryTests
    {
        [Fact]
        public async Task Add_NumeroActivoRepetido_Falla_RN003()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new PisoRepository(context).AddAsync(new PisoAdd { Numero = 1, HotelId = datos.HotelId });

            Assert.False(resultado.Success);
            Assert.Contains("RN-003", resultado.Message);
        }

        [Fact]
        public async Task Add_PisoNuevo_SeGuarda()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new PisoRepository(context);

            var resultado = await repo.AddAsync(new PisoAdd { Numero = 3, HotelId = datos.HotelId });
            var pisos = await repo.GetByHotelAsync(datos.HotelId);

            Assert.True(resultado.Success);
            Assert.Equal(3, pisos.Data!.Count);
        }
    }
}
