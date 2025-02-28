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
                operationResult.Data = estados.Where(e => !e.Deleted).ToList(); // Filtra eliminados
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los estados de habitación.");
                operationResult.Message = "Error al obtener los estados de habitación.";
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
                if (estado?.Data is EstadoHabitacion estadoData && estadoData.Deleted)
                {
                    return new OperationResult { Success = false, Message = "Estado de habitación eliminado." };
                }

                operationResult.Data = estado.Data;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el estado de habitación con ID {id}.");
                operationResult.Message = "Error al obtener el estado de habitación por ID.";
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
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

                return await _estadoHabitacionRepository.SaveEntityAsync(estado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el estado de habitación.");
                return new OperationResult { Success = false, Message = "Error al guardar el estado de habitación." };
            }
        }

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult { Success = false, Message = "ID de estado de habitación inválido." };

            var estadoResult = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estadoResult?.Data is not EstadoHabitacion estado || estado.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o eliminado." };

            estado.Descripcion = dto.Descripcion ?? estado.Descripcion;
            estado.Estado = dto.Estado != default ? dto.Estado : estado.Estado;
            estado.FechaCreacion = dto.FechaCreacion != default ? dto.FechaCreacion : estado.FechaCreacion;
            estado.ModifyDate = DateTime.Now;
            estado.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _estadoHabitacionRepository.UpdateEntityAsync(estado);
        }


        public async Task<OperationResult> Remove(RemoveEstadoHabitacionDto dto)
        {
            if (dto.IdEstadoHabitacion <= 0)
                return new OperationResult { Success = false, Message = "ID de estado de habitación inválido." };

            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
            if (estado?.Data is not EstadoHabitacion estadoData || estadoData.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ya eliminado." };

            estadoData.Deleted = true;
            estadoData.DeletedUser = 1;
            estadoData.ModifyDate = DateTime.Now;

            return await _estadoHabitacionRepository.UpdateEntityAsync(estadoData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estado?.Data is not EstadoHabitacion estadoData || !estadoData.Deleted)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado o ya activo." };

            estadoData.Deleted = false;
            estadoData.ModifyDate = DateTime.Now;
            estadoData.ModifyUser = 1; 

            return await _estadoHabitacionRepository.UpdateEntityAsync(estadoData);
        }

        public async Task<OperationResult> DeletePermanent(int id)
        {
            if (id <= 0)
                return new OperationResult { Success = false, Message = "ID de estado de habitación inválido." };

            var estadoHabitacion = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estadoHabitacion.Data is not EstadoHabitacion estadoHabitacionData)
                return new OperationResult { Success = false, Message = "Estado de habitación no encontrado." };

            return await _estadoHabitacionRepository.DeleteEntityAsync(id);
        }


        private OperationResult ValidateEstadoHabitacion(dynamic estado)
        {
            if (estado == null)
                return new OperationResult { Success = false, Message = "El estado de habitación no puede ser nulo." };

            if (string.IsNullOrWhiteSpace(estado.Descripcion) || estado.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = "La descripción no puede estar vacía y debe tener un máximo de 50 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}