namespace SGRH.DAL.Models.Objects.Piso
{
    public class PisoUpdate : PisoBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
