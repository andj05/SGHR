using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IPisoRepository : IBaseRepository<Piso>
    {
        /// Obtiene todos los pisos registrados en el sistema.
        Task<IEnumerable<Piso>> ObtenerTodosLosPisosAsync();

        /// Verifica si un piso existe en la base de datos.
        Task<bool> ExistePisoAsync(int idPiso);
        
    }
}
