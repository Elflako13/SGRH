

IF DB_ID(N'SGRH') IS NULL
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

CREATE TABLE [Configuration].[Piso] (
    [Id] int NOT NULL IDENTITY,
    [Numero] int NOT NULL,
    [HotelId] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UsuarioCreacion] int NULL,
    [FechaModificacion] datetime2 NULL,
    [UsuarioModificacion] int NULL,
    [Estatus] bit NOT NULL,
    CONSTRAINT [PK_Piso] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Piso_Hotel_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Configuration].[Hotel] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Configuration].[Habitacion] (
    [Id] int NOT NULL IDENTITY,
    [Numero] nvarchar(10) NOT NULL,
    [Tipo] int NOT NULL,
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
    CONSTRAINT [FK_Habitacion_Piso_PisoId] FOREIGN KEY ([PisoId]) REFERENCES [Configuration].[Piso] ([Id]) ON DELETE NO ACTION
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

CREATE UNIQUE INDEX [IX_Habitacion_HotelId_Numero] ON [Configuration].[Habitacion] ([HotelId], [Numero]);
GO

CREATE INDEX [IX_Habitacion_PisoId] ON [Configuration].[Habitacion] ([PisoId]);
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

CREATE UNIQUE INDEX [IX_Piso_HotelId_Numero] ON [Configuration].[Piso] ([HotelId], [Numero]) WHERE [Estatus] = 1;
GO

CREATE UNIQUE INDEX [IX_Reserva_Codigo] ON [Reservation].[Reserva] ([Codigo]);
GO

CREATE INDEX [IX_Reserva_HabitacionId_Estado] ON [Reservation].[Reserva] ([HabitacionId], [Estado]);
GO

CREATE INDEX [IX_Reserva_HuespedId] ON [Reservation].[Reserva] ([HuespedId]);
GO

CREATE UNIQUE INDEX [IX_Usuario_NombreUsuario] ON [Security].[Usuario] ([NombreUsuario]);
GO

/* ------------------------------ Datos de prueba ------------------------------ */

DECLARE @Ahora DATETIME2 = SYSDATETIME();

-- Security: usuarios (Rol: 1 = Administrador, 2 = Recepcionista, 3 = Operaciones)
INSERT INTO [Security].[Usuario] (NombreUsuario, NombreCompleto, Correo, ClaveHash, Rol, FechaCreacion, Estatus)
VALUES (N'admin',      N'Administrador del sistema', N'admin@sgrh.com',      CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Admin123*'), 2), 1, @Ahora, 1),
       (N'recepcion',  N'Recepcionista de turno',     N'recepcion@sgrh.com',  CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Recep123*'), 2), 2, @Ahora, 1),
       (N'operaciones',N'Personal de operaciones',    N'operaciones@sgrh.com',CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', N'Oper123*'),  2), 3, @Ahora, 1);

-- Configuration: cadena, hotel, pisos y habitaciones
INSERT INTO [Configuration].[CadenaHotelera] (Nombre, RNC, FechaCreacion, Estatus)
VALUES (N'Cadena Caribe Hoteles', N'130000001', @Ahora, 1);
DECLARE @CadenaId INT = SCOPE_IDENTITY();

INSERT INTO [Configuration].[Hotel] (Nombre, Direccion, Telefono, Correo, CadenaHoteleraId, FechaCreacion, Estatus)
VALUES (N'Hotel Caribe Santo Domingo', N'Av. George Washington #100, Santo Domingo', N'809-555-0100', N'reservas@caribesd.com', @CadenaId, @Ahora, 1);
DECLARE @HotelId INT = SCOPE_IDENTITY();

INSERT INTO [Configuration].[Piso] (Numero, HotelId, FechaCreacion, Estatus)
VALUES (1, @HotelId, @Ahora, 1), (2, @HotelId, @Ahora, 1), (3, @HotelId, @Ahora, 1);

DECLARE @Piso1 INT = (SELECT Id FROM [Configuration].[Piso] WHERE HotelId = @HotelId AND Numero = 1);
DECLARE @Piso2 INT = (SELECT Id FROM [Configuration].[Piso] WHERE HotelId = @HotelId AND Numero = 2);
DECLARE @Piso3 INT = (SELECT Id FROM [Configuration].[Piso] WHERE HotelId = @HotelId AND Numero = 3);

-- Tipo: 1 = Sencilla, 2 = Doble, 3 = Suite, 4 = Familiar | Estado: 1 = Disponible, 3 = Mantenimiento
INSERT INTO [Configuration].[Habitacion] (Numero, Tipo, Capacidad, TarifaPorNoche, Estado, PisoId, HotelId, FechaCreacion, Estatus)
VALUES (N'101', 1, 1, 1800.00, 1, @Piso1, @HotelId, @Ahora, 1),
       (N'102', 2, 2, 2500.00, 1, @Piso1, @HotelId, @Ahora, 1),
       (N'103', 2, 2, 2500.00, 3, @Piso1, @HotelId, @Ahora, 1),
       (N'201', 2, 2, 2700.00, 1, @Piso2, @HotelId, @Ahora, 1),
       (N'202', 4, 5, 4200.00, 1, @Piso2, @HotelId, @Ahora, 1),
       (N'301', 3, 4, 6000.00, 1, @Piso3, @HotelId, @Ahora, 1);

-- Reservation: huésped de ejemplo
INSERT INTO [Reservation].[Huesped] (Nombre, Apellido, Documento, Correo, Telefono, FechaCreacion, Estatus)
VALUES (N'Ana', N'Pérez', N'001-0000000-1', N'ana.perez@correo.com', N'809-555-0123', @Ahora, 1);
GO
