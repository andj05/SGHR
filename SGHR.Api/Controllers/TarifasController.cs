using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifasController : ControllerBase
    {
        private readonly ITarifasService _tarifasService;
        private readonly ILogger<TarifasController> _logger;
        private readonly MessageMapper _messageMapper;

        public TarifasController(ITarifasService tarifasService, ILogger<TarifasController> logger, MessageMapper messageMapper)
        {
            _tarifasService = tarifasService;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // GET: api/Tarifas/GetTarifas
        [HttpGet("GetTarifas")]
        public async Task<IActionResult> Get()
        {
            var result = await _tarifasService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var tarifas = (IEnumerable<Tarifas>)result.Data;
            return Ok(tarifas.Where(t => !t.Deleted));
        }

        // GET api/Tarifas/GetTarifasByID/5
        [HttpGet("GetTarifasByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var tarifa = (Tarifas)result.Data;
            if (tarifa.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(tarifa);
        }

        // GET api/Tarifas/GetDeletedTarifas
        [HttpGet("GetDeletedTarifas")]
        public async Task<IActionResult> GetDeletedTarifas()
        {
            var result = await _tarifasService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var tarifas = (IEnumerable<Tarifas>)result.Data;
            return Ok(tarifas.Where(t => t.Deleted));
        }

        // GET api/Tarifas/GetDeletedTarifasByID/5
        [HttpGet("GetDeletedTarifasByID/{id}")]
        public async Task<IActionResult> GetDeletedTarifasByID(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var tarifa = (Tarifas)result.Data;
            if (!tarifa.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(tarifa);
        }

        // POST api/Tarifas/SaveTarifas
        [HttpPost("SaveTarifas")]
        public async Task<IActionResult> Post([FromBody] SaveTarifasDto tarifasDto)
        {
            try
            {
                var saveResult = await _tarifasService.Save(tarifasDto);
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

        // PUT api/Tarifas/UpdateTarifa/5
        [HttpPut("UpdateTarifa/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateTarifasDto tarifaDto)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _tarifasService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var existingTarifa = (Tarifas)existingResult.Data;
            if (existingTarifa.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            tarifaDto.IdTarifa = id;
            var updateResult = await _tarifasService.Update(tarifaDto);
            if (updateResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
        }

        // DELETE api/Tarifas/DeleteTarifa/5
        [HttpDelete("DeleteTarifa/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _tarifasService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveTarifasDto { IdTarifa = id };
            var deleteResult = await _tarifasService.Remove(removeDto);
            if (deleteResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        // PUT api/Tarifas/RestoreTarifa/5
        [HttpPut("RestoreTarifa/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _tarifasService.Restore(id);
            if (restoreResult.Success == true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(restoreResult.Message);
        }
    }
}