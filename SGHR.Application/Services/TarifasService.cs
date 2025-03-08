using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;

namespace SGHR.Application.Services
{
    public class TarifasService : ITarifasService
    {
        private readonly ITarifasRepository _tarifasRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public TarifasService(ITarifasRepository tarifasRepository,
                              IConfiguration configuration,
                              MessageMapper messageMapper,
                              ILoggerManager loggerManager)
        {
            _tarifasRepository = tarifasRepository;
            _configuration = configuration;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
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
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Success = false;
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa == null)
                {
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
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
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveTarifasDto dto)
        {
          
            var validationResult = ValidateTarifas(dto);
            if (validationResult.Success != true)
                return validationResult;

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
                    FechaCreacion = DateTime.UtcNow, // Agregado para consistencia
                    CreationUser = 1
                };


                return await _tarifasRepository.SaveEntityAsync(tarifa);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = $"{_messageMapper.ErrorMessages["Operations"]["SaveFailed"]}: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> Update(UpdateTarifasDto dto)
        {
            if (dto.IdTarifa <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (tarifa == null || tarifa.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            tarifa.FechaInicio = dto.FechaInicio;
            tarifa.FechaFin = dto.FechaFin;
            tarifa.PrecioPorNoche = dto.PrecioPorNoche;
            tarifa.Descuento = dto.Descuento;
            tarifa.Descripcion = dto.Descripcion;
            tarifa.IdHabitacion = dto.IdHabitacion;
            tarifa.Estado = dto.Estado;
            tarifa.FechaCreacion = DateTime.UtcNow;
            tarifa.CreationUser = 1;

            var result = await _tarifasRepository.UpdateEntityAsync(tarifa);
            return result;
        }

        public async Task<OperationResult> Remove(RemoveTarifasDto dto)
        {
            if (dto.IdTarifa <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var tarifa = await _tarifasRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (tarifa == null || tarifa.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            tarifa.Deleted = true;
            tarifa.DeletedUser = 1; // En producción, obtener el usuario autenticado.
            tarifa.ModifyDate = DateTime.Now;

            var result = await _tarifasRepository.UpdateEntityAsync(tarifa);
            return result;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifa == null || !tarifa.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            tarifa.Deleted = false;
            tarifa.ModifyDate = DateTime.Now;
            tarifa.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _tarifasRepository.UpdateEntityAsync(tarifa);
            return result;
        }

        private OperationResult ValidateTarifas(dynamic tarifas)
        {
            if (tarifas == null)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["NullTarifa"] };
            }

            if (tarifas.PrecioPorNoche <= 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["InvalidPrice"] };
            }

            if (string.IsNullOrWhiteSpace(tarifas.Descripcion) || tarifas.Descripcion.Length > 255)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["InvalidDescription"] };
            }

            if (tarifas.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["InvalidHabitacionID"] };
            }

            if (tarifas.FechaInicio == default(DateOnly) || tarifas.FechaFin == default(DateOnly))
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["MissingDates"] };
            }

            if (tarifas.Descuento < 0 || tarifas.Descuento > 100)
            {
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Tarifas"]["InvalidDiscount"] };
            }

            return new OperationResult { Success = true };
        }

    }
}