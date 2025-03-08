using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Configurations;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Persistence.Test
{
    public class UnitTestTarifasRepository : IDisposable
    {
        private readonly TarifasRepository _tarifasRepository;
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestTarifasRepository()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            // Inicializar MessageMapper real (con mensajes del sistema)
            _messageMapper = new MessageMapper();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
            });
            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());

            // Inyectar dependencias reales
            _tarifasRepository = new TarifasRepository(_context, _logger, _messageMapper);
        }

        [Fact]
        public async Task AddTarifa_ShouldReturnFailure_WhenTarifaIsNull()
        {
            // Act
            var result = await _tarifasRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTarifas()
        {
            // Arrange
            var tarifa1 = new Tarifas
            {
                Descripcion = "Tarifa 1",
                Estado = true,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 100,
                IdHabitacion = 1
            };

            var tarifa2 = new Tarifas
            {
                Descripcion = "Tarifa 2",
                Estado = true,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 200,
                IdHabitacion = 2
            };

            await _tarifasRepository.SaveEntityAsync(tarifa1);
            await _tarifasRepository.SaveEntityAsync(tarifa2);

            // Act
            var result = await _tarifasRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnTarifa_WhenIdIsValid()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "Tarifa 1",
                Estado = true,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 100,
                IdHabitacion = 1
            };

            await _tarifasRepository.SaveEntityAsync(tarifa);

            // Act
            var result = await _tarifasRepository.GetEntityByIdAsync(tarifa.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tarifa.Descripcion, result.Descripcion);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldUpdateTarifa()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "Original",
                Estado = true,
                PrecioPorNoche = 100,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                IdHabitacion = 1
            };

            await _tarifasRepository.SaveEntityAsync(tarifa);

            tarifa.Descripcion = "Actualizada";
            tarifa.PrecioPorNoche = 150;

            // Act
            var result = await _tarifasRepository.UpdateEntityAsync(tarifa);

            // Assert
            Assert.True(result.Success);
            var updated = await _tarifasRepository.GetEntityByIdAsync(tarifa.Id);
            Assert.Equal("Actualizada", updated.Descripcion);
            Assert.Equal(150, updated.PrecioPorNoche);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldDeleteTarifa()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "Tarifa Test",
                Estado = true,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 100,
                IdHabitacion = 1
            };

            await _tarifasRepository.SaveEntityAsync(tarifa);

            // Act
            var result = await _tarifasRepository.DeleteEntityAsync(tarifa);

            // Assert
            Assert.True(result.Success);
            var deleted = await _tarifasRepository.GetEntityByIdAsync(tarifa.Id);
            Assert.Null(deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}