using Microsoft.AspNetCore.Mvc;
using SGHR.Persistence.Interfaces;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoHabitacionController : ControllerBase
    {
        private readonly IEstadoHabitacionRepository _estadoHabitacionRepository;
        private readonly ILogger<EstadoHabitacionController> _logger;

        public EstadoHabitacionController(IEstadoHabitacionRepository estadoHabitacionRepository, ILogger<EstadoHabitacionController> logger)
        {
            _estadoHabitacionRepository = estadoHabitacionRepository;
            _logger = logger;
        }

        // GET: api/EstadoHabitacion/GetEstadoHabitacion
        [HttpGet("GetEstadoHabitacion")]
        public async Task<IActionResult> Get()
        {
            var estados = await _estadoHabitacionRepository.ObtenerTodosLosEstadosAsync();
            return Ok(estados);
        }

        // GET api/EstadoHabitacion/GetEstadoByID/5
        [HttpGet("GetEstadoByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estado == null)
            {
                return NotFound("Habitacion No Existe");
            }
            return Ok(estado);
        }

        // POST api/EstadoHabitacion/SaveEstado
        [HttpPost("SaveEstado")]
        public async Task<IActionResult> Post([FromBody] EstadoHabitacion estado)
        {
            try
            {
                var savedEstado = await _estadoHabitacionRepository.SaveEntityAsync(estado);
                if (savedEstado.Success == true)
                {
                    var savedEstadoData = savedEstado.Data as EstadoHabitacion;
                    return Ok("Estado de habitación guardado correctamente");
                }
                return BadRequest(savedEstado.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error guardando los datos");
                return BadRequest($"Ocurrió un error guardando los datos: {ex.Message}");
            }
        }

        // PUT api/EstadoHabitacion/UpdateEstado
        [HttpPut("UpdateEstado")]
        public async Task<IActionResult> Put([FromBody] EstadoHabitacion estado)
        {
            var updatedEstado = await _estadoHabitacionRepository.UpdateEntityAsync(estado);
            if (updatedEstado.Success == true)
            {
                return Ok("Estado de habitación actualizado correctamente.");
            }
            return BadRequest(updatedEstado.Message);
        }
    }
}