namespace SGRH.BL.Configuration
{
    /// <summary>
    /// Parámetros de negocio de las reservas. Se leen de la sección "ReservaOptions"
    /// de appsettings.json, así se pueden cambiar sin recompilar.
    /// </summary>
    public class ReservaOptions
    {
        public const string Seccion = "ReservaOptions";

        /// <summary>Cantidad máxima de noches en una sola reserva.</summary>
        public int MaximoNoches { get; set; } = 30;

        /// <summary>Con cuántos días de anticipación como máximo se puede reservar.</summary>
        public int DiasMaximoAnticipacion { get; set; } = 365;
    }
}
