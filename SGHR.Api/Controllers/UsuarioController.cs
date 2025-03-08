using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SGHR.Persistence.Configurations; // Para MessageMapper

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UsuarioController(IUsuarioRepository usuarioRepository,
                                 ILogger<UsuarioController> logger,
                                 IConfiguration configuration,
                                 MessageMapper messageMapper)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _messageMapper = messageMapper;
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            if (keyBytes.Length < 32)
            {
                throw new ArgumentException("La clave JWT debe tener al menos 32 bytes.");
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim("Rol", usuario.IdRolUsuario.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/Usuario/GetUsuarios
        [HttpGet("GetUsuarios")]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            // Se retornan solo los usuarios activos (no eliminados)
            return Ok(usuarios.Where(u => !u.Deleted));
        }

        // GET api/Usuario/GetUsuarioByID/5
        [HttpGet("GetUsuarioByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null || usuario.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }
            return Ok(usuario);
        }

        // GET api/Usuario/GetDeletedUsuarios
        [HttpGet("GetDeletedUsuarios")]
        public async Task<IActionResult> GetDeletedUsuarios()
        {
            var deletedUsers = await _usuarioRepository.GetFilteredAsync(u => u.Deleted);
            if (deletedUsers.Success == true)
            {
                return Ok(deletedUsers.Data);
            }
            return BadRequest(deletedUsers.Message);
        }

        // GET api/Usuario/GetDeletedUsuarioByID/5
        [HttpGet("GetDeletedUsuarioByID/{id}")]
        public async Task<IActionResult> GetDeletedUsuarioByID(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null || !usuario.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }
            return Ok(usuario);
        }

        // POST: api/Usuarios/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Correo);
            if (usuario == null)
            {
                return NotFound(new { message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] });
            }

            if (usuario.Deleted)
            {
                // Se podría agregar una clave específica, pero aquí se reutiliza "NotFound"
                return BadRequest(new { message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] });
            }

            if (usuario.Clave != request.Clave)
            {
                // Se usa el mensaje de credenciales inválidas definido en la categoría Auth
                return BadRequest(new { message = _messageMapper.ErrorMessages["Auth"]["InvalidCredentials"] });
            }

            var token = GenerateJwtToken(usuario);

            return Ok(new
            {
                token,
                message = _messageMapper.SuccessMessages["GenericSuccess"],
                usuario = new
                {
                    usuario.Id,
                    usuario.NombreCompleto,
                    usuario.Correo,
                    usuario.IdRolUsuario,
                    usuario.Deleted
                }
            });
        }

        // POST api/Usuario/SaveUsuario
        [HttpPost("SaveUsuario")]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            try
            {
                var saveResult = await _usuarioRepository.SaveEntityAsync(usuario);
                if (saveResult.Success != true)
                {
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });
                }
                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Usuario/UpdateUsuario/5
        [HttpPut("UpdateUsuario/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingUsuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (existingUsuario == null || existingUsuario.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            usuario.Id = id;
            var updateUsuario = await _usuarioRepository.UpdateEntityAsync(usuario);
            if (updateUsuario.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateUsuario.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateUsuario.Message ?? "Error desconocido" });
        }

        // PUT api/Usuario/RestoreUsuario/5
        [HttpPut("RestoreUsuario/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }
            var result = await _usuarioRepository.RestoreEntityAsync(usuario);
            if (result.Success != true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(result.Message);
        }

        // DELETE api/Usuario/DeleteUsuario/5
        [HttpDelete("DeleteUsuario/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
            if (usuario == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            usuario.Deleted = true;
            usuario.DeletedUser = 1;
            usuario.ModifyDate = DateTime.Now;

            var deleteResult = await _usuarioRepository.UpdateEntityAsync(usuario);
            if (deleteResult.Success != true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }
    }
}
