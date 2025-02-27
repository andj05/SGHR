using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Reportes : BaseEntity<int>
    {
        public int IdUsuario { get; set; }
        public string TipoReporte { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "Pendiente"; // Valores: Pendiente, En Proceso, Resuelto
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaRespuesta { get; set; }
        public string? Respuesta { get; set; }
        public int? IdUsuarioRespondio { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; }
        public Usuario? UsuarioRespondio { get; set; }
    }
}
