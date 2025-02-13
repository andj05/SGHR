using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> ObtenerTodasLasHabitacionesAsync();
        Task<Habitacion> ObtenerHabitacionPorIdAsync(int id);
        Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion);
        Task<List<Habitacion>> ObtenerHabitacionesPorFilterAsync(Expression<Func<Habitacion, bool>> filter);
        Task<OperationResult> GuardarHabitacionAsync(Habitacion habitacion);
        Task<OperationResult> ActualizarHabitacionAsync(Habitacion habitacion);
        Task<bool> ExisteHabitacionAsync(int id);
    }
}
