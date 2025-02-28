using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioController> _logger;
        public UsuarioController(IUsuarioRepository usuarioRepository,
                                 ILogger<UsuarioController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // GET: api/Usuario/GetUsuarios
        [HttpGet("GetUsuarios")]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return Ok(usuarios.Where(c => !c.Deleted));
        }

        // GET api/Usuario/GetDeletedUsuarios
        [HttpGet("GetDeletedUsuarios")]
        public async Task<IActionResult> GetDeletedUsuarios()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return Ok(usuarios.Where(c => c.Deleted));
        }


        // GET api/Usuario/GetUsuarioBayID/5
        [HttpGet("GetUsuarioBayID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null || usuario.Data is Usuario u && u.Deleted)
            {
                return NotFound("Usuario no existe o ha sido eliminado.");
            }
            return Ok(usuario);
        }

        // GET api/Usuario/GetDeletedUsuarioByID/5
        [HttpGet("GetDeletedUsuarioByID/{id}")]
        public async Task<IActionResult> GetDeletedUsuarioByID(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario.Data is not Usuario u || !u.Deleted)
            {
                return NotFound("Usuario no encontrado o no está eliminado.");
            }
            return Ok(usuario);
        }

        // POST api/Usuario/SaveUsuario
        [HttpPost("SaveUsuario")]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                var saveUsuario = await _usuarioRepository.SaveEntityAsync(usuario);
                if (saveUsuario.Success == true)
                {
                    return Ok(new { Message = "Usuario guardado exitosamente", Data = saveUsuario.Data });
                }
                return BadRequest(new { Message = "Error al guardar el usuario", Error = saveUsuario.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el usuario.");
                return StatusCode(500, new { Message = "Error interno al guardar el usuario.", Error = ex.Message });
            }
        }

        // PUT api/Usuario/UpdateUsuario/5
        [HttpPut("UpdateUsuario{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            var existingUsuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (existingUsuario.Data is not Usuario usuarioData || usuarioData.Deleted)
            {
                return NotFound("Usuario no encontrado o ha sido eliminado.");
            }

            usuario.Id = id; // Asegurar que el ID es correcto
            var updateUsuario = await _usuarioRepository.UpdateEntityAsync(usuario);
            if (updateUsuario.Success == true)
            {
                return Ok(new { Message = "Usuario actualizado exitosamente", Data = updateUsuario.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el usuario", Error = updateUsuario.Message ?? "Error desconocido" });
        }


        // DELETE api/User/DeleteUsuario/5
        [HttpDelete("DeleteUsuario{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (usuario.Data is not Usuario usuarioData)
                {
                    return NotFound("Usuario no encontrado.");
                }

                usuarioData.Deleted = true;
                usuarioData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                usuarioData.ModifyDate = DateTime.Now;

                var deleteUsuario = await _usuarioRepository.UpdateEntityAsync(usuarioData);
                if (deleteUsuario.Success == true)
                {
                    return Ok(new { Message = "Usuario eliminado lógicamente.", Data = deleteUsuario.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el usuario", Error = deleteUsuario.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario.");
                return StatusCode(500, new { Message = "Error interno al eliminar el usuario.", Error = ex.Message });
            }
        }

        // RESTORE api/User/RestoreUsuario/5
        [HttpPut("RestoreUsuario/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (usuario.Data is not Usuario usuarioData)
                    return NotFound("Usuario no encontrado.");

                if (!usuarioData.Deleted)
                    return BadRequest("El usuario ya está activo.");

                // Restaurar usuario
                usuarioData.Deleted = false;
                usuarioData.ModifyDate = DateTime.Now;
                usuarioData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreUsuario = await _usuarioRepository.UpdateEntityAsync(usuarioData);
                if (restoreUsuario.Success == true)
                {
                    return Ok(new { Message = "Usuario restaurado exitosamente.", Data = restoreUsuario.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el usuario", Error = restoreUsuario.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el usuario.");
                return StatusCode(500, new { Message = "Error interno al restaurar el usuario.", Error = ex.Message });
            }
        }

        // DELETE api/Cliente/DeleteUsuarioPermanente/5
        [HttpDelete("DeleteUsuarioPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de usuario inválido.");

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (usuario.Data is not Usuario usuarioData)
                {
                    return NotFound("Usuario no encontrado.");
                }

                var deleteResult = await _usuarioRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Usuario eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el cliente permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el usuario permanentemente.", Error = ex.Message });
            }
        }
    }
}
