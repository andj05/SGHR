using SGHR.Application.Base;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface IRolUsuarioService : IBaseService<SaveRolUsuarioDto,UpdateRolUsuarioDto,RemoveRolUsuarioDto>
    {
        /// Obtiene todos los roles de usuario registrados en el sistema.
        Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync();

        /// Verifica si un rol de usuario existe en la base de datos.
        Task<bool> ExisteRolUsuarioAsync(int idRolUsuario);

    }
}
