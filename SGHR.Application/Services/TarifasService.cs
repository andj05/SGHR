using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Services
{
    public class TarifasService : ITarifasService
    {
        private readonly ITarifasRepository _tarifasRepository;
        private readonly ILogger<TarifasService> _logger;
        private readonly IConfiguration _configuration;

        public TarifasService(ITarifasRepository tarifasRepository,
                              ILogger<TarifasService> logger,
                              IConfiguration configuration)
        {
            _tarifasRepository = tarifasRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> AplicarDescuentoTarifaAsync(int idTarifa, decimal nuevoDescuento)
        {
            OperationResult operationResult = new OperationResult();

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(idTarifa);
                if (tarifa == null)
                {
                    operationResult.Message = _configuration["LoggingMessages:AplicarDescuentoNotFound"];
                    _logger.LogWarning(_configuration["LoggingMessages:AplicarDescuentoNotFound"] + ": {IdTarifa}", idTarifa);
                    return false;
                }

                tarifa.Descuento = nuevoDescuento;
                operationResult = await _tarifasRepository.UpdateEntityAsync(tarifa);

                if (operationResult.Success.HasValue && operationResult.Success.Value)
                {
                    _logger.LogInformation("Descuento aplicado correctamente a la tarifa con ID {IdTarifa}.", idTarifa);
                    return true;
                }
                else
                {
                    _logger.LogError("Error al aplicar el descuento a la tarifa con ID {IdTarifa}.", idTarifa);
                    return false;
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:AplicarDescuentoError"];
                _logger.LogError(_configuration["LoggingMessages:AplicarDescuentoError"] + ": {Exception}", ex.ToString());
                return false;
            }
        }

        public async Task<OperationResult> GetAll()
        {
            OperationResult operationResult = new OperationResult();

            try
            {
                var tarifas = await _tarifasRepository.ObtenerTodasLasTarifasAsync();
                operationResult.Data = tarifas;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:GetAllError"];
                _logger.LogError(_configuration["LoggingMessages:GetAllError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            OperationResult operationResult = new OperationResult();

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa == null)
                {
                    operationResult.Message = _configuration["LoggingMessages:GetByIdNotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = tarifa;
                    operationResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:GetByIdError"];
                _logger.LogError(_configuration["LoggingMessages:GetByIdError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }

        public async Task<IEnumerable<Tarifas>> ObtenerTodasLasTarifasAsync()
        {
            return await _tarifasRepository.ObtenerTodasLasTarifasAsync();
        }

        public async Task<OperationResult> Remove(RemoveTarifasDto dto)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
                if (tarifa == null)
                {
                    operationResult.Message = _configuration["LoggingMessages:RemoveNotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _tarifasRepository.DeleteEntityAsync(tarifa);
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:RemoveError"];
                _logger.LogError(_configuration["LoggingMessages:RemoveError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveTarifasDto dto)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var tarifa = new Tarifas
                {
                    IdTarifa = dto.IdTarifa,
                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin,
                    PrecioPorNoche = dto.PrecioPorNoche,
                    Descuento = dto.Descuento,
                    Descripcion = dto.Descripcion,
                    IdHabitacion = dto.IdHabitacion,
                    Estado = dto.Estado
                };
                operationResult = await _tarifasRepository.SaveEntityAsync(tarifa);
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:SaveError"];
                _logger.LogError(_configuration["LoggingMessages:SaveError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdateTarifasDto dto)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
                if (tarifa == null)
                {
                    operationResult.Message = _configuration["LoggingMessages:UpdateNotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    tarifa.FechaInicio = dto.FechaInicio;
                    tarifa.FechaFin = dto.FechaFin;
                    tarifa.PrecioPorNoche = dto.PrecioPorNoche;
                    tarifa.Descuento = dto.Descuento;
                    tarifa.Descripcion = dto.Descripcion;
                    tarifa.IdHabitacion = dto.IdHabitacion;
                    tarifa.Estado = dto.Estado;
                    operationResult = await _tarifasRepository.UpdateEntityAsync(tarifa);
                }
            }
            catch (Exception ex)
            {
                operationResult.Message = _configuration["LoggingMessages:UpdateError"];
                _logger.LogError(_configuration["LoggingMessages:UpdateError"] + ": {Exception}", ex.ToString());
            }
            return operationResult;
        }

        public async Task<bool> VerificarDisponibilidadTarifaAsync(int idHabitacion, DateOnly fechaInicio, DateOnly fechaFin)
        {
            try
            {
                return await _tarifasRepository.VerificarDisponibilidadTarifaAsync(idHabitacion, fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                _logger.LogError(_configuration["LoggingMessages:VerificarDisponibilidadError"] + ": {Exception}", ex.ToString());
                return false;
            }
        }

    }
}