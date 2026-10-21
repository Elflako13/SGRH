using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Enums;
using SGRH.Persistence.Context;

namespace SGRH.Persistence.Test.Base
{
    /// <summary>
    /// Base de datos SQLite en memoria para probar los repositorios sin SQL Server.
    /// Cada prueba crea su propia base de datos limpia.
    /// </summary>
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SGRHContext> _options;

        public TestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<SGRHContext>().UseSqlite(_connection).Options;

            using var context = CrearContexto();
            context.Database.EnsureCreated();
        }

        public SGRHContext CrearContexto() => new SGRHContext(_options);

        public static NullLogger<T> Logger<T>() => NullLogger<T>.Instance;

        /// <summary>
        /// Crea un hotel con el piso 1 (habitaciones 101 y 102) y el piso 2 (habitación 201),
        /// y un huésped. Devuelve los Id generados.
        /// </summary>
        public DatosDePrueba Sembrar()
        {
            using var context = CrearContexto();

            var hotel = new Hotel { Nombre = "Hotel Prueba", Direccion = "Av. Las Américas" };
            var piso1 = hotel.AgregarPiso(1);
            var piso2 = hotel.AgregarPiso(2);
            var h101 = new Habitacion("101", TipoHabitacion.Doble, 2, 2500m);
            var h102 = new Habitacion("102", TipoHabitacion.Sencilla, 1, 1800m);
            var h201 = new Habitacion("201", TipoHabitacion.Suite, 4, 6000m);
            piso1.AgregarHabitacion(h101);
            piso1.AgregarHabitacion(h102);
            piso2.AgregarHabitacion(h201);

            var huesped = new Huesped { Nombre = "Ana", Apellido = "Pérez", Documento = "001-0000000-1", Correo = "ana@correo.com" };

            context.Hoteles.Add(hotel);
            context.Huespedes.Add(huesped);
            context.SaveChanges();

            return new DatosDePrueba(hotel.Id, piso1.Id, piso2.Id, h101.Id, h102.Id, h201.Id, huesped.Id);
        }

        public void Dispose() => _connection.Dispose();
    }

    public record DatosDePrueba(int HotelId, int Piso1Id, int Piso2Id,
                                int Habitacion101Id, int Habitacion102Id, int Habitacion201Id,
                                int HuespedId);
}
