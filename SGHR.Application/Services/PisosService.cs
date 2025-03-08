using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
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
            var operationResult = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                operationResult.Data = pisos;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
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
                var piso = new Piso
                {   
                    FechaCreacion = DateTime.UtcNow,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // Usar el usuario autenticado en producción

                };

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

            piso.Descripcion = dto.Descripcion ?? piso.Descripcion;
            piso.Estado = dto.Estado != default ? dto.Estado : piso.Estado;
            piso.ModifyDate = DateTime.Now;
            piso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _pisoRepository.UpdateEntityAsync(piso);
            return result;
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

            piso.Deleted = true;
            piso.DeletedUser = 1; // En producción, obtener el usuario autenticado.
            piso.ModifyDate = DateTime.Now;

            var result = await _pisoRepository.UpdateEntityAsync(piso);
            return result;
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

            piso.Deleted = false;
            piso.ModifyDate = DateTime.Now;
            piso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _pisoRepository.UpdateEntityAsync(piso);
            return result;
        }

        private OperationResult ValidatePiso(dynamic piso)
        {
            if (piso == null)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Pisos"]["NullPiso"] };

            if (string.IsNullOrWhiteSpace(piso.Descripcion) || piso.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = _messageMapper.ErrorMessages["Pisos"]["InvalidDescription"] };

            return new OperationResult { Success = true };
        }
    }
}