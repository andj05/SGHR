
namespace SGHR.Application.Dtos.Servicios
{
    public class ServiciosDto : DtoBase
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
