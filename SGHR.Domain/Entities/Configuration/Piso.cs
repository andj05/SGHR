using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Piso : AuditEntity
    {
        public int IdPiso { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}