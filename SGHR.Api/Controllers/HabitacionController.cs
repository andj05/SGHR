using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Reservation;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionController> _logger;

        public HabitacionController(IHabitacionRepository habitacionRepository,
                                    ILogger<HabitacionController> logger)
        {
            _habitacionRepository = habitacionRepository;
            _logger = logger;
        }

        // GET: api/<HabitacionController>
        [HttpGet("GetHabitacion")]
        public async Task<IActionResult> Get()
        {
            var habitaciones = await _habitacionRepository.GetAllAsync();
            return Ok(habitaciones);
        }

        // GET api/<HabitacionController>/5
        [HttpGet("GetHabitacionById")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
            if (habitacion == null)
            {
                return NotFound("Habitacion no encontrada");
            }
            return Ok(habitacion);
        }

        // POST api/<HabitacionController>
        [HttpPost("GuardarHabitacion")]
        public async Task<IActionResult> Post([FromBody] Habitacion habitacion)
        {
            var result = await _habitacionRepository.SaveEntityAsync(habitacion);
            if (result.Success == true)
            {
                return Ok("Habitacion guardada.");
            }
            return BadRequest(result.Message);
        }

        // PUT api/<HabitacionController>/5
        [HttpPut("ActualizarHabitacion")]
        public async Task<IActionResult> Put(int id, [FromBody] Habitacion habitacion)
        {
            var existingHabitacion = await _habitacionRepository.GetEntityByIdAsync(id);
            if (existingHabitacion == null)
            {
                return NotFound("Habitacion no encontrada.");
            }
            var result = await _habitacionRepository.UpdateEntityAsync(habitacion);
            if (result.Success == true)
            {
                return Ok("Habitacion actualizada.");
            }
                return BadRequest("Habitacion no encontrada.");
        }
        // DELETE api/<HabitacionController>/5
        [HttpDelete("BorrarHabitacion/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
            if (habitacion == null)
            {
                return NotFound("Habitacion no encontrada.");
            }
            var result = await _habitacionRepository.DeleteEntityAsync(habitacion);
            if (result.Success == true)
            {
                return Ok("Habitacion borrada.");
            }
            return BadRequest(result.Message);
        }
    }
}
