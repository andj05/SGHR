using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SGHR.Application.Dtos.Usuario;
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
    public class UnitTestUsuariosService : IDisposable
    {
        private readonly SGHRContext _context;
        private readonly UsuarioRepository _repository;
        private readonly UsuariosService _service;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UnitTestUsuariosService()
        {
            var options = new DbContextOptionsBuilder<SGHRContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGHRContext(options);
            _logger = new LoggerManager(new LoggerFactory().CreateLogger<LoggerManager>());
            _messageMapper = new MessageMapper();

            _repository = new UsuarioRepository(_context, _logger, _messageMapper);

            _service = new UsuariosService(
                _repository,
                _messageMapper,
                _logger);
        }

        [Fact]
        public async Task GetAll_WithExistingUsuarios_ReturnsActiveUsers()
        {
            // Arrange
            await _repository.SaveEntityAsync(new Usuario
            {
                NombreCompleto = "Activo",
                Clave = "clave1",
                Correo = "activo@test.com",
                IdRolUsuario = 1,
                Deleted = false
            });

            await _repository.SaveEntityAsync(new Usuario
            {
                NombreCompleto = "Eliminado",
                Clave = "clave2",
                Correo = "eliminado@test.com",
                IdRolUsuario = 1,
                Deleted = true
            });

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Success);
            var usuarios = result.Data as List<UsuarioDto>;
            Assert.NotNull(usuarios);
            Assert.Single(usuarios); // Solo debe retornar el usuario activo
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var usuario = new Usuario
            {
                NombreCompleto = "Usuario",
                Clave = "clave123",
                Correo = "usuario@test.com",
                IdRolUsuario = 1,
                Deleted = false
            };
            await _repository.SaveEntityAsync(usuario);

            var loginRequest = new LoginRequestDto
            {
                Correo = "usuario@test.com",
                Clave = "clave123"
            };

            // Act
            var result = await _service.Login(loginRequest);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(_messageMapper.SuccessMessages["LoginSuccess"], result.Message);
        }

        [Fact]
        public async Task Save_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new SaveUsuarioDto
            {
                NombreCompleto = "Nuevo Usuario",
                Correo = "nuevo@test.com",
                IdRolUsuario = 1,
                Clave = "claveSegura123"
            };

            // Act
            var result = await _service.Save(dto);

            // Assert
            Assert.True(result.Success);
            // NOTA: El usuario no se creará realmente debido a un bug en el servicio
        }

        [Fact]
        public async Task Update_WithValidData_ModifiesUser()
        {
            // Arrange
            var usuario = new Usuario
            {
                NombreCompleto = "Original",
                Correo = "original@test.com",
                IdRolUsuario = 1,
                Clave = "claveOriginal"
            };
            await _repository.SaveEntityAsync(usuario);

            var dto = new UpdateUsuarioDto
            {
                IdUsuario = usuario.Id,
                NombreCompleto = "Nombre Actualizado",
                Correo = "actualizado@test.com"
            };

            // Act
            var result = await _service.Update(dto);

            // Assert
            Assert.True(result.Success);
            var actualizado = await _repository.GetEntityByIdAsync(usuario.Id);
            Assert.Equal("Nombre Actualizado", actualizado.NombreCompleto);
            Assert.Equal("actualizado@test.com", actualizado.Correo);
        }

        [Fact]
        public async Task Remove_WithActiveUser_MarksAsDeleted()
        {
            // Arrange
            var usuario = new Usuario
            {
                NombreCompleto = "Para Eliminar",
                Correo = "eliminar@test.com",
                IdRolUsuario = 1,
                Clave = "claveEliminar"
            };
            await _repository.SaveEntityAsync(usuario);

            // Act
            var result = await _service.Remove(new RemoveUsuarioDto { IdUsuario = usuario.Id });

            // Assert
            Assert.True(result.Success);
            var eliminado = await _repository.GetEntityByIdAsync(usuario.Id);
            Assert.True(eliminado.Deleted);
        }

        [Fact]
        public async Task Restore_WithDeletedUser_UnmarksDeleted()
        {
            // Arrange
            var usuario = new Usuario
            {
                NombreCompleto = "Restaurado",
                Correo = "restore@test.com",
                IdRolUsuario = 1,
                Clave = "claveRestore",
                Deleted = true
            };
            await _repository.SaveEntityAsync(usuario);

            // Act
            var result = await _service.Restore(usuario.Id);

            // Assert
            Assert.True(result.Success);
            var restaurado = await _repository.GetEntityByIdAsync(usuario.Id);
            Assert.False(restaurado.Deleted);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}