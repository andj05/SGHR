using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Tarifas : AuditEntity
    {
        public int IdTarifa { get; set; }
        public DateOnly FechaInicio {  get; set; }
        public DateOnly FechaFin {  get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public string Descripcion { get; set; }
        
    }
}
