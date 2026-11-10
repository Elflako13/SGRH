/* =============================================================================
   SGRH - Sistema de Gestión de Reservas de Hoteles
   Script de creación de la base de datos con datos de prueba.
   OJO: si la base de datos SGRH ya existe, se elimina y se vuelve a crear.
   ============================================================================= */
USE [master];
GO

IF DB_ID(N'SGRH') IS NOT NULL
BEGIN
    ALTER DATABASE [SGRH] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [SGRH];
END
GO

CREATE DATABASE [SGRH];
GO

USE [SGRH];
GO

IF SCHEMA_ID(N'Configuration') IS NULL EXEC(N'CREATE SCHEMA [Configuration];');
GO

IF SCHEMA_ID(N'Reservation') IS NULL EXEC(N'CREATE SCHEMA [Reservation];');
GO

IF SCHEMA_ID(N'Notification') IS NULL EXEC(N'CREATE SCHEMA [Notification];');
GO

IF SCHEMA_ID(N'Security') IS NULL EXEC(N'CREATE SCHEMA [Security];');
GO

CREATE TABLE [Configuration].[CadenaHotelera] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [RNC] nvarchar(20) NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_CadenaHotelera] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Reservation].[Huesped] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Apellido] nvarchar(100) NOT NULL,
    [Documento] nvarchar(20) NOT NULL,
    [Correo] nvarchar(150) NULL,
    [Telefono] nvarchar(20) NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Huesped] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Configuration].[TipoHabitacion] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    [Descripcion] nvarchar(250) NULL,
    [TarifaBase] decimal(10,2) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_TipoHabitacion] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Security].[Usuario] (
    [Id] int NOT NULL IDENTITY,
    [NombreUsuario] nvarchar(50) NOT NULL,
    [NombreCompleto] nvarchar(150) NOT NULL,
    [Correo] nvarchar(150) NULL,
    [ClaveHash] nvarchar(256) NOT NULL,
    [Rol] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Configuration].[Hotel] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [Direccion] nvarchar(250) NOT NULL,
    [Telefono] nvarchar(20) NULL,
    [Correo] nvarchar(150) NULL,
    [Ciudad] nvarchar(100) NOT NULL,
    [Estrellas] int NOT NULL,
    [CadenaHoteleraId] int NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Hotel] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Hotel_CadenaHotelera_CadenaHoteleraId] FOREIGN KEY ([CadenaHoteleraId]) REFERENCES [Configuration].[CadenaHotelera] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Configuration].[Bloque] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(250) NULL,
    [HotelId] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Bloque] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bloque_Hotel_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Configuration].[Hotel] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Configuration].[Piso] (
    [Id] int NOT NULL IDENTITY,
    [Numero] int NOT NULL,
    [HotelId] int NOT NULL,
    [BloqueId] int NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Piso] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Piso_Bloque_BloqueId] FOREIGN KEY ([BloqueId]) REFERENCES [Configuration].[Bloque] ([Id]),
    CONSTRAINT [FK_Piso_Hotel_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Configuration].[Hotel] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Configuration].[Habitacion] (
    [Id] int NOT NULL IDENTITY,
    [Numero] nvarchar(10) NOT NULL,
    [TipoHabitacionId] int NOT NULL,
    [Capacidad] int NOT NULL,
    [TarifaPorNoche] decimal(10,2) NOT NULL,
    [Estado] int NOT NULL,
    [PisoId] int NOT NULL,
    [HotelId] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Habitacion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Habitacion_Hotel_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Configuration].[Hotel] ([Id]),
    CONSTRAINT [FK_Habitacion_Piso_PisoId] FOREIGN KEY ([PisoId]) REFERENCES [Configuration].[Piso] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Habitacion_TipoHabitacion_TipoHabitacionId] FOREIGN KEY ([TipoHabitacionId]) REFERENCES [Configuration].[TipoHabitacion] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Reservation].[Reserva] (
    [Id] int NOT NULL IDENTITY,
    [Codigo] nvarchar(12) NOT NULL,
    [HuespedId] int NOT NULL,
    [HabitacionId] int NOT NULL,
    [FechaEntrada] date NOT NULL,
    [FechaSalida] date NOT NULL,
    [CantidadHuespedes] int NOT NULL,
    [PrecioTotal] decimal(12,2) NOT NULL,
    [Estado] int NOT NULL,
    [MotivoCancelacion] nvarchar(250) NULL,
    [RowVersion] rowversion NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Reserva] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reserva_Habitacion_HabitacionId] FOREIGN KEY ([HabitacionId]) REFERENCES [Configuration].[Habitacion] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reserva_Huesped_HuespedId] FOREIGN KEY ([HuespedId]) REFERENCES [Reservation].[Huesped] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Reservation].[HistorialReserva] (
    [Id] int NOT NULL IDENTITY,
    [ReservaId] int NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [UsuarioId] int NOT NULL,
    [Accion] nvarchar(50) NOT NULL,
    [Detalle] nvarchar(500) NULL,
    CONSTRAINT [PK_HistorialReserva] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HistorialReserva_Reserva_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [Reservation].[Reserva] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_HistorialReserva_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Security].[Usuario] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Notification].[Notificacion] (
    [Id] int NOT NULL IDENTITY,
    [ReservaId] int NOT NULL,
    [Tipo] int NOT NULL,
    [Canal] int NOT NULL,
    [Mensaje] nvarchar(1000) NOT NULL,
    [Estado] int NOT NULL,
    [Intentos] int NOT NULL,
    [FechaEnvio] datetime2 NULL,
    [Error] nvarchar(500) NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Notificacion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notificacion_Reserva_ReservaId] FOREIGN KEY ([ReservaId]) REFERENCES [Reservation].[Reserva] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Bloque_HotelId_Nombre] ON [Configuration].[Bloque] ([HotelId], [Nombre]);
