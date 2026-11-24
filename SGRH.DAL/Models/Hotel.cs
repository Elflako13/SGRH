using SGRH.DAL.Models.Base;

namespace SGRH.DAL.Models
{
    public sealed class Hotel : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string Ciudad { get; set; } = string.Empty;

        public int Estrellas { get; set; } = 3;

        public int? CadenaHoteleraId { get; set; }
        public CadenaHotelera? CadenaHotelera { get; set; }

        public List<Piso> Pisos { get; set; } = new();
        public List<Bloque> Bloques { get; set; } = new();

        public Piso AgregarPiso(int numero)
        {
            if (numero <= 0)
                throw new ReglaNegocioException("El número de piso debe ser mayor que cero.");

            if (Pisos.Any(p => p.Estatus && p.Numero == numero))
                throw new ReglaNegocioException($"Ya existe un piso activo con el número {numero} en este hotel (RN-003).");

            var piso = new Piso { Numero = numero, HotelId = Id, Hotel = this };
            Pisos.Add(piso);
            MarcarModificacion();
            return piso;
        }

        public bool ExisteNumeroHabitacion(string numero)
        {
            return Pisos.SelectMany(p => p.Habitaciones)
                        .Any(h => h.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));
        }

        public Habitacion? BuscarHabitacion(string numero)
        {
            return Pisos.SelectMany(p => p.Habitaciones)
                        .FirstOrDefault(h => h.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));
        }
    }
}
