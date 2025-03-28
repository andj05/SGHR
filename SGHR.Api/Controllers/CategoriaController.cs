using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Base;
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

        // GET: api/Categoria/GetCategoria
        [HttpGet("GetCategoria")]
        public async Task<IActionResult> Get()
        {
            var result = await _categoriaService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var categorias = (IEnumerable<CategoriasDto>)result.Data;
            return Ok(categorias);
        }

        // GET api/Categoria/GetCategoriaByID/5
        [HttpGet("GetCategoriaByID")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _categoriaService.GetById(id);
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        // GET api/Cliente/GetDeletedCategorias
        [HttpGet("GetDeletedCategorias")]
        public async Task<IActionResult> GetDeletedCategorias()
        {
            var result = await _categoriaService.GetAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var categoria = result.Data as IEnumerable<CategoriasDto>;
            if (categoria == null || !categoria.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(categoria);
        }


        // GET api/Categoria/GetDeletedCategoriaByID/5
        [HttpGet("GetDeletedCategoriasByID/{id}")]
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

        // POST api/Categoria/SaveCategoria
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

        // PUT api/Categoria/UpdateCategoria/5
        [HttpPut("UpdateCategoria/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateCategoriasDto categoriaDto)
        {
            if (id != categoriaDto.IdCategoria)
            {
                return BadRequest("El ID de la URL y el del categoriaDto no coinciden.");
            }

            OperationResult result = await _categoriaService.Update(categoriaDto);
            if (result.Success == true)
            {
                return Ok("Categoría actualizada.");
            }
            return BadRequest(result.Message);
        }

        // DELETE api/Categoria/DeleteCategoria/5
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

        // PUT api/Categoria/RestoreCategoria/5
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