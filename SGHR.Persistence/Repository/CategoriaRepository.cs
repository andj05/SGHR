using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;

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

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Categoria>> ObtenerTodasLasCategoriasAsync()
        {
            return await _context.Categoria
                .Where(c => c.Estado == true)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerCategoriaPorDescripcionAsync(string descripcion)
        {
            return await _context.Categoria
                .FirstOrDefaultAsync(c => c.Descripcion == descripcion);
        }

        public async Task<bool> ActualizarEstadoCategoriaAsync(int idCategoria, bool estado)
        {
            var categoria = await _context.Categoria.FindAsync(idCategoria);
            if (categoria != null)
            {
                categoria.FechaCreacion = estado ? DateTime.Now : DateTime.MinValue;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExisteCategoriaAsync(int IdCategoria)
        {
            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(c => c.IdCategoria == IdCategoria);
            return categoria != null;
        }

        public async Task<bool> ActualizarDescripcionCategoriaAsync(int idCategoria, string nuevaDescripcion)
        {
            var categoria = await _context.Categoria.FindAsync(idCategoria);
            if (categoria != null)
            {
                categoria.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<OperationResult> DeleteEntityAsync(Categoria categoria)
        {
            if (categoria == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Categoría eliminada correctamente." };
        }

        // Implementación de los métodos de la clase base
        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
        {
            if (categoria == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción de la categoría no puede estar vacía." };

            return await base.SaveEntityAsync(categoria);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
        {
            if (categoria == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            if (string.IsNullOrWhiteSpace(categoria.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción de la categoría no puede estar vacía." };

            return await base.UpdateEntityAsync(categoria);
        }

    }
}
