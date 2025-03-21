using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Dtos.Usuario;
using SGHR.Application.Intefaces;
using SGHR.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SGHR.Web1.Controllers
{
    public class UsuarioAdmController : Controller
    {
        private readonly IUsuariosService usuariosService;
        public UsuarioAdmController(IUsuariosService usuariosService)
        {
            this.usuariosService = usuariosService;
        }

        // GET: UsuarioApiController
        public async Task<IActionResult> Index()
        {
            var result = await usuariosService.GetAll();
            if (result.Success)
            {
                List<UsuarioDto> usuariosList = (List<UsuarioDto>)result.Data;
                return View(usuariosList);
            }
            return View();
        }

        // GET: UsuarioApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await usuariosService.GetById(id);
            if (result.Success)
            {
                UsuarioDto clienteList = (UsuarioDto)result.Data;
                return View(clienteList);
            }
            return View();
        }

        // GET: UsuarioApiController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioApiController/Create
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

        // GET: UsuarioApiController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuarioApiController/Edit/5
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

        // GET: UsuarioApiController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuarioApiController/Delete/5
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
