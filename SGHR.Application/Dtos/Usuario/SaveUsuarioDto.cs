using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Usuario
{
    public class SaveUsuarioDto : PersonaDto
    {
        public int IdRolUsuario { get; set; }
        public string Clave { get; set; } = string.Empty;
    }
}
