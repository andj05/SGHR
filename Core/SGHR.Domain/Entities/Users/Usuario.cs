using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Users
{
    public sealed class Usuario : AuditEntity
    {
        public int IdUsuario {  get; set; }
        public string NombreCompleto { get; set; }
        public string Correo {  get; set; }
        public string Clave { get; set; }
        public DateTime FechaCreacion { get; set; }
    
    }
}
