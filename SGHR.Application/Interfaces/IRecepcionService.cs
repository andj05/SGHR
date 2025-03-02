using SGHR.Application.Base;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Domain.Entities.Reservation;

namespace SGHR.Application.Interfaces
{
    public interface IRecepcionService : IBaseService<SaveRecepcionDto, UpdateRecepcionDto, RemoveRecepcionDto>
    {
        Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva);
        Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente);
        Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
