using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Usuario
{
    public class UsuarioDto : DtoBase
    {
        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Telefono { get; set; }
        public int? IdRolUsuario { get; set; }
        public bool Estado { get; set; }
        public bool Delete { get; set; }
        public bool Deleted { get; set; }
    }
}