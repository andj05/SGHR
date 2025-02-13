using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios : AuditEntity
    {
        public int IdServicios { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}