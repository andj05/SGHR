using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriasService _categoriaService;
        private readonly MessageMapper _messageMapper;

        public CategoriaController(ICategoriasService categoriaService, ILogger<CategoriaController> logger, MessageMapper messageMapper)
        {
            _categoriaService = categoriaService;
            _messageMapper = messageMapper;
        }

        [HttpGet("GetCategorias")]
        public async Task<IActionResult> Get()
        {
            var result = await _categoriaService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var categorias = (IEnumerable<Categoria>)result.Data;
            return Ok(categorias.Where(c => !c.Deleted));
        }

        [HttpGet("GetCategoriaByID/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _categoriaService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);

            var categoria = (Categoria)result.Data;
            if (categoria.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(categoria);
        }

        [HttpGet("GetDeletedCategoria")]
        public async Task<IActionResult> GetDeletedClientes()
        {
            var result = await _categoriaService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var categorias = (IEnumerable<Categoria>)result.Data;
            return Ok(categorias.Where(c => c.Deleted));
        }

        [HttpGet("GetDeletedTarifasByID/{id}")]
        public async Task<IActionResult> GetDeletedClienteByID(int id)
        {
            var result = await _categoriaService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var categoria = (Categoria)result.Data;
            if (!categoria.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(categoria);
        }

        [HttpPost("SaveCategoria")]
        public async Task<IActionResult> Post([FromBody] SaveCategoriasDto categoriaDto)
        {
            try
            {
                var saveResult = await _categoriaService.Save(categoriaDto);
                if (saveResult.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        [HttpPut("UpdateCategoria/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoriasDto categoriaDto)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var existingResult = await _categoriaService.GetById(id);
            if (existingResult.Success != true || existingResult.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var existingCategoria = (Categoria)existingResult.Data;
            if (existingCategoria.Deleted)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            categoriaDto.IdCategoria = id;
            var updateResult = await _categoriaService.Update(categoriaDto);
            if (updateResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["UpdateSuccess"], Data = updateResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"], Error = updateResult.Message ?? "Error desconocido" });
        }

        [HttpDelete("DeleteCategoria/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _categoriaService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveCategoriasDto { IdCategoria = id };
            var deleteResult = await _categoriaService.Remove(removeDto);
            if (deleteResult.Success != true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        [HttpPut("RestoreCategoria/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _categoriaService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _categoriaService.Restore(id);
            if (restoreResult.Success != true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(restoreResult.Message);
        }
    }
}