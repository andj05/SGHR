using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces
{
    public interface IRecepcionRepository : IBaseRepository<Recepcion>
    {
        Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva);
        Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente);
        Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion);
        Task<List<Recepcion>> ObtenerRecepcionesPorPrecioInicialAsync(decimal PrecioInicial);
    }
}