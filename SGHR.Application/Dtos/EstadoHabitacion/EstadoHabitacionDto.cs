
namespace SGHR.Application.Dtos.EstadoHabitacion
{
    public class EstadoHabitacionDto : DtoBase
    {
        public int IdEstadoHabitacion { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
