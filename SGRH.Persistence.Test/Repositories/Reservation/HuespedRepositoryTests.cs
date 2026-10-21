using SGRH.Domain.Entities.Reservation;
using SGRH.Persistence.Repositories.Reservation;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Repositories.Reservation
{
    public class HuespedRepositoryTests
    {
        [Fact]
        public async Task Save_DocumentoRepetido_Falla_RN017()
        {
            using var db = new TestDatabase();
            db.Sembrar(); // ya existe el documento 001-0000000-1
            using var context = db.CrearContexto();
            var repo = new HuespedRepository(context, TestDatabase.Logger<HuespedRepository>());

            var resultado = await repo.SaveEntityAsync(new Huesped { Nombre = "Luis", Apellido = "Gómez", Documento = "001-0000000-1", Telefono = "809-555-0000" });

            Assert.False(resultado.Success);
            Assert.Contains("RN-017", resultado.Message);
        }

        [Fact]
        public async Task Save_SinCorreoNiTelefono_Falla()
        {
            using var db = new TestDatabase();
            using var context = db.CrearContexto();
            var repo = new HuespedRepository(context, TestDatabase.Logger<HuespedRepository>());

            var resultado = await repo.SaveEntityAsync(new Huesped { Nombre = "Luis", Apellido = "Gómez", Documento = "002-0000000-2" });

            Assert.False(resultado.Success);
        }
    }
}
