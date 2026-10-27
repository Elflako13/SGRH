namespace SGRH.DAL.Models.Objects.Hotel
{
    public class HotelUpdate : HotelBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
