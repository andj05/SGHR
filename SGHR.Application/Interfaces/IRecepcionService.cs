using SGHR.Application.Base;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Application.Interfaces
{
    public interface IRecepcionService : IBaseService<SaveRecepcionDto, UpdateRecepcionDto, RemoveRecepcionDto>
    {
        Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReserva(int idEstadoReserva);
        Task<List<Recepcion>> ObtenerRecepcionesPorClienteId(int idCliente);
        Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionId(int idHabitacion);
        Task<List<Recepcion>> ObtenerRecepcionesPorPrecioInicial(decimal PrecioInicial);
    }
}
