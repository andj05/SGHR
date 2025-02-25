using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifasController : ControllerBase
    {
        private readonly ITarifasRepository _tarifasRepository;
        private readonly ILogger<TarifasController> _logger;

        public TarifasController(ITarifasRepository tarifasRepository, ILogger<TarifasController> logger)
        {
            _tarifasRepository = tarifasRepository;
            _logger = logger;
        }

        // GET: api/<TarifasController>
        [HttpGet("GetTarifas")]
        public async Task<IActionResult> Get()
        {
            var tarifas = await _tarifasRepository.ObtenerTodasLasTarifasAsync();
            return Ok(tarifas);
        }

        // GET api/<TarifasController>/5
        [HttpGet("GetTarifasByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifa == null)
            {
                return NotFound();
            }
            return Ok(tarifa);
        }

        // POST api/<TarifasController>
        [HttpPost("SaveTarifas")]
        public async Task<IActionResult> Post([FromBody] Tarifas tarifas)
        {
            var savedTarifas = await _tarifasRepository.SaveEntityAsync(tarifas);
            return Ok("Tarifa Agregada Correctamente");
        }

        [HttpPost("UpdateTarifas")]
        public async Task<IActionResult> Put([FromBody] Tarifas tarifas)
        {
            var savedTarifas = await _tarifasRepository.UpdateEntityAsync(tarifas);
            return Ok("Tarifa Actualizada Correctamente");
        }
    }
}