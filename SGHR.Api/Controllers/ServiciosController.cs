using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Servicios;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Configuration;
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

            var servicios = (IEnumerable<Servicios>)result.Data;
            return Ok(servicios.Where(s => !s.Deleted));
        }

        // GET api/Servicios/GetServiciosByID/5
        [HttpGet("GetServiciosByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _serviciosService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var servicio = (Servicios)result.Data;
            if (servicio.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(servicio);
        }

        // GET api/Servicios/GetDeletedServicios
        [HttpGet("GetDeletedServicios")]
        public async Task<IActionResult> GetDeletedServicios()
        {
            var result = await _serviciosService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var servicios = (IEnumerable<Servicios>)result.Data;
            return Ok(servicios.Where(s => s.Deleted));
        }

        // GET api/Servicios/GetDeletedServisciosByID/5
        [HttpGet("GetDeletedServisciosByID/{id}")]
        public async Task<IActionResult> GetDeletedServicioByID(int id)
        {
            var result = await _serviciosService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var servicio = (Servicios)result.Data;
            if (!servicio.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(servicio);
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
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _serviciosService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var existingServicio = (Servicios)existingResult.Data;
            if (existingServicio.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            serviciosDto.IdServicio = id;
            var updateResult = await _serviciosService.Update(serviciosDto);
            if (updateResult.Success == true)
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });

            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
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
            var result = await _serviciosService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var restoreResult = await _serviciosService.Restore(id);
            if (restoreResult.Success == true)
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);

            return BadRequest(restoreResult.Message);
        }
    }
}