GO

CREATE UNIQUE INDEX [IX_Habitacion_HotelId_Numero] ON [Configuration].[Habitacion] ([HotelId], [Numero]);
GO

CREATE INDEX [IX_Habitacion_PisoId] ON [Configuration].[Habitacion] ([PisoId]);
GO

CREATE INDEX [IX_Habitacion_TipoHabitacionId] ON [Configuration].[Habitacion] ([TipoHabitacionId]);
GO

CREATE INDEX [IX_HistorialReserva_ReservaId] ON [Reservation].[HistorialReserva] ([ReservaId]);
GO

CREATE INDEX [IX_HistorialReserva_UsuarioId] ON [Reservation].[HistorialReserva] ([UsuarioId]);
GO

CREATE INDEX [IX_Hotel_CadenaHoteleraId] ON [Configuration].[Hotel] ([CadenaHoteleraId]);
GO

CREATE UNIQUE INDEX [IX_Huesped_Documento] ON [Reservation].[Huesped] ([Documento]);
GO

CREATE INDEX [IX_Notificacion_Estado] ON [Notification].[Notificacion] ([Estado]);
GO

CREATE INDEX [IX_Notificacion_ReservaId] ON [Notification].[Notificacion] ([ReservaId]);
GO

CREATE INDEX [IX_Piso_BloqueId] ON [Configuration].[Piso] ([BloqueId]);
GO

CREATE UNIQUE INDEX [IX_Piso_HotelId_Numero] ON [Configuration].[Piso] ([HotelId], [Numero]) WHERE [Estatus] = 1;
GO

CREATE UNIQUE INDEX [IX_Reserva_Codigo] ON [Reservation].[Reserva] ([Codigo]);
GO

CREATE INDEX [IX_Reserva_HabitacionId_Estado] ON [Reservation].[Reserva] ([HabitacionId], [Estado]);
GO

CREATE INDEX [IX_Reserva_HuespedId] ON [Reservation].[Reserva] ([HuespedId]);
GO

CREATE UNIQUE INDEX [IX_TipoHabitacion_Nombre] ON [Configuration].[TipoHabitacion] ([Nombre]);
GO

CREATE UNIQUE INDEX [IX_Usuario_NombreUsuario] ON [Security].[Usuario] ([NombreUsuario]);
GO


