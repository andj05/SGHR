
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria : BaseEntity<int>
    {

        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
    }
}
