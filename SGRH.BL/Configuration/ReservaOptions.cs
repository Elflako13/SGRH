namespace SGRH.BL.Configuration
{
    public class ReservaOptions
    {
        public const string Seccion = "ReservaOptions";

        public int MaximoNoches { get; set; } = 30;

        public int DiasMaximoAnticipacion { get; set; } = 365;
    }
}
