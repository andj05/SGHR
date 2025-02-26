using SGHR.Application.Base;
using SGHR.Application.Dtos.Pisos;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface IPisosService : IBaseService<SavePisosDto, UpdatePisosDto, RemovePisosDto>
    {
        /// Obtiene todos los pisos registrados en el sistema.
        Task<IEnumerable<Piso>> ObtenerTodosLosPisosAsync();

        /// Verifica si un piso existe en la base de datos.
        Task<bool> ExistePisoAsync(int idPiso);
    }
}
