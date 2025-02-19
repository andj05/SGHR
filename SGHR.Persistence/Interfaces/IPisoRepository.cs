using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        /// Obtiene todos los pisos registrados en el sistema.
        Task<IEnumerable<Piso>> ObtenerTodosLosPisosAsync();

        /// Busca un piso por su descripción.
        Task<Piso?> ObtenerPisoPorDescripcionAsync(string descripcion);

        /// Cambia el estado de un piso (Activo/Inactivo).
        Task<bool> ActualizarEstadoPisoAsync(int idPiso, bool estado);

        /// Verifica si un piso existe en la base de datos.
        Task<bool> ExistePisoAsync(int idPiso);
        
        /// Actualiza la descripción de un piso.
        Task<bool> ActualizarDescripcionPisoAsync(int idPiso, string nuevaDescripcion);
    }
}
