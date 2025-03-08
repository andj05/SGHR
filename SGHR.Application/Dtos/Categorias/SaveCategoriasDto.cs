

namespace SGHR.Application.Dtos.Categorias
{
    public class SaveCategoriasDto : CategoriasDto
    {
        public int IdCategoria { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
