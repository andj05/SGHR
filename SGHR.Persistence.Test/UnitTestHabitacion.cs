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
    public class UnitTestHabitacion
    {
        private readonly IHabitacionRepository _repository;
        private readonly SGHRContext _context;
        private readonly MessageMapper _messageMapper;

        public UnitTestHabitacion()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: "UnitTestDB")
                .Options;

            _context = new SGHRContext(options);
            var logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            _repository = new HabitacionRepository(_context, logger, _messageMapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_OnDatabaseError()
        {
            // Arrange
            var invalidContext = CreateInvalidContext();
            var repo = new HabitacionRepository(invalidContext,
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
            Habitacion nullEntity = null!;

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
            Habitacion nullEntity = null!;

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
            Habitacion nullEntity = null!;

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
        public async Task ObtenerHabitacionesPorNumeroAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidNumber = "INVALID";

            // Act
            var result = await invalidRepo.ObtenerHabitacionesPorNumeroAsync(invalidNumber);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerHabitacionesPorPisoIdAsync_ShouldReturnEmpty_OnError()
        {
            // Arrange
            var invalidRepo = CreateRepositoryWithInvalidContext();
            var invalidPisoId = -1;

            // Act
            var result = await invalidRepo.ObtenerHabitacionesPorPisoIdAsync(invalidPisoId);

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

        private IHabitacionRepository CreateRepositoryWithInvalidContext()
        {
            var invalidContext = CreateInvalidContext();
            return new HabitacionRepository(
                invalidContext,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper);
        }
    }
}