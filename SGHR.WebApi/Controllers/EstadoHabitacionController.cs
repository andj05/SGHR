using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.EstadoHabitacion;
using SGHR.WebApi.PersistenApi.Interface;

namespace SGHR.WebApi.Controllers
{
    public class EstadoHabitacionController : Controller
    {
        private readonly IEstadoHabitacionService _estadoHabitacionService;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService)
        {
            _estadoHabitacionService = estadoHabitacionService;
        }

        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            var result = await _estadoHabitacionService.GetAll();
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // GET: EstadoHabitacionController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EstadoHabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoHabitacionApiModel estadoHabitacion)
        {
            var result = await _estadoHabitacionService.Save(estadoHabitacion);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(estadoHabitacion);
        }

        // GET: EstadoHabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: EstadoHabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstadoHabitacionApiModel estadoHabitacionApiModel)
        {
            var result = await _estadoHabitacionService.Update(estadoHabitacionApiModel);
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View(estadoHabitacionApiModel);
        }

        // GET: EstadoHabitacionController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estadoHabitacionService.GetById(id);
            if (result.success)
            {
                return View(result.data);
            }
            ViewBag.Message = result.message;
            return View("Error");
        }

        // POST: EstadoHabitacionController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _estadoHabitacionService.Remove(new EstadoHabitacionApiModel { IdEstadoHabitacion = id });
            if (result.success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.message;
            return View();
        }
    }
}

