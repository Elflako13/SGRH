using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Entities.Reservation;
using SGRH.Persistence.Context;
using SGRH.Persistence.Test.Base;

namespace SGRH.Persistence.Test.Context
{
    public class SGRHContextTests
    {
        [Fact]
        public void Modelo_MapeaCadaEntidadASuSchema()
        {
            var options = new DbContextOptionsBuilder<SGRHContext>()
                .UseSqlServer("Server=localhost;Database=SGRH;Trusted_Connection=True;")
                .Options;
            using var context = new SGRHContext(options);

            IEntityType hotel = context.Model.FindEntityType(typeof(Hotel))!;
            IEntityType reserva = context.Model.FindEntityType(typeof(Reserva))!;

            Assert.Equal("Configuration", hotel.GetSchema());
            Assert.Equal("Reservation", reserva.GetSchema());
        }

        [Fact]
        public void Sembrar_GuardaHotelConPisosYHabitaciones()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();

            using var context = db.CrearContexto();
            var hotel = context.Hoteles
                               .Include(h => h.Pisos)
                               .ThenInclude(p => p.Habitaciones)
                               .Single(h => h.Id == datos.HotelId);

            Assert.Equal(2, hotel.Pisos.Count);
            Assert.Equal(3, hotel.Pisos.SelectMany(p => p.Habitaciones).Count());
            Assert.All(hotel.Pisos.SelectMany(p => p.Habitaciones), h => Assert.Equal(datos.HotelId, h.HotelId));
        }
    }
}
