using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.Usuario;
using WebAPI.Models.Interfaces;
using WebAPI.Services;
using WebAPI.Interfaces;
using WebAPI.Models.Cliente;

namespace WebAPI.Controllers
{
    public class UsuarioApiController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILoggerManager _logger;
        private readonly MessageMapper _messageMapper;

        public UsuarioApiController(IUsuarioRepository usuarioRepository, ILoggerManager logger, MessageMapper messageMapper)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
            _messageMapper = messageMapper;
        }

        // GET: UsuarioApiController
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInfo("Obteniendo lista de usuarios");
                var usuarios = await _usuarioRepository.GetAllAsync();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de usuarios: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return View(new List<UsuarioModel>());
            }
        }

        // GET: UsuarioApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioApiController/Create
        public IActionResult Create()
        {
            return View(new UsuarioModel { FechaCreacion = DateTime.Now });
        }

        // POST: UsuarioApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioModel usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                usuario.FechaCreacion = DateTime.Now;
                usuario.Estado = true;
                usuario.Deleted = false;
                usuario.CreationUser = 1;
                usuario.ModifyUser = 1;
                usuario.ModifyDate = DateTime.Now;
                usuario.IdRolUsuario = 5; // Asigna el rol de usuario predeterminado

                var createdUsuario = await _usuarioRepository.CreateAsync(usuario);

                TempData["Success"] = _messageMapper.SuccessMessages["SaveSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear usuario: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["SaveFailed"];
                return View(usuario);
            }
        }

        // GET: UsuarioApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario para editar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioModel usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Clave))
            {
                ModelState.Remove("Clave");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var existingUsuario = await _usuarioRepository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                return RedirectToAction(nameof(Index));
            }

            existingUsuario.NombreCompleto = usuario.NombreCompleto;
            existingUsuario.Correo = usuario.Correo;
            if (!string.IsNullOrEmpty(usuario.Clave))
            {
                existingUsuario.Clave = usuario.Clave;
            }

            existingUsuario.IdRolUsuario = usuario.IdRolUsuario;
            existingUsuario.Estado = usuario.Estado;
            existingUsuario.ModifyDate = DateTime.Now;
            existingUsuario.ModifyUser = 1;

            await _usuarioRepository.UpdateAsync(existingUsuario, id);
            TempData["Success"] = _messageMapper.SuccessMessages["UpdateSuccess"];
            return RedirectToAction(nameof(Index));
        }


        // GET: UsuarioApiController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario para eliminar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuarioApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Index));
                }

                await _usuarioRepository.DeleteAsync(id);
                TempData["Success"] = _messageMapper.SuccessMessages["DeleteSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar usuario {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DeleteFailed"];
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuarioApiController/Deleted
        public async Task<IActionResult> Deleted()
        {
            try
            {
                _logger.LogInfo("Obteniendo lista de usuarios eliminados");
                var usuarios = await _usuarioRepository.GetDeletedAsync();
                return View(usuarios?.ToList() ?? new List<UsuarioModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener la lista de usuarios eliminados: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return View(new List<UsuarioModel>());
            }
        }

        // GET: UsuarioApiController/DeletedDetails/5
        public async Task<IActionResult> DeletedDetails(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetDeletedByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Deleted));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario eliminado {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Deleted));
            }
        }

        // GET: UsuarioApiController/Restore/5
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetDeletedByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["EntityBase"]["NotFound"];
                    return RedirectToAction(nameof(Deleted));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener usuario para restaurar {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["DbException"];
                return RedirectToAction(nameof(Deleted));
            }
        }

        // POST: UsuarioApiController/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.RestoreAsync(id);
                TempData["Success"] = _messageMapper.SuccessMessages["RestoreSuccess"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al restaurar usuario {id}: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["RestoreFailed"];
                return RedirectToAction(nameof(Deleted));
            }
        }

        // GET: UsuarioApiController/Login
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        // POST: UsuarioApiController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(login);
                }

                var result = await _usuarioRepository.LoginAsync(login);
                if (result == null)
                {
                    TempData["Error"] = _messageMapper.ErrorMessages["Login"]["InvalidCredentials"];
                    return View(login);
                }

                TempData["Success"] = _messageMapper.SuccessMessages["LoginSuccess"];
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en inicio de sesión: {ex.Message}");
                TempData["Error"] = _messageMapper.ErrorMessages["Operations"]["LoginFailed"];
                return View(login);
            }
        }
    }
}
