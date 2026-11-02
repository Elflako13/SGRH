# SGRH — Sistema de Gestión de Reservas de Hoteles

Proyecto de **Programación II (ITLA)** — Prof. Francis Ramírez
Estudiante: **Gerald Manuel Lara Díaz** (2024-2404)

Aplicación en **C# / .NET 8** con arquitectura en **N capas**, organizada en 4 proyectos para
mantenerla simple (principio **KISS**).

## Estructura de la solución

```
Solution 'SGRH' (4 proyectos)
├── SGRH.BL                     Capa de lógica de negocio
│   ├── Configuration           ReservaOptions y NotificacionOptions (appsettings.json)
│   ├── Services                Servicios con la lógica de negocio (I...Service y ...Service)
│   ├── ApplicationSetup.cs     AddApplication(): opciones + servicios + repositorios
│   └── DependencyInjection.cs  AddBusinessLayer(): registra los servicios y la DAL
├── SGRH.BL.Tests               Pruebas xUnit (reglas, repositorios y servicios)
├── SGRH.DAL                    Capa de acceso a datos
│   ├── Data                    SGRHDbContext (Entity Framework Core)
│   ├── Models
│   │   ├── Base                AuditEntity, ReglaNegocioException
│   │   ├── Objects             DTOs por entidad: Add, Base, Model, Remove, Update
│   │   ├── Result              OperationResult<T>
│   │   └── *.cs                Entidades, enumeraciones y RangoFechas
│   ├── Repositories            I...Repository y ...Repository
│   └── DependencyInjection.cs  AddDataAccessLayer()
└── SGRH.Web                    Capa de presentación (ASP.NET Core MVC)
```

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| Presentación | SGRH.Web | Interfaz para el personal del hotel. |
| Lógica de negocio | SGRH.BL | Servicios con las reglas de los casos de uso: disponibilidad, reservas, historial, notificaciones, inicio de sesión y permisos. |
| Acceso a datos | SGRH.DAL | Entidades, contexto de EF Core y repositorios. |

Dependencias: `SGRH.Web → SGRH.BL → SGRH.DAL`.

## Base de datos

1. Abrir `Database/SGRH.sql` en SQL Server Management Studio y ejecutarlo (F5). Crea la base de datos
   `SGRH` con los schemas Configuration, Reservation, Notification y Security, y datos de prueba.
2. Si hace falta, cambiar `Server=localhost` en la cadena `SGRHDb` de `SGRH.Web/appsettings.json`
   por el nombre del servidor que aparece en SSMS.

Las reglas configurables están en `SGRH.Web/appsettings.json`:

```json
"ReservaOptions": { "MaximoNoches": 30, "DiasMaximoAnticipacion": 365 },
"NotificacionOptions": { "CanalPorDefecto": "Correo", "Remitente": "Hotel Caribe Santo Domingo" }
```

## Cómo ejecutar

1. Abrir `SGRH.sln` en Visual Studio 2022 (.NET 8).
2. **Compilar → Compilar solución**.
3. **Prueba → Ejecutar todas las pruebas**.
