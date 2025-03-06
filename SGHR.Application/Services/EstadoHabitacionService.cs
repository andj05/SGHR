using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class EstadoHabitacionService : IEstadoHabitacionService
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ILogger<EstadoHabitacionService> _logger;
        private readonly IConfiguration _configuration;

        public EstadoHabitacionService(IEstadoHabitacionRepository estadoHabitacionRepository,
                                       ILogger<EstadoHabitacionService> logger,
                                       IConfiguration configuration)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var estados = await _estadoHabitacionRepository.GetAllAsync();
                operationResult.Success = true;
                operationResult.Message = "Estados de habitación obtenidos correctamente.";
                operationResult.Data = estados.Where(e => !e.Deleted).ToList(); // Filtra eliminados
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los estados de habitación.");
                operationResult.Message = "Error al obtener los estados de habitación.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estado?.Deleted == true)
                {
                    return new OperationResult { Success = false, Message = "Estado de habitación eliminado." };
                }

                operationResult.Success = true;
                operationResult.Message = "Estado de habitación obtenido correctamente.";
                operationResult.Data = estado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el estado de habitación con ID {id}.");
                operationResult.Message = "Error al obtener el estado de habitación por ID.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveEstadoHabitacionDto dto)
        {
            var validationResult = ValidateEstadoHabitacion(dto);
            if (!validationResult.Success != null)
                return validationResult;

            try
            {
                var estado = new EstadoHabitacion
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

                var saveResult = await _estadoHabitacionRepository.SaveEntityAsync(estado);
                return saveResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el estado de habitación.");
                return new OperationResult { Success = false, Message = "Error al guardar el estado de habitación." };
            }
        }

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult { Success = false, Message = "ID de estado de habitación inválido." };

            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estado == null || estado.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o eliminado." };

            estado.Descripcion = dto.Descripcion ?? estado.Descripcion;
            estado.Estado = dto.Estado != default ? dto.Estado : estado.Estado;
            estado.FechaCreacion = dto.FechaCreacion != default ? dto.FechaCreacion : estado.FechaCreacion;
            estado.ModifyDate = DateTime.Now;
            estado.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _estadoHabitacionRepository.UpdateEntityAsync(estado);
        }

        public async Task<OperationResult> Remove(RemoveEstadoHabitacionDto dto)
        {
            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult { Success = false, Message = "ID de estado de habitación inválido." };

            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estado == null || estado.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ya eliminado." };

            estado.Deleted = true;
            estado.DeletedUser = 1;
            estado.ModifyDate = DateTime.Now;

            return await _estadoHabitacionRepository.UpdateEntityAsync(estado);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estado == null || !estado.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ya activo." };

            estado.Deleted = false;
            estado.ModifyDate = DateTime.Now;
            estado.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _estadoHabitacionRepository.UpdateEntityAsync(estado);
        }

        private OperationResult ValidateEstadoHabitacion(dynamic estado)
        {
            if (estado == null)
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(estado.Descripcion) || estado.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = "La descripción no puede estar vacía y debe tener un máximo de 50 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}
