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

        public override async Task<List<Categoria>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Categoria>().Where(h => !h.Deleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Estado de La Habitacion.");
                throw;
            }
        }

        public override async Task<Categoria> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning($"ID de la categoría inválido: {id}");
                return null;
            }

            try
            {
                var categoria = await _context.Categoria.FindAsync(id);
                if (categoria == null || categoria.Deleted)
                {
                    _logger.LogWarning($"La categoría con ID {id} no encontrada o eliminada.");
                    return null;
                }
                return categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la categoría con ID {id}.");
                throw;
            }
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Categoria, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning("El filtro no puede ser nulo.");
                return false;
            }

            try
            {
                return await _context.Categoria.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia de la categoría.");
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
        {
            var validation = ValidateCategoria(categoria);
            if (!validation.Success != null)
                return validation;

            var result = new OperationResult();
            try
            {
                categoria.FechaCreacion = DateTime.Now;
                categoria.ModifyDate = DateTime.Now;
                categoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                await _context.Categoria.AddAsync(categoria);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la categoría.");
                result.Success = false;
                result.Message = $"Error al guardar la categoría: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
        {
            var result = new OperationResult();
            try
            {
                var validationResult = ValidateCategoria(categoria);
                if (!validationResult.Success != null)
                {
                    return validationResult;
                }

                var existingCategoria = await _context.Categoria.FindAsync(categoria.Id);
                if (existingCategoria == null)
                {
                    return new OperationResult { Success = false, Message = "Categoría no encontrada." };
                }

                existingCategoria.Descripcion = categoria.Descripcion ?? existingCategoria.Descripcion;
                existingCategoria.Estado = categoria.Estado;
                existingCategoria.ModifyDate = DateTime.Now;
                existingCategoria.ModifyUser = 1; 

                _context.Entry(existingCategoria).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Data = existingCategoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría.");
                result.Success = false;
                result.Message = $"Error al actualizar la categoría: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Categoria categoria)
        {
            if (categoria == null || categoria.Id <= 0)
                return new OperationResult { Success = false, Message = "El ID de la categoría es inválido." };

            try
            {
                var existingCategoria = await _context.Categoria.FindAsync(categoria.Id);
                if (existingCategoria == null)
                    return new OperationResult { Success = false, Message = "Categoría no encontrada." };

                _context.Categoria.Remove(existingCategoria);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Categoría eliminada exitosamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la categoría con ID {categoria.Id}.");
                return new OperationResult { Success = false, Message = $"Error al eliminar la categoría: {ex.Message}" };
            }
        }

        private OperationResult ValidateCategoria(Categoria categoria)
        {
            if (categoria == null)
            {
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            }

            if (string.IsNullOrWhiteSpace(categoria.Descripcion) || categoria.Descripcion.Length > 50)
            {
                return new OperationResult { Success = false, Message = "La descripción de la categoría es obligatoria y debe tener un máximo de 50 caracteres." };
            }

            if (categoria.CreationUser <= 0)
            {
                return new OperationResult { Success = false, Message = "El usuario de creación debe ser mayor que cero." };
            }

            return new OperationResult { Success = true };
        }
    }
}
