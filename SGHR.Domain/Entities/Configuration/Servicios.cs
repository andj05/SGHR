using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios :BaseEntity<int>
    {
        public required string Nombre { get; set; } 
        public required string Descripcion { get; set; }
        public new bool Estado { get; set; }
    }
}

