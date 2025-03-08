using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repositories;
using SGHR.Persistence.Configurations;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Infraestructure.Logging.Interfaces;
using Xunit;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SGHR.Persistence.Test
{
    public class UnitTestUsuarioRepository : IDisposable
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly SGHRContext _context;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestUsuarioRepository()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _messageMapper = new MessageMapper();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole().SetMinimumLevel(LogLevel.Debug);
            });
            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());

            _usuarioRepository = new UsuarioRepository(_context, _logger, _messageMapper);
        }

        [Fact]
        public async Task SaveEntityAsync_ShouldCreateUsuario_WithValidData()
        {
            // Arrange
            var usuario = new Usuario
            {
                NombreCompleto = "Test User",
                Correo = "test@example.com",
                Clave = "password",
                IdRolUsuario = 1,
                Estado = true
            };

            // Act
            var result = await _usuarioRepository.SaveEntityAsync(usuario);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(usuario.Correo, ((Usuario)result.Data).Correo);
            Assert.NotEqual(DateTime.MinValue, ((Usuario)result.Data).FechaCreacion);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUsuario_WhenEmailExists()
        {
            // Arrange
            var usuario = new Usuario
            {
                Correo = "findme@example.com",
                NombreCompleto = "Searchable User"
            };
            await _usuarioRepository.SaveEntityAsync(usuario);

            // Act
            var result = await _usuarioRepository.GetByEmailAsync("findme@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario.NombreCompleto, result.NombreCompleto);
        }

        [Fact]
        public async Task UpdateEntityAsync_ShouldChangeEmailSuccessfully()
        {
            // Arrange
            var originalUsuario = new Usuario
            {
                Correo = "original@email.com",
                NombreCompleto = "Original Name"
            };
            await _usuarioRepository.SaveEntityAsync(originalUsuario);

            originalUsuario.Correo = "updated@email.com";

            // Act
            var result = await _usuarioRepository.UpdateEntityAsync(originalUsuario);

            // Assert
            Assert.True(result.Success);
            var updated = await _usuarioRepository.GetEntityByIdAsync(originalUsuario.Id);
            Assert.Equal("updated@email.com", updated.Correo);
        }

        [Fact]
        public async Task GetUsersByStatusAsync_ShouldFilterActiveUsers()
        {
            // Arrange
            await CreateTestUsers();

            // Act
            var activeUsers = await _usuarioRepository.GetUsersByStatusAsync(1);

            // Assert
            Assert.True(activeUsers.Success);
            Assert.Equal(2, ((System.Collections.Generic.List<Usuario>)activeUsers.Data).Count);
        }

        [Fact]
        public async Task DeleteEntityAsync_ShouldRemoveFromDatabase()
        {
            // Arrange
            var usuario = new Usuario { Correo = "todelete@test.com" };
            await _usuarioRepository.SaveEntityAsync(usuario);

            // Act
            var deleteResult = await _usuarioRepository.DeleteEntityAsync(usuario);
            var deletedUser = await _usuarioRepository.GetEntityByIdAsync(usuario.Id);

            // Assert
            Assert.True(deleteResult.Success);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task RestoreEntityAsync_ShouldRecoverDeletedUser()
        {
            // Arrange
            var usuario = new Usuario
            {
                Correo = "restore@test.com",
                Deleted = true
            };
            await _usuarioRepository.SaveEntityAsync(usuario);

            // Act
            var restoreResult = await _usuarioRepository.RestoreEntityAsync(usuario);
            var restoredUser = await _usuarioRepository.GetEntityByIdAsync(usuario.Id);

            // Assert
            Assert.True(restoreResult.Success);
            Assert.False(restoredUser.Deleted);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldApplyCustomFilters()
        {
            // Arrange
            await CreateTestUsers();

            // Act
            var result = await _usuarioRepository.GetFilteredAsync(u => u.IdRolUsuario == 2);

            // Assert
            Assert.True(result.Success);
            Assert.Single((System.Collections.Generic.List<Usuario>)result.Data);
        }

        [Fact]
        public async Task ExistsAsync_ShouldDetectExistingUser()
        {
            // Arrange
            var usuario = new Usuario { Correo = "exists@test.com" };
            await _usuarioRepository.SaveEntityAsync(usuario);

            // Act
            var exists = await _usuarioRepository.ExistsAsync(u => u.Correo == "exists@test.com");

            // Assert
            Assert.True(exists);
        }

        private async Task CreateTestUsers()
        {
            var users = new[]
            {
                new Usuario { Estado = true, IdRolUsuario = 1 },
                new Usuario { Estado = true, IdRolUsuario = 2 },
                new Usuario { Estado = false, IdRolUsuario = 1 }
            };

            foreach (var user in users)
            {
                await _usuarioRepository.SaveEntityAsync(user);
            }
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}