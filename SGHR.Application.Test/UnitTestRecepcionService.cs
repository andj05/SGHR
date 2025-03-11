using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Reservation;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;

namespace SGHR.Application.Test
{
    public class UnitTestRecepcionService
    {
        private readonly IRecepcionService _service;
        private readonly SGHRContext _context;
        private readonly MessageMapper _messageMapper;

        public UnitTestRecepcionService()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: "UnitTestDB")
                .Options;

            _context = new SGHRContext(options);
            var logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            var repo = new RecepcionRepository(_context, logger, _messageMapper);
            _service = new RecepcionService(repo, logger, _messageMapper);
        }

        [Fact]
        public async Task GetAll_ShouldReturnFailure_OnDatabaseError()
        {
            // Arrange
            var invalidContext = CreateInvalidContext();

            
            var repo = new RecepcionRepository(
                invalidContext,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper
            );

            
            await invalidContext.DisposeAsync();

            var service = new RecepcionService(
                repo,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper
            );

            // Act
            var result = await service.GetAll();

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Generic"]["GenericError"], result.Message);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailure_WhenEntityDeleted()
        {
            // Arrange
            var deletedRecepcion = new Recepcion { Id = 205, Deleted = true };
            await _context.Recepcion.AddAsync(deletedRecepcion);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["NotFound"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_ShouldReturnFailure_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.GetById(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"], result.Message);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenMissingEntryDate()
        {
            // Arrange
            var dto = new SaveRecepcionDto
            {
                FechaEntrada = default,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["MissingEntryDate"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-88)]
        public async Task Save_ShouldReturnFailure_WhenInvalidClientId(int clientId)
        {
            // Arrange
            var dto = new SaveRecepcionDto
            {
                FechaEntrada = DateTime.Now,
                IdCliente = clientId,
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidClientID"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-34)]
        public async Task Save_ShouldReturnFailure_WhenInvalidRoomId(int roomId)
        {
            // Arrange
            var dto = new SaveRecepcionDto
            {
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = roomId,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidRoomID"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-31)]
        public async Task Save_ShouldReturnFailure_WhenInvalidStatusId(int statusId)
        {
            // Arrange
            var dto = new SaveRecepcionDto
            {
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = statusId
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidStatusID"], result.Message);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenObservationTooLong()
        {
            // Arrange
            var dto = new SaveRecepcionDto
            {
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = 1,
                Observacion = new string('A', 502)
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["ObservationTooLong"], result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenMissingEntryDate()
        {
            // Arrange
            var existing = new Recepcion
            {
                Id = 100,
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };
            await _context.Recepcion.AddAsync(existing);
            await _context.SaveChangesAsync();

            var dto = new UpdateRecepcionDto
            {
                Id = 100,
                FechaEntrada = default, 
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["MissingEntryDate"], result.Message);
        }

        [Theory]
        [InlineData(0, 200)]
        [InlineData(-88, 201)]
        public async Task Update_ShouldReturnFailure_WhenInvalidClientId(int clientId, int testId)
        {
            // Arrange
            var existing = new Recepcion
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = 1, 
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };
            await _context.Recepcion.AddAsync(existing);
            await _context.SaveChangesAsync();

            var dto = new UpdateRecepcionDto
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = clientId, 
                IdHabitacion = 1,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidClientID"], result.Message);
        }

        [Theory]
        [InlineData(0, 300)]
        [InlineData(-34, 301)]
        public async Task Update_ShouldReturnFailure_WhenInvalidRoomId(int roomId, int testId)
        {
            // Arrange
            var existing = new Recepcion
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1, 
                IdEstadoReserva = 1
            };
            await _context.Recepcion.AddAsync(existing);
            await _context.SaveChangesAsync();

            var dto = new UpdateRecepcionDto
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = roomId, 
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidRoomID"], result.Message);
        }

        [Theory]
        [InlineData(0, 400)]
        [InlineData(-31, 401)]
        public async Task Update_ShouldReturnFailure_WhenInvalidStatusId(int statusId, int testId)
        {
            // Arrange
            var existing = new Recepcion
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = 1 
            };
            await _context.Recepcion.AddAsync(existing);
            await _context.SaveChangesAsync();

            var dto = new UpdateRecepcionDto
            {
                Id = testId,
                FechaEntrada = DateTime.Now,
                IdCliente = 1,
                IdHabitacion = 1,
                IdEstadoReserva = statusId 
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["InvalidStatusID"], result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenEntityNotFound()
        {
            // Arrange
            var dto = new UpdateRecepcionDto { Id = 99999 };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["NotFound"], result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenObservationTooLong()
        {
            // Arrange
            var existing = new Recepcion
            {
                Id = 2,
                Observacion = "Valid",
                FechaEntrada = DateTime.Now,
                IdEstadoReserva = 1
            };
            await _context.Recepcion.AddAsync(existing);
            await _context.SaveChangesAsync();

            var dto = new UpdateRecepcionDto
            {
                Id = 2,
                Observacion = new string('A', 501),
                FechaEntrada = DateTime.Now,
                IdEstadoReserva = 1
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["ObservationTooLong"], result.Message);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenInProgress()
        {
            // Arrange
            var recepcion = new Recepcion
            {
                Id = 1,
                IdEstadoReserva = 2,
                FechaEntrada = DateTime.Now
            };
            await _context.Recepcion.AddAsync(recepcion);
            await _context.SaveChangesAsync();

            var dto = new RemoveRecepcionDto { Id = 1 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["DeleteInProgress"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-16)]
        public async Task Restore_ShouldReturnFailure_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.Restore(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"], result.Message);
        }

        [Fact]
        public async Task Restore_ShouldReturnFailure_WhenAlreadyActive()
        {
            // Arrange
            var active = new Recepcion { Id = 777, Deleted = false };
            await _context.Recepcion.AddAsync(active);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.Restore(777);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["AlreadyActive"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-12345)]
        public async Task ObtenerRecepcionesPorEstadoReserva_ShouldReturnEmpty_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerRecepcionesPorEstadoReserva(invalidId);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-123)]
        public async Task ObtenerRecepcionesPorClienteId_ShouldReturnEmpty_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerRecepcionesPorClienteId(invalidId);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-999)]
        public async Task ObtenerRecepcionesPorHabitacionId_ShouldReturnEmpty_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerRecepcionesPorHabitacionId(invalidId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerRecepcionesPorPrecioInicial_ShouldReturnEmpty_WhenNegativePrice()
        {
            // Act
            var result = await _service.ObtenerRecepcionesPorPrecioInicial(-100m);

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
    }
}