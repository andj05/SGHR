namespace SGHR.Application.Dtos.Habitacion
{
    public class HabitacionDto : DtoBase
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
        public int IdPiso { get; set; }
        public int IdCategoria { get; set; }
        public string Numero { get; set; }
        public string? Detalle { get; set; }
        public int IdEstadoHabitacion { get; set; }
    }
}
