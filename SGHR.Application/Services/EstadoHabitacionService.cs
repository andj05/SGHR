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
                operationResult.Data = estados;
                operationResult.Success = true;
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener todos los estados de habitación";
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
                if (estado == null)
                {
                    operationResult.Message = "Estado de habitación no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = estado;
                    operationResult.Success = true;
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener el estado de habitación por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveEstadoHabitacionDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var estado = new EstadoHabitacion
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    FechaCreacion = dto.FechaCreacion
                };
                operationResult = await _estadoHabitacionRepository.SaveEntityAsync(estado);
            }
            catch (Exception)
            {
                operationResult.Message = "Error al guardar el estado de habitación";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdateEstadoHabitacionDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estado == null)
                {
                    operationResult.Message = "Estado de habitación no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    estado.Descripcion = dto.Descripcion;
                    estado.Estado = dto.Estado;
                    estado.FechaCreacion = dto.FechaCreacion;
                    operationResult = await _estadoHabitacionRepository.UpdateEntityAsync(estado);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al actualizar el estado de habitación";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveEstadoHabitacionDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(dto.IdEstadoHabitacion);
                if (estado == null)
                {
                    operationResult.Message = "Estado de habitación no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _estadoHabitacionRepository.DeleteEntityAsync(estado);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al eliminar el estado de habitación";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<IEnumerable<EstadoHabitacion>> ObtenerTodosLosEstadosAsync()
        {
            return await _estadoHabitacionRepository.ObtenerTodosLosEstadosAsync();
        }

        public async Task<bool> ExisteEstadoHabitacionAsync(int idEstado)
        {
            return await _estadoHabitacionRepository.ExisteEstadoHabitacionAsync(idEstado);
        }
    }
}
