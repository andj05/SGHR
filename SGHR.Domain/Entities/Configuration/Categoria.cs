using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria : BaseEntity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<Categoria_Servicio> CategoriaServicios { get; set; }
    }
}
