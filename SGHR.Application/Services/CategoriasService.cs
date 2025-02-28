using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;


namespace SGHR.Application.Services
{
    public class CategoriasService : ICategoriasService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriasService> _logger;
        private readonly IConfiguration _configuration;

        public CategoriasService(ICategoriaRepository categoriaRepository,
                                  ILogger<CategoriasService> logger,
                                  IConfiguration configuration)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var categorias = await _categoriaRepository.GetAllAsync();
                operationResult.Data = categorias.Where(c => !c.Deleted).ToList(); 
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las categorías.");
                operationResult.Message = "Error al obtener todas las categorías.";
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
                if (categoria.Data is Categoria categoriaData && categoriaData.Deleted)
                {
                    return new OperationResult { Success = false, Message = "Categoría eliminada." };
                }

                operationResult.Data = categoria.Data;
                operationResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la categoría con ID {id}.");
                operationResult.Message = "Error al obtener la categoría por ID.";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveCategoriasDto dto)
        {
            var validationResult = ValidateCategoria(dto);
            if (!validationResult.Success != null)
                return validationResult;

            try
            {
                var categoria = new Categoria
                {
                    Descripcion = dto.Descripcion,
                    CreationUser = 1 // En producción, obtener el usuario autenticado
                };

                return await _categoriaRepository.SaveEntityAsync(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la categoría.");
                return new OperationResult { Success = false, Message = "Error al guardar la categoría." };
            }
        }

        public async Task<OperationResult> Update(UpdateCategoriasDto dto)
        {
            if (dto.IdCategoria <= 0)
                return new OperationResult { Success = false, Message = "ID de categoría inválido." };

            var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
            if (categoria.Data is not Categoria categoriaData || categoriaData.Deleted)
                return new OperationResult { Success = false, Message = "Categoría no encontrada o eliminada." };

            categoriaData.Descripcion = dto.Descripcion ?? categoriaData.Descripcion;
            categoriaData.ModifyDate = DateTime.Now;
            categoriaData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _categoriaRepository.UpdateEntityAsync(categoriaData);
        }

        public async Task<OperationResult> Remove(RemoveCategoriasDto dto)
        {
            if (dto.IdCategoria <= 0)
                return new OperationResult { Success = false, Message = "ID de categoría inválido." };

            var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
            if (categoria.Data is not Categoria categoriaData || categoriaData.Deleted)
                return new OperationResult { Success = false, Message = "Categoría no encontrada o ya eliminada." };

            categoriaData.Deleted = true;
            categoriaData.DeletedUser = 1;
            categoriaData.ModifyDate = DateTime.Now;

            return await _categoriaRepository.UpdateEntityAsync(categoriaData);
        }

        public async Task<OperationResult> Restore(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria.Data is not Categoria categoriaData || !categoriaData.Deleted)
                return new OperationResult { Success = false, Message = "Categoría no encontrada o ya activa." };

            categoriaData.Deleted = false;
            categoriaData.ModifyDate = DateTime.Now;
            categoriaData.ModifyUser = 1; // En producción, obtener el usuario autenticado

            return await _categoriaRepository.UpdateEntityAsync(categoriaData);
        }

        public async Task<OperationResult> DeletePermanent(int id)
        {
            if (id <= 0)
                return new OperationResult { Success = false, Message = "ID de categoría inválido." };

            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria.Data is not Categoria categoriaData)
                return new OperationResult { Success = false, Message = "Categoría no encontrada." };

            return await _categoriaRepository.DeleteEntityAsync(id);
        }


        private OperationResult ValidateCategoria(dynamic categoria)
        {
            if (categoria == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };

            if (string.IsNullOrWhiteSpace(categoria.Descripcion) || categoria.Descripcion.Length > 50)
                return new OperationResult { Success = false, Message = "La descripción no puede estar vacía y debe tener un máximo de 50 caracteres." };

            return new OperationResult { Success = true };
        }
    }
}