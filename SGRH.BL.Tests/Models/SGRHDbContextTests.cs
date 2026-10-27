using Microsoft.EntityFrameworkCore;
using SGRH.BL.Tests.Base;
using SGRH.DAL.Data;
using SGRH.DAL.Models;

namespace SGRH.BL.Tests.Models
{
    public class SGRHDbContextTests
    {
        [Fact]
        public void Modelo_MapeaCadaEntidadASuSchema()
        {
            var options = new DbContextOptionsBuilder<SGRHDbContext>()
                .UseSqlServer("Server=localhost;Database=SGRH;Trusted_Connection=True;")
                .Options;
            using var context = new SGRHDbContext(options);

            Assert.Equal("Configuration", context.Model.FindEntityType(typeof(Hotel))!.GetSchema());
            Assert.Equal("Reservation", context.Model.FindEntityType(typeof(Reserva))!.GetSchema());
            Assert.Equal("Notification", context.Model.FindEntityType(typeof(Notificacion))!.GetSchema());
            Assert.Equal("Security", context.Model.FindEntityType(typeof(Usuario))!.GetSchema());
        }

        [Fact]
        public void Sembrar_GuardaHotelConPisosYHabitaciones()
        {
            using var db = new TestDatabase();
            var datos = db.Sembrar();

            using var context = db.CrearContexto();
            var hotel = context.Hoteles.Include(h => h.Pisos).ThenInclude(p => p.Habitaciones)
                               .Single(h => h.Id == datos.HotelId);

            Assert.Equal(2, hotel.Pisos.Count);
            Assert.Equal(3, hotel.Pisos.SelectMany(p => p.Habitaciones).Count());
        }
    }
}
