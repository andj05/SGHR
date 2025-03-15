using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;
using SGHR.Infraestructure.Logging.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Repository;
using SGHR.Persistence.Configurations;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Infraestructure.Logging.Interfaces;

namespace SGHR.Application.Tests.Services
{
    public class UnitTestRolUsuarioService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly RolUsuarioRepository _repository;
        private readonly RolUsuarioService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestRolUsuarioService()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            var loggerFactory = new LoggerFactory();

            _logger = new LoggerManager(loggerFactory.CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            _repository = new RolUsuarioRepository(
                _context,
                loggerFactory.CreateLogger<RolUsuarioRepository>(),
                config,
                _messageMapper);

            _service = new RolUsuarioService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingRoles_ReturnsAllRoles()
        {
            // Arrange
            await _repository.SaveEntityAsync(new RolUsuario
            {
                Descripcion = "Admin",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new RolUsuario
            {
                Descripcion = "Usuario",
                Estado = true,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new RolUsuario
            {
                Descripcion = "Eliminado",
                Estado = true,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var roles = result.Data as List<RolUsuarioDto>;
            Assert.NotNull(roles);
            Assert.Equal(2, roles.Count);
            Assert.DoesNotContain(roles, r => r.Descripcion == "Eliminado");
        }



        [Fact]
        public async Task GetById_WithValidId_ReturnsRolUsuario()
        {
            // Arrange
            var rol = new RolUsuario { Descripcion = "Test Rol", Estado = true };
            await _repository.SaveEntityAsync(rol);

            // Act
            var result = await _service.GetById(rol.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(rol.Descripcion, ((RolUsuario)result.Data).Descripcion);
        }

        [Fact]
        public async Task Save_WithValidData_CreatesNewRol()
        {
            // Arrange
            var dto = new SaveRolUsuarioDto
            {
                Descripcion = "Nuevo Rol",
                Estado = true
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            var created = await _repository.GetEntityByIdAsync(((RolUsuario)result.Data).Id);
            Assert.Equal(dto.Descripcion, created.Descripcion);
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesExistingRol()
        {
            // Arrange
            var rol = new RolUsuario { Descripcion = "Original", Estado = true };
            await _repository.SaveEntityAsync(rol);

            var dto = new UpdateRolUsuarioDto
            {
                IdRolUsuario = rol.Id,
                Descripcion = "Rol Actualizado",
                Estado = false
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var updated = await _repository.GetEntityByIdAsync(rol.Id);
            Assert.Equal(dto.Descripcion, updated.Descripcion);
            Assert.Equal(dto.Estado, updated.Estado);
        }

        [Fact]
        public async Task Remove_WithExistingId_MarksAsDeleted()
        {
            // Arrange
            var rol = new RolUsuario { Descripcion = "To Delete" };
            await _repository.SaveEntityAsync(rol);

            // Act
            var result = await _service.Remove(new RemoveRolUsuarioDto { IdRolUsuario = rol.Id });

            // Assert
            Assert.True(result.Success);
            var deleted = await _repository.GetEntityByIdAsync(rol.Id);
            Assert.True(deleted.Deleted); 
        }

        [Fact]
        public async Task Restore_WithSoftDeletedRol_UnmarksDeleted()
        {
            // Arrange
            var rol = new RolUsuario
            {
                Descripcion = "Deleted Rol",
                Deleted = true // Soft-delete previo
            };
            await _repository.SaveEntityAsync(rol);

            // Act
            var result = await _service.Restore(rol.Id);

            // Assert
            Assert.True(result.Success);
            var restored = await _repository.GetEntityByIdAsync(rol.Id);
            Assert.False(restored.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}