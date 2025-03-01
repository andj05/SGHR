using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;
using SGHR.Persistence.Repository;

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
            var servicios = await _serviciosRepository.GetAllAsync();
            return Ok(servicios.Where(s => !s.Deleted));
        }

        // GET api/Servicios/GetServiciosByID/5
        [HttpGet("GetServiciosByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (servicio == null || servicio.Data is Servicios s && s.Deleted)
            {
                return NotFound("Servicio no existe o ha sido eliminado.");
            }
            return Ok(servicio);
        }

        // POST api/Servicios/SaveServicios
        [HttpPost("SaveServicio")]
        public async Task<IActionResult> Post([FromBody] Servicios servicio)
        {
            try
            {
                var saveServicio = await _serviciosRepository.SaveEntityAsync(servicio);
                if (saveServicio.Success == true)
                {
                    return Ok(new { Message = "Servicio Guardado", Data = saveServicio.Data });
                }
                return BadRequest(new { Message = "Error al guardar servicio", Error = saveServicio.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando el servicio");
                return StatusCode(500, new { Message = "Error.", Error = ex.Message });
            }
        }

        // PUT api/Servicios/UpdateServicios/5
        [HttpPut("UpdateServicios/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Servicios servicio)
        {
            if (id <= 0)
                return BadRequest("ID de servicio inválido.");

            var existingServicio = await _serviciosRepository.GetEntityByIdAsync(id);
            if (existingServicio.Data is not Servicios servicioData || servicioData.Deleted)
            {
                return NotFound("Servicio no encontrado o ha sido eliminado.");
            }

            servicio.Id = id; // Asegurar que el ID es correcto
            var updateServicio = await _serviciosRepository.UpdateEntityAsync(servicio);
            if (updateServicio.Success == true)
            {
                return Ok(new { Message = "Servicio actualizado exitosamente", Data = updateServicio.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el servicio", Error = updateServicio.Message ?? "Error desconocido" });
        }

        // DELETE api/Servicios/DeleteServicio/5
        [HttpDelete("DeleteServicio/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de servicio inválido.");

            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
                if (servicio.Data is not Servicios servicioData)
                {
                    return NotFound("Servicio no encontrado.");
                }

                servicioData.Deleted = true;
                servicioData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                servicioData.ModifyDate = DateTime.Now;

                var deleteServicio = await _serviciosRepository.UpdateEntityAsync(servicioData);
                if (deleteServicio.Success == true)
                {
                    return Ok(new { Message = "Servicio eliminado lógicamente.", Data = deleteServicio.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el servicio", Error = deleteServicio.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el servicio.");
                return StatusCode(500, new { Message = "Error interno al eliminar el servicio.", Error = ex.Message });
            }
        }

        // PUT api/Servicios/RestoreServicio/5
        [HttpPut("RestoreServicio/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de servicio inválido.");

            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
                if (servicio.Data is not Servicios servicioData)
                    return NotFound("Servicio no encontrado.");

                if (!servicioData.Deleted)
                    return BadRequest("El servicio ya está activo.");

                // Restaurar servicio
                servicioData.Deleted = false;
                servicioData.ModifyDate = DateTime.Now;
                servicioData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreServicio = await _serviciosRepository.UpdateEntityAsync(servicioData);
                if (restoreServicio.Success == true)
                {
                    return Ok(new { Message = "Servicio restaurado exitosamente.", Data = restoreServicio.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el servicio", Error = restoreServicio.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el servicio.");
                return StatusCode(500, new { Message = "Error interno al restaurar el servicio.", Error = ex.Message });
            }
        }

        // DELETE api/Servicios/DeleteServicioPermanente/5
        [HttpDelete("DeleteServicioPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de servicio inválido.");

            try
            {
                var servicio = await _serviciosRepository.GetEntityByIdAsync(id);
                if (servicio.Data is not Servicios servicioData)
                {
                    return NotFound("Servicio no encontrado.");
                }

                var deleteResult = await _serviciosRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Servicio eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el servicio permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el servicio permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el servicio permanentemente.", Error = ex.Message });
            }
        }
    }
}

