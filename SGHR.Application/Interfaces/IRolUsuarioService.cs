using SGHR.Application.Base;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface IRolUsuarioService : IBaseService<SaveRolUsuarioDto,UpdateRolUsuarioDto,RemoveRolUsuarioDto>
    {


    }
}
