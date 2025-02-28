using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
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
            var estados = await _estadoHabitacionRepository.GetAllAsync();
            return Ok(estados.Where(e => !e.Deleted));
        }

        // GET api/EstadoHabitacion/GetEstadoByID/5
        [HttpGet("GetEstadoByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (estado == null || estado.Data is EstadoHabitacion e && e.Deleted)
            {
                return NotFound("Estado de habitación no existe o ha sido eliminado.");
            }
            return Ok(estado);
        }

        // POST api/EstadoHabitacion/SaveEstado
        [HttpPost("SaveEstado")]
        public async Task<IActionResult> Post([FromBody] EstadoHabitacion estado)
        {
            try
            {
                var saveEstado = await _estadoHabitacionRepository.SaveEntityAsync(estado);
                if (saveEstado.Success == true)
                {
                    return Ok(new { Message = "Estado de habitación guardado exitosamente", Data = saveEstado.Data });
                }
                return BadRequest(new { Message = "Error al guardar el estado de habitación", Error = saveEstado.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error guardando los datos");
                return StatusCode(500, new { Message = "Error interno al guardar el estado de habitación.", Error = ex.Message });
            }
        }

        // PUT api/EstadoHabitacion/UpdateEstado/5
        [HttpPut("UpdateEstado/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] EstadoHabitacion estado)
        {
            if (id <= 0)
                return BadRequest("ID de estado de habitación inválido.");

            var existingEstado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
            if (existingEstado.Data is not EstadoHabitacion estadoData || estadoData.Deleted)
            {
                return NotFound("Estado de habitación no encontrado o ha sido eliminado.");
            }

            estado.Id = id; // Asegurar que el ID es correcto
            var updateEstado = await _estadoHabitacionRepository.UpdateEntityAsync(estado);
            if (updateEstado.Success == true)
            {
                return Ok(new { Message = "Estado de habitación actualizado exitosamente", Data = updateEstado.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el estado de habitación", Error = updateEstado.Message ?? "Error desconocido" });
        }

        // DELETE api/EstadoHabitacion/DeleteEstado/5
        [HttpDelete("DeleteEstado/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de estado de habitación inválido.");

            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estado.Data is not EstadoHabitacion estadoData)
                {
                    return NotFound("Estado de habitación no encontrado.");
                }

                estadoData.Deleted = true;
                estadoData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                estadoData.ModifyDate = DateTime.Now;

                var deleteEstado = await _estadoHabitacionRepository.UpdateEntityAsync(estadoData);
                if (deleteEstado.Success == true)
                {
                    return Ok(new { Message = "Estado de habitación eliminado lógicamente.", Data = deleteEstado.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el estado de habitación", Error = deleteEstado.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado de habitación.");
                return StatusCode(500, new { Message = "Error interno al eliminar el estado de habitación.", Error = ex.Message });
            }
        }

        // PUT api/EstadoHabitacion/RestoreEstado/5
        [HttpPut("RestoreEstado/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de estado de habitación inválido.");

            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estado.Data is not EstadoHabitacion estadoData)
                    return NotFound("Estado de habitación no encontrado.");

                if (!estadoData.Deleted)
                    return BadRequest("El estado de habitación ya está activo.");

                // Restaurar estado de habitación
                estadoData.Deleted = false;
                estadoData.ModifyDate = DateTime.Now;
                estadoData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreEstado = await _estadoHabitacionRepository.UpdateEntityAsync(estadoData);
                if (restoreEstado.Success == true)
                {
                    return Ok(new { Message = "Estado de habitación restaurado exitosamente.", Data = restoreEstado.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el estado de habitación", Error = restoreEstado.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el estado de habitación.");
                return StatusCode(500, new { Message = "Error interno al restaurar el estado de habitación.", Error = ex.Message });
            }
        }

        // DELETE api/EstadoHabitacion/DeleteEstadoPermanente/5
        [HttpDelete("DeleteEstadoPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de estado de habitación inválido.");

            try
            {
                var estado = await _estadoHabitacionRepository.GetEntityByIdAsync(id);
                if (estado.Data is not EstadoHabitacion estadoData)
                {
                    return NotFound("Estado de habitación no encontrado.");
                }

                var deleteResult = await _estadoHabitacionRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Estado de habitación eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el estado de habitación permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado de habitación permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el estado de habitación permanentemente.", Error = ex.Message });
            }
        }
    }
}
