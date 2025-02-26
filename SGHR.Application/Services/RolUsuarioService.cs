using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;


namespace SGHR.Application.Services
{
    public class RolUsuarioService : IRolUsuarioService
    {
        private readonly IRolUsuarioRepository _rolUsuarioRepository;
        private readonly ILogger<RolUsuarioService> _logger;
        private readonly IConfiguration _configuration;

        public RolUsuarioService(IRolUsuarioRepository rolUsuarioRepository,
                                ILogger<RolUsuarioService> logger,
                                IConfiguration configuration)
        {
            _rolUsuarioRepository = rolUsuarioRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var roles = await _rolUsuarioRepository.ObtenerTodosLosRolesAsync();
                operationResult.Data = roles;
                operationResult.Success = true;
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener todos los roles de usuario";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rolUsuario == null)
                {
                    operationResult.Message = "Rol de usuario no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = rolUsuario;
                    operationResult.Success = true;
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener el rol de usuario por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SaveRolUsuarioDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = new RolUsuario
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    FechaCreacion = dto.FechaCreacion
                };
                operationResult = await _rolUsuarioRepository.SaveEntityAsync(rolUsuario);
            }
            catch (Exception)
            {
                operationResult.Message = "Error al guardar el rol de usuario";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdateRolUsuarioDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
                if (rolUsuario == null)
                {
                    operationResult.Message = "Rol de usuario no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    rolUsuario.Descripcion = dto.Descripcion;
                    rolUsuario.Estado = dto.Estado;
                    rolUsuario.FechaCreacion = dto.FechaCreacion;
                    operationResult = await _rolUsuarioRepository.UpdateEntityAsync(rolUsuario);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al actualizar el rol de usuario";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemoveRolUsuarioDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var rolUsuario = await _rolUsuarioRepository.GetEntityByIdAsync(dto.IdRolUsuario);
                if (rolUsuario == null)
                {
                    operationResult.Message = "Rol de usuario no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _rolUsuarioRepository.DeleteEntityAsync(rolUsuario);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al eliminar el rol de usuario";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<IEnumerable<RolUsuario>> ObtenerTodosLosRolesAsync()
        {
            return await _rolUsuarioRepository.ObtenerTodosLosRolesAsync();
        }

        public async Task<bool> ExisteRolUsuarioAsync(int idRolUsuario)
        {
            return await _rolUsuarioRepository.ExisteRolUsuarioAsync(idRolUsuario);
        }

    }
}

