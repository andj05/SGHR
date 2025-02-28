using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repository
{
    public class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<PisoRepository> _logger;
        private readonly IConfiguration _configuration;

        public PisoRepository(SGHRContext context,
                              ILogger<PisoRepository> logger,
                              IConfiguration configuration) : base(context)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Piso>> GetAllAsync()
        {
            try
            {
                return await _context.Pisos
                    .Where(p => p.Estado)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pisos.");
                throw;
            }
        }

        public async Task<OperationResult> GetEntityByIdAsync(int idPiso)
        {
            var result = new OperationResult();
            try
            {
                var piso = await _context.Pisos.FindAsync(idPiso);
                if (piso == null)
                {
                    result.Success = false;
                    result.Message = "Piso no encontrado.";
                }
                else
                {
                    result.Success = true;
                    result.Data = piso;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el piso con ID {idPiso}.");
                result.Success = false;
                result.Message = $"Error al obtener el piso: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ExistsAsync(int idPiso)
        {
            var result = new OperationResult();
            if (idPiso <= 0)
            {
                result.Success = false;
                result.Message = "El ID del piso es inválido.";
                return result;
            }
            try
            {
                bool exists = await _context.Pisos.AnyAsync(p => p.Id == idPiso);
                result.Success = exists;
                result.Data = exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar la existencia del piso con ID {idPiso}.");
                result.Success = false;
                result.Message = $"Error al verificar la existencia del piso: {ex.Message}";
            }
            return result;
        }

        public async Task<OperationResult> ActualizarDescripcionPisoAsync(int idPiso, string nuevaDescripcion)
        {
            var result = new OperationResult();
            try
            {
                var piso = await _context.Pisos.FindAsync(idPiso);
                if (piso == null)
                {
                    return new OperationResult { Success = false, Message = "Piso no encontrado." };
                }

                piso.Descripcion = nuevaDescripcion;
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Descripción del piso actualizada correctamente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la descripción del piso con ID {idPiso}.");
                return new OperationResult { Success = false, Message = $"Error al actualizar la descripción del piso: {ex.Message}" };
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Piso piso)
        {
            var validationResult = ValidatePiso(piso);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                await _context.Pisos.AddAsync(piso);
                await _context.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Piso guardado correctamente.", Data = piso };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el piso.");
                return new OperationResult { Success = false, Message = $"Error al guardar el piso: {ex.Message}" };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Piso piso)
        {
            var validationResult = ValidatePiso(piso);
            if (!validationResult.Success != null)
            {
                return validationResult;
            }

            try
            {
                var existingPiso = await _context.Pisos.FindAsync(piso.Id);
                if (existingPiso == null)
                {
                    return new OperationResult { Success = false, Message = "Piso no encontrado." };
                }

                existingPiso.Descripcion = piso.Descripcion ?? existingPiso.Descripcion;
                existingPiso.Estado = piso.Estado;
                existingPiso.ModifyDate = DateTime.Now;
                existingPiso.ModifyUser = 1; // En producción, obtener el usuario autenticado.

                _context.Pisos.Update(existingPiso);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Piso actualizado correctamente.", Data = existingPiso };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el piso.");
                return new OperationResult { Success = false, Message = $"Error al actualizar el piso: {ex.Message}" };
            }
        }

        public async Task<OperationResult> DeleteEntityAsync(int idPiso)
        {
            try
            {
                var piso = await _context.Pisos.FindAsync(idPiso);
                if (piso == null)
                {
                    return new OperationResult { Success = false, Message = "Piso no encontrado." };
                }

                _context.Pisos.Remove(piso);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Piso eliminado permanentemente." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el piso.");
                return new OperationResult { Success = false, Message = $"Error al eliminar el piso: {ex.Message}" };
            }
        }

        private OperationResult ValidatePiso(Piso piso)
        {
            if (piso == null)
            {
                return new OperationResult { Success = false, Message = "El piso no puede ser nulo." };
            }

            if (string.IsNullOrWhiteSpace(piso.Descripcion) || piso.Descripcion.Length > 100)
            {
                return new OperationResult { Success = false, Message = "La descripción del piso es obligatoria y debe tener un máximo de 100 caracteres." };
            }

            return new OperationResult { Success = true };
        }
    }
}