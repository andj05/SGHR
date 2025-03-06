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

        public async Task<OperationResult> GetAll()
        {
            OperationResult operationResult = new OperationResult();

            try
            {
                var tarifas = await _tarifasRepository.GetAllAsync();
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

        public async Task<OperationResult> Save(SaveTarifasDto dto)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                var tarifa = new Tarifas
                {

                    FechaInicio = dto.FechaInicio,
                    FechaFin = dto.FechaFin,
                    PrecioPorNoche = dto.PrecioPorNoche,
                    Descuento = dto.Descuento,
                    Descripcion = dto.Descripcion,
                    IdHabitacion = dto.IdHabitacion,
                    Estado = dto.Estado,
                    CreationUser = 1
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
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdTarifa<= 0)
                    return new OperationResult { Success = false, Message = "ID de Tarifa inválido." };

                var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
                if (tarifa is not Tarifas TarifaData || TarifaData.Deleted)
                    return new OperationResult { Success = false, Message = "Tarifa no encontrado o eliminado." };

                tarifa.FechaInicio = dto.FechaInicio != default ? dto.FechaInicio : tarifa.FechaInicio;
                tarifa.FechaFin = dto.FechaFin != default ? dto.FechaFin : tarifa.FechaFin;
                tarifa.PrecioPorNoche = dto.PrecioPorNoche != default ? dto.PrecioPorNoche : tarifa.PrecioPorNoche;
                tarifa.Descuento = dto.Descuento != default ? dto.Descuento : tarifa.Descuento;
                tarifa.Descripcion = dto.Descripcion ?? tarifa.Descripcion;
                tarifa.IdHabitacion = dto.IdHabitacion != default ? dto.IdHabitacion : tarifa.IdHabitacion;
                tarifa.Estado = dto.Estado != default ? dto.Estado : tarifa.Estado;
                tarifa.ModifyDate = DateTime.Now;
                tarifa.ModifyUser = 1;

                var updateResult = await _tarifasRepository.UpdateEntityAsync(TarifaData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la tarifa.");
                operationResult.Success = false;
                operationResult.Message = "Error al actualizar la tarifa: " + ex.Message;
            }
            return operationResult;
        }


        public async Task<OperationResult> Remove(RemoveTarifasDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                if (dto.IdTarifa <= 0)
                    return new OperationResult { Success = false, Message = "ID de cliente inválido." };

                var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
                if (tarifa is not Tarifas clienteData || clienteData.Deleted)
                    return new OperationResult { Success = false, Message = "Cliente no encontrado o ya eliminado." };

                clienteData.Deleted = true;
                clienteData.DeletedUser = 1;
                clienteData.ModifyDate = DateTime.Now;

                var updateResult = await _tarifasRepository.UpdateEntityAsync(clienteData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa.");
                operationResult.Success = false;
                operationResult.Message = "Error al eliminar la tarifa: " + ex.Message;
            }
            return operationResult;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var tarifas = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifas is not Tarifas tarifasData || !tarifasData.Deleted)
                    return new OperationResult { Success = false, Message = "Tarifa no encontrado o ya activo." };

                tarifasData.Deleted = false;
                tarifasData.ModifyDate = DateTime.Now;
                tarifasData.ModifyUser = 1;

                var updateResult = await _tarifasRepository.UpdateEntityAsync(tarifasData);
                operationResult.Success = updateResult.Success;
                operationResult.Message = updateResult.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa.");
                operationResult.Success = false;
                operationResult.Message = "Error al eliminar la tarifa: " + ex.Message;
            }
            return operationResult;
        }

    }

}
