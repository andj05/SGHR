using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;
using System.Linq.Expressions;


namespace SGHR.Persistence.Interfaces
{
    public interface IRecepcionRepository : IBaseRepository<Recepcion>
    {
        Task<List<Recepcion>> ObtenerTodasLasRecepcionesAsync();
        Task<Recepcion> ObtenerRecepcionPorIdAsync(int id);
        Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva);
        Task<List<Recepcion>> ObtenerRecepcionesPorFilterAsync(Expression<Func<Recepcion, bool>> filter);
        Task<OperationResult> GuardarRecepcionAsync(Recepcion recepcion);
        Task<OperationResult> ActualizarRecepcionAsync(Recepcion recepcion);
        Task<OperationResult> BorrarRecepcionAsync(int id);
        Task<bool> ExisteRecepcionAsync(int id);
        Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente);
        Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}