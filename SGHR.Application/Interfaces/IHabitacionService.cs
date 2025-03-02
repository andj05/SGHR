using SGHR.Application.Base;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Application.Interfaces
{
    public interface IHabitacionService : IBaseService<SaveHabitacionDto,UpdateHabitacionDto,RemoveHabitacionDto>

    {
        Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion);
    }
}
