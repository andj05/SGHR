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
            var categorias = await _categoriaRepository.ObtenerTodasLasCategoriasAsync();
            return Ok(categorias);
        }

        // GET api/Categoria/GetCategoriaByID/5
        [HttpGet("GetCategoriaByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null)
            {
                return NotFound("Categoría No Existe");
            }
            return Ok(categoria);
        }

        // POST api/Categoria/SaveCategoria
        [HttpPost("SaveCategoria")]
        public async Task<IActionResult> Post([FromBody] Categoria categoria)
        {
            try
            {
                var savedCategoria = await _categoriaRepository.SaveEntityAsync(categoria);
                if (savedCategoria.Success == true)
                {
                    var savedCategoriaData = savedCategoria.Data as Categoria;
                    return Ok("Categoría guardada correctamente");
                }
                return BadRequest(savedCategoria.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error guardando los datos");
                return BadRequest($"Ocurrió un error guardando los datos: {ex.Message}");
            }
        }

        // PUT api/Categoria/UpdateCategoria
        [HttpPut("UpdateCategoria")]
        public async Task<IActionResult> Put([FromBody] Categoria categoria)
        {
            var updatedCategoria = await _categoriaRepository.UpdateEntityAsync(categoria);
            if (updatedCategoria.Success == true)
            {
                return Ok("Categoría actualizada correctamente.");
            }
            return BadRequest(updatedCategoria.Message);
        }

        // DELETE api/Categoria/DeleteCategoria/5
        [HttpDelete("DeleteCategoria/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
            if (categoria == null)
            {
                return NotFound("Categoría No Existe");
            }

            var result = await _categoriaRepository.DeleteEntityAsync(categoria);
            if (result.Success == true)
            {
                return Ok("Categoría eliminada correctamente.");
            }
            return BadRequest(result.Message);
        }
    }
}
