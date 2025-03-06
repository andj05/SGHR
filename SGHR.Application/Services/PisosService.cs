using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class PisosService : IPisosService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisosService> _logger;
        private readonly IConfiguration _configuration;

        public PisosService(IPisoRepository pisoRepository,
                            ILogger<PisosService> logger,
                            IConfiguration configuration)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                operationResult.Data = pisos.Where(p => !p.Deleted).ToList();
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pisos.");
                operationResult.Message = "Error al obtener los pisos.";
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
                if (piso.Deleted)
                {
                    return new OperationResult { Success = false, Message = "Piso eliminado." };
                }

                operationResult.Data = piso;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el piso con ID {id}.");
                operationResult.Message = "Error al obtener el piso por ID.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SavePisosDto dto)
        {
            var validationResult = ValidatePiso(dto);
            if (!validationResult.Success != null)
                return validationResult;

            try
            {
                var piso = new Piso
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // Usar el usuario autenticado en producción
                };

                return await _pisoRepository.SaveEntityAsync(piso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el piso.");
                return new OperationResult { Success = false, Message = "Error al guardar el piso." };
            }
        }

        public async Task<OperationResult> Update(UpdatePisosDto dto)
        {
            if (dto.IdPiso <= 0)
                return new OperationResult { Success = false, Message = "ID de piso inválido." };

            var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
            if (piso == null || piso.Deleted)
                return new OperationResult { Success = false, Message = "Piso no encontrado o eliminado." };

            piso.Descripcion = dto.Descripcion ?? piso.Descripcion;
            piso.Estado = dto.Estado != default ? dto.Estado : piso.Estado;
            piso.ModifyDate = DateTime.Now;
            piso.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        public async Task<OperationResult> Remove(RemovePisosDto dto)
        {
            if (dto.IdPiso <= 0)
                return new OperationResult { Success = false, Message = "ID de piso inválido." };

            var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
            if (piso == null || piso.Deleted)
                return new OperationResult { Success = false, Message = "Piso no encontrado o ya eliminado." };

            piso.Deleted = true;
            piso.DeletedUser = 1;
            piso.ModifyDate = DateTime.Now;

            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var piso = await _pisoRepository.GetEntityByIdAsync(id);
            if (piso == null || !piso.Deleted)
                return new OperationResult { Success = false, Message = "Piso no encontrado o ya activo." };

            piso.Deleted = false;
            piso.ModifyDate = DateTime.Now;
            piso.ModifyUser = 1;

            return await _pisoRepository.UpdateEntityAsync(piso);
        }

        private OperationResult ValidatePiso(dynamic piso)
        {
            if (piso == null)
                return new OperationResult { Success = false, Message = "El piso no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(piso.Descripcion) || piso.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = "La descripción no puede estar vacía y debe tener un máximo de 50 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}
