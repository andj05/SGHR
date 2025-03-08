
namespace SGHR.Application.Dtos.EstadoHabitacion
{
    public class UpdateEstadoHabitacionDto : EstadoHabitacionDto
    {
        public int IdEstadoHabitacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
