using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Services;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;
using Microsoft.Extensions.Logging;
using SGHR.Infraestructure.Logging.Interfaces;


namespace SGHR.Application.Tests.Services
{
    public class UnitTestEstadoHabitacionService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly EstadoHabitacionRepository _repository;
        private readonly EstadoHabitacionService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestEstadoHabitacionService()
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
            _repository = new EstadoHabitacionRepository(
                _context,
                new LoggerFactory().CreateLogger<EstadoHabitacionRepository>(),
                new ConfigurationBuilder().AddInMemoryCollection().Build(),
                _messageMapper);

            // Configurar servicio con dependencias reales
            _service = new EstadoHabitacionService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingEstados_ReturnsAllEstados()
        {
            // Arrange
            await _repository.SaveEntityAsync(new EstadoHabitacion
            {
                Descripcion = "Disponible",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new EstadoHabitacion
            {
                Descripcion = "Ocupado",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new EstadoHabitacion
            {
                Descripcion = "Eliminado",
                Estado = true,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var estados = result.Data as List<EstadoHabitacionDto>;
            Assert.NotNull(estados);
            Assert.Equal(2, estados.Count);
            Assert.DoesNotContain(estados, e => e.Descripcion == "Eliminado");
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsEstado()
        {
            // Arrange
            var estado = new EstadoHabitacion { Descripcion = "Mantenimiento", Estado = false };
            await _repository.SaveEntityAsync(estado);

            // Act
            var result = await _service.GetById(estado.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(estado.Descripcion, ((EstadoHabitacionDto)result.Data).Descripcion);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewEstado()
        {
            // Arrange
            var dto = new SaveEstadoHabitacionDto
            {
                Descripcion = "Nuevo Estado",
                Estado = true
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((EstadoHabitacion)result.Data).Id);
            Assert.Equal(dto.Descripcion, created.Descripcion);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingEstado()
        {
            // Arrange
            var estado = new EstadoHabitacion
            { Descripcion = "Original", Estado = true };
            await _repository.SaveEntityAsync(estado);

            var dto = new UpdateEstadoHabitacionDto
            {
                IdEstadoHabitacion = estado.Id,
                Descripcion = "Actualizado",
                Estado = false // Cambiar a false
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(estado.Id);
            Assert.Equal(dto.Estado, updated.Estado); // ✅ Verifica el valor del DTO
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var estado = new EstadoHabitacion { Descripcion = "To Delete" };
            await _repository.SaveEntityAsync(estado);

            // Act
            var result = await _service.Remove(new RemoveEstadoHabitacionDto { IdEstadoHabitacion = estado.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(estado.Id);
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedEstado_UnmarksDeleted()
        {
            // Arrange
            var estado = new EstadoHabitacion { Descripcion = "Deleted", Deleted = true };
            await _repository.SaveEntityAsync(estado);

            // Act
            var result = await _service.Restore(estado.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(estado.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}