namespace SGHR.WebApi.Models.Categorias
{
    public class CategoriasApiModel
    {
        public int IdCategoria { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
