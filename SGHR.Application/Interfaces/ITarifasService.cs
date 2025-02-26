
using SGHR.Application.Base;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Interfaces
{
    public interface ITarifasService : IBaseService<SaveTarifasDto, UpdateTarifasDto, RemoveTarifasDto>
    {
        Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync();
        Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin);
        Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento);
    }
}
