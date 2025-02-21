using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;


namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ControllerBase
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoController> _logger;

        public PisoController(IPisoRepository pisoRepository, ILogger<PisoController> logger)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
        }

        // GET: api/Piso/GetPisos
        [HttpGet("GetPisos")]
        public async Task<IActionResult> Get()
        {
            var pisos = await _pisoRepository.ObtenerTodosLosPisosAsync();
            return Ok(pisos);
        }

        // GET api/Piso/GetPisoByID/5
        [HttpGet("GetPisoByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var piso = await _pisoRepository.GetEntityByIdAsync(id);
            if (piso == null)
            {
                return NotFound();
            }
            return Ok(piso);
        }

        // POST api/Piso/SavePiso
        [HttpPost("SavePiso")]
        public async Task<IActionResult> Post([FromBody] Piso piso)
        {
            try
            {
                var savedPiso = await _pisoRepository.SaveEntityAsync(piso);
                if (savedPiso.Success == true)
                {
                    return Ok("Piso guardado exitosamente");
                }
                return BadRequest(savedPiso.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando el piso");
                return BadRequest($"Error guardando el piso: {ex.Message}");
            }
        }

        // PUT api/Piso/UpdatePiso
        [HttpPut("UpdatePiso")]
        public async Task<IActionResult> Put([FromBody] Piso piso)
        {
            var updatedPiso = await _pisoRepository.UpdateEntityAsync(piso);
            if (updatedPiso.Success == true)
            {
                return Ok( "Piso actualizado correctamente." );
            }
            return BadRequest(updatedPiso.Message);

        }
    }

}
