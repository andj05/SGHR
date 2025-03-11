using SGHR.Application.Base;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Application.Interfaces
{
    public interface IHabitacionService : IBaseService<SaveHabitacionDto,UpdateHabitacionDto,RemoveHabitacionDto>

    {
        Task<List<Habitacion>> ObtenerHabitacionesPorEstadoId(int idEstadoHabitacion);
        Task<List<Habitacion>> ObtenerHabitacionesPorNumero(string Numero);
        Task<List<Habitacion>> ObtenerHabitacionesPorPisoId(int idPiso);
        Task<List<Habitacion>> ObtenerHabitacionesPorCategoriaId(int idCategoria);
    }
}
