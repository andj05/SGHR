using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly IServiciosRepository _serviciosRepository;
        private readonly ILogger<ServiciosController> _logger;

        public ServiciosController(IServiciosRepository serviciosRepository, ILogger<ServiciosController> logger)
        {
            _serviciosRepository = serviciosRepository;
            _logger = logger;
        }

        // GET: api/Servicios/GetServicios
        [HttpGet("GetServicios")]
        public async Task<IActionResult> Get()
        {
            var servicios = await _serviciosRepository.ObtenerTodosLosServiciosAsync();
            return Ok(servicios); 
        }

        // GET api/Servicios/GetServiciosByID/5
        [HttpGet("GetServiciosByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }
            return Ok(servicio);
        }

        // POST api/Servicios/SaveServicios
        [HttpPost("SaveServicios")]
        public async Task<IActionResult> Post([FromBody] Servicios servicio)
        {
            var savedServicio = await _serviciosRepository.SaveEntityAsync(servicio);
            if (savedServicio.Success == true)
            {
                return Ok("Servicio guardado exitosamente");
            }
            return BadRequest(savedServicio.Message);
        }

        // PUT api/Servicios/UpdateServicios
        [HttpPut("UpdateServicios")]
        public async Task<IActionResult> Put([FromBody] Servicios servicio)
        {
            var updatedServicio = await _serviciosRepository.UpdateEntityAsync(servicio);
            if (updatedServicio.Success == true)
            {
                return Ok("Servicio actualizado exitosamente");
            }
            return BadRequest(updatedServicio.Message);
        }
    }
}
