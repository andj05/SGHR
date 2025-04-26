using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Domain.Base;
using SGHR.Persistence.Configurations;
using Xunit;

namespace SGHR.Persistence.Test
{
    public class UnitTestRolUsuarioRepository : IDisposable
    {
        private readonly RolUsuarioRepository _rolUsuarioRepository;
        private readonly SGHRContext _context;
        private readonly ILogger<RolUsuarioRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UnitTestRolUsuarioRepository()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<RolUsuarioRepository>();

            _configuration = new ConfigurationBuilder().Build();
            _messageMapper = new MessageMapper();

            _rolUsuarioRepository = new RolUsuarioRepository(
                _context,
                _logger,
                _configuration,
                _messageMapper
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRoles()
        {
            // Arrange
            var rol1 = new RolUsuario
            {
                Descripcion = "Admin",
                Estado = true
            };
            var rol2 = new RolUsuario
            {
                Descripcion = "Usuario",
                Estado = true
            };

            await _rolUsuarioRepository.SaveEntityAsync(rol1);
            await _rolUsuarioRepository.SaveEntityAsync(rol2);

            // Act
            var result = await _rolUsuarioRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsRol_WhenIdIsValid()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Test Rol",
                Estado = true
            };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Act
            var result = await _rolUsuarioRepository.GetEntityByIdAsync(rol.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(rol.Descripcion, result.Descripcion);
        }

        [Fact]
        public async Task GetEntityByIdAsync_ReturnsNull_WhenIdIsInvalid()
        {
            // Act
            var result = await _rolUsuarioRepository.GetEntityByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEntityAsync_Success_WhenRolIsValid()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Nuevo Rol",
                Estado = true
            };

            // Act
            var result = await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["SaveSuccess"], result.Message);
        }

        [Fact]
        public async Task SaveEntityAsync_Fails_WhenRolIsNull()
        {
            // Act
            var result = await _rolUsuarioRepository.SaveEntityAsync(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(_messageMapper.ErrorMessages["Operations"]["SaveFailed"], result.Message);
        }

        [Fact]
        public async Task UpdateEntityAsync_UpdatesRol()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Original",
                Estado = true
            };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            rol.Descripcion = "Actualizado";

            // Act
            var result = await _rolUsuarioRepository.UpdateEntityAsync(rol);

            // Assert
            Assert.True(result.Success);
            var updated = await _rolUsuarioRepository.GetEntityByIdAsync(rol.Id);
            Assert.Equal("Actualizado", updated.Descripcion);
        }

        [Fact]
        public async Task DeleteEntityAsync_DeletesRol()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Rol a Eliminar",
                Estado = true
            };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Act
            var result = await _rolUsuarioRepository.DeleteEntityAsync(rol);

            // Assert
            Assert.True(result.Success);
            var deleted = await _rolUsuarioRepository.GetEntityByIdAsync(rol.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task RestoreEntityAsync_RestoresDeletedRol()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Rol Restaurado",
                Estado = true,
                Deleted = true // Marcar como eliminado
            };
            await _context.RolUsuario.AddAsync(rol);
            await _context.SaveChangesAsync();

            // Act
            var result = await _rolUsuarioRepository.RestoreEntityAsync(rol);

            // Assert
            Assert.True(result.Success);
            var restored = await _rolUsuarioRepository.GetEntityByIdAsync(rol.Id);
            Assert.False(restored.Deleted);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenRolExists()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Rol Existente",
                Estado = true
            };
            await _rolUsuarioRepository.SaveEntityAsync(rol);

            // Act
            var exists = await _rolUsuarioRepository.ExistsAsync(r => r.Descripcion == "Rol Existente");

            // Assert
            Assert.True(exists);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}