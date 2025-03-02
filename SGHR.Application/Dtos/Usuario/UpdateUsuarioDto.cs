using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Usuario
{
    public class UpdateUsuarioDto : PersonaDto
    {
        public int IdUsuario { get; set; }
        public int? IdRolUsuario { get; set; }
    }
}
