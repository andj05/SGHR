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
            if (dto.IdTarifa <= 0)
                return new OperationResult { Success = false, Message = "ID de tarifa inválido." };

            var tarifaResult = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (tarifaResult?.Data is not Tarifas tarifa || tarifa.Deleted)
                return new OperationResult { Success = false, Message = "Tarifa no encontrada o eliminada." };

            tarifa.FechaInicio = dto.FechaInicio != default ? dto.FechaInicio : tarifa.FechaInicio;
            tarifa.FechaFin = dto.FechaFin != default ? dto.FechaFin : tarifa.FechaFin;
            tarifa.PrecioPorNoche = dto.PrecioPorNoche != default ? dto.PrecioPorNoche : tarifa.PrecioPorNoche;
            tarifa.Descuento = dto.Descuento != default ? dto.Descuento : tarifa.Descuento;
            tarifa.Descripcion = dto.Descripcion ?? tarifa.Descripcion;
            tarifa.IdHabitacion = dto.IdHabitacion != default ? dto.IdHabitacion : tarifa.IdHabitacion;
            tarifa.Estado = dto.Estado != default ? dto.Estado : tarifa.Estado;
            tarifa.ModifyDate = DateTime.Now;
            tarifa.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _tarifasRepository.UpdateEntityAsync(tarifa);
        }

        public async Task<OperationResult> Remove(RemoveTarifasDto dto)
        {
            if (dto.IdTarifa <= 0)
                return new OperationResult { Success = false, Message = "ID de la Tarifa es  inválido." };

            var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (tarifa.Data is not Tarifas tarifaData || tarifaData.Deleted)
                return new OperationResult { Success = false, Message = "Tarifa no encontrado o ya eliminado." };

            tarifaData.Deleted = true;
            tarifaData.DeletedUser = 1;
            tarifaData.ModifyDate = DateTime.Now;

            return await _tarifasRepository.UpdateEntityAsync(tarifaData);
        }


        public async Task<OperationResult> Restore(int id)
        {
            var tarifas = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifas.Data is not Tarifas tarifasData || !tarifasData.Deleted)
                return new OperationResult { Success = false, Message = "Tarifa no encontrado o ya activo." };

            tarifasData.Deleted = false;
            tarifasData.ModifyDate = DateTime.Now;
            tarifasData.ModifyUser = 1;

            return await _tarifasRepository.UpdateEntityAsync(tarifasData);
        }

        public async Task<OperationResult> DeletePermanent(int id)
        {
            if (id <= 0)
                return new OperationResult { Success = false, Message = "ID de cliente inválido." };

            var tarifas = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifas.Data is not Tarifas tarifasData)
                return new OperationResult { Success = false, Message = "Cliente no encontrado." };

            return await _tarifasRepository.DeleteEntityAsync(id);
        }

    }
}