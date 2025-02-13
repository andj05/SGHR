using SGHR.Domain.Base;

public sealed class EstadoHabitacion : AuditEntity
{
    public int IdEstadoHabitacion { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaFin { get; set; } 
}
