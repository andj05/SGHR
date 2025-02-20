using Microsoft.AspNetCore.Mvc;
using SGHR.Persistence.Interfaces;
using SGHR.Domain.Entities.Users;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClienteController> _logger;
        public ClienteController(IClienteRepository clienteRepository, ILogger<ClienteController> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        // GET: api/Cliente/GetClientes
        [HttpGet("GetClientes")]
        public async Task<IActionResult> Get()
        {
            var clientes = await _clienteRepository.ObtenerTodosLosClientesAsync();
            return Ok(clientes);
        }

        // GET api/Cliente/GetClienteByID/5
        [HttpGet("GetClienteByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente No Existe");
            }
            return Ok(cliente);
        }

        // POST api/Cliente/Post
        [HttpPost("SaveCliente")]
        public async Task<IActionResult> Post([FromBody] Cliente cliente)
        {
            try
            {
                var saveCliente = await _clienteRepository.SaveEntityAsync(cliente);
                if (saveCliente.Success == true)
                {
                    return Ok("Cliente guardado exitosamente");

                }
                return BadRequest("Error al guardar el cliente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el cliente.");
                return BadRequest("Error al guardar el cliente");
            }
        }

        // PUT api/Cliente/UptadeCliente/5
        [HttpPut("UptadeCliente/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Cliente cliente)
        {
            var updateCliente = await _clienteRepository.UpdateEntityAsync(cliente);
            if (updateCliente.Success == true)
            {
                return Ok("Cliente actualizado exitosamente");
            }
            return BadRequest("Error al actualizar el cliente");
        }

        // DELETE api/Cliente/DeleteCliente/5
        [HttpDelete("DeleteCliente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteCliente = await _clienteRepository.DeleteEntityAsync(id);
            if (deleteCliente.Success == true)
            {
                return Ok("Cliente eliminado exitosamente");
            }
            return BadRequest("Error al eliminar el cliente");
        }
    }
}
