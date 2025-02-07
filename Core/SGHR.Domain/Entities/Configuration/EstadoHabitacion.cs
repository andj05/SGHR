using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Configuration
{
    public sealed class EstadoHabitacion : AuditEntity
    {
        public int IdEstadoHabitacion {  get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
