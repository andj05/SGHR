using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IRolUsuarioRepository : IBaseRepository<RolUsuario>
    {
        /// Obtiene todos los roles de usuario registrados en el sistema.
        Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync();

        /// Verifica si un rol de usuario existe en la base de datos.
        Task<bool> ExisteRolUsuarioAsync(int idRolUsuario);

        /// Elimina un rol de usuario por su ID.
        Task<OperationResult> EliminarRolAsync(int idRolUsuario);
    }
}
