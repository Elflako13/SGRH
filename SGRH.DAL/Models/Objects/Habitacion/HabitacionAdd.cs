namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionAdd : HabitacionBase
    {
        public string? Numero { get; set; }
        public int PisoId { get; set; }
        public int? UsuarioCreacion { get; set; }
    }
}
