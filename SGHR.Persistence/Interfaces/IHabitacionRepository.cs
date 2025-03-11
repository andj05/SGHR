using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion);
        Task<List<Habitacion>> ObtenerHabitacionesPorNumeroAsync(string Numero);
        Task<List<Habitacion>> ObtenerHabitacionesPorPisoIdAsync(int idPiso);
        Task<List<Habitacion>> ObtenerHabitacionesPorCategoriaIdAsync(int idCategoria);
    }
}
