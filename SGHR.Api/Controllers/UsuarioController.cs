using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SGHR.Domain.Entities.Users;
using SGHR.Application.Dtos.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SGHR.Persistence.Configurations;
using SGHR.Application.Intefaces; // Para MessageMapper
using SGHR.Application.Mappers;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuariosService _usuariosService;
        private readonly IConfiguration _configuration;
        private readonly MessageMapper _messageMapper;

        public UsuarioController(IUsuariosService usuarioService,
                                 ILogger<UsuarioController> logger,
                                 IConfiguration configuration,
                                 MessageMapper messageMapper)
        {
            _usuariosService = usuarioService;
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
            var result = await _usuariosService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var usuario = result.Data as IEnumerable<UsuarioDto>;
            if (usuario == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(usuario);
        }

        // GET api/Usuario/GetUsuarioByID/5
        [HttpGet("GetUsuarioByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _usuariosService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var usuario = (Usuario)result.Data;
            if (usuario.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(UsuarioMapper.ToDto(usuario));
        }

        // GET api/Usuario/GetDeletedUsuarios
        [HttpGet("GetDeletedUsuarios")]
        public async Task<IActionResult> GetDeletedUsuarios()
        {
            var result = await _usuariosService.GerAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var usuario = result.Data as IEnumerable<UsuarioDto>;
            if (usuario == null || !usuario.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(usuario);
        }

        // GET api/Usuario/GetDeletedUsuarioByID/5
        [HttpGet("GetDeletedUsuarioByID/{id}")]
        public async Task<IActionResult> GetDeletedUsuarioByID(int id)
        {
            var result = await _usuariosService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var usuario = (Usuario)result.Data;
            if (!usuario.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(UsuarioMapper.ToDto(usuario));
        }

        // POST: api/Usuarios/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _usuariosService.Login(request);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            var usuario = result.Data as UsuarioDto;
            if (usuario == null)
            {
                return NotFound(new { message = _messageMapper.ErrorMessages["EntityBase"]["NotFound"] });
            }

            var token = GenerateJwtToken(new Usuario
            {
                Id = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                IdRolUsuario = usuario.IdRolUsuario
            });

            return Ok(new
            {
                token,
                message = _messageMapper.SuccessMessages["LoginSuccess"],
                usuario
            });
        }


        // POST api/Usuario/SaveUsuario
        [HttpPost("SaveUsuario")]
        public async Task<IActionResult> Post([FromBody] SaveUsuarioDto usuarioDto)
        {
            try
            {
                var saveResult = await _usuariosService.Save(usuarioDto);
                if (saveResult.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Usuario/UpdateUsuario/5
        [HttpPut("UpdateUsuario/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUsuarioDto usuarioDto)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _usuariosService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var usuario = (Usuario)existingResult.Data;
            if (usuario.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            usuarioDto.IdUsuario = id;
            var updateResult = await _usuariosService.Update(usuarioDto);
            if (updateResult.Success)
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
        }

        // PUT api/Usuario/RestoreUsuario/5
        [HttpPut("RestoreUsuario/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _usuariosService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _usuariosService.Restore(id);
            if (restoreResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["RestoreSuccess"], Data = restoreResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"], Error = restoreResult.Message });
        }

        // DELETE api/Usuario/DeleteUsuario/5
        [HttpDelete("DeleteUsuario/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _usuariosService.GetById(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveUsuarioDto { IdUsuario = id };
            var deleteResult = await _usuariosService.Remove(removeDto);

            if (deleteResult.Success)
            {
                return Ok(new
                {
                    Message = _messageMapper.SuccessMessages["DeleteSuccess"],
                    Data = deleteResult.Data
                });
            }
            else
            {
                return BadRequest(new
                {
                    Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"],
                    Error = deleteResult.Message
                });
            }
        }
    }
}
