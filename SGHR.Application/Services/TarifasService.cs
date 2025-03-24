using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
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
            var result = new OperationResult();
            try
            {
                var tarifas = await _tarifasRepository.GetAllAsync();
                var activeTarifas = tarifas
                    .Where(t => !t.Deleted)
                    .Select(TarifasMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activeTarifas;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"{_messageMapper.ErrorMessages["Generic"]["GenericError"]}: {ex}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        public async Task<OperationResult> GetAllDelete()
        {
            var result = new OperationResult();
            try
            {
                var tarifa = await _tarifasRepository.GetAllAsync();
                var activetarifa = tarifa
                    .Where(t => t.Deleted)
                    .Select(TarifasMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activetarifa;
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
            var validationResult = await ValidateTarifas(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var tarifa = TarifasMapper.ToEntity(dto);
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

            var validationResult = await ValidateTarifas(dto);
            if (validationResult.Success != true)
                return validationResult;

            tarifa.UpdateFromDto(dto);
            return await _tarifasRepository.UpdateEntityAsync(tarifa);
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

            // Validación de negocio: No se pueden eliminar tarifas que estén actualmente vigentes
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (tarifa.FechaInicio <= currentDate && tarifa.FechaFin >= currentDate)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "No se puede eliminar una tarifa que está actualmente vigente"
                };
            }

            tarifa.Deleted = true;
            return await _tarifasRepository.UpdateEntityAsync(tarifa);
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

            // Validación de negocio: Verificar si las fechas de la tarifa ya han pasado
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (tarifa.FechaFin < currentDate)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "No se puede restaurar una tarifa con fechas ya vencidas"
                };
            }

            var allTarifas = await _tarifasRepository.GetAllAsync();
            var overlappingTarifa = allTarifas
                .Where(t => !t.Deleted &&
                       t.IdHabitacion == tarifa.IdHabitacion &&
                       t.Id != tarifa.Id &&
                       ((t.FechaInicio <= tarifa.FechaInicio && t.FechaFin >= tarifa.FechaInicio) ||
                        (t.FechaInicio <= tarifa.FechaFin && t.FechaFin >= tarifa.FechaFin) ||
                        (t.FechaInicio >= tarifa.FechaInicio && t.FechaFin <= tarifa.FechaFin)))
                .FirstOrDefault();

            if (overlappingTarifa != null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "La tarifa se solapa con otra tarifa existente para la misma habitación"
                };
            }

            tarifa.Deleted = false;
            return await _tarifasRepository.RestoreEntityAsync(tarifa);
        }

        private async Task<OperationResult> ValidateTarifas(dynamic tarifas)
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

            // VALIDACIONES DE NEGOCIO

            // 1. Validar que la fecha de inicio sea anterior a la fecha de fin
            if (tarifas.FechaInicio >= tarifas.FechaFin)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["InvalidStartDate"]
                };
            }

            // 2. Validar que las fechas no estén en el pasado
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (tarifas.FechaInicio < currentDate)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["PastStartDate"]
                };
            }

            // 3. Validar la duración mínima y máxima de la tarifa
            TimeSpan duration = tarifas.FechaFin.ToDateTime(TimeOnly.MinValue) - tarifas.FechaInicio.ToDateTime(TimeOnly.MinValue);
            if (duration.TotalDays < 7)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["ShortDuration"]
                };
            }

            if (duration.TotalDays > 365)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["LongDuration"]
                };
            }

            // 4. Validar el descuento según la duración del periodo
            if (duration.TotalDays < 30 && tarifas.Descuento > 20)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["HighDiscount"]
                };
            }

            // 5. Validar el precio según tipo de temporada (alta o baja)
            bool isHighSeason = IsHighSeason(tarifas.FechaInicio, tarifas.FechaFin);
            if (isHighSeason && tarifas.Descuento > 15)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["HighSeasonDiscount"]
                };
            }

            // 6. Validar que no existan solapamientos con otras tarifas para la misma habitación
            int tarifaId = 0;
            if (tarifas is UpdateTarifasDto dto)
            {
                tarifaId = dto.IdTarifa;
            }

            var allTarifas = await _tarifasRepository.GetAllAsync();
            var overlappingTarifa = allTarifas
                .Where(t => !t.Deleted &&
                       t.IdHabitacion == tarifas.IdHabitacion &&
                       t.Id != tarifaId &&
                       ((t.FechaInicio <= tarifas.FechaInicio && t.FechaFin >= tarifas.FechaInicio) ||
                        (t.FechaInicio <= tarifas.FechaFin && t.FechaFin >= tarifas.FechaFin) ||
                        (t.FechaInicio >= tarifas.FechaInicio && t.FechaFin <= tarifas.FechaFin)))
                .FirstOrDefault();

            if (overlappingTarifa != null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Tarifas"]["OverlappingTarifa"]
                };
            }

            // 7. Validar cambios excesivos de precio (para actualizaciones)
            if (tarifas is UpdateTarifasDto)
            {
                var existingTarifa = await _tarifasRepository.GetEntityByIdAsync(tarifaId);
                if (existingTarifa != null)
                {
                    double priceChange = Math.Abs((double)(tarifas.PrecioPorNoche - existingTarifa.PrecioPorNoche) / (double)existingTarifa.PrecioPorNoche * 100);
                    if (priceChange > 30)
                    {
                        return new OperationResult
                        {
                            Success = false,
                            Message = _messageMapper.ErrorMessages["Tarifas"]["ExcessivePriceChange"]
                        };
                    }
                }
            }

            return new OperationResult { Success = true };
        }


        // Método auxiliar para determinar temporada alta sin depender de un servicio externo
        private bool IsHighSeason(DateOnly startDate, DateOnly endDate)
        {
            // Considera temporada alta: 
            // - Navidad y Año Nuevo (15 Dic - 15 Ene)
            // - Semana Santa (cambia cada año, pero para simplificar usamos Marzo)
            // - Verano (Junio-Agosto)

            bool containsChristmas = (startDate.Month == 12 && startDate.Day >= 15) ||
                                    (endDate.Month == 1 && endDate.Day <= 15) ||
                                    (startDate.Month <= 1 && endDate.Month >= 12);

            bool containsSummerMonths = (startDate.Month <= 8 && endDate.Month >= 6) &&
                                        !(startDate.Month <= 5 && endDate.Month <= 5);

            bool containsMarch = (startDate.Month <= 3 && endDate.Month >= 3);

            return containsChristmas || containsSummerMonths || containsMarch;
        }
    }
}