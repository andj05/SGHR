using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;
using System.Linq.Expressions;


namespace SGHR.Persistence.Interfaces
{
    public interface IRecepcionRepository : IBaseRepository<Recepcion>
    {
        Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva);
        Task<OperationResult> BorrarRecepcionAsync(int id);
        Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente);
        Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}