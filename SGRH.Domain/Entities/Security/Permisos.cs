namespace SGRH.Domain.Entities.Security
{
    /// <summary>Nombres de las acciones que se controlan por rol (RNF-001).</summary>
    public static class Permisos
    {
        public const string GestionarEstructura = "GestionarEstructura";        // CU-001 a CU-004
        public const string CambiarEstadoHabitacion = "CambiarEstadoHabitacion"; // CU-005
        public const string ConsultarEstructura = "ConsultarEstructura";        // CU-006
        public const string GestionarHuespedes = "GestionarHuespedes";          // CU-007, CU-008
        public const string GestionarReservas = "GestionarReservas";            // CU-009 a CU-013
        public const string ConsultarHistorial = "ConsultarHistorial";          // CU-014
        public const string ReintentarNotificaciones = "ReintentarNotificaciones"; // CU-016
    }
}
