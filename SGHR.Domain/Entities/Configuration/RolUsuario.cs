using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class RolUsuario : AuditEntity
    {
        public int IdRolUsuario {  get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