/* ------------------------------ Datos de prueba ------------------------------ */

DECLARE @Ahora DATETIME2 = SYSDATETIME();

-- Security: usuarios (Rol: 1 = Administrador, 2 = Recepcionista, 3 = Operaciones)
INSERT INTO [Security].[Usuario] (NombreUsuario, NombreCompleto, Correo, ClaveHash, Rol, FechaCreacion, Estatus)
VALUES (N'admin',       N'Administrador del sistema', N'admin@sgrh.com',       CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Admin123*'), 2), 1, @Ahora, 1),
       (N'recepcion',   N'Recepcionista de turno',    N'recepcion@sgrh.com',   CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Recep123*'), 2), 2, @Ahora, 1),
       (N'operaciones', N'Personal de operaciones',   N'operaciones@sgrh.com', CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Oper123*'),  2), 3, @Ahora, 1);

-- Configuration: tipos de habitación
INSERT INTO [Configuration].[TipoHabitacion] (Nombre, Descripcion, TarifaBase, FechaCreacion, UsuarioCreacion, Estatus)
VALUES (N'Sencilla', N'Una cama individual, ideal para una persona.',          1800.00, @Ahora, 1, 1),
       (N'Doble',    N'Dos camas o una cama matrimonial para dos personas.',  2500.00, @Ahora, 1, 1),
       (N'Familiar', N'Habitación amplia para familias de hasta 5 personas.', 4200.00, @Ahora, 1, 1),
       (N'Suite',    N'Sala independiente, jacuzzi y vista preferencial.',    6000.00, @Ahora, 1, 1);

DECLARE @Sencilla INT = (SELECT Id FROM [Configuration].[TipoHabitacion] WHERE Nombre = N'Sencilla');
DECLARE @Doble    INT = (SELECT Id FROM [Configuration].[TipoHabitacion] WHERE Nombre = N'Doble');
DECLARE @Familiar INT = (SELECT Id FROM [Configuration].[TipoHabitacion] WHERE Nombre = N'Familiar');
DECLARE @Suite    INT = (SELECT Id FROM [Configuration].[TipoHabitacion] WHERE Nombre = N'Suite');

-- Configuration: cadena y hoteles
INSERT INTO [Configuration].[CadenaHotelera] (Nombre, RNC, FechaCreacion, Estatus)
VALUES (N'Cadena Caribe Hoteles', N'130000001', @Ahora, 1);
DECLARE @CadenaId INT = SCOPE_IDENTITY();

INSERT INTO [Configuration].[Hotel] (Nombre, Direccion, Ciudad, Estrellas, Telefono, Correo, CadenaHoteleraId, FechaCreacion, UsuarioCreacion, Estatus)
VALUES (N'Hotel Caribe Santo Domingo', N'Av. George Washington #100', N'Santo Domingo', 4, N'809-555-0100', N'reservas@caribesd.com',     @CadenaId, @Ahora, 1, 1),
       (N'Hotel Bahía Punta Cana',     N'Playa Bávaro, Carretera Arena Gorda', N'Punta Cana', 5, N'809-555-0200', N'reservas@bahiapc.com', @CadenaId, @Ahora, 1, 1),
       (N'Hotel Cibao Santiago',       N'Av. 27 de Febrero #45',       N'Santiago',      3, N'809-555-0300', N'reservas@cibaostgo.com',    @CadenaId, @Ahora, 1, 1);

DECLARE @HotelSD INT = (SELECT Id FROM [Configuration].[Hotel] WHERE Nombre = N'Hotel Caribe Santo Domingo');
DECLARE @HotelPC INT = (SELECT Id FROM [Configuration].[Hotel] WHERE Nombre = N'Hotel Bahía Punta Cana');
DECLARE @HotelST INT = (SELECT Id FROM [Configuration].[Hotel] WHERE Nombre = N'Hotel Cibao Santiago');

-- Configuration: bloques (torres o edificios de cada hotel)
INSERT INTO [Configuration].[Bloque] (Nombre, Descripcion, HotelId, FechaCreacion, UsuarioCreacion, Estatus)
VALUES (N'Torre A',         N'Torre principal frente al malecón.', @HotelSD, @Ahora, 1, 1),
       (N'Torre B',         N'Torre ejecutiva.',                   @HotelSD, @Ahora, 1, 1),
       (N'Edificio Playa',  N'Frente a la playa.',                 @HotelPC, @Ahora, 1, 1),
       (N'Edificio Jardín', N'Rodeado de jardines y piscina.',     @HotelPC, @Ahora, 1, 1);

DECLARE @TorreA  INT = (SELECT Id FROM [Configuration].[Bloque] WHERE HotelId = @HotelSD AND Nombre = N'Torre A');
DECLARE @TorreB  INT = (SELECT Id FROM [Configuration].[Bloque] WHERE HotelId = @HotelSD AND Nombre = N'Torre B');
DECLARE @Playa   INT = (SELECT Id FROM [Configuration].[Bloque] WHERE HotelId = @HotelPC AND Nombre = N'Edificio Playa');
DECLARE @Jardin  INT = (SELECT Id FROM [Configuration].[Bloque] WHERE HotelId = @HotelPC AND Nombre = N'Edificio Jardín');

-- Configuration: pisos (el bloque es opcional)
INSERT INTO [Configuration].[Piso] (Numero, HotelId, BloqueId, FechaCreacion, UsuarioCreacion, Estatus)
VALUES (1, @HotelSD, @TorreA, @Ahora, 1, 1), (2, @HotelSD, @TorreA, @Ahora, 1, 1), (3, @HotelSD, @TorreB, @Ahora, 1, 1),
       (1, @HotelPC, @Playa,  @Ahora, 1, 1), (2, @HotelPC, @Playa,  @Ahora, 1, 1), (3, @HotelPC, @Jardin, @Ahora, 1, 1),
       (1, @HotelST, NULL,    @Ahora, 1, 1), (2, @HotelST, NULL,    @Ahora, 1, 1);

-- Configuration: habitaciones (Estado: 1 = Disponible, 2 = Limpieza, 3 = Mantenimiento, 4 = Fuera de servicio, 5 = Inactiva)
INSERT INTO [Configuration].[Habitacion] (Numero, TipoHabitacionId, Capacidad, TarifaPorNoche, Estado, PisoId, HotelId, FechaCreacion, UsuarioCreacion, Estatus)
SELECT h.Numero, h.TipoId, h.Capacidad, h.Tarifa, h.Estado, p.Id, p.HotelId, @Ahora, 1, 1
FROM (VALUES
        (@HotelSD, 1, N'101', @Sencilla, 1, 1800.00, 1),
        (@HotelSD, 1, N'102', @Doble,    2, 2500.00, 1),
        (@HotelSD, 1, N'103', @Doble,    2, 2500.00, 3),
        (@HotelSD, 2, N'201', @Doble,    2, 2700.00, 1),
        (@HotelSD, 2, N'202', @Familiar, 5, 4200.00, 2),
        (@HotelSD, 3, N'301', @Suite,    4, 6000.00, 1),
        (@HotelPC, 1, N'101', @Doble,    2, 3800.00, 1),
        (@HotelPC, 1, N'102', @Familiar, 5, 5500.00, 1),
        (@HotelPC, 2, N'201', @Suite,    4, 9500.00, 1),
        (@HotelPC, 3, N'301', @Doble,    2, 3500.00, 4),
        (@HotelST, 1, N'101', @Sencilla, 1, 1500.00, 1),
        (@HotelST, 2, N'201', @Doble,    2, 2100.00, 1)
     ) AS h(HotelId, NumeroPiso, Numero, TipoId, Capacidad, Tarifa, Estado)
JOIN [Configuration].[Piso] p ON p.HotelId = h.HotelId AND p.Numero = h.NumeroPiso;

-- Reservation: huésped de ejemplo
INSERT INTO [Reservation].[Huesped] (Nombre, Apellido, Documento, Correo, Telefono, FechaCreacion, Estatus)
VALUES (N'Ana', N'Pérez', N'001-0000000-1', N'ana.perez@correo.com', N'809-555-0123', @Ahora, 1);
GO
