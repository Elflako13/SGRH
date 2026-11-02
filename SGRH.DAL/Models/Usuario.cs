using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
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
            return Estatus && PuedeRealizar(Rol, accion);
        }

        /// <summary>RNF-001: acciones permitidas para cada rol.</summary>
        public static bool PuedeRealizar(Rol rol, string accion)
        {
            return rol switch
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
