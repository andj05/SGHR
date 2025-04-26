using SGHR.Domain.Base;

public sealed class EstadoHabitacion : BaseEntity<int>
{
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
}