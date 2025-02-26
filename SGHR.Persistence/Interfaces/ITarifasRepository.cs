using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

public interface ITarifasRepository : IBaseRepository<Tarifas>
{
    Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync();
    Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin);
    Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento);
}

