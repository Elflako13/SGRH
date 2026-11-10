using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;

namespace SGRH.BL.Tests.Base
{
    /// <summary>
    /// Base de datos SQLite en memoria para probar los repositorios sin SQL Server.
    /// Cada prueba crea su propia base de datos limpia.
    /// </summary>
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SGRHDbContext> _options;

        public TestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<SGRHDbContext>().UseSqlite(_connection).Options;

            using var context = CrearContexto();
            context.Database.EnsureCreated();
        }

        public SGRHDbContext CrearContexto() => new SGRHDbContext(_options);

        /// <summary>
        /// Crea un hotel con el piso 1 (habitaciones 101 y 102) y el piso 2 (habitación 201),
        /// y un huésped. Devuelve los Id generados.
        /// </summary>
        public DatosDePrueba Sembrar()
        {
            using var context = CrearContexto();

            var sencilla = Tipos.Sencilla;
            var doble = Tipos.Doble;
            var suite = Tipos.Suite;
            context.TiposHabitacion.AddRange(sencilla, doble, suite);

            var hotel = new Hotel { Nombre = "Hotel Prueba", Direccion = "Av. Las Américas", Ciudad = "Santo Domingo", Estrellas = 4 };
            var piso1 = hotel.AgregarPiso(1);
            var piso2 = hotel.AgregarPiso(2);
            var h101 = new Habitacion("101", doble, 2, 2500m);
            var h102 = new Habitacion("102", sencilla, 1, 1800m);
            var h201 = new Habitacion("201", suite, 4, 6000m);
            piso1.AgregarHabitacion(h101);
            piso1.AgregarHabitacion(h102);
            piso2.AgregarHabitacion(h201);

            var huesped = new Huesped { Nombre = "Ana", Apellido = "Pérez", Documento = "001-0000000-1", Correo = "ana@correo.com" };

            context.Hoteles.Add(hotel);
            context.Huespedes.Add(huesped);
            context.SaveChanges();

            return new DatosDePrueba(hotel.Id, piso1.Id, piso2.Id, h101.Id, h102.Id, h201.Id, huesped.Id, sencilla.Id, doble.Id, suite.Id);
        }

        public void Dispose() => _connection.Dispose();
    }

    public record DatosDePrueba(int HotelId, int Piso1Id, int Piso2Id,
                                int Habitacion101Id, int Habitacion102Id, int Habitacion201Id,
                                int HuespedId,
                                int TipoSencillaId, int TipoDobleId, int TipoSuiteId);

    /// <summary>Tipos de habitación de prueba. Cada llamada crea un objeto nuevo.</summary>
    public static class Tipos
    {
        public static TipoHabitacion Sencilla => new() { Nombre = "Sencilla", TarifaBase = 1800m };
        public static TipoHabitacion Doble => new() { Nombre = "Doble", TarifaBase = 2500m };
        public static TipoHabitacion Suite => new() { Nombre = "Suite", TarifaBase = 6000m };
    }
}
