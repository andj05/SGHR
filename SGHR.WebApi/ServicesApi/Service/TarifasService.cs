using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Tarifas;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class TarifasService : ITarifasService
    {
        private readonly IRepository<TarifasApiModel> _tarifasRepository;
        private readonly ILoggerManger<TarifasService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public TarifasService(IRepository<TarifasApiModel> tarifasRepository, ILoggerManger<TarifasService> logger, IErrorMessageService errorMessageService)
        {
            _tarifasRepository = tarifasRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var tarifas = await _tarifasRepository.GetAllAsync();
                result.success = true;
                result.data = tarifas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            try
            {
                var tarifa = await _tarifasRepository.GetByIdAsync(id);
                if (tarifa == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = tarifa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(TarifasApiModel dto)
        {
            var validationResult = await ValidateTarifas(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                await _tarifasRepository.AddAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed");
            }
            return result;
        }

        public async Task<OperationResult> Update(TarifasApiModel dto)
        {
            var validationResult = await ValidateTarifas(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                await _tarifasRepository.UpdateAsync(dto);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "UpdateFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "UpdateFailed");
            }
            return result;
        }

        public async Task<OperationResult> Remove(TarifasApiModel dto)
        {
            var result = new OperationResult();
            try
            {
                await _tarifasRepository.DeleteAsync(dto.IdTarifa);
                result.success = true;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteSuccess");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "DeleteFailed"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "DeleteFailed");
            }
            return result;
        }

        private async Task<OperationResult> ValidateTarifas(TarifasApiModel tarifas)
        {
            if (tarifas == null)
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "NullTarifa") };
            }

            if (tarifas.PrecioPorNoche <= 0)
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "InvalidPrice") };
            }

            if (string.IsNullOrWhiteSpace(tarifas.Descripcion) || tarifas.Descripcion.Length > 255)
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "InvalidDescription") };
            }

            if (tarifas.IdHabitacion <= 0)
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "InvalidHabitacionID") };
            }

            if (tarifas.FechaInicio == default(DateOnly) || tarifas.FechaFin == default(DateOnly))
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "MissingDates") };
            }

            if (tarifas.Descuento < 0 || tarifas.Descuento > 100)
            {
                return new OperationResult { success = false, message = _errorMessageService.GetErrorMessage("Tarifas", "InvalidDiscount") };
            }

            // VALIDACIONES DE NEGOCIO

            // 1. Validar que la fecha de inicio sea anterior a la fecha de fin
            if (tarifas.FechaInicio >= tarifas.FechaFin)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "InvalidStartDate")
                };
            }

            // 2. Validar que las fechas no estén en el pasado
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (tarifas.FechaInicio < currentDate)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "PastStartDate")
                };
            }

            // 3. Validar la duración mínima y máxima de la tarifa
            TimeSpan duration = tarifas.FechaFin.ToDateTime(TimeOnly.MinValue) - tarifas.FechaInicio.ToDateTime(TimeOnly.MinValue);
            if (duration.TotalDays < 7)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "ShortDuration")
                };
            }

            if (duration.TotalDays > 365)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "LongDuration")
                };
            }

            // 4. Validar el descuento según la duración del periodo
            if (duration.TotalDays < 30 && tarifas.Descuento > 20)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "HighDiscount")
                };
            }

            // 5. Validar el precio según tipo de temporada (alta o baja)
            bool isHighSeason = IsHighSeason(tarifas.FechaInicio, tarifas.FechaFin);
            if (isHighSeason && tarifas.Descuento > 15)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "HighSeasonDiscount")
                };
            }

            // 6. Validar que no existan solapamientos con otras tarifas para la misma habitación
            var allTarifas = await _tarifasRepository.GetAllAsync();
            var overlappingTarifa = allTarifas
                .Where(t => t.IdHabitacion == tarifas.IdHabitacion &&
                       t.IdTarifa != tarifas.IdTarifa &&
                       ((t.FechaInicio <= tarifas.FechaInicio && t.FechaFin >= tarifas.FechaInicio) ||
                        (t.FechaInicio <= tarifas.FechaFin && t.FechaFin >= tarifas.FechaFin) ||
                        (t.FechaInicio >= tarifas.FechaInicio && t.FechaFin <= tarifas.FechaFin)))
                .FirstOrDefault();

            if (overlappingTarifa != null)
            {
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Tarifas", "OverlappingTarifa")
                };
            }

            // 7. Validar cambios excesivos de precio (para actualizaciones)
            if (tarifas.IdTarifa > 0)
            {
                var existingTarifa = await _tarifasRepository.GetByIdAsync(tarifas.IdTarifa);
                if (existingTarifa != null)
                {
                    double priceChange = Math.Abs((double)(tarifas.PrecioPorNoche - existingTarifa.PrecioPorNoche) / (double)existingTarifa.PrecioPorNoche * 100);
                    if (priceChange > 30)
                    {
                        return new OperationResult
                        {
                            success = false,
                            message = _errorMessageService.GetErrorMessage("Tarifas", "ExcessivePriceChange")
                        };
                    }
                }
            }

            return new OperationResult { success = true };
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

