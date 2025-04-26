using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Tarifas;
using SGHR.WebApi.PersistenApi.Interface;


namespace SGHR.WebApi.Controllers
{
    public class TarifasController : Controller
    {
        private readonly ITarifasService _tarifasService;

        public TarifasController(ITarifasService tarifasService)
        {
            _tarifasService = tarifasService;
        }

        // GET: TarifasController
        public async Task<IActionResult> Index()
        {
            var result = await _tarifasService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: TarifasController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: TarifasController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TarifasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarifasApiModel tarifas)
        {
            var result = await _tarifasService.Save(tarifas);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(tarifas);
        }

        // GET: TarifasController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: TarifasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TarifasApiModel tarifasApiModel)
        {
            var result = await _tarifasService.Update(tarifasApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(tarifasApiModel);
        }

        // GET: TarifasController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tarifasService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: TarifasController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _tarifasService.Remove(new TarifasApiModel { IdTarifa = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}
