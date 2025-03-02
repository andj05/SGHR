using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Users
{
    public sealed class Cliente : BaseEntity<int>
    {
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Telefono { get; set; }
        public string? Nacionalidad { get; set; }
    }
}
