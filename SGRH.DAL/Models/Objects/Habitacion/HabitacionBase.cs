namespace SGRH.DAL.Models.Objects.Habitacion
{
    public class HabitacionBase
    {
        public TipoHabitacion Tipo { get; set; }
        public int Capacidad { get; set; }
        public decimal TarifaPorNoche { get; set; }
    }
}
