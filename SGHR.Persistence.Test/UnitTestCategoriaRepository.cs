using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Domain.Base;
using SGHR.Persistence.Configurations;
using Xunit;

namespace SGHR.Persistence.Test
{
    public class UnitTestCategoriaRepository : IDisposable
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly SGHRContext _context;
        private readonly ILogger<CategoriaRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UnitTestCategoriaRepository()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            // Configurar logger real (consola)
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<CategoriaRepository>();

            // Configurar IConfiguration (vacío para pruebas)
            _configuration = new ConfigurationBuilder().Build();

            // Inicializar MessageMapper real
            _messageMapper = new MessageMapper();

            // Inyectar dependencias
            _categoriaRepository = new CategoriaRepository(
                _context,
                _logger,
                _configuration,
                _messageMapper
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategorias()
        {
            // Arrange
            var categoria1 = new Categoria
            {
                Descripcion = "Categoría 1",
                Estado = true
            };
            var categoria2 = new Categoria
            {
                Descripcion = "Categoría 2",
                Estado = true
            };

            await _categoriaRepository.SaveEntityAsync(categoria1);
            await _categoriaRepository.SaveEntityAsync(categoria2);

            // Act
            var result = await _categoriaRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsCategoria_WhenIdIsValid()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Categoría Test",
                Estado = true
            };
            await _categoriaRepository.SaveEntityAsync(categoria);

            // Act
            var result = await _categoriaRepository.GetEntityByIdAsync(categoria.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoria.Descripcion, result.Descripcion);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _categoriaRepository.GetEntityByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_Success_WhenCategoriaIsValid()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Nueva Categoría",
                Estado = true
            };

            // Act
            var result = await _categoriaRepository.SaveEntityAsync(categoria);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["SaveSuccess"], result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_Fails_WhenCategoriaIsNull()
        {
            // Act
            var result = await _categoriaRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_UpdatesCategoria()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Original",
                Estado = true
            };
            await _categoriaRepository.SaveEntityAsync(categoria);

            categoria.Descripcion = "Actualizado";
            categoria.Estado = false;

            // Act
            var result = await _categoriaRepository.UpdateEntityAsync(categoria);

            // Assert
            Assert.True(result.Success);
            var updated = await _categoriaRepository.GetEntityByIdAsync(categoria.Id);
            Assert.Equal("Actualizado", updated.Descripcion);
            Assert.False(updated.Estado);
        }

        [Fact]
        public async Task DeleteEntityAsync_DeletesCategoria()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Categoría a Eliminar",
                Estado = true
            };
            await _categoriaRepository.SaveEntityAsync(categoria);

            // Act
            var result = await _categoriaRepository.DeleteEntityAsync(categoria);

            // Assert
            Assert.True(result.Success);
            var deleted = await _categoriaRepository.GetEntityByIdAsync(categoria.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_RestoresSoftDeletedCategoria()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Categoría Restaurada",
                Estado = true,
                Deleted = true // Simular eliminación lógica
            };
            await _context.Categoria.AddAsync(categoria);
            await _context.SaveChangesAsync();

            // Act
            var result = await _categoriaRepository.RestoreEntityAsync(categoria);

            // Assert
            Assert.True(result.Success);
            var restored = await _categoriaRepository.GetEntityByIdAsync(categoria.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenCategoriaExists()
        {
            // Arrange
            var categoria = new Categoria
            {
                Descripcion = "Categoría Existente",
                Estado = true
            };
            await _categoriaRepository.SaveEntityAsync(categoria);

            // Act
            var exists = await _categoriaRepository.ExistsAsync(c => c.Descripcion == "Categoría Existente");

            // Assert
            Assert.True(exists);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}