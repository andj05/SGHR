using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.RolUsuario;
using SGHR.WebApi.PersistenApi.Interface;


namespace SGHR.WebApi.Controllers
{
    public class RolUsuarioController : Controller
    {
        private readonly IRolUsuarioService _rolUsuarioService;

        public RolUsuarioController(IRolUsuarioService rolUsuarioService)
        {
            _rolUsuarioService = rolUsuarioService;
        }

        // GET: RolUsuarioController
        public async Task<IActionResult> Index()
        {
            var result = await _rolUsuarioService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: RolUsuarioController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _rolUsuarioService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: RolUsuarioController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RolUsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolUsuarioApiModel rolUsuario)
        {
            var result = await _rolUsuarioService.Save(rolUsuario);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(rolUsuario);
        }

        // GET: RolUsuarioController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _rolUsuarioService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: RolUsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RolUsuarioApiModel rolUsuarioApiModel)
        {
            var result = await _rolUsuarioService.Update(rolUsuarioApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(rolUsuarioApiModel);
        }

        // GET: RolUsuarioController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rolUsuarioService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: RolUsuarioController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _rolUsuarioService.Remove(new RolUsuarioApiModel { IdRolUsuario = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}