using SGHR.Application.Base;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Application.Interfaces
{
    public interface IRecepcionService : IBaseService<SaveRecepcionDto, UpdateRecepcionDto, RemoveRecepcionDto>
    {
        Task<List<RecepcionDto>> ObtenerRecepcionesPorEstadoReserva(int idEstadoReserva);
        Task<List<RecepcionDto>> ObtenerRecepcionesPorClienteId(int idCliente);
        Task<List<RecepcionDto>> ObtenerRecepcionesPorHabitacionId(int idHabitacion);
        Task<List<RecepcionDto>> ObtenerRecepcionesPorPrecioInicial(decimal PrecioInicial);
    }
}
