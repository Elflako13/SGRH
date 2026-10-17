# SGRH — Sistema de Gestión de Reservas de Hoteles

Proyecto de **Programación II (ITLA)** — Prof. Francis Ramírez
Estudiante: **Gerald Manuel Lara Díaz** (2024-2404)

Solución en **C# / .NET 8** organizada por capas siguiendo la arquitectura de referencia del curso
([francisramirez/SGA](https://github.com/francisramirez/SGA)), con el dominio en el centro (enfoque Clean Architecture).

## Estructura de la solución

```
Solution 'SGRH' (10 proyectos)
├── API
│   └── SGRH.Api                  ASP.NET Core Web API (controladores REST)
├── Application
│   └── SGRH.Application          Casos de uso: Contracts, Services, Dtos, Validations
├── Core
│   ├── SGRH.Domain               Núcleo: entidades y reglas de negocio (no depende de nadie)
│   │   ├── Base                  AuditEntity, OperationResult
│   │   ├── Entities
│   │   │   ├── Configuration     CadenaHotelera, Hotel, Piso, Habitacion
│   │   │   ├── Reservation       Huesped, Reserva, HistorialReserva
│   │   │   ├── Notification      Notificacion
│   │   │   └── Security          Usuario, Permisos
│   │   ├── Repository            IBaseRepository<T> + interfaces por schema
│   │   ├── Enums                 Estados, tipos, canal y rol
│   │   ├── ValueObjects          RangoFechas
│   │   ├── Services              ICalculadoraPrecio, CalculadoraPrecioPorNoche
│   │   └── Exceptions            ReglaNegocioException
│   └── SGRH.Model                Modelos de consulta (vistas con datos combinados)
├── Infraestructure
│   ├── SGRH.Infraestructure      Servicios externos (notificaciones, fecha del sistema)
│   └── SGRH.Persistence          Acceso a datos: Context, repositorios por schema
├── IOC
│   └── SGRH.IOC                  Registro de dependencias (punto de composición)
├── Test
│   ├── SGRH.Application.Test     Pruebas xUnit de las reglas del dominio
│   └── SGRH.Persistence.Test     Pruebas de persistencia (próximas fases)
└── Web
    └── SGRH.Web                  ASP.NET Core MVC
```

Las subcarpetas de `Entities` y `Repository` corresponden a los **schemas** de la base de datos:
`Configuration`, `Reservation`, `Notification` y `Security`.

## Dependencias entre proyectos

| Proyecto | Referencia a |
|---|---|
| SGRH.Domain | — (ninguna) |
| SGRH.Model | — |
| SGRH.Application | Domain, Model |
| SGRH.Persistence | Domain, Model |
| SGRH.Infraestructure | Application, Domain |
| SGRH.IOC | Application, Persistence, Infraestructure |
| SGRH.Api / SGRH.Web | IOC, Application, Model |
| SGRH.Application.Test | Application, Domain |
| SGRH.Persistence.Test | Persistence, Domain |

## Ramas (una por módulo)

| Rama | Contenido |
|---|---|
| `main` | Versión estable |
| `develop` | Integración de los módulos |
| `feature/configuration` | Hotel, pisos y habitaciones (RN-001 a RN-007) |
| `feature/security` | Usuarios y permisos por rol (RNF-001) |
| `feature/reservation` | Huéspedes, reservas, historial y cálculo de precio (RN-008 a RN-013, RN-016, RN-017) |
| `feature/notification` | Notificaciones y reintentos (RN-014, RN-015) |

## Principios aplicados en el dominio

- **SRP:** cada entidad valida solo sus reglas (`Habitacion.EsReservable()`, `Piso.AgregarHabitacion()`, `RangoFechas`).
- **OCP:** `ICalculadoraPrecio` permite otra forma de cobrar sin tocar `Reserva`.
- **ISP:** `IBaseRepository<T>` con lo común y una interfaz pequeña por entidad.
- **DIP:** las capas dependen de interfaces; solo `SGRH.IOC` conoce las clases concretas.
- **DRY:** `AuditEntity` concentra Id y auditoría; `RangoFechas` concentra las reglas de fechas.

## Cómo ejecutar

1. Abrir `SGRH.sln` en Visual Studio 2022 (.NET 8).
2. Compilar: **Compilar → Compilar solución**.
3. Pruebas: **Prueba → Ejecutar todas las pruebas** (12 pruebas de reglas de negocio).
