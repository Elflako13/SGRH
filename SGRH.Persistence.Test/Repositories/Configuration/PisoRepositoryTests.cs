using SGRH.Domain.Entities.Configuration;
using SGRH.Persistence.Repositories.Configuration;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Configuration
{
    public class PisoRepositoryTests
    {
        [Fact]
        public async Task Save_NumeroActivoRepetido_Falla_RN003()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new PisoRepository(context, TestDatabase.Logger<PisoRepository>());

            var resultado = await repo.SaveEntityAsync(new Piso { Numero = 1, HotelId = datos.HotelId });

            Assert.False(resultado.Success);
            Assert.Contains("RN-003", resultado.Message);
        }

        [Fact]
        public async Task Save_PisoNuevo_SeGuarda()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new PisoRepository(context, TestDatabase.Logger<PisoRepository>());

            var resultado = await repo.SaveEntityAsync(new Piso { Numero = 3, HotelId = datos.HotelId });
            var pisos = await repo.GetByHotelAsync(datos.HotelId);

            Assert.True(resultado.Success);
            Assert.Equal(3, pisos.Count);
        }
    }
}
