using SGRH.BL.Tests.Base;
using SGRH.DAL.Models.Objects.Bloque;
using SGRH.DAL.Models.Objects.Piso;
using SGRH.DAL.Models.Objects.TipoHabitacion;
using SGRH.DAL.Repositories;

namespace SGRH.BL.Tests.Repositories
{
    public class BloqueYTipoHabitacionTests
    {
        [Fact]
        public async Task Bloque_NombreRepetidoEnElHotel_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new BloqueRepository(context);

            await repo.AddAsync(new BloqueAdd { Nombre = "Torre A", HotelId = datos.HotelId });
            var resultado = await repo.AddAsync(new BloqueAdd { Nombre = "Torre A", HotelId = datos.HotelId });

            Assert.False(resultado.Success);
            Assert.Contains("Torre A", resultado.Message);
        }

        [Fact]
        public async Task Piso_ConBloqueDeOtroHotel_Falla()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var otroHotel = new DAL.Models.Hotel { Nombre = "Otro", Direccion = "X", Ciudad = "Santiago", Estrellas = 3 };
            context.Hoteles.Add(otroHotel);
            context.SaveChanges();
            await new BloqueRepository(context).AddAsync(new BloqueAdd { Nombre = "Torre B", HotelId = otroHotel.Id });
            int bloqueId = context.Bloques.Single().Id;

            var resultado = await new PisoRepository(context).AddAsync(new PisoAdd { Numero = 9, HotelId = datos.HotelId, BloqueId = bloqueId });

            Assert.False(resultado.Success);
            Assert.Contains("otro hotel", resultado.Message);
        }

        [Fact]
        public async Task Bloque_ConPisosActivos_NoSeInactiva()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            await new BloqueRepository(context).AddAsync(new BloqueAdd { Nombre = "Torre A", HotelId = datos.HotelId });
            int bloqueId = context.Bloques.Single().Id;
            await new PisoRepository(context).AddAsync(new PisoAdd { Numero = 5, HotelId = datos.HotelId, BloqueId = bloqueId });

            var resultado = await new BloqueRepository(context).RemoveAsync(new BloqueRemove { Id = bloqueId });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task TipoHabitacion_ConHabitacionesActivas_NoSeInactiva()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new TipoHabitacionRepository(context).RemoveAsync(new TipoHabitacionRemove { Id = datos.TipoDobleId });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task TipoHabitacion_NombreRepetido_Falla()
        {
            using var db = new TestDatabase();
            db.Sembrar();
            using var context = db.CrearContexto();

            var resultado = await new TipoHabitacionRepository(context).AddAsync(new TipoHabitacionAdd { Nombre = "Doble", TarifaBase = 2000m });

            Assert.False(resultado.Success);
        }

        [Fact]
        public async Task Piso_Reactivar_ConNumeroOcupado_Falla_RN003()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();
            using var context = db.CrearContexto();
            var repo = new PisoRepository(context);
            await repo.AddAsync(new PisoAdd { Numero = 3, HotelId = datos.HotelId });
            int piso3 = context.Pisos.Single(p => p.Numero == 3).Id;
            await repo.RemoveAsync(new PisoRemove { Id = piso3 });
            await repo.AddAsync(new PisoAdd { Numero = 3, HotelId = datos.HotelId });

            var resultado = await repo.ActivarAsync(piso3, null);

            Assert.False(resultado.Success);
            Assert.Contains("RN-003", resultado.Message);
        }
    }
}
