using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Reservation;
using SGHR.Domain.Base;

namespace SGHR.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecepcionController : ControllerBase
    {
        private readonly IRecepcionService _recepcionService;
        private readonly ILogger<RecepcionController> _logger;

        public RecepcionController(IRecepcionService recepcionService, ILogger<RecepcionController> logger)
        {
            _recepcionService = recepcionService;
            _logger = logger;
        }

        // GET: api/Recepcion/GetRecepciones
        [HttpGet("GetRecepciones")]
        public async Task<IActionResult> Get()
        {
            OperationResult result = await _recepcionService.GetAll();
            if (result.Success != true)
                return BadRequest(result.Message);
            return Ok(result.Data);
        }

        // GET: api/Recepcion/GetRecepcionById/{id}
        [HttpGet("GetRecepcionById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            OperationResult result = await _recepcionService.GetById(id);
            if (result.Success != true)
                return NotFound(result.Message);
            return Ok(result.Data);
        }

        // GET: api/Recepcion/GetRecepcionesByCliente/{idCliente}
        [HttpGet("GetRecepcionesByCliente/{idCliente}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            List<Recepcion> recepciones = await _recepcionService.ObtenerRecepcionesPorClienteId(idCliente);
            if (recepciones == null || !recepciones.Any())
                return NotFound("No se encontraron recepciones para este cliente.");
            return Ok(recepciones);
        }

        // GET: api/Recepcion/GetRecepcionesByHabitacion/{idHabitacion}
        [HttpGet("GetRecepcionesByHabitacion/{idHabitacion}")]
        public async Task<IActionResult> GetByHabitacion(int idHabitacion)
        {
            List<Recepcion> recepciones = await _recepcionService.ObtenerRecepcionesPorHabitacionId(idHabitacion);
            if (recepciones == null || !recepciones.Any())
                return NotFound("No se encontraron recepciones para esta habitación.");
            return Ok(recepciones);
        }

        // GET: api/Recepcion/GetRecepcionesByEstadoReserva/{idEstadoReserva}
        [HttpGet("GetRecepcionesByEstadoReserva/{idEstadoReserva}")]
        public async Task<IActionResult> GetByEstadoReserva(int idEstadoReserva)
        {
            List<Recepcion> recepciones = await _recepcionService.ObtenerRecepcionesPorEstadoReserva(idEstadoReserva);
            if (recepciones == null || !recepciones.Any())
                return NotFound("No se encontraron recepciones con este estado de reserva.");
            return Ok(recepciones);
        }

        // GET: api/Recepcion/GetRecepcionesByPrecioInicial?precioInicial=...
        [HttpGet("GetRecepcionesByPrecioInicial")]
        public async Task<IActionResult> GetByPrecioInicial([FromQuery] decimal precioInicial)
        {
            List<Recepcion> recepciones = await _recepcionService.ObtenerRecepcionesPorPrecioInicial(precioInicial);
            if (recepciones == null || !recepciones.Any())
                return NotFound("No se encontraron recepciones con este precio inicial.");
            return Ok(recepciones);
        }

        // POST: api/Recepcion/GuardarRecepcion
        [HttpPost("GuardarRecepcion")]
        public async Task<IActionResult> Post([FromBody] SaveRecepcionDto dto)
        {
            OperationResult result = await _recepcionService.Save(dto);
            if (result.Success != true)
                return BadRequest(result.Message);
            return Ok("Recepcion guardada.");
        }

        // PUT: api/Recepcion/ActualizarRecepcion/{id}
        [HttpPut("ActualizarRecepcion/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateRecepcionDto dto)
        {
            // Ensure the DTO has the correct ID
            dto.Id = id;
            OperationResult result = await _recepcionService.Update(dto);
            if (result.Success != true)
                return BadRequest(result.Message);
            return Ok("Recepcion actualizada.");
        }

        // DELETE: api/Recepcion/BorrarRecepcion/{id}
        [HttpDelete("BorrarRecepcion/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            RemoveRecepcionDto dto = new RemoveRecepcionDto { Id = id };
            OperationResult result = await _recepcionService.Remove(dto);
            if (result.Success!=true)
                return BadRequest(result.Message);
            return Ok("Recepcion borrada.");
        }

        // PUT: api/Recepcion/RestoreRecepcion/{id}
        [HttpPut("RestoreRecepcion/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            OperationResult result = await _recepcionService.Restore(id);
            if (result.Success != true)
                return BadRequest(result.Message);
            return Ok("Recepcion restaurada.");
        }
    }
}