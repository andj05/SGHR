using Microsoft.AspNetCore.Mvc;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ICategoriaRepository categoriaRepository, ILogger<CategoriaController> logger)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        // GET: api/Categoria/GetCategorias
        [HttpGet("GetCategorias")]
        public async Task<IActionResult> Get()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return Ok(categorias.Where(c => !c.Deleted));
        }

        // GET api/Categoria/GetCategoriaByID/5
        [HttpGet("GetCategoriaByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null || categoria.Data is Categoria c && c.Deleted)
            {
                return NotFound("Categoría no existe o ha sido eliminada.");
            }
            return Ok(categoria);
        }

        // POST api/Categoria/SaveCategoria
        [HttpPost("SaveCategoria")]
        public async Task<IActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                var saveCategoria = await _categoriaRepository.SaveEntityAsync(categoria);
                if (saveCategoria.Success == true)
                {
                    return Ok(new { Message = "Categoría guardada exitosamente", Data = saveCategoria.Data });
                }
                return BadRequest(new { Message = "Error al guardar la categoría", Error = saveCategoria.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error guardando los datos");
                return StatusCode(500, new { Message = "Error interno al guardar la categoría.", Error = ex.Message });
            }
        }

        // PUT api/Categoria/UpdateCategoria/5
        [HttpPut("UpdateCategoria/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Categoria categoria)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            var existingCategoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (existingCategoria.Data is not Categoria categoriaData || categoriaData.Deleted)
            {
                return NotFound("Categoría no encontrada o ha sido eliminada.");
            }

            categoria.Id = id; // Asegurar que el ID es correcto
            var updateCategoria = await _categoriaRepository.UpdateEntityAsync(categoria);
            if (updateCategoria.Success == true)
            {
                return Ok(new { Message = "Categoría actualizada exitosamente", Data = updateCategoria.Data });
            }
            return BadRequest(new { Message = "Error al actualizar la categoría", Error = updateCategoria.Message ?? "Error desconocido" });
        }

        // DELETE api/Categoria/DeleteCategoria/5
        [HttpDelete("DeleteCategoria/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria.Data is not Categoria categoriaData)
                {
                    return NotFound("Categoría no encontrada.");
                }

                categoriaData.Deleted = true;
                categoriaData.DeletedUser = 1; // En producción, obtener el usuario autenticado.
                categoriaData.ModifyDate = DateTime.Now;

                var deleteCategoria = await _categoriaRepository.UpdateEntityAsync(categoriaData);
                if (deleteCategoria.Success == true)
                {
                    return Ok(new { Message = "Categoría eliminada lógicamente.", Data = deleteCategoria.Data });
                }
                return BadRequest(new { Message = "Error al eliminar la categoría", Error = deleteCategoria.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría.");
                return StatusCode(500, new { Message = "Error interno al eliminar la categoría.", Error = ex.Message });
            }
        }

        // PUT api/Categoria/RestoreCategoria/5
        [HttpPut("RestoreCategoria/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria.Data is not Categoria categoriaData)
                    return NotFound("Categoría no encontrada.");

                if (!categoriaData.Deleted)
                    return BadRequest("La categoría ya está activa.");

                // Restaurar categoría
                categoriaData.Deleted = false;
                categoriaData.ModifyDate = DateTime.Now;
                categoriaData.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                var restoreCategoria = await _categoriaRepository.UpdateEntityAsync(categoriaData);
                if (restoreCategoria.Success == true)
                {
                    return Ok(new { Message = "Categoría restaurada exitosamente.", Data = restoreCategoria.Data });
                }
                return BadRequest(new { Message = "Error al restaurar la categoría", Error = restoreCategoria.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la categoría.");
                return StatusCode(500, new { Message = "Error interno al restaurar la categoría.", Error = ex.Message });
            }
        }

        // DELETE api/Categoria/DeleteCategoriaPermanente/5
        [HttpDelete("DeleteCategoriaPermanente/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria.Data is not Categoria categoriaData)
                {
                    return NotFound("Categoría no encontrada.");
                }

                var deleteResult = await _categoriaRepository.DeleteEntityAsync(id);
                if (deleteResult.Success != null)
                {
                    return Ok(new { Message = "Categoría eliminada permanentemente.", Data = deleteResult.Data });
                }

                return BadRequest(new { Message = "Error al eliminar la categoría permanentemente.", Error = deleteResult.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría permanentemente.");
                return StatusCode(500, new { Message = "Error interno al eliminar la categoría permanentemente.", Error = ex.Message });
            }
        }
    }
}
