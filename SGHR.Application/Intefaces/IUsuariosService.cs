using SGHR.Application.Base;
using SGHR.Application.Dtos.Usuario;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Users;

namespace SGHR.Application.Intefaces
{
    public interface IUsuariosService : IBaseService<SaveUsuarioDto, UpdateUsuarioDto, RemoveUsuarioDto>
    {
        Task<OperationResult> Login(LoginRequestDto loginRequest);
    }
}
