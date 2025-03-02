using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionService> _logger;
        private readonly IConfiguration _configuration;

        public HabitacionService(IHabitacionRepository habitacionRepository,
                                 ILogger<HabitacionService> logger,
                                 IConfiguration configuration)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                result.Data = habitaciones.Where(h => !h.Deleted).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consiguiendo las habitaciones: {ex.Message}");
                result.Success = false;
                result.Message = "Error consiguiendo las habitaciones.";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                if (habitacion == null || habitacion.Deleted)
                {
                    result.Success = false;
                    result.Message = "Habitacion no encontrada o eliminada.";
                }
                else
                {
                    result.Success = true;
                    result.Data = habitacion;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consiguiendo la habitacion por ID: {ex.Message}");
                result.Success = false;
                result.Message = "Error consiguiendo la habitacion por ID.";
            }
            return result;
        }

        public async Task<OperationResult> Save(SaveHabitacionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = new Habitacion
                {
                    IdPiso = dto.IdPiso,
                    IdCategoria = dto.IdCategoria,
                    Numero = dto.Numero,
                    Detalle = dto.Detalle,
                    IdEstadoHabitacion = dto.IdEstadoHabitacion,
                    FechaCreacion = DateTime.Now,
                    Deleted = false
                };

                var validationResult = ValidateHabitacion(habitacion);
                if (!validationResult.Success.GetValueOrDefault())
                {
                    return validationResult;
                }

                await _habitacionRepository.SaveEntityAsync(habitacion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error guardando la habitacion: {ex.Message}");
                result.Success = false;
                result.Message = "Error guardando la habitacion.";
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateHabitacionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (habitacion == null || habitacion.Deleted)
                {
                    result.Success = false;
                    result.Message = "Habitacion no encontrada o eliminada.";
                }
                else
                {
                    habitacion.IdPiso = dto.IdPiso;
                    habitacion.IdCategoria = dto.IdCategoria;
                    habitacion.Numero = dto.Numero;
                    habitacion.Detalle = dto.Detalle;
                    habitacion.IdEstadoHabitacion = dto.IdEstadoHabitacion;

                    var validationResult = ValidateHabitacion(habitacion);
                    if (!validationResult.Success.GetValueOrDefault())
                    {
                        return validationResult;
                    }

                    await _habitacionRepository.UpdateEntityAsync(habitacion);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando la habitacion: {ex.Message}");
                result.Success = false;
                result.Message = "Error actualizando la habitacion.";
            }
            return result;
        }

        public async Task<OperationResult> Remove(RemoveHabitacionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (habitacion == null)
                {
                    result.Success = false;
                    result.Message = "Habitacion no encontrada.";
                }
                else if (habitacion.IdEstadoHabitacion == 2)
                {
                    result.Success = false;
                    result.Message = "No se puede eliminar una habitacion con una reserva en curso.";
                }
                else
                {
                    habitacion.Deleted = true;
                    await _habitacionRepository.UpdateEntityAsync(habitacion);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error eliminando la habitacion: {ex.Message}");
                result.Success = false;
                result.Message = "Error eliminando la habitacion.";
            }
            return result;
        }

        private OperationResult ValidateHabitacion(Habitacion habitacion)
        {
            if (habitacion == null)
            {
                return new OperationResult { Success = false, Message = "La habitacion no puede ser nula." };
            }
            if (string.IsNullOrWhiteSpace(habitacion.Numero) || habitacion.Numero.Length > 50)
            {
                return new OperationResult { Success = false, Message = "El número de la habitacion no puede estar vacío y debe tener un máximo de 50 caracteres." };
            }
            if (habitacion.Detalle != null && habitacion.Detalle.Length > 100)
            {
                return new OperationResult { Success = false, Message = "El detalle de la habitacion debe tener un máximo de 100 caracteres." };
            }
            if (habitacion.IdEstadoHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del estado de la habitacion debe ser mayor que cero." };
            }
            if (habitacion.IdPiso <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del piso debe ser mayor que cero." };
            }
            if (habitacion.IdCategoria <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la categoría debe ser mayor que cero." };
            }
            if (habitacion.FechaCreacion == default)
            {
                return new OperationResult { Success = false, Message = "La fecha de creación es obligatoria." };
            }
            if (habitacion.Deleted == true)
            {
                return new OperationResult { Success = false, Message = "La habitacion fue eliminada." };
            }
            return new OperationResult { Success = true };
        }

        public async Task<List<Habitacion>> ObtenerHabitacionesPorEstadoIdAsync(int idEstadoHabitacion)
        {
            try
            {
                return await _habitacionRepository.ObtenerHabitacionesPorEstadoIdAsync(idEstadoHabitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener habitaciones por estado: {ex.Message}");
                return new List<Habitacion>();
            }
        }
    }
}