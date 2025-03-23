using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Configurations;
using Microsoft.Extensions.Logging;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestTarifasService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly TarifasRepository _repository;
        private readonly TarifasService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestTarifasService()
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
            _repository = new TarifasRepository(_context, _logger, _messageMapper);

            // Configurar servicio con dependencias reales
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _service = new TarifasService(
                _repository,
                config,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingTarifas_ReturnsAllTarifas()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Tarifas
            {
                Descripcion = "Tarifa 1",
                PrecioPorNoche = 100,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Tarifas
            {
                Descripcion = "Tarifa 2",
                PrecioPorNoche = 200,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Tarifas
            {
                Descripcion = "Tarifa Eliminada",
                PrecioPorNoche = 300,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var tarifas = result.Data as List<TarifasDto>;
            Assert.NotNull(tarifas);
            Assert.Equal(2, tarifas.Count);
            Assert.DoesNotContain(tarifas, t => t.Descripcion == "Tarifa Eliminada");
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsTarifa()
        {
            // Arrange
            var tarifa = new Tarifas { Descripcion = "Test", PrecioPorNoche = 150 };
            await _repository.SaveEntityAsync(tarifa);

            // Act
            var result = await _service.GetById(tarifa.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(tarifa.Descripcion, ((Tarifas)result.Data).Descripcion);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewTarifa()
        {
            // Arrange
            var dto = new SaveTarifasDto
            {
                Descripcion = "Nueva Tarifa",
                PrecioPorNoche = 99.99m,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                IdHabitacion = 1,
                Descuento = 10
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((Tarifas)result.Data).Id);
            Assert.NotNull(created);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingTarifa()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "Original",
                PrecioPorNoche = 100,
                FechaInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                // Aumentar la duración a 14 días para cumplir con la regla de duración mínima de 7 días
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                IdHabitacion = 1,
                Descuento = 0,
                Deleted = false
            };
            await _repository.SaveEntityAsync(tarifa);

            var dto = new UpdateTarifasDto
            {
                IdTarifa = tarifa.Id,
                Descripcion = "Actualizada",
                // Modificar el precio para que no exceda el 30% de cambio
                PrecioPorNoche = 130, // 30% de aumento sobre 100
                FechaInicio = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                IdHabitacion = 1,
                Descuento = 10
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(tarifa.Id);
            Assert.Equal("Actualizada", updated.Descripcion);
            Assert.Equal(130, updated.PrecioPorNoche);
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "To Delete",
                PrecioPorNoche = 100,
                FechaInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
                IdHabitacion = 1,
                Descuento = 0,
                Deleted = false
            };
            await _repository.SaveEntityAsync(tarifa);

            var dto = new RemoveTarifasDto
            {
                IdTarifa = tarifa.Id,
            };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(tarifa.Id);
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedTarifa_UnmarksDeleted()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                Descripcion = "Deleted",
                PrecioPorNoche = 100,
                FechaInicio = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
                IdHabitacion = 1,
                Descuento = 0,
                Deleted = true
            };
            await _repository.SaveEntityAsync(tarifa);

            // Act
            var result = await _service.Restore(tarifa.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(tarifa.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}