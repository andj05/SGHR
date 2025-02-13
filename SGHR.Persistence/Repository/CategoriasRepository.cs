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
    public class CategoriasRepository : BaseRepository<Categorias>, ICategoriasRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<CategoriasRepository> _logger;
        private readonly IConfiguration _configuration;

        public CategoriasRepository(SGHRContext context,
            ILogger<CategoriasRepository> logger,
            IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        // Implementación de los métodos de la interfaz

        public async Task<IEnumerable<Categorias>> ObtenerTodasLasCategoriasAsync()
        {
            return await _context.Categorias.ToListAsync();
        }

        public async Task<IEnumerable<Categorias>> ObtenerCategoriasActivasAsync()
        {
            return await _context.Categorias
                .Where(c => c.FechaCreacion <= DateTime.Now) // Ejemplo de condición para categorías activas
                .ToListAsync();
        }

        public async Task<Categorias?> ObtenerCategoriaPorDescripcionAsync(string descripcion)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Descripcion == descripcion);
        }

        public async Task<bool> ActualizarEstadoCategoriaAsync(int idCategoria, bool estado)
        {
            var categoria = await _context.Categorias.FindAsync(idCategoria);
            if (categoria != null)
            {
                // Actualiza el estado de la categoría según el parámetro 'estado'
                categoria.FechaCreacion = estado ? DateTime.Now : DateTime.MinValue;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExisteCategoriaAsync(int idCategoria)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == idCategoria);
            return categoria != null;
        }

        public async Task<bool> ActualizarDescripcionCategoriaAsync(int idCategoria, string nuevaDescripcion)
        {
            var categoria = await _context.Categorias.FindAsync(idCategoria);
            if (categoria != null)
            {
                categoria.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Otros métodos de la clase (como SaveEntityAsync y UpdateEntityAsync)
        public override async Task<OperationResult> SaveEntityAsync(Categorias entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción de la categoría no puede estar vacía." };

            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Categorias entity)
        {
            if (entity == null)
                return new OperationResult { Success = false, Message = "La categoría no puede ser nula." };
            if (string.IsNullOrWhiteSpace(entity.Descripcion))
                return new OperationResult { Success = false, Message = "La descripción de la categoría no puede estar vacía." };

            return await base.UpdateEntityAsync(entity);
        }
    }
}
