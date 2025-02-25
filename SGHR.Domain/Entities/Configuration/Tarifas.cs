namespace SGHR.Domain.Entities.Configuration
{
    using SGHR.Domain.Base;

    public sealed class Tarifas : AuditEntity
    {
        public int IdTarifa { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public required string Descripcion { get; set; }
        public int IdHabitacion { get; set; }
        public bool Estado { get; set; }
    }
}