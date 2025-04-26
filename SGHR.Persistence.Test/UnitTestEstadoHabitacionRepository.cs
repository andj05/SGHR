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
    public class UnitTestEstadoHabitacionRepository : IDisposable
    {
        private readonly EstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly SGHRContext _context;
        private readonly ILogger<EstadoHabitacionRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UnitTestEstadoHabitacionRepository()
        {

            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);


            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<EstadoHabitacionRepository>();


            _configuration = new ConfigurationBuilder().Build();
            _messageMapper = new MessageMapper();

            _estadoHabitacionRepository = new EstadoHabitacionRepository(
                _context,
                _logger,
                _configuration,
                _messageMapper
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEstados()
        {
            // Arrange
            var estado1 = new EstadoHabitacion
            {
                Descripcion = "Disponible",
                Estado = true
            };
            var estado2 = new EstadoHabitacion
            {
                Descripcion = "Ocupado",
                Estado = true
            };

            await _estadoHabitacionRepository.SaveEntityAsync(estado1);
            await _estadoHabitacionRepository.SaveEntityAsync(estado2);

            // Act
            var result = await _estadoHabitacionRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsEstado_WhenIdIsValid()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Mantenimiento",
                Estado = true
            };
            await _estadoHabitacionRepository.SaveEntityAsync(estado);

            // Act
            var result = await _estadoHabitacionRepository.GetEntityByIdAsync(estado.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(estado.Descripcion, result.Descripcion);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _estadoHabitacionRepository.GetEntityByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_Success_WhenEstadoIsValid()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Nuevo Estado",
                Estado = true
            };

            // Act
            var result = await _estadoHabitacionRepository.SaveEntityAsync(estado);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["SaveSuccess"], result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_Fails_WhenEstadoIsNull()
        {
            // Act
            var result = await _estadoHabitacionRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_UpdatesEstado()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Original",
                Estado = true
            };
            await _estadoHabitacionRepository.SaveEntityAsync(estado);

            estado.Descripcion = "Actualizado";
            estado.Estado = false;

            // Act
            var result = await _estadoHabitacionRepository.UpdateEntityAsync(estado);

            // Assert
            Assert.True(result.Success);
            var updated = await _estadoHabitacionRepository.GetEntityByIdAsync(estado.Id);
            Assert.Equal("Actualizado", updated.Descripcion);
            Assert.False(updated.Estado);
        }

        [Fact]
        public async Task DeleteEntityAsync_DeletesEstado()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Estado a Eliminar",
                Estado = true
            };
            await _estadoHabitacionRepository.SaveEntityAsync(estado);

            // Act
            var result = await _estadoHabitacionRepository.DeleteEntityAsync(estado);

            // Assert
            Assert.True(result.Success);
            var deleted = await _estadoHabitacionRepository.GetEntityByIdAsync(estado.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_RestoresSoftDeletedEstado()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Estado Restaurado",
                Estado = true,
                Deleted = true // Simular eliminación lógica
            };
            await _context.EstadoHabitacion.AddAsync(estado);
            await _context.SaveChangesAsync();

            // Act
            var result = await _estadoHabitacionRepository.RestoreEntityAsync(estado);

            // Assert
            Assert.True(result.Success);
            var restored = await _estadoHabitacionRepository.GetEntityByIdAsync(estado.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenEstadoExists()
        {
            // Arrange
            var estado = new EstadoHabitacion
            {
                Descripcion = "Estado Existente",
                Estado = true
            };
            await _estadoHabitacionRepository.SaveEntityAsync(estado);

            // Act
            var exists = await _estadoHabitacionRepository.ExistsAsync(e => e.Descripcion == "Estado Existente");

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