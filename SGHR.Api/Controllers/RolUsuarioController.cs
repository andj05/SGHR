using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolUsuarioController : ControllerBase
    {
        private readonly IRolUsuarioRepository _rolUsuarioRepository;
        private readonly ILogger<RolUsuarioController> _logger;

        public RolUsuarioController(IRolUsuarioRepository rolUsuarioRepository, ILogger<RolUsuarioController> logger)
        {
            _rolUsuarioRepository = rolUsuarioRepository;
            _logger = logger;
        }

        // GET: api/RolUsuario/GetRolUsuario
        [HttpGet("GetRolUsuario")]
        public async Task<IActionResult> Get()
        {
            var rolUsuarios = await _rolUsuarioRepository.GetAllAsync();
            return Ok(rolUsuarios.Where(r => !r.Deleted));
        }

        // GET api/RolUsuario/GetRolByID/5
        [HttpGet("GetRolByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var rol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (rol == null || rol.Data is RolUsuario r && r.Deleted)
            {
                return NotFound("Rol de usuario no existe o ha sido eliminado.");
            }
            return Ok(rol);
        }

        // POST api/RolUsuario/SaveRol
        [HttpPost("SaveRol")]
        public async Task<IActionResult> Post([FromBody] RolUsuario rol)
        {
            try
            {
                var saveRol = await _rolUsuarioRepository.SaveEntityAsync(rol);
                if (saveRol.Success == true)
                {
                    return Ok(new { Message = "Rol guardado exitosamente", Data = saveRol.Data });
                }
                return BadRequest(new { Message = "Error al guardar el rol", Error = saveRol.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error guardando los datos");
                return StatusCode(500, new { Message = "Error interno al guardar el rol.", Error = ex.Message });
            }
        }

        // PUT api/RolUsuario/UpdateRol/5
        [HttpPut("UpdateRol/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolUsuario rol)
        {
            if (id <= 0)
                return BadRequest("ID de rol de usuario inválido.");

            var existingRol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (existingRol.Data is not RolUsuario rolData || rolData.Deleted)
            {
                return NotFound("Rol de usuario no encontrado o ha sido eliminado.");
            }

            rol.Id = id; // Asegurar que el ID es correcto
            var updateRol = await _rolUsuarioRepository.UpdateEntityAsync(rol);
            if (updateRol.Success == true)
            {
                return Ok(new { Message = "Rol de usuario actualizado exitosamente", Data = updateRol.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el rol de usuario", Error = updateRol.Message ?? "Error desconocido" });
        }

        // DELETE api/RolUsuario/DeleteRol/5
        [HttpDelete("DeleteRol/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de rol de usuario inválido.");

            try
            {
                var rol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rol.Data is not RolUsuario rolData)
                {
                    return NotFound("Rol de usuario no encontrado.");
                }

                rolData.Deleted = true;
                rolData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                rolData.ModifyDate = DateTime.Now;

                var deleteRol = await _rolUsuarioRepository.UpdateEntityAsync(rolData);
                if (deleteRol.Success == true)
                {
                    return Ok(new { Message = "Rol de usuario eliminado lógicamente.", Data = deleteRol.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el rol de usuario", Error = deleteRol.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol de usuario.");
                return StatusCode(500, new { Message = "Error interno al eliminar el rol de usuario.", Error = ex.Message });
            }
        }

        // PUT api/RolUsuario/RestoreRol/5
        [HttpPut("RestoreRol/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de rol de usuario inválido.");

            try
            {
                var rol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rol.Data is not RolUsuario rolData)
                    return NotFound("Rol de usuario no encontrado.");

                if (!rolData.Deleted)
                    return BadRequest("El rol de usuario ya está activo.");

                // Restaurar rol de usuario
                rolData.Deleted = false;
                rolData.ModifyDate = DateTime.Now;
                rolData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreRol = await _rolUsuarioRepository.UpdateEntityAsync(rolData);
                if (restoreRol.Success == true)
                {
                    return Ok(new { Message = "Rol de usuario restaurado exitosamente.", Data = restoreRol.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el rol de usuario", Error = restoreRol.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el rol de usuario.");
                return StatusCode(500, new { Message = "Error interno al restaurar el rol de usuario.", Error = ex.Message });
            }
        }

        // DELETE api/RolUsuario/DeleteRolPermanente/5
        [HttpDelete("DeleteRolPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de rol de usuario inválido.");

            try
            {
                var rol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
                if (rol.Data is not RolUsuario rolData)
                {
                    return NotFound("Rol de usuario no encontrado.");
                }

                var deleteResult = await _rolUsuarioRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Rol de usuario eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el rol de usuario permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol de usuario permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el rol de usuario permanentemente.", Error = ex.Message });
            }
        }
    }
}

