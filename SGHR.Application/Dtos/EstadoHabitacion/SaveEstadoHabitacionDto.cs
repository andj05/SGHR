
namespace SGHR.Application.Dtos.EstadoHabitacion
{
    public class SaveEstadoHabitacionDto : EstadoHabitacionDto
    {
        public int IdEstadoHabitacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
