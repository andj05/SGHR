using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria_Servicio : BaseEntity<int>
    {
        public int IdCategoria { get; set; }
        public int IdServicio { get; set; }

        // Relaciones
        public Categoria Categoria { get; set; }
        public Servicios Servicio { get; set; }
    }
}
