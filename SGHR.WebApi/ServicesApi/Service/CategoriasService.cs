using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Categorias;
using SGHR.WebApi.PersistenApi.Interface;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.ServicesApi.Service
{
    public class CategoriasService : ICategoriasService
    {
        private readonly IRepository<CategoriasApiModel> _categoriasRepository;
        private readonly ILoggerManger<CategoriasService> _logger;
        private readonly IErrorMessageService _errorMessageService;

        public CategoriasService(IRepository<CategoriasApiModel> categoriasRepository, ILoggerManger<CategoriasService> logger, IErrorMessageService errorMessageService)
        {
            _categoriasRepository = categoriasRepository;
            _logger = logger;
            _errorMessageService = errorMessageService;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var categorias = await _categoriasRepository.GetAllAsync();
                result.success = true;
                result.data = categorias;
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
            if (id <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }
            try
            {
                var categoria = await _categoriasRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    _logger.LogError(new Exception(_errorMessageService.GetErrorMessage("Entity", "NotFound")), _errorMessageService.GetErrorMessage("Entity", "NotFound"));
                    result.success = false;
                    result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                    return result;
                }
                result.success = true;
                result.data = categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "GenericError"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Operations", "GenericError");
            }
            return result;
        }

        public async Task<OperationResult> Save(CategoriasApiModel dto)
        {
            var validationResult = ValidateCategoria(dto);
            if (!validationResult.success)
                return validationResult;

            var existingCategorias = await _categoriasRepository.GetAllAsync();
            var duplicateCategoria = existingCategorias
                .Where(c => !c.Estado && string.Equals(c.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (duplicateCategoria != null)
            {
                return new OperationResult
                {
                    success = false,
                    message = "Ya existe una categoría activa con esta descripción"
                };
            }

            try
            {
                await _categoriasRepository.AddAsync(dto);
                return new OperationResult
                {
                    success = true,
                    message = _errorMessageService.GetErrorMessage("Operations", "SaveSuccess")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _errorMessageService.GetErrorMessage("Operations", "SaveFailed"));
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Operations", "SaveFailed")
                };
            }
        }

        public async Task<OperationResult> Update(CategoriasApiModel dto)
        {
            var result = new OperationResult();
            if (dto.IdCategoria <= 0)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "InvalidID"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }

            var categoria = await _categoriasRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria == null)
            {
                _logger.LogWarning(_errorMessageService.GetErrorMessage("Entity", "NotFound"));
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                return result;
            }

            var validationResult = ValidateCategoria(dto);
            if (!validationResult.success)
            {
                return validationResult;
            }

            if (!string.Equals(categoria.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
            {
                var existingCategorias = await _categoriasRepository.GetAllAsync();
                var duplicateCategoria = existingCategorias
                    .Where(c => c.IdCategoria != dto.IdCategoria && !c.Estado && string.Equals(c.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (duplicateCategoria != null)
                {
                    return new OperationResult
                    {
                        success = false,
                        message = "Ya existe una categoría activa con esta descripción"
                    };
                }
            }

            try
            {
                await _categoriasRepository.UpdateAsync(dto);
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

        public async Task<OperationResult> Remove(CategoriasApiModel dto)
        {
            var result = new OperationResult();
            if (dto.IdCategoria <= 0)
            {
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "InvalidID");
                return result;
            }

            var categoria = await _categoriasRepository.GetByIdAsync(dto.IdCategoria);
            if (categoria == null)
            {
                result.success = false;
                result.message = _errorMessageService.GetErrorMessage("Entity", "NotFound");
                return result;
            }

            if (categoria.GetType().GetProperty("EsCategoriaSistema") != null)
            {
                var esCategoriaSistema = (bool?)categoria.GetType().GetProperty("EsCategoriaSistema").GetValue(categoria, null);
                if (esCategoriaSistema == true)
                {
                    return new OperationResult
                    {
                        success = false,
                        message = "No se puede eliminar una categoría del sistema"
                    };
                }
            }

            var todasLasEntidades = await _categoriasRepository.GetAllAsync();
            var tieneEntidadesAsociadas = todasLasEntidades.Any(e =>
                e.GetType().GetProperty("IdCategoria") != null &&
                (int)e.GetType().GetProperty("IdCategoria").GetValue(e, null) == dto.IdCategoria);

            if (tieneEntidadesAsociadas)
            {
                return new OperationResult
                {
                    success = false,
                    message = "No se puede eliminar la categoría porque tiene entidades asociadas"
                };
            }

            try
            {
                await _categoriasRepository.DeleteAsync(dto.IdCategoria);
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

        private OperationResult ValidateCategoria(CategoriasApiModel categoria)
        {
            if (categoria == null)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Categorias", "NullCategoria")
                };

            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Categorias", "EmptyDescription")
                };

            if (categoria.Descripcion.Length > 50)
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Categorias", "DescriptionTooLong")
                };

            if (!System.Text.RegularExpressions.Regex.IsMatch(categoria.Descripcion, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-_\.]+$"))
                return new OperationResult
                {
                    success = false,
                    message = _errorMessageService.GetErrorMessage("Categorias", "InvalidDescriptionCharacters")
                };

            if (categoria.GetType().GetProperty("IdUsuario") != null)
            {
                var idUsuario = (int?)categoria.GetType().GetProperty("IdUsuario").GetValue(categoria, null);
                if (!idUsuario.HasValue || idUsuario.Value <= 0)
                    return new OperationResult
                    {
                        success = false,
                        message = _errorMessageService.GetErrorMessage("Categorias", "InvalidUser")
                    };
            }

            return new OperationResult { success = true };
        }
    }
}
