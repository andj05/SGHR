using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Categorias;
using SGHR.WebApi.PersistenApi.Interface;

namespace SGHR.WebApi.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriasService _categoriasService;

        public CategoriasController(ICategoriasService categoriasService)
        {
            _categoriasService = categoriasService;
        }

        // GET: CategoriasController
        public async Task<IActionResult> Index()
        {
            var result = await _categoriasService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: CategoriasController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoriasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: CategoriasController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriasApiModel categoria)
        {
            var result = await _categoriasService.Save(categoria);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(categoria);
        }

        // GET: CategoriasController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoriasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: CategoriasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriasApiModel categoriaApiModel)
        {
            var result = await _categoriasService.Update(categoriaApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(categoriaApiModel);
        }

        // GET: CategoriasController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: CategoriasController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _categoriasService.Remove(new CategoriasApiModel { IdCategoria = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}