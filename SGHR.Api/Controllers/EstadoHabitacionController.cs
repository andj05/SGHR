using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacionController : ControllerBase
    {
        private readonly IEstadoHabitacionService _estadoHabitacionService;
        private readonly MessageMapper _messageMapper;
        private readonly ILogger<EstadoHabitacionController> _logger;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService,
                                            ILogger<EstadoHabitacionController> logger,
                                            MessageMapper messageMapper)
        {
            _estadoHabitacionService = estadoHabitacionService;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // GET: api/EstadoHabitacion/GetEstadoHabitacion
        [HttpGet("GetEstadoHabitacion")]
        public async Task<IActionResult> Get()
        {
            var result = await _estadoHabitacionService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var estadoHabitacion = (IEnumerable<EstadoHabitacionDto>)result.Data;
            return Ok(estadoHabitacion);
        }

        // GET api/EstadoHabitacion/GetEstadoByID/5
        [HttpGet("GetEstadoByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.Success != true)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var estadoHabitacion = (EstadoHabitacion)result.Data;
            if (estadoHabitacion.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(estadoHabitacion);
        }

        // GET api/EstadoHabitacion/GetDeletedEstadoHabitacion
        [HttpGet("GetDeletedEstadoHabitacion")]
        public async Task<IActionResult> GetDeletedEstadoHabitacion()
        {
            var result = await _estadoHabitacionService.GetAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var estadoHabitacion = result.Data as IEnumerable<EstadoHabitacionDto>;
            if (estadoHabitacion == null || !estadoHabitacion.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(estadoHabitacion);
        }

        // GET api/EstadoHabitacion/GetDeletedEstadoHabitacionByID/5
        [HttpGet("GetDeletedEstadoHabitacionByID/{id}")]
        public async Task<IActionResult> GetDeletedClienteByID(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var estadoHabitacion = (EstadoHabitacion)result.Data;
            if (!estadoHabitacion.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(estadoHabitacion);
        }

        // POST api/EstadoHabitacion/SaveEstadoHabitacion
        [HttpPost("SaveEstadoHabitacion")]
        public async Task<IActionResult> Post([FromBody] SaveEstadoHabitacionDto estadoHabitacionDto)
        {
            try
            {
                var saveResult = await _estadoHabitacionService.Save(estadoHabitacionDto);
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

        // PUT api/EstadoHabitacion/UpdateEstadoHabitacion/5
        [HttpPut("UpdateEstadoHabitacion/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateEstadoHabitacionDto estadoHabitacionDto)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _estadoHabitacionService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var existingEstadoHabitacion = (EstadoHabitacion)existingResult.Data;
            if (existingEstadoHabitacion.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            estadoHabitacionDto.IdEstadoHabitacion = id;
            var updateResult = await _estadoHabitacionService.Update(estadoHabitacionDto);
            if (updateResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
        }

        // DELETE api/EstadoHabitacion/DeleteEstado/5
        [HttpDelete("DeleteEstado/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _estadoHabitacionService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveEstadoHabitacionDto { IdEstadoHabitacion = id };
            var deleteResult = await _estadoHabitacionService.Remove(removeDto);
            if (deleteResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        // PUT api/EstadoHabitacion/RestoreEstado/5
        [HttpPut("RestoreEstado/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _estadoHabitacionService.Restore(id);
            if (restoreResult.Success == true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(restoreResult.Message);
        }
    }
}