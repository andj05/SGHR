using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly IServiciosService _serviciosService;
        private readonly MessageMapper _messageMapper;

        public ServiciosController(IServiciosService serviciosService,
                                   ILogger<ServiciosController> logger,
                                   MessageMapper messageMapper)
        {
            _serviciosService = serviciosService;
            _messageMapper = messageMapper;
        }

        // GET: api/Servicios/GetServicios
        [HttpGet("GetServicios")]
        public async Task<IActionResult> Get()
        {
            var result = await _serviciosService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var servicios = (IEnumerable<ServiciosDto>)result.Data;
            return Ok(servicios);
        }

        // GET: api/Servicios/GetServiciosByid?id=5
        [HttpGet("GetServiciosByID")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _serviciosService.GetById(id);
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        // GET api/Servicios/GetDeletedServicios
        [HttpGet("GetDeletedServicios")]
        public async Task<IActionResult> GetDeletedServicios()
        {
            var result = await _serviciosService.GetAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var servicio = result.Data as IEnumerable<ServiciosDto>;
            if (servicio == null || !servicio.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(servicio);
        }

        // GET api/Servicios/GetDeletedServisciosByID/5
        [HttpGet("GetDeletedServisciosByID/{id}")]
        public async Task<IActionResult> GetDeletedServiciosByID(int id)
        {
            var result = await _serviciosService.GetDeletedById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var servicios = (ServiciosDto)result.Data;
            return Ok(servicios);
        }

        // POST api/Servicios/SaveServicio
        [HttpPost("SaveServicio")]
        public async Task<IActionResult> Post([FromBody] SaveServiciosDto serviciosDto)
        {
            try
            {
                var saveResult = await _serviciosService.Save(serviciosDto);
                if (saveResult.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/Servicios/UpdateServicio/5
        [HttpPut("UpdateServicio/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateServiciosDto serviciosDto)
        {
            if (id != serviciosDto.IdServicio)
            {
                return BadRequest("El ID de la URL y el del serviciosDto no coinciden.");
            }

            OperationResult result = await _serviciosService.Update(serviciosDto);
            if (result.Success == true)
            {
                return Ok("Servicio actualizado.");
            }
            return BadRequest(result.Message);
        }

        // DELETE api/Servicios/DeleteServicio/5
        [HttpDelete("DeleteServicio/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _serviciosService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var servicioDto = new RemoveServiciosDto { IdServicio = id };
            var deleteResult = await _serviciosService.Remove(servicioDto);
            if (deleteResult.Success == true)
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });

            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        // PUT api/Servicios/RestoreServicio/5
        [HttpPut("RestoreServicio/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _serviciosService.Restore(id);
            if (result.Success != true)
            {
                return BadRequest(result.Message);
            }

            return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
        }
    }
}