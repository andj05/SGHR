using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestPisosService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly PisoRepository _repository;
        private readonly PisosService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestPisosService()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            var loggerFactory = new LoggerFactory();

            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _repository = new PisoRepository(
                _context,
                loggerFactory.CreateLogger<PisoRepository>(),
                config,
                _messageMapper);

            _service = new PisosService(
                _repository,
                config,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingPisos_ReturnsAllPisos()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Piso
            {
                Descripcion = "Piso 1",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Piso
            {
                Descripcion = "Piso 2",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Piso
            {
                Descripcion = "Piso Eliminado",
                Estado = true,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var pisos = result.Data as List<PisosDto>;
            Assert.NotNull(pisos);
            Assert.Equal(2, pisos.Count);
            Assert.DoesNotContain(pisos, p => p.Descripcion == "Piso Eliminado");
        }



        [Fact]
        public async Task GetById_WithValidId_ReturnsPiso()
        {
            // Arrange
            var piso = new Piso { Descripcion = "Test Piso", Estado = true };
            await _repository.SaveEntityAsync(piso);

            // Act
            var result = await _service.GetById(piso.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(piso.Descripcion, ((Piso)result.Data).Descripcion);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewPiso()
        {
            // Arrange
            var dto = new SavePisosDto
            {
                Descripcion = "Nuevo Piso",
                Estado = true
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((Piso)result.Data).Id);
            Assert.Equal(dto.Descripcion, created.Descripcion);
        }


        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var piso = new Piso { Id = 2, Descripcion = "To Delete", Estado = true }; // Establece Id explícitamente
            await _repository.SaveEntityAsync(piso);

            // Act
            var result = await _service.Remove(new RemovePisosDto { IdPiso = piso.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(piso.Id);
            Assert.True(deleted.Deleted);
        }


        [Fact]
        public async Task Restore_WithDeletedPiso_UnmarksDeleted()
        {
            // Arrange
            var piso = new Piso
            {
                Descripcion = "Deleted Piso",
                Deleted = true 
            };
            await _repository.SaveEntityAsync(piso);

            // Act
            var result = await _service.Restore(piso.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(piso.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}