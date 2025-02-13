using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

public interface ITarifasRepository : IBaseRepository<Tarifas>
{
    Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync();
    Task<IEnumerable<Tarifas>> ObtenerTarifasActivasAsync();
    Task<IEnumerable<Tarifas?>> ObtenerTarifasPorHabitacionAsync(int idHabitacion);
    Task<IEnumerable<Tarifas>> ObtenerTarifasPorRangoDeFechasAsync(DateOnly fechaInicio, DateOnly fechaFin);
    Task<bool> ActualizarEstadoTarifaAsync(int idTarifa, bool estado);
    Task<Tarifas?> ObtenerTarifaActualPorHabitacionAsync(int idHabitacion); 
    Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin);
    Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento);
}


