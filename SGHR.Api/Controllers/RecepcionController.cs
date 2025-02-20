using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;
using System.Threading.Tasks;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecepcionController : ControllerBase
    {
        private readonly IRecepcionRepository _recepcionRepository;
        private readonly ILogger<RecepcionController> _logger;

        public RecepcionController(IRecepcionRepository recepcionRepository,
                                   ILogger<RecepcionController> logger)
        {
            _recepcionRepository = recepcionRepository;
            _logger = logger;
        }

        // GET: api/<RecepcionController>
        [HttpGet("GetRecepciones")]
        public async Task<IActionResult> Get()
        {
            var recepciones = await _recepcionRepository.GetAllAsync();
            return Ok(recepciones);
        }

        // GET api/<RecepcionController>/5
        [HttpGet("GetRecepcionById/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var recepcion = await _recepcionRepository.GetEntityByIdAsync(id);
            if (recepcion == null)
            {
                return NotFound("Recepcion no encontrada.");
            }
            return Ok(recepcion);
        }
        
        // POST api/<RecepcionController>
        [HttpPost("GuardarRecepcion")]
        public async Task<IActionResult> Post([FromBody] Recepcion recepcion)
        {
            var result = await _recepcionRepository.SaveEntityAsync(recepcion);
            if (result.Success == true)
            {
                return Ok("Recepcion guardada.");
            }
            return BadRequest(result.Message);
        }

        // PUT api/<RecepcionController>/5
        [HttpPut("ActualizarRecepcion/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Recepcion recepcion)
        {
            if (id != recepcion.IdRecepcion)
            {
                return BadRequest("Recepcion no encontrada.");
            }

            var result = await _recepcionRepository.UpdateEntityAsync(recepcion);
            if (result.Success == true)
            {
                return Ok("Recepcion actualizada.");
            }
            return BadRequest(result.Message);
        }

        // DELETE api/<RecepcionController>/5
        [HttpDelete("BorrarRecepcion/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var recepcion = await _recepcionRepository.GetEntityByIdAsync(id);
            if (recepcion == null)
            {
                return NotFound("Recepcion no encontrada.");
            }

            var result = await _recepcionRepository.BorrarRecepcionAsync(id);
            if (result.Success == true)
            {
                return Ok("Recepcion borrada.");
            }
            return BadRequest(result.Message);
        }
    }
}
