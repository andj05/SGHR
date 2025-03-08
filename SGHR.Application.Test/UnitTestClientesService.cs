using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Users;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Configurations;
using Microsoft.Extensions.Logging;
using SGHR.Infraestructure.Logging.Interfaces;
using Xunit;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestClientesService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly ClienteRepository _repository;
        private readonly ClientesService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestClientesService()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            _logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            _repository = new ClienteRepository(_context, _logger, _messageMapper);

            _service = new ClientesService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingClientes_ReturnsAllClientes()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Cliente
            {
                NombreCompleto = "Cliente 1",
                Documento = "12345678",
                Clave = "clave123"
            });

            await _repository.SaveEntityAsync(new Cliente
            {
                NombreCompleto = "Cliente 2",
                Documento = "87654321",
                Clave = "clave456"
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var clientes = result.Data as List<Cliente>;
            Assert.Equal(2, clientes?.Count);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                NombreCompleto = "Test Cliente",
                Documento = "11223344",
                Clave = "clave789"
            };
            await _repository.SaveEntityAsync(cliente);

            // Act
            var result = await _service.GetById(cliente.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(cliente.NombreCompleto, ((Cliente)result.Data).NombreCompleto);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewCliente()
        {
            // Arrange
            var dto = new SaveClienteDto
            {
                TipoDocumento = "DNI",
                Documento = "12345678",
                NombreCompleto = "Nuevo Cliente",
                Correo = "test@example.com",
                Clave = "claveSegura",
                Telefono = "123456789",
                Nacionalidad = "Argentina"
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((Cliente)result.Data).Id);
            Assert.NotNull(created);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                NombreCompleto = "Cliente Original",
                Documento = "11223344",
                Clave = "claveOriginal"
            };
            await _repository.SaveEntityAsync(cliente);

            var dto = new UpdateClienteDto
            {
                IdCliente = cliente.Id,
                NombreCompleto = "Cliente Actualizado",
                Telefono = "987654321",
                Nacionalidad = "Chile"
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(cliente.Id);
            Assert.Equal("Cliente Actualizado", updated.NombreCompleto);
            Assert.Equal("987654321", updated.Telefono);
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var cliente = new Cliente
            {
                NombreCompleto = "Cliente a Eliminar",
                Documento = "55667788",
                Clave = "claveEliminar"
            };
            await _repository.SaveEntityAsync(cliente);

            // Act
            var result = await _service.Remove(new RemoveClienteDto { IdCliente = cliente.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(cliente.Id);
            Assert.True(deleted.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedCliente_UnmarksDeleted()
        {
            // Arrange
            var cliente = new Cliente
            {
                NombreCompleto = "Cliente Eliminado",
                Documento = "99887766",
                Clave = "claveRestore",
                Deleted = true
            };
            await _repository.SaveEntityAsync(cliente);

            // Act
            var result = await _service.Restore(cliente.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(cliente.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}