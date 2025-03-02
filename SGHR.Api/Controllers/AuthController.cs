using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SGHR.Domain.Entities.Users;
using SGHR.Persistence.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;

        public AuthController(IConfiguration config, IUsuarioRepository usuarioRepository, IClienteRepository clienteRepository)
        {
            _config = config;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await ValidateUser(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token, user });
        }

        private async Task<dynamic> ValidateUser(string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsersByFilterAsync(u => u.Correo == email && u.Clave == password);
            if (usuario.Success == true && usuario.Data is Usuario u && !u.Deleted)
            {
                return u;
            }

            var cliente = await _clienteRepository.GetFilteredAsync(c => c.Correo == email && c.Clave == password);
            if (cliente.Success == true && cliente.Data is Cliente c && !c.Deleted)
            {
                return c;
            }

            return null;
        }

        private string GenerateJwtToken(dynamic user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Correo),
                new Claim(ClaimTypes.Role, user is Usuario ? "Usuario" : "Cliente")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
