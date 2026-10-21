using Microsoft.EntityFrameworkCore;
using SGRH.Domain.Entities.Configuration;
using SGRH.Domain.Entities.Notification;
using SGRH.Domain.Entities.Reservation;
using SGRH.Domain.Entities.Security;

namespace SGRH.Persistence.Context
{
    /// <summary>
    /// Contexto de Entity Framework Core de la base de datos SGRH.
    /// Cada entidad se mapea a la tabla de su schema: Configuration, Reservation, Notification y Security.
    /// </summary>
    public class SGRHContext : DbContext
    {
        public SGRHContext(DbContextOptions<SGRHContext> options) : base(options)
        {
        }

        #region Configuration
        public DbSet<CadenaHotelera> CadenasHoteleras { get; set; }
        public DbSet<Hotel> Hoteles { get; set; }
        public DbSet<Piso> Pisos { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        #endregion

        #region Reservation
        public DbSet<Huesped> Huespedes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<HistorialReserva> HistorialReservas { get; set; }
        #endregion

        #region Notification
        public DbSet<Notificacion> Notificaciones { get; set; }
        #endregion

        #region Security
        public DbSet<Usuario> Usuarios { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurarConfiguration(modelBuilder);
            ConfigurarReservation(modelBuilder);
            ConfigurarNotification(modelBuilder);
            ConfigurarSecurity(modelBuilder);
        }

        private static void ConfigurarConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CadenaHotelera>(e =>
            {
                e.ToTable("CadenaHotelera", "Configuration");
                e.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
                e.Property(c => c.RNC).HasMaxLength(20);
            });

            modelBuilder.Entity<Hotel>(e =>
            {
                e.ToTable("Hotel", "Configuration");
                e.Property(h => h.Nombre).IsRequired().HasMaxLength(150);
                e.Property(h => h.Direccion).IsRequired().HasMaxLength(250);
                e.Property(h => h.Telefono).HasMaxLength(20);
                e.Property(h => h.Correo).HasMaxLength(150);

                e.HasOne(h => h.CadenaHotelera)
                 .WithMany(c => c.Hoteles)
                 .HasForeignKey(h => h.CadenaHoteleraId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Piso>(e =>
            {
                e.ToTable("Piso", "Configuration");

                // RN-002: un piso pertenece a un solo hotel.
                e.HasOne(p => p.Hotel)
                 .WithMany(h => h.Pisos)
                 .HasForeignKey(p => p.HotelId)
                 .OnDelete(DeleteBehavior.Restrict);

                // RN-003: no puede haber dos pisos ACTIVOS con el mismo número en el hotel.
                e.HasIndex(p => new { p.HotelId, p.Numero })
                 .IsUnique()
                 .HasFilter("[Estatus] = 1");
            });

            modelBuilder.Entity<Habitacion>(e =>
            {
                e.ToTable("Habitacion", "Configuration");
                e.Property(h => h.Numero).IsRequired().HasMaxLength(10);
                e.Property(h => h.TarifaPorNoche).HasPrecision(10, 2);

                // RN-004: la habitación pertenece a un piso.
                e.HasOne(h => h.Piso)
                 .WithMany(p => p.Habitaciones)
                 .HasForeignKey(h => h.PisoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(h => h.Hotel)
                 .WithMany()
                 .HasForeignKey(h => h.HotelId)
                 .OnDelete(DeleteBehavior.NoAction);

                // RN-005: el número de habitación es único dentro del hotel.
                e.HasIndex(h => new { h.HotelId, h.Numero }).IsUnique();
            });
        }

        private static void ConfigurarReservation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Huesped>(e =>
            {
                e.ToTable("Huesped", "Reservation");
                e.Property(h => h.Nombre).IsRequired().HasMaxLength(100);
                e.Property(h => h.Apellido).IsRequired().HasMaxLength(100);
                e.Property(h => h.Documento).IsRequired().HasMaxLength(20);
                e.Property(h => h.Correo).HasMaxLength(150);
                e.Property(h => h.Telefono).HasMaxLength(20);
                e.Ignore(h => h.NombreCompleto);

                // RN-017: el documento del huésped es único.
                e.HasIndex(h => h.Documento).IsUnique();
            });

            modelBuilder.Entity<Reserva>(e =>
            {
                e.ToTable("Reserva", "Reservation");
                e.Property(r => r.Codigo).IsRequired().HasMaxLength(12);
                e.Property(r => r.PrecioTotal).HasPrecision(12, 2);
                e.Property(r => r.MotivoCancelacion).HasMaxLength(250);

                // Control de concurrencia: evita dos guardados simultáneos sobre la misma reserva.
                e.Property(r => r.RowVersion).IsRowVersion();

                // El objeto de valor RangoFechas se guarda en la misma tabla (FechaEntrada / FechaSalida).
                e.OwnsOne(r => r.Fechas, f =>
                {
                    f.Property(x => x.Entrada).HasColumnName("FechaEntrada").IsRequired();
                    f.Property(x => x.Salida).HasColumnName("FechaSalida").IsRequired();
                });
                e.Navigation(r => r.Fechas).IsRequired();

                e.HasOne(r => r.Huesped)
                 .WithMany(h => h.Reservas)
                 .HasForeignKey(r => r.HuespedId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(r => r.Habitacion)
                 .WithMany()
                 .HasForeignKey(r => r.HabitacionId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(r => r.Codigo).IsUnique();
                e.HasIndex(r => new { r.HabitacionId, r.Estado });
            });

            modelBuilder.Entity<HistorialReserva>(e =>
            {
                e.ToTable("HistorialReserva", "Reservation");
                e.Property(h => h.Accion).IsRequired().HasMaxLength(50);
                e.Property(h => h.Detalle).HasMaxLength(500);

                e.HasOne(h => h.Reserva)
                 .WithMany(r => r.Historial)
                 .HasForeignKey(h => h.ReservaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(h => h.Usuario)
                 .WithMany()
                 .HasForeignKey(h => h.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurarNotification(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notificacion>(e =>
            {
                e.ToTable("Notificacion", "Notification");
                e.Property(n => n.Mensaje).IsRequired().HasMaxLength(1000);
                e.Property(n => n.Error).HasMaxLength(500);

                e.HasOne(n => n.Reserva)
                 .WithMany()
                 .HasForeignKey(n => n.ReservaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(n => n.Estado);
            });
        }

        private static void ConfigurarSecurity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("Usuario", "Security");
                e.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
                e.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(150);
                e.Property(u => u.Correo).HasMaxLength(150);
                e.Property(u => u.ClaveHash).IsRequired().HasMaxLength(256);

                e.HasIndex(u => u.NombreUsuario).IsUnique();
            });
        }
    }
}
