using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios : AuditEntity
    {
     
        public int IdServicio { get; set; } 
        public required string Nombre { get; set; } 
        public required string Descripcion { get; set; }
        public new bool Estado { get; set; } 
    }
}

