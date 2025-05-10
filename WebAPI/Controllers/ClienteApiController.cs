using Microsoft.AspNetCore.Mvc;
using WebAPI.Interfaces;
using WebAPI.Models;
using WebAPI.Models.Cliente;
using WebAPI.Models.Interfaces;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class ClienteApiController : Controller
    {
        private readonly IRepository<ClienteModel> _clienteRepository;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public ClienteApiController(IRepository<ClienteModel> clienteRepository, ILoggerManager logger, MessageMapper messageMapper)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // GET: ClienteApiController
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInfo("Obteniendo lista de clientes");
                var clientes = await _clienteRepository.GetAllAsync();
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de clientes: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return View(new List<ClienteModel>());
            }
        }

        // GET: ClienteApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ClienteApiController/Create
        public IActionResult Create()
        {
            return View(new ClienteModel { Estado = true });
        }

        // POST: ClienteApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteModel cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(cliente);
                }

                cliente.FechaCreacion = DateTime.Now;
                cliente.Estado = true;
                cliente.Deleted = false;
                cliente.CreationUser = 1;
                cliente.ModifyUser = 1;
                cliente.ModifyDate = DateTime.Now;

                await _clienteRepository.CreateAsync(cliente);
                TempData["Success"] = _messageMapper.SuccessMessages["SaveSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear cliente: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["SaveFailed"];
                return View(cliente);
            }
        }

        // GET: ClienteApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente para editar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteModel cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(cliente);
                }

                var existingCliente = await _clienteRepository.GetByIdAsync(id);
                if (existingCliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }

                existingCliente.TipoDocumento = cliente.TipoDocumento;
                existingCliente.Documento = cliente.Documento;
                existingCliente.NombreCompleto = cliente.NombreCompleto;
                existingCliente.Correo = cliente.Correo;
                existingCliente.Telefono = cliente.Telefono;
                existingCliente.Nacionalidad = cliente.Nacionalidad;
                existingCliente.Clave = cliente.Clave ?? existingCliente.Clave;
                existingCliente.ModifyDate = DateTime.Now;
                existingCliente.ModifyUser = 1;

                await _clienteRepository.UpdateAsync(existingCliente, id);
                TempData["Success"] = _messageMapper.SuccessMessages["UpdateSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar cliente {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["UpdateFailed"];
                return View(cliente);
            }
        }

        // GET: ClienteApiController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente para eliminar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }

                cliente.Deleted = true;
                cliente.DeletedUser = 1;
                await _clienteRepository.DeleteAsync(id);
                TempData["Success"] = _messageMapper.SuccessMessages["DeleteSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar cliente {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"];
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ClienteApiController/Deleted
        public async Task<IActionResult> Deleted()
        {
            try
            {
                _logger.LogInfo("Obteniendo lista de clientes eliminados");
                var clientes = await _clienteRepository.GetDeletedAsync();
                return View(clientes?.ToList() ?? new List<ClienteModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de clientes eliminados: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return View(new List<ClienteModel>());
            }
        }

        // GET: ClienteApiController/DeletedDetails/5
        public async Task<IActionResult> DeletedDetails(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetDeletedByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Deleted));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente eliminado {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Deleted));
            }
        }

        // GET: ClienteApiController/Restore/5
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetDeletedByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Deleted));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cliente para restaurar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Deleted));
            }
        }

        // POST: ClienteApiController/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                var cliente = await _clienteRepository.RestoreAsync(id);
                TempData["Success"] = _messageMapper.SuccessMessages["RestoreSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al restaurar cliente {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"];
                return RedirectToAction(nameof(Deleted));
            }
        }
    }
}

