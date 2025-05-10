using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Users;
using SGHR.Application.Intefaces;
using SGHR.Application.Dtos.Cliente;
using SGHR.Persistence.Configurations;
using SGHR.Application.Mappers;

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

            var cliente = result.Data as IEnumerable<ClienteDto>;
            if (cliente == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(cliente);
        }

        // GET api/Cliente/GetClienteByID/5
        [HttpGet("GetClienteByID/{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var result = await _clienteService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var cliente = (Cliente)result.Data;
            if (cliente.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(ClienteMapper.ToDto(cliente));
        }

        // GET api/Cliente/GetDeletedClientes
        [HttpGet("GetDeletedClientes")]
        public async Task<IActionResult> GetDeletedClientes()
        {
            var result = await _clienteService.GerAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var clientes = result.Data as IEnumerable<ClienteDto>;
            if (clientes == null || !clientes.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(clientes);
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

            return Ok(ClienteMapper.ToDto(cliente));
        }

        // POST api/Cliente/SaveCliente
        [HttpPost("SaveCliente")]
        public async Task<IActionResult> Post([FromBody] SaveClienteDto clienteDto)
        {
            try
            {
                var saveCliente = await _clienteService.Save(clienteDto);
                if (saveCliente.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveCliente.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveCliente.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Cliente/UpdateCliente/5
        [HttpPut("UpdateCliente/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateClienteDto clienteDto)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _clienteService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var cliente = (Cliente)existingResult.Data;
            if (cliente.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            clienteDto.IdCliente = id;
            var updateResult = await _clienteService.Update(clienteDto);
            if (updateResult.Success)
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            
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
            if (restoreResult.Success)
            {
                return Ok(new { 
                    Message = _messageMapper.SuccessMessages["RestoreSuccess"], 
                    Data = restoreResult.Data 
                });
            }
            return BadRequest(new { 
                Message = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"], 
                Error = restoreResult.Message 
            });
        }

        [HttpDelete("DeleteCliente/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _clienteService.GetById(id);
            if (!result.Success || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveClienteDto { IdCliente = id };
            var deleteResult = await _clienteService.Remove(removeDto);

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
