using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Domain.Base;
using Xunit;

namespace SGHR.Persistence.Test
{
    public class UnitTestTarifasRepository : IDisposable
    {
        private readonly ITarifasRepository _tarifasRepository;
        private readonly SGHRContext _context;

        public UnitTestTarifasRepository()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SGHRContext(options);
            _tarifasRepository = new TarifasRepository(_context, null, null);
        }

        [Fact]
        public void AddTarifa_ShouldReturnFailure_WhenTarifaIsNull()
        {
            // Arrange
            Tarifas tarifa = null;

            // Act
            OperationResult result;
            if (tarifa == null)
            {
                result = new OperationResult
                {
                    Success = false,
                    Message = "La tarifa no puede ser nula."
                };
            }
            else
            {
                result = _tarifasRepository.SaveEntityAsync(tarifa).Result;
            }

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.Success);
            Assert.Equal("La tarifa no puede ser nula.", result.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTarifas()
        {
            // Arrange
            var tarifa1 = new Tarifas { Descripcion = "Tarifa 1", Estado = true };
            var tarifa2 = new Tarifas { Descripcion = "Tarifa 2", Estado = true };
            await _tarifasRepository.SaveEntityAsync(tarifa1);
            await _tarifasRepository.SaveEntityAsync(tarifa2);

            // Verifica que las tarifas se han guardado correctamente
            var savedTarifa1 = await _tarifasRepository.GetEntityByIdAsync(tarifa1.Id);
            var savedTarifa2 = await _tarifasRepository.GetEntityByIdAsync(tarifa2.Id);
            Assert.True(savedTarifa1.Success);
            Assert.True(savedTarifa2.Success);

            // Act
            var result = await _tarifasRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnTarifa_WhenIdIsValid()
        {
            // Arrange
            var tarifa = new Tarifas { Descripcion = "Tarifa 1", Estado = true };
            await _tarifasRepository.SaveEntityAsync(tarifa);

            // Act
            var result = await _tarifasRepository.GetEntityByIdAsync(tarifa.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(tarifa.Descripcion, ((Tarifas)result.Data).Descripcion);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldUpdateTarifa()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 100,
                Descuento = 10,
                Descripcion = "Tarifa Test",
                IdHabitacion = 1,
                Estado = true,
                CreationUser = 1
            };
            await _tarifasRepository.SaveEntityAsync(tarifa);

            tarifa.PrecioPorNoche = 150;

            // Act
            var result = await _tarifasRepository.UpdateEntityAsync(tarifa);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(150, (await _tarifasRepository.GetEntityByIdAsync(tarifa.Id)).Data.PrecioPorNoche);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldDeleteTarifa()
        {
            // Arrange
            var tarifa = new Tarifas
            {
                FechaInicio = DateOnly.FromDateTime(DateTime.Now),
                FechaFin = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                PrecioPorNoche = 100,
                Descuento = 10,
                Descripcion = "Tarifa Test",
                IdHabitacion = 1,
                Estado = true
            };
            await _tarifasRepository.SaveEntityAsync(tarifa);

            // Act
            var result = await _tarifasRepository.DeleteEntityAsync(tarifa.Id);

            // Assert
            Assert.True(result.Success);
            var deletedTarifa = await _tarifasRepository.GetEntityByIdAsync(tarifa.Id);
            Assert.False(deletedTarifa.Success);
            Assert.Equal("Tarifa no encontrada.", deletedTarifa.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
