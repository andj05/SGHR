using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Users;
using SGHR.Application.Intefaces;
using SGHR.Application.Dtos.Cliente;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClientesService _clienteService;
        private readonly MessageMapper _messageMapper;

        public ClienteController(IClientesService clientesService,
                                 ILogger<ClienteController> logger,
                                 MessageMapper messageMapper)
        {
            _clienteService = clientesService;
            _messageMapper = messageMapper;
        }

        // GET: api/Cliente/GetClientes
        [HttpGet("GetClientes")]
        public async Task<IActionResult> Get()
        {
            var result = await _clienteService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var clientes = (IEnumerable<Cliente>)result.Data;
            return Ok(clientes.Where(c => !c.Deleted));
        }

        // GET api/Cliente/GetClienteByID/5
        [HttpGet("GetClienteByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _clienteService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var cliente = (Cliente)result.Data;
            if (cliente.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(cliente);
        }

        // GET api/Cliente/GetDeletedClientes
        [HttpGet("GetDeletedClientes")]
        public async Task<IActionResult> GetDeletedClientes()
        {
            var result = await _clienteService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var clientes = (IEnumerable<Cliente>)result.Data;
            return Ok(clientes.Where(c => c.Deleted));
        }

        // GET api/Cliente/GetDeletedClienteByID/5
        [HttpGet("GetDeletedClienteByID/{id}")]
        public async Task<IActionResult> GetDeletedClienteByID(int id)
        {
            var result = await _clienteService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var cliente = (Cliente)result.Data;
            if (!cliente.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(cliente);
        }

        // POST api/Cliente/SaveCliente
        [HttpPost("SaveCliente")]
        public async Task<IActionResult> Post([FromBody] SaveClienteDto cliente)
        {
            try
            {
                var saveResult = await _clienteService.Save(cliente);
                if (saveResult.Success != true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Cliente/UpdateCliente/5
        [HttpPut("UpdateCliente/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateClienteDto cliente)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _clienteService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var existingCliente = (Cliente)existingResult.Data;
            if (existingCliente.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            cliente.IdCliente = id;
            var updateResult = await _clienteService.Update(cliente);
            if (updateResult.Success != true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
        }

        // PUT api/Cliente/RestoreCliente/5
        [HttpPut("RestoreCliente/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _clienteService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _clienteService.Restore(id);
            if (restoreResult.Success != true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(restoreResult.Message);
        }

        // DELETE api/Cliente/DeleteCliente/5
        [HttpDelete("DeleteCliente/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _clienteService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveClienteDto { IdCliente = id };
            var deleteResult = await _clienteService.Remove(removeDto);
            if (deleteResult.Success != true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }
    }
}
