using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class Usuario : AuditEntity
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string ClaveHash { get; set; } = string.Empty;
        public Rol Rol { get; set; }

        public int? HuespedId { get; set; }
        public Huesped? Huesped { get; set; }

        private static readonly string[] PermisosRecepcion =
        {
            Permisos.ConsultarEstructura,
            Permisos.GestionarHuespedes,
            Permisos.GestionarReservas,
            Permisos.ConsultarHistorial
        };

        public bool TienePermiso(string accion)
        {
            return Estatus && PuedeRealizar(Rol, accion);
        }

        public static bool PuedeRealizar(Rol rol, string accion)
        {
            return rol switch
            {
                Rol.Administrador => true,
                Rol.Recepcionista => PermisosRecepcion.Contains(accion),
                Rol.Operaciones => accion == Permisos.CambiarEstadoHabitacion
                                   || accion == Permisos.ConsultarEstructura,
                Rol.Cliente => accion == Permisos.ReservarEnLinea,
                _ => false
            };
        }
    }
}
