using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Usuario;
using SGHR.Application.Intefaces;

namespace SGHR.Web.Controllers
{
    public class UsuarioAdmController : Controller
    {
        public IUsuariosService UsuariosService { get; }

        public UsuarioAdmController(IUsuariosService usuariosService)
        {
            UsuariosService = usuariosService;
        }

        // GET: UsuarioAdmController
        public async Task<IActionResult> Index()
        {
            var result = await UsuariosService.GetAll();
            if (result.Success)
            {
                List<UsuarioDto> usuariosList = (List<UsuarioDto>)result.Data;
                return View(usuariosList);
            }
            return View("Error", result.Message);
        }

        // GET: UsuarioAdmController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuarioAdmController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioAdmController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioAdmController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioAdmController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsuarioAdmController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioAdmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
