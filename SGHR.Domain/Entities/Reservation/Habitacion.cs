using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Reservation
{
    public sealed class Habitacion : AuditEntity
    {
        public int IdHabitacion { get; set; }
        public int IdPiso { get; set; }
        public int IdCategoria { get; set; }
        public string Numero { get; set; }
        public string? Detalle { get; set; }
        public int IdEstadoHabitacion { get; set; }
        public DateTime FechaCreacion { get; set; }


    }
}