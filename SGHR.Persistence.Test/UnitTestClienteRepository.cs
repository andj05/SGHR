using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Configurations;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Persistence.Test
{
    public class UnitTestClienteRepository : IDisposable
    {
        private readonly ClienteRepository _clienteRepository;
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestClienteRepository()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            // Inicializar MessageMapper real
            _messageMapper = new MessageMapper();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
            });

            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());

            // Inyectar dependencias reales
            _clienteRepository = new ClienteRepository(_context, _logger, _messageMapper);
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldReturnFailure_WhenClienteIsNull()
        {
            // Act
            var result = await _clienteRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllClientes()
        {
            // Arrange
            var cliente1 = new Cliente
            {
                Documento = "12345678",
                NombreCompleto = "Juan Pérez",
                Estado = true
            };

            var cliente2 = new Cliente
            {
                Documento = "87654321",
                NombreCompleto = "María García",
                Estado = false
            };

            await _clienteRepository.SaveEntityAsync(cliente1);
            await _clienteRepository.SaveEntityAsync(cliente2);

            // Act
            var result = await _clienteRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ShouldReturnNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _clienteRepository.GetEntityByIdAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldUpdateCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                Documento = "11111111",
                NombreCompleto = "Original Name",
                Estado = true
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            cliente.NombreCompleto = "Updated Name";
            cliente.Documento = "22222222";

            // Act
            var result = await _clienteRepository.UpdateEntityAsync(cliente);

            // Assert
            Assert.True(result.Success);
            var updated = await _clienteRepository.GetEntityByIdAsync(cliente.Id);
            Assert.Equal("Updated Name", updated.NombreCompleto);
            Assert.Equal("22222222", updated.Documento);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldRemoveCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                Documento = "33333333",
                NombreCompleto = "Cliente a Eliminar",
                Estado = true
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            // Act
            var result = await _clienteRepository.DeleteEntityAsync(cliente);

            // Assert
            Assert.True(result.Success);
            var deleted = await _clienteRepository.GetEntityByIdAsync(cliente.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetClientsByStatusAsync_ShouldReturnError_WhenInvalidStatus()
        {
            // Act
            var result = await _clienteRepository.GetClientsByStatusAsync(2);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Estado de cliente inválido", result.Message);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnFilteredClientes()
        {
            // Arrange
            var cliente = new Cliente
            {
                Documento = "44444444",
                NombreCompleto = "Cliente Filtrado",
                Estado = true
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            // Act
            var result = await _clienteRepository.GetFilteredAsync(c => c.Documento == "44444444");

            // Assert
            Assert.True(result.Success);
            var clientes = result.Data as System.Collections.Generic.List<Cliente>;
            Assert.Single(clientes);
            Assert.Equal("Cliente Filtrado", clientes.First().NombreCompleto);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenClienteExists()
        {
            // Arrange
            var cliente = new Cliente
            {
                Documento = "55555555",
                NombreCompleto = "Cliente Existente",
                Estado = true
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            // Act
            var exists = await _clienteRepository.ExistsAsync(c => c.Documento == "55555555");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task RestoreEntityAsync_ShouldReactivateCliente()
        {
            // Arrange
            var cliente = new Cliente
            {
                Documento = "66666666",
                NombreCompleto = "Cliente Eliminado",
                Estado = true,
                Deleted = true
            };

            await _clienteRepository.SaveEntityAsync(cliente);

            // Act
            var result = await _clienteRepository.RestoreEntityAsync(cliente);

            // Assert
            Assert.True(result.Success);
            var restored = await _clienteRepository.GetEntityByIdAsync(cliente.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}