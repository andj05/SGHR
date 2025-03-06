using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

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

        [HttpGet("GetTarifas")]
        public async Task<IActionResult> Get()
        {
            var tarifas = await _tarifasRepository.GetAllAsync();
            return Ok(tarifas.Where(t => !t.Deleted));
        }

        [HttpGet("GetTarifaByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifa == null || tarifa.Deleted)
                return NotFound("Tarifa no encontrada o eliminada.");

            return Ok(tarifa);
        }

        [HttpPost("SaveTarifa")]
        public async Task<IActionResult> Post([FromBody] Tarifas tarifa)
        {
            try
            {
                var saveTarifa = await _tarifasRepository.SaveEntityAsync(tarifa);
                if (saveTarifa.Success != null)
                    return Ok(new { Message = "Tarifa guardada exitosamente", Data = saveTarifa.Data });

                return BadRequest(new { Message = "Error al guardar la tarifa", Error = saveTarifa.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la tarifa.");
                return StatusCode(500, new { Message = "Error interno al guardar la tarifa.", Error = ex.Message });
            }
        }

        [HttpPut("UpdateTarifa/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tarifas tarifa)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            var existingTarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (existingTarifa == null || existingTarifa.Deleted)
                return NotFound("Tarifa no encontrada o eliminada.");

            tarifa.Id = id;
            var updateTarifa = await _tarifasRepository.UpdateEntityAsync(tarifa);
            if (updateTarifa.Success != null)
                return Ok(new { Message = "Tarifa actualizada exitosamente", Data = updateTarifa.Data });

            return BadRequest(new { Message = "Error al actualizar la tarifa", Error = updateTarifa.Message ?? "Error desconocido" });
        }

        [HttpDelete("DeleteTarifa/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa == null)
                    return NotFound("Tarifa no encontrada.");

                tarifa.Deleted = true;
                tarifa.DeletedUser = 1;

                var deleteTarifa = await _tarifasRepository.UpdateEntityAsync(tarifa);
                if (deleteTarifa.Success != null)
                    return Ok(new { Message = "Tarifa eliminada lógicamente.", Data = deleteTarifa.Data });

                return BadRequest(new { Message = "Error al eliminar la tarifa", Error = deleteTarifa.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa.");
                return StatusCode(500, new { Message = "Error interno al eliminar la tarifa.", Error = ex.Message });
            }
        }

        [HttpPut("RestoreTarifa/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifa == null)
                return NotFound("Tarifa no encontrada.");

            var result = await _tarifasRepository.RestoreEntityAsync(tarifa);
            if (result.Success != null)
                return Ok("Tarifa restaurada exitosamente.");

            return BadRequest(result.Message);
        }
    }
}
