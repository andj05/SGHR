using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Interfaces;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces;

namespace SGHR.Application.Services
{
    public class PisosService : IPisosService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisosService> _logger;
        private readonly IConfiguration _configuration;

        public PisosService(IPisoRepository pisoRepository,
                            ILogger<PisosService> logger,
                            IConfiguration configuration)
        {
            _pisoRepository = pisoRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var operationResult = new OperationResult();
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                operationResult.Data = pisos;
                operationResult.Success = true;
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener todos los pisos";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var operationResult = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso == null)
                {
                    operationResult.Message = "Piso no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult.Data = piso;
                    operationResult.Success = true;
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al obtener el piso por ID";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Save(SavePisosDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var piso = new Piso
                {
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    FechaCreacion = dto.FechaCreacion
                };
                operationResult = await _pisoRepository.SaveEntityAsync(piso);
            }
            catch (Exception)
            {
                operationResult.Message = "Error al guardar el piso";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Update(UpdatePisosDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null)
                {
                    operationResult.Message = "Piso no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    piso.Descripcion = dto.Descripcion;
                    piso.Estado = dto.Estado;
                    piso.FechaCreacion = dto.FechaCreacion;
                    operationResult = await _pisoRepository.UpdateEntityAsync(piso);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al actualizar el piso";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<OperationResult> Remove(RemovePisosDto dto)
        {
            var operationResult = new OperationResult();
            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null)
                {
                    operationResult.Message = "Piso no encontrado";
                    operationResult.Success = false;
                }
                else
                {
                    operationResult = await _pisoRepository.DeleteEntityAsync(piso);
                }
            }
            catch (Exception)
            {
                operationResult.Message = "Error al eliminar el piso";
                operationResult.Success = false;
            }
            return operationResult;
        }

        public async Task<IEnumerable<Piso>> ObtenerTodosLosPisosAsync()
        {
            return await _pisoRepository.ObtenerTodosLosPisosAsync();
        }

        public async Task<bool> ExistePisoAsync(int idPiso)
        {
            return await _pisoRepository.ExistePisoAsync(idPiso);
        }
    }
}
