using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Users
{
    public sealed class Usuario : AuditEntity
    {
        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public int? IdRolUsuario { get; set; }
        public string? Clave { get; set; }
        public bool? Estado { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
