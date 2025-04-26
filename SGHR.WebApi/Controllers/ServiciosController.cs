using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Servicios;
using SGHR.WebApi.PersistenApi.Interface;

namespace SGHR.WebApi.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServiciosService _serviciosService;

        public ServiciosController(IServiciosService serviciosService)
        {
            _serviciosService = serviciosService;
        }

        // GET: ServiciosController
        public async Task<IActionResult> Index()
        {
            var result = await _serviciosService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: ServiciosController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _serviciosService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: ServiciosController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiciosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiciosApiModel servicio)
        {
            var result = await _serviciosService.Save(servicio);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(servicio);
        }

        // GET: ServiciosController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _serviciosService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: ServiciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiciosApiModel serviciosApiModel)
        {
            var result = await _serviciosService.Update(serviciosApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(serviciosApiModel);
        }

        // GET: ServiciosController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _serviciosService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: ServiciosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _serviciosService.Remove(new ServiciosApiModel { IdServicio = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}