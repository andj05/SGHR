using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Configurations;
using SGHR.Persistence.Interfaces;
using System.Linq;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ControllerBase
    {
        private readonly IPisosService _pisosService;
        private readonly ILogger<PisoController> _logger;
        private readonly MessageMapper _messageMapper;

        public PisoController(IPisosService pisosService, ILogger<PisoController> logger, MessageMapper messageMapper)
        {
            _pisosService = pisosService;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // GET: api/Piso/GetPisos
        [HttpGet("GetPisos")]
        public async Task<IActionResult> Get()
        {
            var result = await _pisosService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var pisos = (IEnumerable<PisosDto>)result.Data;
            return Ok(pisos);
        }

        // GET api/Piso/GetPisoByID/5
        [HttpGet("GetPisoByID")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _pisosService.GetById(id);
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        // GET api/Piso/GetDeletedPiso
        [HttpGet("GetDeletedPiso")]
        public async Task<IActionResult> GetDeletedPisos()
        {
            var result = await _pisosService.GetAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var piso = result.Data as IEnumerable<PisosDto>;
            if (piso == null || !piso.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(piso);
        }

        // GET api/Piso/GetDeletedPisoByID/5
        [HttpGet("GetDeletedPisoByID/{id}")]
        public async Task<IActionResult> GetDeletedPisoByID(int id)
        {
            var result = await _pisosService.GetDeletedById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var pisos = (PisosDto)result.Data;
            return Ok(pisos);
        }

        // POST api/Piso/SavePiso
        [HttpPost("SavePiso")]
        public async Task<IActionResult> Post([FromBody] SavePisosDto pisosDto)
        {
            try
            {
                var saveResult = await _pisosService.Save(pisosDto);
                if (saveResult.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _messageMapper.ErrorMessages["Operations"]["SaveFailed"]);
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Piso/UpdatePiso/5
        [HttpPut("UpdatePiso/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdatePisosDto pisosDto)
        {
            if (id != pisosDto.IdPiso)
            {
                return BadRequest("El ID de la URL y el del pisosDto no coinciden.");
            }

            OperationResult result = await _pisosService.Update(pisosDto);
            if (result.Success == true)
            {
                return Ok("Piso actualizado.");
            }
            return BadRequest(result.Message);
        }

        // DELETE api/Piso/DeletePiso/5
        [HttpDelete("DeletePiso/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _pisosService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemovePisosDto { IdPiso = id };
            var deleteResult = await _pisosService.Remove(removeDto);
            if (deleteResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        // PUT api/Piso/RestorePiso/5
        [HttpPut("RestorePiso/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _pisosService.Restore(id);
            if (result.Success != true)
            {
                return BadRequest(result.Message);
            }

            return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
        }
    }
}