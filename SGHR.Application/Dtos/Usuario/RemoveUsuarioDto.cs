using SGHR.Application.Dtos.Base;

namespace SGHR.Application.Dtos.Usuario
{
    public class RemoveUsuarioDto : DtoBase
    {
        public int IdUsuario { get; set; }
        public bool Removed { get; set; }
    }
}
