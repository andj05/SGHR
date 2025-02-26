using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IEstadoHabitacionRepository : IBaseRepository<EstadoHabitacion>
    {
        Task<IEnumerable<EstadoHabitacion>> ObtenerTodosLosEstadosAsync();
        Task<bool> ExisteEstadoHabitacionAsync(int idEstado);
    }
}

