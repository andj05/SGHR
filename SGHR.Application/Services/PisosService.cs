using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Repository;

namespace SGHR.Application.Services
{
    public class PisosService : IPisosService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public PisosService(IPisoRepository pisoRepository,
                            IConfiguration configuration,
                            MessageMapper messageMapper,
                            ILoggerManager loggerManager)
        {
            _pisoRepository = pisoRepository;
            _configuration = configuration;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }
        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                var activeTarifas = pisos
                    .Where(t => !t.Deleted)
                    .Select(PisoMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activeTarifas;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                result.Success = false;
            }
            return result;
        }

        public async Task<OperationResult> GetAllDelete()
        {
            var result = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetAllAsync();
                var activepiso = piso
                    .Where(t => t.Deleted)
                    .Select(PisoMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activepiso;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                result.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                result.Success = false;
            }
            return result;
        }


        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso == null)
                {
                    operationResult.Success = false;
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                }
                else
                {
                    operationResult.Data = piso;
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

        public async Task<OperationResult> Save(SavePisosDto dto)
        {
            var validationResult = ValidatePiso(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var piso = PisoMapper.ToEntity(dto);
                return await _pisoRepository.SaveEntityAsync(piso);
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

        public async Task<OperationResult> Update(UpdatePisosDto dto)
        {
            if (dto.IdPiso <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
            if (piso == null || piso.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            piso.UpdateFromDto(dto);
            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        public async Task<OperationResult> Remove(RemovePisosDto dto)
        {
            if (dto.IdPiso <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
            if (piso == null || piso.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            piso.RemoveFromDto(dto);
            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var piso = await _pisoRepository.GetEntityByIdAsync(id);
            if (piso == null || !piso.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            piso.RestoreFromDto(1); // En producción, obtener el usuario autenticado.
            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        private OperationResult ValidatePiso(SavePisosDto dto)
        {
            if (dto == null)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Pisos"]["NullPiso"] };

            if (string.IsNullOrWhiteSpace(dto.Descripcion) || dto.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Pisos"]["InvalidDescription"] };

            return new OperationResult { Success = true };
        }
    }
}