
namespace SGHR.Application.Dtos.Categorias
{
    public class CategoriasDto :DtoBase
    {
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
