using SGHR.Application.Base;
using SGHR.Application.Dtos.Habitacion;

namespace SGHR.Application.Interfaces
{
    public interface IHabitacionService : IBaseService<SaveHabitacionDto,UpdateHabitacionDto,RemoveHabitacionDto>

    {
        Task<List<HabitacionDto>> ObtenerHabitacionesPorEstadoId(int idEstadoHabitacion);
        Task<List<HabitacionDto>> ObtenerHabitacionesPorNumero(string Numero);
        Task<List<HabitacionDto>> ObtenerHabitacionesPorPisoId(int idPiso);
        Task<List<HabitacionDto>> ObtenerHabitacionesPorCategoriaId(int idCategoria);
    }
}
