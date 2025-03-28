using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class CategoriasService : ICategoriasService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly MessageMapper _messageMapper;
        private readonly ILoggerManager _loggerManager;

        public CategoriasService(ICategoriaRepository categoriaRepository,
                                  MessageMapper messageMapper,
                                  ILoggerManager loggerManager)
        {
            _categoriaRepository = categoriaRepository;
            _messageMapper = messageMapper;
            _loggerManager = loggerManager;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var categoria = await _categoriaRepository.GetAllAsync();
                var activecategoria = categoria
                    .Where(t => !t.Deleted)
                    .Select(CategoriaMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activecategoria;
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

        public async Task<OperationResult> GetAllDelete()
        {
            var result = new OperationResult();
            try
            {
                var categoria = await _categoriaRepository.GetAllAsync();
                var activecategoria = categoria
                    .Where(t => t.Deleted)
                    .Select(CategoriaMapper.ToDto)
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();
                result.Data = activecategoria;
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
            var result = new OperationResult();
            if (id <= 0)
            {
                _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                return result;
            }
            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null || categoria.Deleted)
                {
                    _loggerManager.LogError(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }
                result.Data = CategoriaMapper.ToDto(categoria);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Generic"]["GenericError"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Generic"]["GenericError"];
            }
            return result;
        }

        public async Task<OperationResult> Save(SaveCategoriasDto dto)
        {
            // Validación de negocio antes de guardar
            var validationResult = ValidateCategoria(dto);
            if (validationResult.Success != true)
                return validationResult;

            // Validar duplicidad con datos existentes (validación de negocio)
            var existingCategorias = await _categoriaRepository.GetAllAsync();
            var duplicateCategoria = existingCategorias
                .Where(c => !c.Deleted && string.Equals(c.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (duplicateCategoria != null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Ya existe una categoría activa con esta descripción"
                };
            }

            try
            {
                var categoria = CategoriaMapper.ToEntity(dto);
                return await _categoriaRepository.SaveEntityAsync(categoria);
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
        public async Task<OperationResult> Update(UpdateCategoriasDto dto)
        {
            var result = new OperationResult();
            try
            {
                if (dto.IdCategoria <= 0)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"];
                    return result;
                }

                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null || categoria.Deleted)
                {
                    _loggerManager.LogWarn(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    result.Success = false;
                    result.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return result;
                }

                var validationResult = ValidateCategoria(dto);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                if (!string.IsNullOrEmpty(dto.Descripcion) &&
                    !string.Equals(categoria.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
                {
                    var existingCategorias = await _categoriaRepository.GetAllAsync();
                    var duplicateCategoria = existingCategorias
                        .Where(c => c.Id != dto.IdCategoria &&
                                    !c.Deleted &&
                                    string.Equals(c.Descripcion, dto.Descripcion, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (duplicateCategoria != null)
                    {
                        return new OperationResult
                        {
                            Success = false,
                            Message = "Ya existe una categoría activa con esta descripción"
                        };
                    }
                }

                categoria.UpdateFromDto(dto);
                await _categoriaRepository.UpdateEntityAsync(categoria);

                result.Success = true;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"{_messageMapper.ErrorMessages["Operations"]["UpdateFailed"]}: {ex}");
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }


        public async Task<OperationResult> Remove(RemoveCategoriasDto dto)
        {
            // Validación de negocio: ID debe ser válido
            if (dto.IdCategoria <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
            if (categoria == null || categoria.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            // Validación de negocio: Verificar si es una categoría del sistema (protegida)
            if (categoria.GetType().GetProperty("EsCategoriaSistema") != null)
            {
                var esCategoriaSistema = (bool?)categoria.GetType().GetProperty("EsCategoriaSistema").GetValue(categoria, null);
                if (esCategoriaSistema == true)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "No se puede eliminar una categoría del sistema"
                    };
                }
            }
            var todasLasEntidades = await _categoriaRepository.GetAllAsync(); // Reemplaza con el repositorio adecuado
            var tieneEntidadesAsociadas = todasLasEntidades.Any(e =>
                e.GetType().GetProperty("IdCategoria") != null &&
                (int)e.GetType().GetProperty("IdCategoria").GetValue(e, null) == dto.IdCategoria);

            if (tieneEntidadesAsociadas)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "No se puede eliminar la categoría porque tiene entidades asociadas"
                };
            }

            categoria.RemoveFromDto(dto);
            return await _categoriaRepository.UpdateEntityAsync(categoria);
        }

        public async Task<OperationResult> Restore(int id)
        {
            // Validación de negocio: ID debe ser válido
            if (id <= 0)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };

            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null || !categoria.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            // Validación de negocio: Verificar duplicados al restaurar
            var existingCategorias = await _categoriaRepository.GetAllAsync();
            var duplicateCategoria = existingCategorias
                .Where(c => c.Id != id &&
                       !c.Deleted &&
                       string.Equals(c.Descripcion, categoria.Descripcion, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (duplicateCategoria != null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "No se puede restaurar la categoría porque ya existe una categoría activa con la misma descripción"
                };
            }

            categoria.RestoreFromDto(1);
            return await _categoriaRepository.UpdateEntityAsync(categoria);
        }

        private OperationResult ValidateCategoria(dynamic categoria)
        {
            // Validación de negocio: Objeto debe existir
            if (categoria == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["NullCategoria"]
                };

            // Validación de negocio: Descripción requerida y con longitud adecuada
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["EmptyDescription"]
                };

            if (categoria.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["DescriptionTooLong"]
                };

            // Validación de negocio: Descripción debe tener caracteres válidos
            if (!System.Text.RegularExpressions.Regex.IsMatch(categoria.Descripcion, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\-_\.]+$"))
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["InvalidDescriptionCharacters"]
                };

            // Validación de negocio: Usuario responsable debe ser válido
            if (categoria.GetType().GetProperty("IdUsuario") != null)
            {
                var idUsuario = (int?)categoria.GetType().GetProperty("IdUsuario").GetValue(categoria, null);
                if (!idUsuario.HasValue || idUsuario.Value <= 0)
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["Categorias"]["InvalidUser"]
                    };
            }

            return new OperationResult { Success = true };
        }
    }
}