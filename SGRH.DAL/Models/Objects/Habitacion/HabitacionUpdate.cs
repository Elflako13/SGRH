namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionUpdate : HabitacionBase
    {
        public int Id { get; set; }
        public int? UsuarioModificacion { get; set; }
    }
}
