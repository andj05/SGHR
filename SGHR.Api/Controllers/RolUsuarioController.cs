using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Configurations;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolUsuarioController : ControllerBase
    {
        private readonly IRolUsuarioService _rolUsuarioService;
        private readonly MessageMapper _messageMapper;

        public RolUsuarioController(IRolUsuarioService rolUsuarioService,
                                    ILogger<RolUsuarioController> logger,
                                    MessageMapper messageMapper)
        {
            _rolUsuarioService = rolUsuarioService;
            _messageMapper = messageMapper;
        }

        // GET: api/RolUsuario/GetRolUsuario
        [HttpGet("GetRolUsuario")]
        public async Task<IActionResult> Get()
        {
            var result = await _rolUsuarioService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);

            var roles = (IEnumerable<RolUsuarioDto>)result.Data;
            return Ok(roles);
        }

        // GET api/RolUsuario/GetRolByID/5
        [HttpGet("GetRolByID")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _rolUsuarioService.GetById(id);
            if (result.Success == true)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Message);
        }

        // GET api/RolUsuario/GetDeletedRolUsuario
        [HttpGet("GetDeletedRolUsuario")]
        public async Task<IActionResult> GetDeletedRoles()
        {
            var result = await _rolUsuarioService.GetAllDelete();
            if (result.Success != true)
                return BadRequest(result.Message);

            var rolUsuario = result.Data as IEnumerable<RolUsuarioDto>;
            if (rolUsuario == null || !rolUsuario.Any())
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(rolUsuario);
        }

        // GET api/RolUsuario/GetDeletedRolUsuarioByID/5
        [HttpGet("GetDeletedRolUsuarioByID/{id}")]
        public async Task<IActionResult> GetDeletedRoleByID(int id)
        {
            var result = await _rolUsuarioService.GetById(id);
            if (result.Success != true || result.Data == null)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            var rolUsuario = (RolUsuario)result.Data;
            if (!rolUsuario.Deleted)
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);

            return Ok(rolUsuario);
        }

        // POST api/RolUsuario/SaveRolUsuario
        [HttpPost("SaveRolUsuario")]
        public async Task<IActionResult> Post([FromBody] SaveRolUsuarioDto rolUsuarioDto)
        {
            try
            {
                var saveResult = await _rolUsuarioService.Save(rolUsuarioDto);
                if (saveResult.Success == true)
                    return Ok(new { Message = _messageMapper.SuccessMessages["SaveSuccess"], Data = saveResult.Data });

                return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = saveResult.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = _messageMapper.ErrorMessages["Operations"]["SaveFailed"], Error = ex.Message });
            }
        }

        // PUT api/RolUsuario/UpdateRol/5
        [HttpPut("UpdateRol/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateRolUsuarioDto rolUsuarioDto)
        {
            if (id != rolUsuarioDto.IdRolUsuario)
            {
                return BadRequest("El ID de la URL y el del rolUsuarioDto no coinciden.");
            }

            OperationResult result = await _rolUsuarioService.Update(rolUsuarioDto);
            if (result.Success == true)
            {
                return Ok("Rol de usuario actualizado.");
            }
            return BadRequest(result.Message);
        }

        // DELETE api/RolUsuario/DeleteRolUsuario/5
        [HttpDelete("DeleteRolUsuario/{id}")]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            if (id <= 0)
                return BadRequest(_messageMapper.ErrorMessages["EntityBase"]["InvalidID"]);

            var result = await _rolUsuarioService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var removeDto = new RemoveRolUsuarioDto { IdRolUsuario = id };
            var deleteResult = await _rolUsuarioService.Remove(removeDto);
            if (deleteResult.Success == true)
            {
                return Ok(new { Message = _messageMapper.SuccessMessages["DeleteSuccess"], Data = deleteResult.Data });
            }
            return BadRequest(new { Message = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"], Error = deleteResult.Message });
        }

        // PUT api/RolUsuario/RestoreRol/5
        [HttpPut("RestoreRol/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _rolUsuarioService.GetById(id);
            if (result.Success != true || result.Data == null)
            {
                return NotFound(_messageMapper.ErrorMessages["EntityBase"]["NotFound"]);
            }

            var restoreResult = await _rolUsuarioService.Restore(id);
            if (restoreResult.Success == true)
            {
                return Ok(_messageMapper.SuccessMessages["RestoreSuccess"]);
            }
            return BadRequest(restoreResult.Message);
        }
    }
}