using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;


namespace SGHR.Persistence.Test
{
    public class UnitTestServiciosRepository : IDisposable
    {
        private readonly ServiciosRepository _serviciosRepository;
        private readonly SGHRContext _context;
        private readonly ILogger<ServiciosRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UnitTestServiciosRepository()
        {

            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<ServiciosRepository>();

            _configuration = new ConfigurationBuilder().Build();
            _messageMapper = new MessageMapper();

            _serviciosRepository = new ServiciosRepository(
                _context,
                _logger,
                _configuration,
                _messageMapper
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllServicios()
        {
            // Arrange
            var servicio1 = new Servicios
            {
                Nombre = "Servicio 1",
                Descripcion = "Descripción 1",
                Estado = true
            };
            var servicio2 = new Servicios
            {
                Nombre = "Servicio 2",
                Descripcion = "Descripción 2",
                Estado = true
            };

            await _serviciosRepository.SaveEntityAsync(servicio1);
            await _serviciosRepository.SaveEntityAsync(servicio2);

            // Act
            var result = await _serviciosRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsServicio_WhenIdIsValid()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Test",
                Descripcion = "Descripción Test",
                Estado = true
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            // Act
            var result = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(servicio.Nombre, result.Nombre);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _serviciosRepository.GetEntityByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_Success_WhenServicioIsValid()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Nuevo Servicio",
                Descripcion = "Nueva Descripción",
                Estado = true
            };

            // Act
            var result = await _serviciosRepository.SaveEntityAsync(servicio);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["SaveSuccess"], result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_Fails_WhenServicioIsNull()
        {
            // Act
            var result = await _serviciosRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_UpdatesServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Original",
                Descripcion = "Original",
                Estado = true
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            servicio.Nombre = "Actualizado";
            servicio.Descripcion = "Actualizado";

            // Act
            var result = await _serviciosRepository.UpdateEntityAsync(servicio);

            // Assert
            Assert.True(result.Success);
            var updated = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);
            Assert.Equal("Actualizado", updated.Nombre);
        }

        [Fact]
        public async Task DeleteEntityAsync_DeletesServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio a Eliminar",
                Descripcion = "Descripción",
                Estado = true
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            // Act
            var result = await _serviciosRepository.DeleteEntityAsync(servicio);

            // Assert
            Assert.True(result.Success);
            var deleted = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_RestoresDeletedServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Restaurado",
                Descripcion = "Descripción",
                Estado = true,
                Deleted = true // Marcar como eliminado
            };
            await _context.Servicios.AddAsync(servicio);
            await _context.SaveChangesAsync();

            // Act
            var result = await _serviciosRepository.RestoreEntityAsync(servicio);

            // Assert
            Assert.True(result.Success);
            var restored = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenServicioExists()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Existente",
                Descripcion = "Descripción",
                Estado = true
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            // Act
            var exists = await _serviciosRepository.ExistsAsync(s => s.Nombre == "Servicio Existente");

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