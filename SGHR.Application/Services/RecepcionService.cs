using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class RecepcionService : IRecepcionService
    {
        private readonly IRecepcionRepository _recepcionRepository;
        private readonly ILogger<RecepcionService> _logger;
        private readonly IConfiguration _configuration;

        public RecepcionService(IRecepcionRepository recepcionRepository,
                                ILogger<RecepcionService> logger,
                                IConfiguration configuration)
        {
            _recepcionRepository = recepcionRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var recepciones = await _recepcionRepository.GetAllAsync();
                result.Data = recepciones.Where(r => !r.Deleted).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consiguiendo las recepciones: {ex.Message}");
                result.Success = false;
                result.Message = "Error consiguiendo las recepciones.";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(id);
                if (recepcion == null || recepcion.Deleted)
                {
                    result.Success = false;
                    result.Message = "Recepcion no encontrada o eliminada.";
                }
                else
                {
                    result.Success = true;
                    result.Data = recepcion;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error consiguiendo la recepcion por ID: {ex.Message}");
                result.Success = false;
                result.Message = "Error consiguiendo la recepcion por ID.";
            }
            return result;
        }

        public async Task<OperationResult> Save(SaveRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = new Recepcion
                {
                    IdCliente = dto.IdCliente,
                    IdHabitacion = dto.IdHabitacion,
                    IdEstadoReserva = dto.IdEstadoReserva,
                    FechaEntrada = dto.FechaEntrada,
                    FechaSalida = dto.FechaSalida,
                    FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion,
                    PrecioInicial = dto.PrecioInicial,
                    Adelanto = dto.Adelanto,
                    PrecioRestante = dto.PrecioRestante,
                    TotalPagado = dto.TotalPagado,
                    CostoPenalidad = dto.CostoPenalidad,
                    Observacion = dto.Observacion,
                    Deleted = false,
                    CreationUser = 1
                };

                var validationResult = ValidateRecepcion(recepcion);
                if (!validationResult.Success.GetValueOrDefault())
                {
                    return validationResult;
                }

                await _recepcionRepository.SaveEntityAsync(recepcion);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error guardando la recepcion: {ex.Message}");
                result.Success = false;
                result.Message = "Error guardando la recepcion.";
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(dto.Id);
                if (recepcion == null || recepcion.Deleted)
                {
                    result.Success = false;
                    result.Message = "Recepcion no encontrada o eliminada.";
                }
                else
                {
                    recepcion.IdCliente = dto.IdCliente;
                    recepcion.IdHabitacion = dto.IdHabitacion;
                    recepcion.IdEstadoReserva = dto.IdEstadoReserva;
                    recepcion.FechaEntrada = dto.FechaEntrada;
                    recepcion.FechaSalida = dto.FechaSalida;
                    recepcion.FechaSalidaConfirmacion = dto.FechaSalidaConfirmacion;
                    recepcion.PrecioInicial = dto.PrecioInicial;
                    recepcion.Adelanto = dto.Adelanto;
                    recepcion.PrecioRestante = dto.PrecioRestante;
                    recepcion.TotalPagado = dto.TotalPagado;
                    recepcion.CostoPenalidad = dto.CostoPenalidad;
                    recepcion.Observacion = dto.Observacion;
                    recepcion.ModifyDate = DateTime.Now;
                    recepcion.ModifyUser = 1;

                    var validationResult = ValidateRecepcion(recepcion);
                    if (!validationResult.Success.GetValueOrDefault())
                    {
                        return validationResult;
                    }

                    await _recepcionRepository.UpdateEntityAsync(recepcion);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error actualizando la recepcion: {ex.Message}");
                result.Success = false;
                result.Message = "Error actualizando la recepcion.";
            }
            return result;
        }

        public async Task<OperationResult> Remove(RemoveRecepcionDto dto)
        {
            var result = new OperationResult();
            try
            {
                var recepcion = await _recepcionRepository.GetEntityByIdAsync(dto.Id);
                if (recepcion == null)
                {
                    result.Success = false;
                    result.Message = "Recepcion no encontrada.";
                }
                else if (recepcion.IdEstadoReserva == 2)
                {
                    result.Success = false;
                    result.Message = "No se puede eliminar una recepcion en curso.";
                }
                else
                {
                    recepcion.Deleted = true;
                    await _recepcionRepository.UpdateEntityAsync(recepcion);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error eliminando la recepcion: {ex.Message}");
                result.Success = false;
                result.Message = "Error eliminando la recepcion.";
            }
            return result;
        }

        private OperationResult ValidateRecepcion(Recepcion recepcion)
        {
            if (recepcion == null)
            {
                return new OperationResult { Success = false, Message = "La recepcion no puede ser nula." };
            }
            if (recepcion.FechaEntrada == default)
            {
                return new OperationResult { Success = false, Message = "La fecha de entrada es obligatoria." };
            }
            if (recepcion.IdCliente.HasValue && recepcion.IdCliente <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del cliente debe ser mayor que cero o nulo." };
            }
            if (recepcion.IdHabitacion.HasValue && recepcion.IdHabitacion <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID de la habitacion debe ser mayor que cero o nulo." };
            }
            if (recepcion.IdEstadoReserva.HasValue && recepcion.IdEstadoReserva <= 0)
            {
                return new OperationResult { Success = false, Message = "El ID del estado de la reserva debe ser mayor que cero o nulo." };
            }
            if (recepcion.Observacion != null && recepcion.Observacion.Length > 500)
            {
                return new OperationResult { Success = false, Message = "La observacion de la recepcion debe tener un máximo de 500 caracteres." };
            }
            if (recepcion.Deleted == true)
            {
                return new OperationResult { Success = false, Message = "La recepcion fue eliminada." };
            }
            return new OperationResult { Success = true };
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorEstadoReservaAsync(int idEstadoReserva)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorEstadoReservaAsync(idEstadoReserva);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por estado de reserva: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorClienteIdAsync(int idCliente)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorClienteIdAsync(idCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por cliente: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorHabitacionIdAsync(int idHabitacion)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorHabitacionIdAsync(idHabitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por habitacion: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaEntradaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorFechaEntradaAsync(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por fecha de entrada: {ex.Message}");
                return new List<Recepcion>();
            }
        }

        public async Task<List<Recepcion>> ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return await _recepcionRepository.ObtenerRecepcionesPorFechaSalidaConfirmadaAsync(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener recepciones por fecha de salida confirmada: {ex.Message}");
                return new List<Recepcion>();
            }
        }
    }
}
