

namespace SGHR.Model.Models
{
    public class TarifasModel
    {
        public int IdTarifa { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public string? Descripcion { get; set; }

        public int? IdHabitacion { get; set; }
    }
}
