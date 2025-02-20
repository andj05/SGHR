using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;
using System.Linq.Expressions;

namespace SGHR.Persistence.Interfaces
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion);
    }
}
