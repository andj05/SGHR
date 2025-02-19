using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IRolUsuarioRepository : IBaseRepository<RolUsuario>
    {
        /// Obtiene todos los roles de usuario registrados en el sistema.
        Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync();

        /// Busca un rol por su descripción.
        Task<RolUsuario?> ObtenerRolPorDescripcionAsync(string descripcion);

        /// Cambia el estado de un rol de usuario (Activo/Inactivo).
        Task<bool> ActualizarEstadoRolAsync(int idRolUsuario, bool estado);

        /// Verifica si un rol de usuario existe en la base de datos.
        Task<bool> ExisteRolUsuarioAsync(int idRolUsuario);

        /// Actualiza la descripción de un rol de usuario.
        Task<bool> ActualizarDescripcionRolAsync(int idRolUsuario, string nuevaDescripcion);

        /// Elimina un rol de usuario por su ID.
        Task<OperationResult> EliminarRolAsync(int idRolUsuario);
    }
}
