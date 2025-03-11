using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Reservation;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

namespace SGHR.Persistence.Test
{
    public class UnitTestRecepcion
    {
        private readonly IRecepcionRepository _repository;
        private readonly SGHRContext _context;
        private readonly MessageMapper _messageMapper;

        public UnitTestRecepcion()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                                                                  .UseInMemoryDatabase(databaseName: "UnitTestDB")
                                                                  .Options;

            _context = new SGHRContext(options);
            var logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            _repository = new RecepcionRepository(_context, logger, _messageMapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_OnDatabaseError()
        {
            // Arrange
            var invalidContext = CreateInvalidContext();
            var repo = new RecepcionRepository(invalidContext,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnNull_WhenEntityNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _repository.GetEntityByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldReturnFailure_WhenEntityIsNull()
        {
            // Arrange
            Recepcion nullEntity = null!;

            // Act
            var result = await _repository.SaveEntityAsync(nullEntity);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldReturnFailure_WhenEntityIsNull()
        {
            // Arrange
            Recepcion nullEntity = null!;

            // Act
            var result = await _repository.UpdateEntityAsync(nullEntity);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["NullEntity"], result.Message);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldReturnValidationError_WhenEntityIsNull()
        {
            // Arrange
            Recepcion nullEntity = null!;

            // Act
            var result = await _repository.DeleteEntityAsync(nullEntity);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                _messageMapper.ErrorMessages["EntityBase"]["NullEntity"],
                result.Message
            );
        }

        [Fact]
        public async Task RestoreEntityAsync_ShouldReturnFailure_WhenEntityNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _repository.RestoreEntityAsync(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("no encontrada", result.Message);
        }

        [Fact]
        public async Task ObtenerRecepcionesPorClienteIdAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidClienteId = -1;

            // Act
            var result = await invalidRepo.ObtenerRecepcionesPorClienteIdAsync(invalidClienteId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerRecepcionesPorHabitacionIdAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidHabitacionId = -1;

            // Act
            var result = await invalidRepo.ObtenerRecepcionesPorHabitacionIdAsync(invalidHabitacionId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerRecepcionesPorEstadoReservaAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidEstadoId = -1;

            // Act
            var result = await invalidRepo.ObtenerRecepcionesPorEstadoReservaAsync(invalidEstadoId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerRecepcionesPorPrecioInicialAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidPrecio = -1.0m;

            // Act
            var result = await invalidRepo.ObtenerRecepcionesPorPrecioInicialAsync(invalidPrecio);

            // Assert
            Assert.Empty(result);
        }

        private SGHRContext CreateInvalidContext()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: "InvalidDB")
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new SGHRContext(options);
            context.Database.EnsureDeleted();
            return context;
        }

        private IRecepcionRepository CreateRepositoryWithInvalidContext()
        {
            var invalidContext = CreateInvalidContext();
            return new RecepcionRepository(
                invalidContext,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper);
        }
    }
}
