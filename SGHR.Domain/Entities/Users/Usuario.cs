using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Users
{
    public sealed class Usuario : BaseEntity<int>
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int IdRolUsuario { get; set; }
        public string Clave { get; set; } = string.Empty;
    }
}
