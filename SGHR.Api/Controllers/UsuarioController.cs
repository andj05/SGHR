using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;

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
            var usuarios = await _usuarioRepository.ObtenerTodosLosUsuariosAsync();
            return Ok(usuarios);
        }

        // GET api/Usuario/GetUsuarioBayID/5
        [HttpGet("GetUsuarioBayID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null)
            {
                return NotFound("Usuario No Existe");
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
                    return Ok("Usuario guardado exitosamente");
                }
                return BadRequest("Error al guardar el usuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el usuario.");
                return BadRequest("Error al guardar el usuario");
            }
        }

        // PUT api/Usuario/UpdateUsuario/5
        [HttpPut("UpdateUsuario{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            var updateUsuario = await _usuarioRepository.UpdateEntityAsync(usuario);
            if (updateUsuario.Success == true)
            {
                return Ok("Usuario actualizado exitosamente");
            }
            return BadRequest("Error al actualizar el cliente");
        }

        // DELETE api/User/DeleteUsuario/5
        [HttpDelete("DeleteUsuario{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteUsuario = await _usuarioRepository.DeleteEntityAsync(id);
            if (deleteUsuario.Success == true)
            {
                return Ok("Usuario eliminado exitosamente");
            }
            return BadRequest("Error al eliminar el cliente");
        }
    }
}