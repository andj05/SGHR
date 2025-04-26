using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;

namespace SGHR.Persistence.Test
{
    public class UnitTestPisoRepository : IDisposable
    {
        private readonly PisoRepository _pisoRepository;
        private readonly SGHRContext _context;
        private readonly ILogger<PisoRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UnitTestPisoRepository()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<PisoRepository>();

            _configuration = new ConfigurationBuilder().Build();
            _messageMapper = new MessageMapper();

            _pisoRepository = new PisoRepository(
                _context,
                _logger,
                _configuration,
                _messageMapper
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPisos()
        {
            // Arrange
            var piso1 = new Piso
            {
                Descripcion = "Piso 1",
                Estado = true
            };
            var piso2 = new Piso
            {
                Descripcion = "Piso 2",
                Estado = true
            };

            await _pisoRepository.SaveEntityAsync(piso1);
            await _pisoRepository.SaveEntityAsync(piso2);

            // Act
            var result = await _pisoRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsPiso_WhenIdIsValid()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Piso Test",
                Estado = true
            };
            await _pisoRepository.SaveEntityAsync(piso);

            // Act
            var result = await _pisoRepository.GetEntityByIdAsync(piso.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(piso.Descripcion, result.Descripcion);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _pisoRepository.GetEntityByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_Success_WhenPisoIsValid()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Nuevo Piso",
                Estado = true
            };

            // Act
            var result = await _pisoRepository.SaveEntityAsync(piso);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["SaveSuccess"], result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_Fails_WhenPisoIsNull()
        {
            // Act
            var result = await _pisoRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_UpdatesPisoDescription()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Original",
                Estado = true
            };
            await _pisoRepository.SaveEntityAsync(piso);

            piso.Descripcion = "Actualizado";

            // Act
            var result = await _pisoRepository.UpdateEntityAsync(piso);

            // Assert
            Assert.True(result.Success);
            var updated = await _pisoRepository.GetEntityByIdAsync(piso.Id);
            Assert.Equal("Actualizado", updated.Descripcion);
        }

        [Fact]
        public async Task UpdateEntityAsync_KeepsDescription_WhenNewDescriptionIsNull()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Original",
                Estado = true
            };
            await _pisoRepository.SaveEntityAsync(piso);

            var pisoToUpdate = new Piso
            {
                Id = piso.Id,
                Descripcion = null, // Campo a null
                Estado = false
            };

            // Act
            var result = await _pisoRepository.UpdateEntityAsync(pisoToUpdate);

            // Assert
            Assert.True(result.Success);
            var updated = await _pisoRepository.GetEntityByIdAsync(piso.Id);
            Assert.Equal("Original", updated.Descripcion); // Mantiene el valor original
        }

        [Fact]
        public async Task DeleteEntityAsync_DeletesPiso()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Piso a Eliminar",
                Estado = true
            };
            await _pisoRepository.SaveEntityAsync(piso);

            // Act
            var result = await _pisoRepository.DeleteEntityAsync(piso);

            // Assert
            Assert.True(result.Success);
            var deleted = await _pisoRepository.GetEntityByIdAsync(piso.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_RestoresSoftDeletedPiso()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Piso Restaurado",
                Estado = true,
                Deleted = true // Simular eliminación lógica
            };
            await _context.Pisos.AddAsync(piso);
            await _context.SaveChangesAsync();

            // Act
            var result = await _pisoRepository.RestoreEntityAsync(piso);

            // Assert
            Assert.True(result.Success);
            var restored = await _pisoRepository.GetEntityByIdAsync(piso.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenPisoExists()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Piso Existente",
                Estado = true
            };
            await _pisoRepository.SaveEntityAsync(piso);

            // Act
            var exists = await _pisoRepository.ExistsAsync(p => p.Descripcion == "Piso Existente");

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