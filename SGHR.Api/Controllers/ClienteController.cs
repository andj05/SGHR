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
            var clientes = await _clienteRepository.GetAllAsync();
            return Ok(clientes.Where(c => !c.Deleted));
        }

        // GET api/Cliente/GetClienteByID/5
        [HttpGet("GetClienteByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente == null || cliente.Data is Cliente c && c.Deleted)
            {
                return NotFound("Cliente no existe o ha sido eliminado.");
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
                if (saveCliente.Success != true)
                {
                    return Ok(new { Message = "Cliente guardado exitosamente", Data = saveCliente.Data });
                }
                return BadRequest(new { Message = "Error al guardar el cliente", Error = saveCliente.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el cliente.");
                return StatusCode(500, new { Message = "Error interno al guardar el cliente.", Error = ex.Message });
            }
        }

        // PUT api/Cliente/UptadeCliente/5
        [HttpPut("UptadeCliente/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Cliente cliente)
        {
            if (id <= 0)
                return BadRequest("ID de cliente inválido.");

            var existingCliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (existingCliente.Data is not Cliente clienteData || clienteData.Deleted)
            {
                return NotFound("Cliente no encontrado o ha sido eliminado.");
            }

            cliente.Id = id;
            var updateCliente = await _clienteRepository.UpdateEntityAsync(cliente);
            if (updateCliente.Success != true)
            {
                return Ok(new { Message = "Cliente actualizado exitosamente", Data = updateCliente.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el cliente", Error = updateCliente.Message });
        }

        // DELETE api/Cliente/DeleteCliente/5
        [HttpDelete("DeleteCliente/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de cliente inválido.");

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente.Data is not Cliente clienteData)
                {
                    return NotFound("Cliente no encontrado.");
                }

                if (clienteData.Deleted)
                {
                    return BadRequest("El cliente ya está eliminado.");
                }

                clienteData.Deleted = true;
                clienteData.DeletedUser = 1;
                clienteData.ModifyDate = DateTime.Now;

                var deleteCliente = await _clienteRepository.UpdateEntityAsync(clienteData);
                if (deleteCliente.Success != true)
                {
                    return Ok(new { Message = "Cliente eliminado lógicamente.", Data = deleteCliente.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el cliente", Error = deleteCliente.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el cliente.", Error = ex.Message });
            }
        }

        // PUT api/Cliente/RestoreCliente/5
        [HttpPut("RestoreCliente/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de cliente inválido.");

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente.Data is not Cliente clienteData)
                    return NotFound("Cliente no encontrado.");

                if (!clienteData.Deleted)
                    return BadRequest("El cliente ya está activo.");

                // Restaurar cliente
                clienteData.Deleted = false;
                clienteData.ModifyDate = DateTime.Now;
                clienteData.ModifyUser = 1;

                var restoreCliente = await _clienteRepository.UpdateEntityAsync(clienteData);
                if (restoreCliente.Success != true)
                {
                    return Ok(new { Message = "Cliente restaurado exitosamente.", Data = restoreCliente.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el cliente", Error = restoreCliente.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el cliente.");
                return StatusCode(500, new { Message = "Error interno al restaurar el cliente.", Error = ex.Message });
            }
        }

        // DELETE api/Cliente/DeleteClientePermanente/5
        [HttpDelete("DeleteClientePermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de cliente inválido.");

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente.Data is not Cliente clienteData)
                {
                    return NotFound("Cliente no encontrado.");
                }

                if (!clienteData.Deleted)
                {
                    return BadRequest("Debe eliminar lógicamente el cliente antes de eliminarlo permanentemente.");
                }

                var deleteResult = await _clienteRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Cliente eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el cliente permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el cliente permanentemente.", Error = ex.Message });
            }
        }
    }
}
