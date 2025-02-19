
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IServiciosRepository : IBaseRepository<Servicios>
    {

        /// Obtiene todos los servicios registrados en el sistema.
        Task<IEnumerable<Servicios>> ObtenerTodosLosServiciosAsync();

        /// Busca un servicio por su nombre.
        Task<Servicios?> ObtenerServicioPorNombreAsync(string nombre);

        /// Cambia el estado de un servicio (Activo/Inactivo).
        Task<bool> ActualizarEstadoServicioAsync(int idServicio, bool estado);

        /// Verifica si un servicio existe en la base de datos.
        Task<bool> ExisteServicioAsync(int idServicio);

        /// Actualiza la descripción de un servicio.
        Task<bool> ActualizarDescripcionServicioAsync(int idServicio, string nuevaDescripcion);
    }
}
