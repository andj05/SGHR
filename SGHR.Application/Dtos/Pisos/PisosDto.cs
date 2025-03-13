namespace SGHR.Application.Dtos.Pisos
{
    public class PisosDto : DtoBase
    {
        public int IdPiso { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
