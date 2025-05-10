using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models.Habitacion;
using SGHR.WebApi.ServicesApi.Interface;

namespace SGHR.WebApi.Controllers

{
    public class HabitacionApiController : Controller
    {
        private readonly IHabitacionService _habitacionService;

        public HabitacionApiController(IHabitacionService habitacionService)
        {
            _habitacionService = habitacionService;
        }

        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            var result = await _habitacionService.GetAll();
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _habitacionService.GetById(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // GET: HabitacionController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveHabitacionModel habitacion)
        {
            var result = await _habitacionService.Save(habitacion);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(habitacion);
        }

        // GET: HabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _habitacionService.GetForUpdate(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // POST: HabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateHabitacionModel habitacionModel)
        {
            var result = await _habitacionService.Update(habitacionModel);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(habitacionModel);
        }

        // GET: HabitacionController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _habitacionService.GetForRemove(id);
            if (result.Success)
            {
                return View(result.Data);
            }
            ViewBag.Message = result.Message;
            return View("Error");
        }

        // POST: HabitacionController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, RemoveHabitacionModel removeHabitacion)
        {
            var result = await _habitacionService.Remove(removeHabitacion);
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Message = result.Message;
            return View(removeHabitacion);
        }
    }
    }
