using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Piso;
using SGHR.WebApi.PersistenApi.Interface;

namespace SGHR.WebApi.Controllers
{
    public class PisoController : Controller
    {
        private readonly IPisoService _pisoService;

        public PisoController(IPisoService pisoService)
        {
            _pisoService = pisoService;
        }

        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            var result = await _pisoService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: PisoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _pisoService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: PisoController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PisoApiModel piso)
        {
            var result = await _pisoService.Save(piso);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(piso);
        }

        // GET: PisoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _pisoService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: PisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PisoApiModel pisoApiModel)
        {
            var result = await _pisoService.Update(pisoApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(pisoApiModel);
        }

        // GET: PisoController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pisoService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: PisoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _pisoService.Remove(new PisoApiModel { IdPiso = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}
