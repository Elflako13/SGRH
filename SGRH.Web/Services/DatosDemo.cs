using Microsoft.EntityFrameworkCore;
using SGRH.DAL.Data;
using SGRH.DAL.Models;

namespace SGRH.Web.Services
{
    /// <summary>
    /// Solo se usa cuando "ProveedorBaseDatos" es "Sqlite": crea la base de datos y carga los mismos
    /// datos de ejemplo del script Database/SGRH.sql. Con SQL Server no hace nada (se usa el script).
    /// </summary>
    public static class DatosDemo
    {
        public static async Task InicializarAsync(IServiceProvider servicios, IConfiguration configuration)
        {
            if (!string.Equals(configuration["ProveedorBaseDatos"], "Sqlite", StringComparison.OrdinalIgnoreCase))
                return;

            using var scope = servicios.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SGRHDbContext>();

            await context.Database.EnsureCreatedAsync();
            if (await context.Hoteles.AnyAsync())
                return;

            context.Usuarios.Add(new Usuario { NombreUsuario = "admin", NombreCompleto = "Administrador del sistema", Correo = "admin@sgrh.com", ClaveHash = "-", Rol = Rol.Administrador });

            var sencilla = new TipoHabitacion { Nombre = "Sencilla", Descripcion = "Una cama individual, ideal para una persona.", TarifaBase = 1800m };
            var doble = new TipoHabitacion { Nombre = "Doble", Descripcion = "Dos camas o una cama matrimonial para dos personas.", TarifaBase = 2500m };
            var familiar = new TipoHabitacion { Nombre = "Familiar", Descripcion = "Habitación amplia para familias de hasta 5 personas.", TarifaBase = 4200m };
            var suite = new TipoHabitacion { Nombre = "Suite", Descripcion = "Sala independiente, jacuzzi y vista preferencial.", TarifaBase = 6000m };
            context.TiposHabitacion.AddRange(sencilla, doble, familiar, suite);

            var sd = Hotel("Hotel Caribe Santo Domingo", "Av. George Washington #100", "Santo Domingo", 4, "809-555-0100", "reservas@caribesd.com");
            var pc = Hotel("Hotel Bahía Punta Cana", "Playa Bávaro, Carretera Arena Gorda", "Punta Cana", 5, "809-555-0200", "reservas@bahiapc.com");
            var st = Hotel("Hotel Cibao Santiago", "Av. 27 de Febrero #45", "Santiago", 3, "809-555-0300", "reservas@cibaostgo.com");

            var torreA = new Bloque { Nombre = "Torre A", Descripcion = "Torre principal frente al malecón.", Hotel = sd };
            var torreB = new Bloque { Nombre = "Torre B", Descripcion = "Torre ejecutiva.", Hotel = sd };
            var playa = new Bloque { Nombre = "Edificio Playa", Descripcion = "Frente a la playa.", Hotel = pc };
            var jardin = new Bloque { Nombre = "Edificio Jardín", Descripcion = "Rodeado de jardines y piscina.", Hotel = pc };
            context.Bloques.AddRange(torreA, torreB, playa, jardin);

            Piso(sd, 1, torreA, ("101", sencilla, 1, 1800m, EstadoHabitacion.Disponible), ("102", doble, 2, 2500m, EstadoHabitacion.Disponible), ("103", doble, 2, 2500m, EstadoHabitacion.Mantenimiento));
            Piso(sd, 2, torreA, ("201", doble, 2, 2700m, EstadoHabitacion.Disponible), ("202", familiar, 5, 4200m, EstadoHabitacion.Limpieza));
            Piso(sd, 3, torreB, ("301", suite, 4, 6000m, EstadoHabitacion.Disponible));
            Piso(pc, 1, playa, ("101", doble, 2, 3800m, EstadoHabitacion.Disponible), ("102", familiar, 5, 5500m, EstadoHabitacion.Disponible));
            Piso(pc, 2, playa, ("201", suite, 4, 9500m, EstadoHabitacion.Disponible));
            Piso(pc, 3, jardin, ("301", doble, 2, 3500m, EstadoHabitacion.FueraDeServicio));
            Piso(st, 1, null, ("101", sencilla, 1, 1500m, EstadoHabitacion.Disponible));
            Piso(st, 2, null, ("201", doble, 2, 2100m, EstadoHabitacion.Disponible));

            context.Hoteles.AddRange(sd, pc, st);
            await context.SaveChangesAsync();
        }

        private static Hotel Hotel(string nombre, string direccion, string ciudad, int estrellas, string telefono, string correo) =>
            new() { Nombre = nombre, Direccion = direccion, Ciudad = ciudad, Estrellas = estrellas, Telefono = telefono, Correo = correo };

        private static void Piso(Hotel hotel, int numero, Bloque? bloque,
                                 params (string Numero, TipoHabitacion Tipo, int Capacidad, decimal Tarifa, EstadoHabitacion Estado)[] habitaciones)
        {
            var piso = hotel.AgregarPiso(numero);
            piso.Bloque = bloque;
            foreach (var h in habitaciones)
            {
                var habitacion = new Habitacion(h.Numero, h.Tipo, h.Capacidad, h.Tarifa);
                if (h.Estado != EstadoHabitacion.Disponible)
                    habitacion.CambiarEstado(h.Estado);
                piso.AgregarHabitacion(habitacion);
            }
        }
    }
}
