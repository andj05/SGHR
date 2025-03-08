using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
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
            var operationResult = new OperationResult();
            try
            {
                var categorias = await _categoriaRepository.GetAllAsync();
                operationResult.Success = true;
                operationResult.Data = categorias;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DbException"]);
                operationResult.Message = $"{_messageMapper.ErrorMessages["Operations"]["DbException"]}: {ex.Message}";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null )
                {
                    operationResult.Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = categoria;
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

        public async Task<OperationResult> Save(SaveCategoriasDto dto)
        {
            var validationResult = ValidateCategoria(dto);
            if (validationResult.Success != true)
                return validationResult;

            try
            {
                var categoria = new Categoria
                {
                    
                    Estado = dto.Estado,
                    FechaCreacion = DateTime.Now,
                    Descripcion = dto.Descripcion,
                    CreationUser = 1 // En producción, obtener el usuario autenticado.
                };

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

            categoria.Descripcion = dto.Descripcion ?? categoria.Descripcion;
            categoria.Estado = dto.Estado;
            categoria.ModifyDate = DateTime.Now;
            categoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _categoriaRepository.UpdateEntityAsync(categoria);
            return result;
        }

        public async Task<OperationResult> Remove(RemoveCategoriasDto dto)
        {
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

            categoria.Deleted = true;
            categoria.DeletedUser = 1; // En producción, obtener el usuario autenticado.
            categoria.ModifyDate = DateTime.Now;

            var result = await _categoriaRepository.UpdateEntityAsync(categoria);
            return result;
        }

        public async Task<OperationResult> Restore(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null || !categoria.Deleted)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                };

            categoria.Deleted = false;
            categoria.ModifyDate = DateTime.Now;
            categoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

            var result = await _categoriaRepository.UpdateEntityAsync(categoria);
            return result;
        }

        private OperationResult ValidateCategoria(dynamic categoria)
        {
            if (categoria == null)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["NullCategoria"]
                };

            if (string.IsNullOrWhiteSpace(categoria.Descripcion) || categoria.Descripcion.Length > 50)
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Categorias"]["InvalidDescription"]
                };

            return new OperationResult { Success = true };
        }
    }
}