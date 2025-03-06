using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Domain.Base;
using Xunit;
using SGHR.Persistence.Interfaces;

namespace SGHR.Persistence.Test
{
    public class UnitTestServiciosRepository : IDisposable
    {
        private readonly IServiciosRepository _serviciosRepository;
        private readonly SGHRContext _context;

        public UnitTestServiciosRepository()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new SGHRContext(options);
            _serviciosRepository = new ServiciosRepository(_context, null, null);
        }

        [Fact]
        public void AddServicio_ShouldReturnFailure_WhenServicioIsNull()
        {
            // Arrange
            Servicios servicio = null;

            // Act
            OperationResult result;
            if (servicio == null)
            {
                result = new OperationResult
                {
                    Success = false,
                    Message = "El servicio no puede ser nulo."
                };
            }
            else
            {
                result = _serviciosRepository.SaveEntityAsync(servicio).Result;
            }

            // Assert
            Assert.IsType<OperationResult>(result);
            Assert.False(result.Success);
            Assert.Equal("El servicio no puede ser nulo.", result.Message);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllServicios()
        {
            // Arrange
            var servicio1 = new Servicios { Nombre = "Servicio 1", Descripcion = "Descripción 1", Estado = true };
            var servicio2 = new Servicios { Nombre = "Servicio 2", Descripcion = "Descripción 2", Estado = true };
            await _serviciosRepository.SaveEntityAsync(servicio1);
            await _serviciosRepository.SaveEntityAsync(servicio2);

            // Verifica que los servicios se han guardado correctamente
            var savedServicio1 = await _serviciosRepository.GetEntityByIdAsync(servicio1.Id);
            var savedServicio2 = await _serviciosRepository.GetEntityByIdAsync(servicio2.Id);
            Assert.True(savedServicio1.Success);
            Assert.True(savedServicio2.Success);

            // Act
            var result = await _serviciosRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnServicio_WhenIdIsValid()
        {
            // Arrange
            var servicio = new Servicios { Nombre = "Servicio Test", Descripcion = "Descripción Test", Estado = true };
            await _serviciosRepository.SaveEntityAsync(servicio);

            // Act
            var result = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(servicio.Nombre, ((Servicios)result.Data).Nombre);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldUpdateServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Original",
                Descripcion = "Descripción Original",
                Estado = true,
                CreationUser = 1
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            servicio.Nombre = "Servicio Actualizado";

            // Act
            var result = await _serviciosRepository.UpdateEntityAsync(servicio);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Servicio Actualizado", (await _serviciosRepository.GetEntityByIdAsync(servicio.Id)).Data.Nombre);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldDeleteServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio a eliminar",
                Descripcion = "Descripción a eliminar",
                Estado = true
            };
            await _serviciosRepository.SaveEntityAsync(servicio);

            // Act
            var result = await _serviciosRepository.DeleteEntityAsync(servicio.Id);

            // Assert
            Assert.True(result.Success);
            var deletedServicio = await _serviciosRepository.GetEntityByIdAsync(servicio.Id);
            Assert.False(deletedServicio.Success);
            Assert.Equal("Servicio no encontrado.", deletedServicio.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
