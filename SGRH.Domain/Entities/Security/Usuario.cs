using SGRH.Domain.Base;
using SGRH.Domain.Enums;

namespace SGRH.Domain.Entities.Security
{
    /// <summary>Persona que usa el sistema (Administrador, Recepcionista u Operaciones).</summary>
    public sealed class Usuario : AuditEntity
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string ClaveHash { get; set; } = string.Empty;
        public Rol Rol { get; set; }

        private static readonly string[] PermisosRecepcion =
        {
            Permisos.ConsultarEstructura,
            Permisos.GestionarHuespedes,
            Permisos.GestionarReservas,
            Permisos.ConsultarHistorial
        };

        /// <summary>RNF-001: indica si el rol del usuario permite realizar la acción.</summary>
        public bool TienePermiso(string accion)
        {
            if (!Estatus)
                return false;

            return Rol switch
            {
                Rol.Administrador => true,
                Rol.Recepcionista => PermisosRecepcion.Contains(accion),
                Rol.Operaciones => accion == Permisos.CambiarEstadoHabitacion
                                   || accion == Permisos.ConsultarEstructura,
                _ => false
            };
        }
    }
}
