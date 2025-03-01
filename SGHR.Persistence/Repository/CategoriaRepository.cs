using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repository
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<CategoriaRepository> _logger;
        private readonly IConfiguration _configuration;

        public CategoriaRepository(SGHRContext context,
                                    ILogger<CategoriaRepository> logger,
                                    IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // Obtener todas las categorías activas
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            try
            {
                return await _context.Categoria
                    .Where(c => c.Estado)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las categorías.");
                throw;
            }
        }

        // Obtener categoría por ID
        public async Task<OperationResult> GetEntityByIdAsync(int id)
        {
            var result = new OperationResult();
            try
            {
                var categoria = await _context.Categoria.FindAsync(id);
                if (categoria == null)
                {
                    result.Success = false;
                    result.Message = "Categoría no encontrada.";
                }
                else
                {
                    result.Success = true;
                    result.Data = categoria;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la categoría con ID {id}.");
                result.Success = false;
                result.Message = $"Error al obtener la categoría: {ex.Message}";
            }
            return result;
        }

        // Verificar si la categoría existe
        public async Task<OperationResult> ExistsAsync(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "El ID de la categoría es inválido.";
                return result;
            }
            try
            {
                bool exists = await _context.Categoria.AnyAsync(c => c.Id == id);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia de la categoría con ID {id}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia de la categoría: {ex.Message}";
            }
            return result;
        }

        // Guardar una nueva categoría
        public async Task<OperationResult> SaveEntityAsync(int idCategoria)
        {
            var categoria = await _context.Set<Categoria>().FindAsync(idCategoria);
            if (categoria == null)
            {
                return new OperationResult { Success = false, Message = "La categoria no fue encontrado." };
            }
            return await SaveEntityAsync(categoria);
        }

        // Actualizar una categoría
        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
{
         var result = new OperationResult();
        try
        {
        var existingCategoria = await _context.Set<Categoria>().FindAsync(categoria.Id);
        if (existingCategoria == null)
        {
            return new OperationResult { Success = false, Message = "Categoría no encontrada." };
        }

        // Actualizar los datos modificables
        existingCategoria.Descripcion = categoria.Descripcion ?? existingCategoria.Descripcion;
        existingCategoria.Estado = categoria.Estado;
        existingCategoria.ModifyDate = DateTime.Now;
        existingCategoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

        // Guardar cambios
        _context.Update(existingCategoria);
        await _context.SaveChangesAsync();

        result.Success = true;
        result.Data = existingCategoria;
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar la categoría.");
        result.Success = false;
        result.Message = $"Error al actualizar la categoría: {ex.Message}";
    }
    return result;
    }

        // Eliminar una categoría permanentemente
        public async Task<OperationResult> DeleteEntityAsync(int id)
        {
            try
            {
                var categoria = await _context.Categoria.FindAsync(id);
                if (categoria == null)
                {
                    return new OperationResult { Success = false, Message = "Categoría no encontrada." };
                }

                _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Categoría eliminada correctamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría.");
                return new OperationResult { Success = false, Message = $"Error al eliminar la categoría: {ex.Message}" };
            }
        }

        // Obtener categorías por filtro
        public async Task<OperationResult> GetCategoriasByFilterAsync(Expression<Func<Categoria, bool>> filter)
        {
            if (filter == null)
            {
                return new OperationResult { Success = false, Message = "El filtro no puede ser nulo." };
            }

            var result = new OperationResult();
            try
            {
                var categorias = await _context.Categoria.Where(filter).ToListAsync();
                result.Data = categorias;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías por filtro.");
                result.Success = false;
                result.Message = $"Error al obtener categorías por filtro: {ex.Message}";
            }
            return result;
        }

        // Validación de categoría
        private OperationResult ValidateCategoria(Categoria categoria)
        {
            if (categoria == null)
            {
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            }

            if (string.IsNullOrWhiteSpace(categoria.Descripcion) || categoria.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción de la categoría es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}