
namespace SGHR.Model.Models
{
    public class HabitacionModel
    {
        public int IdHabitacion { get; set; }
        public string Numero { get; set; }
        public string? Detalle { get; set; }
        public int IdEstadoHabitacion { get; set; }
        public int IdPiso { get; set; }
        public int IdCategoria { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
