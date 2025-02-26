
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IServiciosRepository : IBaseRepository<Servicios>
    {

        /// Obtiene todos los servicios registrados en el sistema.
        Task<IEnumerable<Servicios>> ObtenerTodosLosServiciosAsync();

        /// Cambia el estado de un servicio (Activo/Inactivo).
        Task<bool> ActualizarEstadoServicioAsync(int idServicio, bool estado);

        /// Verifica si un servicio existe en la base de datos.
        Task<bool> ExisteServicioAsync(int idServicio);

    }
}
