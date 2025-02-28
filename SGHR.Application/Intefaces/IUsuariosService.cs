using SGHR.Application.Base;
using SGHR.Application.Dtos.Usuario;

namespace SGHR.Application.Intefaces
{
    public interface IUsuariosService : IBaseService<SaveUsuarioDto, UpdateUsuarioDto, RemoveUsuarioDto>
    {
    }
}
