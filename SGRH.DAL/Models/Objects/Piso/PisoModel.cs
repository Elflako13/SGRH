namespace SGRH.DAL.Models.Objects.Piso
{
    public class PisoModel
    {
        public int PisoId { get; set; }
        public int Numero { get; set; }
        public int HotelId { get; set; }
        public string Hotel { get; set; } = string.Empty;
    }
}
