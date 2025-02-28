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
            var pisos = await _pisoRepository.GetAllAsync();
            return Ok(pisos.Where(p => !p.Deleted));
        }

        // GET api/Piso/GetPisoByID/5
        [HttpGet("GetPisoByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var piso = await _pisoRepository.GetEntityByIdAsync(id);
            if (piso == null || piso.Data is Piso p && p.Deleted)
            {
                return NotFound("Piso no existe o ha sido eliminado.");
            }
            return Ok(piso);
        }

        // POST api/Piso/SavePiso
        [HttpPost("SavePiso")]
        public async Task<IActionResult> Post([FromBody] Piso piso)
        {
            try
            {
                var savePiso = await _pisoRepository.SaveEntityAsync(piso);
                if (savePiso.Success == true)
                {
                    return Ok(new { Message = "Piso guardado exitosamente", Data = savePiso.Data });
                }
                return BadRequest(new { Message = "Error al guardar el piso", Error = savePiso.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando el piso");
                return StatusCode(500, new { Message = "Error interno al guardar el piso.", Error = ex.Message });
            }
        }

        // PUT api/Piso/UpdatePiso/5
        [HttpPut("UpdatePiso/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Piso piso)
        {
            if (id <= 0)
                return BadRequest("ID de piso inválido.");

            var existingPiso = await _pisoRepository.GetEntityByIdAsync(id);
            if (existingPiso.Data is not Piso pisoData || pisoData.Deleted)
            {
                return NotFound("Piso no encontrado o ha sido eliminado.");
            }

            piso.Id = id; // Asegurar que el ID es correcto
            var updatePiso = await _pisoRepository.UpdateEntityAsync(piso);
            if (updatePiso.Success == true)
            {
                return Ok(new { Message = "Piso actualizado exitosamente", Data = updatePiso.Data });
            }
            return BadRequest(new { Message = "Error al actualizar el piso", Error = updatePiso.Message ?? "Error desconocido" });
        }

        // DELETE api/Piso/DeletePiso/5
        [HttpDelete("DeletePiso/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de piso inválido.");

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso.Data is not Piso pisoData)
                {
                    return NotFound("Piso no encontrado.");
                }

                pisoData.Deleted = true;
                pisoData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                pisoData.ModifyDate = DateTime.Now;

                var deletePiso = await _pisoRepository.UpdateEntityAsync(pisoData);
                if (deletePiso.Success == true)
                {
                    return Ok(new { Message = "Piso eliminado lógicamente.", Data = deletePiso.Data });
                }
                return BadRequest(new { Message = "Error al eliminar el piso", Error = deletePiso.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el piso.");
                return StatusCode(500, new { Message = "Error interno al eliminar el piso.", Error = ex.Message });
            }
        }

        // PUT api/Piso/RestorePiso/5
        [HttpPut("RestorePiso/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de piso inválido.");

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso.Data is not Piso pisoData)
                    return NotFound("Piso no encontrado.");

                if (!pisoData.Deleted)
                    return BadRequest("El piso ya está activo.");

                // Restaurar piso
                pisoData.Deleted = false;
                pisoData.ModifyDate = DateTime.Now;
                pisoData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restorePiso = await _pisoRepository.UpdateEntityAsync(pisoData);
                if (restorePiso.Success == true)
                {
                    return Ok(new { Message = "Piso restaurado exitosamente.", Data = restorePiso.Data });
                }
                return BadRequest(new { Message = "Error al restaurar el piso", Error = restorePiso.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el piso.");
                return StatusCode(500, new { Message = "Error interno al restaurar el piso.", Error = ex.Message });
            }
        }

        // DELETE api/Piso/DeletePisoPermanente/5
        [HttpDelete("DeletePisoPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de piso inválido.");

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso.Data is not Piso pisoData)
                {
                    return NotFound("Piso no encontrado.");
                }

                var deleteResult = await _pisoRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Piso eliminado permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar el piso permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el piso permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar el piso permanentemente.", Error = ex.Message });
            }
        }
    }
}


