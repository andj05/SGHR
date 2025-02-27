using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class LogReservas : BaseEntity<int>
    {
        public int IdRecepcion { get; set; }
        public string? EstadoAnterior { get; set; }
        public string? EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.Now;
        public string UsuarioModifico { get; set; } = string.Empty;
    }
}
