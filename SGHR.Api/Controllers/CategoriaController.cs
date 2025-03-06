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

        [HttpGet("GetCategorias")]
        public async Task<IActionResult> Get()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return Ok(categorias.Where(c => !c.Deleted));
        }

        [HttpGet("GetCategoriaByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null || categoria.Deleted)
            {
                return NotFound("Categoría no existe o ha sido eliminada.");
            }
            return Ok(categoria);
        }

        [HttpPost("SaveCategoria")]
        public async Task<IActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                _logger.LogInformation("Intentando guardar categoría: {@categoria}", categoria);
                var saveCategoria = await _categoriaRepository.SaveEntityAsync(categoria);
                _logger.LogInformation("Respuesta del repositorio: {@saveCategoria}", saveCategoria);

                if (saveCategoria.Success != null)
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

        [HttpPut("UpdateCategoria/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Categoria categoria)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            var existingCategoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (existingCategoria == null || existingCategoria.Deleted)
            {
                return NotFound("Categoría no encontrada o ha sido eliminada.");
            }

            categoria.Id = id;
            var updateCategoria = await _categoriaRepository.UpdateEntityAsync(categoria);
            _logger.LogInformation("Respuesta del repositorio: {@updateCategoria}", updateCategoria);

            if (updateCategoria.Success != null)
            {
                return Ok(new { Message = "Categoría actualizada exitosamente", Data = updateCategoria.Data });
            }
            return BadRequest(new { Message = "Error al actualizar la categoría", Error = updateCategoria.Message ?? "Error desconocido" });
        }

        [HttpDelete("DeleteCategoria/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null)
                {
                    return NotFound("Categoría no encontrada.");
                }

                if (categoria.Deleted)
                {
                    return BadRequest("La categoría ya está eliminada.");
                }

                categoria.Deleted = true;
                categoria.DeletedUser = 1;
                categoria.ModifyDate = DateTime.Now;

                _logger.LogInformation("Actualizando estado de eliminado para categoría: {@categoria}", categoria);

                var deleteCategoria = await _categoriaRepository.UpdateEntityAsync(categoria);
                if (deleteCategoria.Success != null)
                {
                    return Ok(new { Message = "Categoría eliminada lógicamente.", Data = deleteCategoria.Data });
                }
                return BadRequest(new { Message = "Error al eliminar la categoría", Error = deleteCategoria.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría.");
                return StatusCode(500, new { Message = "Error interno al eliminar la categoría.", Error = ex.Message });
            }
        }

        [HttpPut("RestoreCategoria/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
                return BadRequest("ID de categoría inválido.");

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null)
                    return NotFound("Categoría no encontrada.");

                if (!categoria.Deleted)
                    return BadRequest("La categoría ya está activa.");

                categoria.Deleted = false;
                categoria.ModifyDate = DateTime.Now;
                categoria.ModifyUser = 1;

                _logger.LogInformation("Restaurando categoría: {@categoria}", categoria);

                var restoreCategoria = await _categoriaRepository.UpdateEntityAsync(categoria);
                if (restoreCategoria.Success != null)
                {
                    return Ok(new { Message = "Categoría restaurada exitosamente." });
                }
                return BadRequest(new { Message = "Error al restaurar la categoría", Error = restoreCategoria.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la categoría.");
                return StatusCode(500, new { Message = "Error interno al restaurar la categoría.", Error = ex.Message });
            }
        }
    }
}
