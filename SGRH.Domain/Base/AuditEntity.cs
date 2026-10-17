namespace SGRH.Domain.Base
{
    /// <summary>
    /// Clase base de todas las entidades del sistema.
    /// Agrupa el Id y los datos de auditoría para no repetirlos en cada entidad (DRY).
    /// </summary>
    public abstract class AuditEntity
    {
        protected AuditEntity()
        {
            Estatus = true;
            FechaCreacion = DateTime.Now;
        }

        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? UsuarioCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }

        /// <summary>true = activo, false = inactivo.</summary>
        public bool Estatus { get; set; }

        public virtual void Activar()
        {
            Estatus = true;
            MarcarModificacion();
        }

        public virtual void Inactivar()
        {
            Estatus = false;
            MarcarModificacion();
        }

        protected void MarcarModificacion()
        {
            FechaModificacion = DateTime.Now;
        }
    }
}
