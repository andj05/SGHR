using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestServiciosService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly ServiciosRepository _repository;
        private readonly ServiciosService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestServiciosService()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            // Configurar dependencias reales
            var loggerFactory = new LoggerFactory();
            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            // Configurar IConfiguration
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            // Inicializar repositorio
            _repository = new ServiciosRepository(
                _context,
                loggerFactory.CreateLogger<ServiciosRepository>(),
                config,
                _messageMapper);

            // Configurar servicio
            // Configurar servicio solo con 3 parámetros
            _service = new ServiciosService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingServicios_ReturnsAllServicios()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Servicios
            {
                Nombre = "Servicio Prueba Uno",
                Descripcion = "Esta es una descripción detallada para el servicio de prueba uno.",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Servicios
            {
                Nombre = "Servicio Prueba Dos",
                Descripcion = "Esta es una descripción detallada para el servicio de prueba dos.",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Servicios
            {
                Nombre = "Servicio Eliminado Prueba",
                Descripcion = "Esta es una descripción detallada para el servicio eliminado.",
                Estado = true,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var servicios = result.Data as List<ServiciosDto>;
            Assert.NotNull(servicios);
            Assert.Equal(2, servicios.Count);
            Assert.DoesNotContain(servicios, s => s.Nombre == "Servicio Eliminado Prueba");
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Prueba Completo",
                Descripcion = "Esta es una descripción detallada para el servicio de prueba completo.",
                Estado = true
            };
            await _repository.SaveEntityAsync(servicio);

            // Act
            var result = await _service.GetById(servicio.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(servicio.Nombre, ((ServiciosDto)result.Data).Nombre);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewServicio()
        {
            // Arrange
            var dto = new SaveServiciosDto
            {
                Nombre = "Nuevo Servicio Prueba Completo",
                Descripcion = "Esta es una descripción detallada para el nuevo servicio de prueba completo.",
                Estado = true
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((Servicios)result.Data).Id);
            Assert.Equal(dto.Nombre, created.Nombre);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingServicio()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Original Completo",
                Descripcion = "Esta es una descripción detallada para el servicio original completo.",
                Estado = true
            };
            await _repository.SaveEntityAsync(servicio);

            var dto = new UpdateServiciosDto
            {
                IdServicio = servicio.Id,
                Nombre = "Servicio Actualizado Completo",
                Descripcion = "Esta es una descripción detallada para el servicio actualizado completo.",
                Estado = false
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(servicio.Id);
            Assert.Equal(dto.Nombre, updated.Nombre);
            Assert.Equal(dto.Estado, updated.Estado);
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Para Eliminar Completo",
                Descripcion = "Esta es una descripción detallada para el servicio que será eliminado."
            };
            await _repository.SaveEntityAsync(servicio);

            // Act
            var result = await _service.Remove(new RemoveServiciosDto { IdServicio = servicio.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(servicio.Id);
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedServicio_UnmarksDeleted()
        {
            // Arrange
            var servicio = new Servicios
            {
                Nombre = "Servicio Eliminado Completo",
                Descripcion = "Esta es una descripción detallada para el servicio eliminado que será restaurado.",
                Deleted = true
            };
            await _repository.SaveEntityAsync(servicio);

            // Act
            var result = await _service.Restore(servicio.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(servicio.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}