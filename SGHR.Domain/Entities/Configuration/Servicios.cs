using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Servicios : BaseEntity<int>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        public ICollection<Categoria_Servicio> CategoriaServicios { get; set; }
    }
}
