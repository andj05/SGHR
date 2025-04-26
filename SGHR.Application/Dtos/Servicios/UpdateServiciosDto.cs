
namespace SGHR.Application.Dtos.Servicios
{
    public class UpdateServiciosDto : ServiciosDto
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
