using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Configurations;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repository
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<CategoriaRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public CategoriaRepository(SGHRContext context,
                                    ILogger<CategoriaRepository> logger,
                                    IConfiguration configuration,
                                    MessageMapper messageMapper) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        private OperationResult ValidateCategoriaPersistence(Categoria categoria)
        {
            if (categoria == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
            return new OperationResult { Success = true };
        }

        public override async Task<List<Categoria>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Categoria>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Categoria"]["GetAllError"]);
                throw;
            }
        }

        public override async Task<Categoria> GetEntityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);
                return null;
            }

            try
            {
                var categoria = await _context.Set<Categoria>()
                                              .IgnoreQueryFilters() // Include deleted entities
                                              .FirstOrDefaultAsync(c => c.Id == id);

                if (categoria == null)
                {
                    _logger.LogWarning(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
                    return null;
                }
                return categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Categoria"]["GetByIdError"]);
                throw;
            }
        }


        public override async Task<bool> ExistsAsync(Expression<Func<Categoria, bool>> filter)
        {
            if (filter == null)
            {
                _logger.LogWarning(_messageMapper.ErrorMessages["Generic"]["NullFilter"]);
                return false;
            }

            try
            {
                return await _context.Categoria.AnyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Categoria"]["ExistsError"]);
                throw;
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
        {
            var validationResult = ValidateCategoriaPersistence(categoria);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                categoria.FechaCreacion = DateTime.UtcNow;
                categoria.ModifyDate = DateTime.UtcNow;
                categoria.ModifyUser = 1;

                await _context.Set<Categoria>().AddAsync(categoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["SaveSuccess"],
                    Data = categoria
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"]
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
        {
            var validationResult = ValidateCategoriaPersistence(categoria);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            var result = new OperationResult();
            try
            {
                var existingCategoria = await _context.Set<Categoria>().FindAsync(categoria.Id);
                if (existingCategoria == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingCategoria.Descripcion = categoria.Descripcion;
                existingCategoria.Estado = categoria.Estado;
                existingCategoria.ModifyDate = DateTime.UtcNow;
                existingCategoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Update(existingCategoria);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = _messageMapper.SuccessMessages["UpdateSuccess"];
                result.Data = existingCategoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["UpdateFailed"]);
                result.Success = false;
                result.Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
            }
            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Categoria categoria)
        {
            var validationResult = ValidateCategoriaPersistence(categoria);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            if (categoria.Id <= 0)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["EntityBase"]["InvalidID"]
                };
            }

            try
            {
                var existingCategoria = await _context.Categoria.FindAsync(categoria.Id);
                if (existingCategoria == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                _context.Categoria.Remove(existingCategoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["DeleteSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"]
                };
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(Categoria categoria)
        {
            var validationResult = ValidateCategoriaPersistence(categoria);
            if (validationResult.Success != true)
            {
                return validationResult;
            }

            try
            {
                // Buscamos la categoría ignorando los filtros (para poder encontrar los eliminados)
                var existingCategoria = await _context.Set<Categoria>()
                                              .IgnoreQueryFilters()
                                              .FirstOrDefaultAsync(e => e.Id == categoria.Id);

                if (existingCategoria == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"]
                    };
                }

                existingCategoria.Deleted = false;
                existingCategoria.ModifyDate = DateTime.UtcNow;
                existingCategoria.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Update(existingCategoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    Success = true,
                    Message = _messageMapper.SuccessMessages["RestoreSuccess"]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]);
                return new OperationResult
                {
                    Success = false,
                    Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"]
                };
            }
        }
    }
}