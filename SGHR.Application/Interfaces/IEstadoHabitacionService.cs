using SGHR.Application.Base;
using SGHR.Application.Dtos.EstadoHabitacion;

namespace SGHR.Application.Interfaces
{
    public interface IEstadoHabitacionService : IBaseService<SaveEstadoHabitacionDto,UpdateEstadoHabitacionDto,RemoveEstadoHabitacionDto>
    {
        Task<IEnumerable<EstadoHabitacion>> ObtenerTodosLosEstadosAsync();
        Task<bool> ExisteEstadoHabitacionAsync(int idEstado);
    }
}
