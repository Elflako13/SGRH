namespace SGRH.DAL.Models.Objects.TipoHabitacion
{
    public class TipoHabitacionUpdate : TipoHabitacionBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
