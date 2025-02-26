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
                operationResult.Data = categorias;
                operationResult.Success = true;
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener todas las categorías";
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
                if (categoria == null)
                {
                    operationResult.Message = "Categoría no encontrada";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = categoria;
                    operationResult.Success = true;
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener la categoría por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveCategoriasDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var categoria = new Categoria
                {
                    Descripcion = dto.Descripcion
                };
                operationResult = await _categoriaRepository.SaveEntityAsync(categoria);
            }
            catch (Exception)
            {
                operationResult.Message = "Error al guardar la categoría";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdateCategoriasDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null)
                {
                    operationResult.Message = "Categoría no encontrada";
                    operationResult.Success = false;
                }
                else
                {
                    categoria.Descripcion = dto.Descripcion;
                    operationResult = await _categoriaRepository.UpdateEntityAsync(categoria);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al actualizar la categoría";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveCategoriasDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null)
                {
                    operationResult.Message = "Categoría no encontrada";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _categoriaRepository.DeleteEntityAsync(categoria);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al eliminar la categoría";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodasLasCategoriasAsync()
        {
            return await _categoriaRepository.ObtenerTodasLasCategoriasAsync();
        }

        public async Task<bool> ExisteCategoriaAsync(int idCategoria)
        {
            return await _categoriaRepository.ExisteCategoriaAsync(idCategoria);
        }

        public async Task<OperationResult> DeleteEntityAsync(Categoria entity)
        {
            return await _categoriaRepository.DeleteEntityAsync(entity);
        }
    }
}