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

        // GET: api/Tarifas/GetTarifas
        [HttpGet("GetTarifas")]
        public async Task<IActionResult> Get()
        {
            var tarifas = await _tarifasRepository.GetAllAsync();
            return Ok(tarifas.Where(t => !t.Deleted));
        }

        // GET api/Tarifas/GetTarifasByID/5
        [HttpGet("GetTarifasByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (tarifa == null || tarifa.Data is Tarifas t && t.Deleted)
            {
                return NotFound("Tarifa no encontrada o eliminada.");
            }
            return Ok(tarifa);
        }

        // POST api/Tarifas/SaveTarifas
        [HttpPost("SaveTarifas")]
        public async Task<IActionResult> Post([FromBody] Tarifas tarifas)
        {
            try
            {
                var saveTarifas = await _tarifasRepository.SaveEntityAsync(tarifas);
                if (saveTarifas.Success == true)
                {
                    return Ok(new { Message = "Tarifa guardada exitosamente", Data = saveTarifas.Data });
                }
                return BadRequest(new { Message = "Error al guardar la tarifa", Error = saveTarifas.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la tarifa.");
                return StatusCode(500, new { Message = "Error interno al guardar la tarifa.", Error = ex.Message });
            }
        }

        // PUT api/Tarifas/UpdateTarifas/5
        [HttpPut("UpdateTarifas/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tarifas tarifas)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            var existingTarifa = await _tarifasRepository.GetEntityByIdAsync(id);
            if (existingTarifa.Data is not Tarifas tarifaData || tarifaData.Deleted)
            {
                return NotFound("Tarifa no encontrada o eliminada.");
            }

            tarifas.Id = id; // Asegurar que el ID es correcto
            var updateTarifas = await _tarifasRepository.UpdateEntityAsync(tarifas);
            if (updateTarifas.Success == true)
            {
                return Ok(new { Message = "Tarifa actualizada exitosamente", Data = updateTarifas.Data });
            }
            return BadRequest(new { Message = "Error al actualizar la tarifa", Error = updateTarifas.Message ?? "Error desconocido" });
        }

        // DELETE api/Tarifas/DeleteTarifa/5
        [HttpDelete("DeleteTarifa/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa.Data is not Tarifas tarifaData)
                {
                    return NotFound("Tarifa no encontrada.");
                }

                tarifaData.Deleted = true;
                tarifaData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                tarifaData.ModifyDate = DateTime.Now;

                var deleteTarifa = await _tarifasRepository.UpdateEntityAsync(tarifaData);
                if (deleteTarifa.Success == true)
                {
                    return Ok(new { Message = "Tarifa eliminada lógicamente.", Data = deleteTarifa.Data });
                }
                return BadRequest(new { Message = "Error al eliminar la tarifa", Error = deleteTarifa.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa.");
                return StatusCode(500, new { Message = "Error interno al eliminar la tarifa.", Error = ex.Message });
            }
        }


        // PUT api/Tarifas/RestoreTarifa/5
        [HttpPut("RestoreTarifa/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa.Data is not Tarifas tarifaData)
                    return NotFound("Tarifa no encontrada.");

                if (!tarifaData.Deleted)
                    return BadRequest("La tarifa ya está activa.");

                // Restaurar tarifa
                tarifaData.Deleted = false;
                tarifaData.ModifyDate = DateTime.Now;
                tarifaData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreTarifa = await _tarifasRepository.UpdateEntityAsync(tarifaData);
                if (restoreTarifa.Success == true)
                {
                    return Ok(new { Message = "Tarifa restaurada exitosamente.", Data = restoreTarifa.Data });
                }
                return BadRequest(new { Message = "Error al restaurar la tarifa", Error = restoreTarifa.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la tarifa.");
                return StatusCode(500, new { Message = "Error interno al restaurar la tarifa.", Error = ex.Message });
            }
        }

        // DELETE api/Tarifas/DeleteTarifaPermanente/5
        [HttpDelete("DeleteTarifaPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de tarifa inválido.");

            try
            {
                var tarifa = await _tarifasRepository.GetEntityByIdAsync(id);
                if (tarifa.Data is not Tarifas tarifaData)
                {
                    return NotFound("Tarifa no encontrada.");
                }

                var deleteResult = await _tarifasRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Tarifa eliminada permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar la tarifa permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar la tarifa permanentemente.", Error = ex.Message });
            }
        }
    }
}
