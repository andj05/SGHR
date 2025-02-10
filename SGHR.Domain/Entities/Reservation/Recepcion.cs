using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Reservation
{
    public sealed class Recepcion : AuditEntity
    {
        public int IdRecepcion { get; set; }
        public int? IdEstadoReserva { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime? FechaSalidaConfirmacion { get; set; }
        public decimal? PrecioInicial { get; set; }
        public decimal? Adelanto { get; set; }
        public decimal? PrecioRestante { get; set; }
        public decimal? TotalPagado { get; set; }
        public decimal? CostoPenalidad { get; set; }
        public string? Observacion { get; set; }

    }
}