using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        private readonly IHabitacionService _habitacionService;

        public HabitacionController(IHabitacionService habitacionService)
        {
            _habitacionService = habitacionService;
        }

        // GET: api/Habitacion/GetHabitacion
        [HttpGet("GetHabitacion")]
        public async Task<IActionResult> Get()
        {
            OperationResult result = await _habitacionService.GetAll();
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        // GET: api/Habitacion/GetHabitacionById?id=5
        [HttpGet("GetHabitacionById")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _habitacionService.GetById(id);
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        // GET: api/Habitacion/GetHabitacionesPorEstado/{idEstadoHabitacion}
        [HttpGet("GetHabitacionesPorEstado/{idEstadoHabitacion:int}")]
        public async Task<IActionResult> GetByEstado(int idEstadoHabitacion)
        {
            List<HabitacionDto> habitaciones = await _habitacionService.ObtenerHabitacionesPorEstadoId(idEstadoHabitacion);
            if (habitaciones == null || !habitaciones.Any())
            {
                return NotFound("No se encontraron habitaciones con el estado especificado.");
            }
            return Ok(habitaciones);
        }

        // GET: api/Habitacion/GetHabitacionesPorNumero?numero=ABC123
        [HttpGet("GetHabitacionesPorNumero")]
        public async Task<IActionResult> GetByNumero([FromQuery] string numero)
        {
            List<HabitacionDto> habitaciones = await _habitacionService.ObtenerHabitacionesPorNumero(numero);
            if (habitaciones == null || !habitaciones.Any())
            {
                return NotFound("No se encontraron habitaciones con el número especificado.");
            }
            return Ok(habitaciones);
        }

        // GET: api/Habitacion/GetHabitacionesPorPiso/{idPiso}
        [HttpGet("GetHabitacionesPorPiso/{idPiso:int}")]
        public async Task<IActionResult> GetByPiso(int idPiso)
        {
            List<HabitacionDto> habitaciones = await _habitacionService.ObtenerHabitacionesPorPisoId(idPiso);
            if (habitaciones == null || !habitaciones.Any())
            {
                return NotFound("No se encontraron habitaciones para el piso especificado.");
            }
            return Ok(habitaciones);
        }

        // GET: api/Habitacion/GetHabitacionesPorCategoria/{idCategoria}
        [HttpGet("GetHabitacionesPorCategoria/{idCategoria:int}")]
        public async Task<IActionResult> GetByCategoria(int idCategoria)
        {
            List<HabitacionDto> habitaciones = await _habitacionService.ObtenerHabitacionesPorCategoriaId(idCategoria);
            if (habitaciones == null || !habitaciones.Any())
            {
                return NotFound("No se encontraron habitaciones para la categoría especificada.");
            }
            return Ok(habitaciones);
        }

        // POST: api/Habitacion/GuardarHabitacion
        [HttpPost("GuardarHabitacion")]
        public async Task<IActionResult> Save([FromBody] SaveHabitacionDto dto)
        {
            OperationResult result = await _habitacionService.Save(dto);
            if (result.Success == true)
            {
                return Ok("Habitacion guardada.");
            }
            return BadRequest(result.Message);
        }

        // PUT: api/Habitacion/ActualizarHabitacion/{id}
        [HttpPut("ActualizarHabitacion/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHabitacionDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la URL y el del DTO no coinciden.");
            }

            OperationResult result = await _habitacionService.Update(dto);
            if (result.Success == true)
            {
                return Ok("Habitacion actualizada.");
            }
            return BadRequest(result.Message);
        }

        // DELETE: api/Habitacion/BorrarHabitacion/{id}
        [HttpDelete("BorrarHabitacion/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new RemoveHabitacionDto { Id = id };
            OperationResult result = await _habitacionService.Remove(dto);
            if (result.Success==true)
            {
                return Ok("Habitacion borrada.");
            }
            return BadRequest(result.Message);
        }

        // PUT: api/Habitacion/RestoreHabitacion/{id}
        [HttpPut("RestoreHabitacion/{id:int}")]
        public async Task<IActionResult> Restore(int id)
        {
            OperationResult result = await _habitacionService.Restore(id);
            if (result.Success == true)
            {
                return Ok("Habitacion restaurada.");
            }
            return BadRequest(result.Message);
        }
    }
}