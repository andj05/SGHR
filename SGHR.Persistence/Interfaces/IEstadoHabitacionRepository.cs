
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IEstadoHabitacionRepository : IBaseRepository<EstadoHabitacion>
    {
        Task<IEnumerable<EstadoHabitacion>> ObtenerTodosLosEstadosAsync();
        Task<IEnumerable<EstadoHabitacion>> ObtenerEstadosActivosAsync();
        Task<EstadoHabitacion?> ObtenerEstadoPorDescripcionAsync(string descripcion);
        Task<bool> ActualizarEstadoAsync(int idEstado, bool estado);
        Task<bool> ExisteEstadoHabitacionAsync(int idEstado);
        Task<bool> ActualizarDescripcionEstadoAsync(int idEstado, string nuevaDescripcion);
    }
}

