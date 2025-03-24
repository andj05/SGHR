using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Reservation;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;

namespace SGHR.Application.Test
{
    public class UnitTestHabitacionService
    {
            private readonly IHabitacionService _service;
            private readonly SGHRContext _context;
            private readonly MessageMapper _messageMapper;

            public UnitTestHabitacionService()
            {
                var options = new DbContextOptionsBuilder<SGHRContext>()
                    .UseInMemoryDatabase(databaseName: "UnitTestDB")
                    .Options;

                _context = new SGHRContext(options);
                var logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
                _messageMapper = new MessageMapper();

                var repo = new HabitacionRepository(_context, logger, _messageMapper);
                _service = new HabitacionService(repo, logger, _messageMapper);
            }

        [Fact]
        public async Task GetAll_ShouldReturnFailure_OnDatabaseError()
        {
            // Arrange
            var invalidContext = CreateInvalidContext();

            
            var repo = new HabitacionRepository(
                invalidContext,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper
            );

            
            var service = new HabitacionService(
                repo,
                new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>()),
                _messageMapper
            );

            
            await invalidContext.DisposeAsync();

            // Act
            var result = await service.GetAll();

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Generic"]["GenericError"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-321)]
        public async Task GetById_ShouldReturnFailure_WhenInvalidId(int invalidId)
        {
            // Act
            var result = await _service.GetById(invalidId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"], result.Message);
        }

        [Fact]
        public async Task GetById_ShouldReturnFailure_WhenEntityDeleted()
        {
            // Arrange
            var deletedRoom = new Habitacion
            {
                Id = 1,
                Deleted = true,
                Numero = "101",
                IdEstadoHabitacion = 1
            };
            await _context.Habitacion.AddAsync(deletedRoom);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetById(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["EntityBase"]["NotFound"], result.Message);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDuplicateNumber()
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 27,
                Numero = "101",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                CreationUser = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var dto = new SaveHabitacionDto
            {
                Numero = "101", 
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                ChangeUser = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"]["DuplicateNumber"], result.Message);
        }

        [Theory]
        [InlineData(null, "MissingNumber")]
        [InlineData("", "MissingNumber")]
        [InlineData("123456789012345678901234567890123456789012345678901", "NumberTooLong")]
        public async Task Save_ShouldReturnFailure_WhenNumberInvalid(string number, string expectedErrorKey)
        {
            // Arrange
            var dto = new SaveHabitacionDto
            {
                Numero = number,
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDetailTooLong()
        {
            // Arrange
            var dto = new SaveHabitacionDto
            {
                Numero = "101",
                Detalle = new string('A', 101),
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"]["DetailTooLong"], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidStatusID")]
        [InlineData(-1, "InvalidStatusID")]
        public async Task Save_ShouldReturnFailure_WhenInvalidStatusId(int statusId, string expectedErrorKey)
        {
            // Arrange
            var dto = new SaveHabitacionDto
            {
                Numero = "101",
                IdEstadoHabitacion = statusId,
                IdPiso = 1,
                IdCategoria = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidFloorID")]
        [InlineData(-1, "InvalidFloorID")]
        public async Task Save_ShouldReturnFailure_WhenInvalidFloorId(int floorId, string expectedErrorKey)
        {
            // Arrange
            var dto = new SaveHabitacionDto
            {
                Numero = "101",
                IdEstadoHabitacion = 1,
                IdPiso = floorId,
                IdCategoria = 1
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidCategoryID")]
        [InlineData(-1, "InvalidCategoryID")]
        public async Task Save_ShouldReturnFailure_WhenInvalidCategoryId(int categoryId, string expectedErrorKey)
        {
            // Arrange
            var dto = new SaveHabitacionDto
            {
                Numero = "101",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = categoryId
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenDuplicateNumber()
        {
            // Arrange
            var existingRoom1 = new Habitacion
            {
                Id = 33,
                Numero = "101",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                CreationUser = 1
            };
            var existingRoom2 = new Habitacion
            {
                Id = 2,
                Numero = "102",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                CreationUser = 1
            };
            await _context.Habitacion.AddAsync(existingRoom1);
            await _context.Habitacion.AddAsync(existingRoom2);
            await _context.SaveChangesAsync();

            var dto = new UpdateHabitacionDto
            {
                Id = 2,
                Numero = "101", 
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                ChangeUser = 1
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"]["DuplicateNumber"], result.Message);
        }


        [Fact]
        public async Task Update_ShouldReturnFailure_WhenEntityNotFound()
        {
            // Arrange
            var dto = new UpdateHabitacionDto { Id = 99999 };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["UpdateFailed"], result.Message);
        }

        [Theory]
        [InlineData(null, "MissingNumber")]
        [InlineData("", "MissingNumber")]
        [InlineData("1234567890123456789012345678901234567890123456789003200000012221", "NumberTooLong")]
        public async Task Update_ShouldReturnFailure_WhenNumberInvalid(string number, string expectedErrorKey)
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 593,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                CreationUser = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var invalidDto = new UpdateHabitacionDto
            {
                Id = 593,
                Numero = number,
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1,
                ChangeUser = 1
            };

            // Act
            var result = await _service.Update(invalidDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidStatusID")]
        [InlineData(-10, "InvalidStatusID")]
        public async Task Update_ShouldReturnFailure_WhenInvalidStatusId(int statusId, string expectedErrorKey)
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 787,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var invalidDto = new UpdateHabitacionDto
            {
                Id = 787,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = statusId, 
                IdPiso = 1,
                IdCategoria = 1
            };

            // Act
            var result = await _service.Update(invalidDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidFloorID")]
        [InlineData(-32, "InvalidFloorID")]
        public async Task Update_ShouldReturnFailure_WhenInvalidFloorId(int floorId, string expectedErrorKey)
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 91,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var invalidDto = new UpdateHabitacionDto
            {
                Id = 91,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = floorId, 
                IdCategoria = 1
            };

            // Act
            var result = await _service.Update(invalidDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Theory]
        [InlineData(0, "InvalidCategoryID")]
        [InlineData(-25, "InvalidCategoryID")]
        public async Task Update_ShouldReturnFailure_WhenInvalidCategoryId(int categoryId, string expectedErrorKey)
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 42,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var invalidDto = new UpdateHabitacionDto
            {
                Id = 42,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = categoryId 
            };

            // Act
            var result = await _service.Update(invalidDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Room"][expectedErrorKey], result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenDetailTooLong()
        {
            // Arrange
            var existingRoom = new Habitacion
            {
                Id = 55,
                Numero = "2",
                Detalle = "Valid detail",
                IdEstadoHabitacion = 1,
                IdPiso = 1,
                IdCategoria = 1
            };
            await _context.Habitacion.AddAsync(existingRoom);
            await _context.SaveChangesAsync();

            var invalidDto = new UpdateHabitacionDto
            {
                Id = 55,
                Numero = "2",
                Detalle = new string('A', 101),
                IdEstadoHabitacion = 1,  
                IdPiso = 1,              
                IdCategoria = 1           
            };

            // Act
            var result = await _service.Update(invalidDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                _messageMapper.ErrorMessages["Room"]["DetailTooLong"],
                result.Message
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
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
            var activeRoom = new Habitacion
            {
                Id = 7,
                Deleted = false,
                Numero = "109"
            };
            await _context.Habitacion.AddAsync(activeRoom);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.Restore(7);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Reservation"]["AlreadyActive"], result.Message);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenDeleted()
        {
            // Arrange
            var deletedRoom = new Habitacion
            {
                Id = 6,
                Deleted = true,
                Numero = "103"
            };
            await _context.Habitacion.AddAsync(deletedRoom);
            await _context.SaveChangesAsync();

            var dto = new RemoveHabitacionDto { Id = 6 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["DeleteFailed"], result.Message);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenRoomReserved()
        {
            // Arrange
            var reservedRoom = new Habitacion
            {
                Id = 9,
                IdEstadoHabitacion = 3,
                Numero = "308",
                CreationUser = 1
            };
            await _context.Habitacion.AddAsync(reservedRoom);
            await _context.SaveChangesAsync();

            var dto = new RemoveHabitacionDto { Id = 9 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["DeleteInProgress"], result.Message);
        }


        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenRoomOccupied()
        {
            // Arrange
            var occupiedRoom = new Habitacion
            {
                Id = 8,
                IdEstadoHabitacion = 2,
                Numero = "307",
                CreationUser = 1
            };
            await _context.Habitacion.AddAsync(occupiedRoom);
            await _context.SaveChangesAsync();

            var dto = new RemoveHabitacionDto { Id = 3 };

            // Act
            var result = await _service.Remove(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["DeleteFailed"], result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ObtenerHabitacionesPorEstadoId_ShouldReturnFailure_WhenInvalidStatusId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerHabitacionesPorEstadoId(invalidId);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ObtenerHabitacionesPorNumero_ShouldReturnFailure_WhenMissingNumber(string invalidNumber)
        {
            // Act
            var result = await _service.ObtenerHabitacionesPorNumero(invalidNumber);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerHabitacionesPorNumero_ShouldReturnFailure_WhenNumberTooLong()
        {
            // Arrange
            var longNumber = new string('A', 51);

            // Act
            var result = await _service.ObtenerHabitacionesPorNumero(longNumber);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ObtenerHabitacionesPorPisoId_ShouldReturnFailure_WhenInvalidFloorId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerHabitacionesPorPisoId(invalidId);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ObtenerHabitacionesPorCategoriaId_ShouldReturnFailure_WhenInvalidCategoryId(int invalidId)
        {
            // Act
            var result = await _service.ObtenerHabitacionesPorCategoriaId(invalidId);

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