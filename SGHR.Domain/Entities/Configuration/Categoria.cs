using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria : AuditEntity
    {
        public int IdCategoria { get; set; }
        public string Descripcion {  get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
