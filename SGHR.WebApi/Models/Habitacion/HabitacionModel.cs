namespace SGHR.WebApi.Models.Habitacion
{
    public class HabitacionModel
    {
        public int Id { get; set; }
        public bool Estado { get; set; }
        public int IdPiso { get; set; }
        public int IdCategoria { get; set; }
        public string Numero { get; set; }
        public string? Detalle { get; set; }
        public int IdEstadoHabitacion { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? ChangeUser { get; set; }
    }
}
