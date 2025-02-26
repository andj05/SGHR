using SGHR.Application.Base;
using SGHR.Application.Dtos.Servicios;  
using SGHR.Domain.Entities.Configuration;  

namespace SGHR.Application.Interfaces
{
    public interface IServiciosService : IBaseService<SaveServiciosDto, UpdateServiciosDto, RemoveServiciosDto>
    {
        /// Obtiene todos los servicios registrados en el sistema.
        Task<IEnumerable<Servicios>> ObtenerTodosLosServiciosAsync();

        /// Cambia el estado de un servicio (Activo/Inactivo).
        Task<bool> ActualizarEstadoServicioAsync(int idServicio, bool estado);

        /// Verifica si un servicio existe en la base de datos.
        Task<bool> ExisteServicioAsync(int idServicio);
    }
}
