using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using SGHR.Persistence.Context;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;
using Xunit;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestCategoriasService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly CategoriaRepository _repository;
        private readonly CategoriasService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestCategoriasService()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            // Configurar dependencias reales
            _logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            // Inicializar repositorio real
            _repository = new CategoriaRepository(
                _context,
                new LoggerFactory().CreateLogger<CategoriaRepository>(),
                new ConfigurationBuilder().AddInMemoryCollection().Build(),
                _messageMapper);

            // Configurar servicio con dependencias reales
            _service = new CategoriasService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingCategorias_ReturnsAllCategorias()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Categoria
            {
                Descripcion = "Categoría 1",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Categoria
            {
                Descripcion = "Categoría 2",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Categoria
            {
                Descripcion = "Categoría Eliminada",
                Estado = true,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var categorias = result.Data as List<CategoriasDto>;
            Assert.NotNull(categorias);
            Assert.Equal(2, categorias.Count);
            Assert.DoesNotContain(categorias, c => c.Descripcion == "Categoría Eliminada");
        }



        [Fact]
        public async Task GetById_WithValidId_ReturnsCategoria()
        {
            // Arrange
            var categoria = new Categoria { Descripcion = "Categoría Test", Estado = true };
            await _repository.SaveEntityAsync(categoria);

            // Act
            var result = await _service.GetById(categoria.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(categoria.Descripcion, ((Categoria)result.Data).Descripcion);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewCategoria()
        {
            // Arrange
            var dto = new SaveCategoriasDto
            {
                Descripcion = "Nueva Categoría",
                Estado = true
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((Categoria)result.Data).Id);
            Assert.Equal(dto.Descripcion, created.Descripcion);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingCategoria()
        {
            // Arrange
            var categoria = new Categoria
            { Descripcion = "Original", Estado = true };
            await _repository.SaveEntityAsync(categoria);

            var dto = new UpdateCategoriasDto
            {
                IdCategoria = categoria.Id,
                Descripcion = "Actualizado",
                Estado = false
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(categoria.Id);
            Assert.Equal(dto.Descripcion, updated.Descripcion);
            Assert.Equal(dto.Estado, updated.Estado);
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var categoria = new Categoria { Descripcion = "Para Eliminar" };
            await _repository.SaveEntityAsync(categoria);

            // Act
            var result = await _service.Remove(new RemoveCategoriasDto { IdCategoria = categoria.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(categoria.Id);
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedCategoria_UnmarksDeleted()
        {
            // Arrange
            var categoria = new Categoria { Descripcion = "Eliminada", Deleted = true };
            await _repository.SaveEntityAsync(categoria);

            // Act
            var result = await _service.Restore(categoria.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(categoria.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task Save_WithInvalidDescription_ReturnsError()
        {
            var dto = new SaveCategoriasDto { Descripcion = "", Estado = true };

            var result = await _service.Save(dto);

            Assert.False(result.Success);
            Assert.Contains("debe tener entre 1 y 50 caracteres", result.Message); // Nueva validación
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _service.GetById(-1);

            // Assert
            Assert.False(result.Success);
            Assert.Contains(_messageMapper.ErrorMessages["EntityBase"]["NotFound"], result.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}