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
            var rolUsuarios = await _rolUsuarioRepository.ObtenerTodosLosRolesAsync();
            return Ok(rolUsuarios); 
        }

        // GET api/RolUsuario/GetRolByID/5
        [HttpGet("GetRolByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var rol = await _rolUsuarioRepository.GetEntityByIdAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return Ok(rol); 
        }

        // POST api/RolUsuario/SaveRol
        [HttpPost("SaveRol")]
        public async Task<IActionResult> Post([FromBody] RolUsuario rol)
            {
                 try
                 {
                 var savedRol = await _rolUsuarioRepository.SaveEntityAsync(rol);
              if (savedRol.Success == true)
             {
                    var savedRolData = savedRol.Data as RolUsuario;
                     return Ok("Rol Guardado Correctamente"); 
                }
                return BadRequest(savedRol.Message);
                 }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Ocurrió un error guardando los datos");
               return BadRequest($"Ocurrió un error guardando los datos: {ex.Message}");
              }
         }

        // PUT api/RolUsuario/UpdateRol
        [HttpPut("UpdateRol")]
        public async Task<IActionResult> Put([FromBody] RolUsuario rol)
        {
            var updatedRol = await _rolUsuarioRepository.UpdateEntityAsync(rol);
            if (updatedRol.Success == true)
            {
                return Ok("Rol de usuario actualizado correctamente." ); 
            }
            return BadRequest(updatedRol.Message);
        }

        // DELETE api/RolUsuario/DeleteRol/5
        [HttpDelete("DeleteRol/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedRol = await _rolUsuarioRepository.EliminarRolAsync(id);
            if (deletedRol.Success == true)
            {
                return Ok(deletedRol.Message);
            }
            return BadRequest(deletedRol.Message);
        }
    }
}
