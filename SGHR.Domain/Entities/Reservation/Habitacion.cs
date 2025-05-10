using SGHR.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SGHR.Domain.Entities.Reservation
{
    public sealed class Habitacion : BaseEntity<int>
    {
        [Column("IdHabitacion")]
        [Key]
        public override int Id { get; set; }
        public int IdPiso { get; set; }
        public int IdCategoria { get; set; }
        public string Numero { get; set; }
        public string? Detalle { get; set; }
        public int IdEstadoHabitacion { get; set; }
    }
}