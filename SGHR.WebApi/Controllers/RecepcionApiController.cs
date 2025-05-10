using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Recepcion;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.Controllers
{
    public class RecepcionApiController : Controller
    {
        private readonly IRecepcionService _recepcionService;

        public RecepcionApiController(IRecepcionService recepcionService)
        {
            _recepcionService = recepcionService;
        }

        // GET: RecepcionApiController
        public async Task<IActionResult> Index()
        {
            var result = await _recepcionService.GetAll();
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // GET: RecepcionApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _recepcionService.GetById(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // GET: RecepcionApiController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RecepcionApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveRecepcionModel recepcion)
        {
            var result = await _recepcionService.Save(recepcion);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(recepcion);
        }

        // GET: RecepcionApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _recepcionService.GetForUpdate(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // POST: RecepcionApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRecepcionModel recepcionModel)
        {
            var result = await _recepcionService.Update(recepcionModel);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(recepcionModel);
        }

        // GET: RecepcionApiController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recepcionService.GetForRemove(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // POST: RecepcionApiController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, RemoveRecepcionModel removeRecepcion)
        {
            var result = await _recepcionService.Remove(removeRecepcion);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(removeRecepcion);
        }
    }
}