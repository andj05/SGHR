using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Habitacion;
using SGHR.Application.Interfaces;

namespace SGHR.Web.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly IHabitacionService _habitacionService;
        public HabitacionController(IHabitacionService habitacionService)
        {
            _habitacionService = habitacionService;
        }

        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            var result = await _habitacionService.GetAll();
            if (result.Success == true)
            {
                List<HabitacionDto> habitaciones = (List<HabitacionDto>) result.Data;
                return View(habitaciones);
            }
            return View();
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _habitacionService.GetById(id);
            if (result.Success == true )
            {
                HabitacionDto habitacion = (HabitacionDto)result.Data;
                return View(habitacion);
            }
            return View();
        }

        // GET: HabitacionController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: HabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveHabitacionDto saveHabitacionDto)
        {
            try
            {
                var result = await _habitacionService.Save(saveHabitacionDto);
                if (result.Success == true)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: HabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _habitacionService.GetById(id);
            if (result.Success == true)
            {
                HabitacionDto habitacion = (HabitacionDto)result.Data;
                return View(habitacion);
            }
            return View();
        }

        // POST: HabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateHabitacionDto updateHabitacionDto)
        {
            try
            {
                var result = await _habitacionService.Update(updateHabitacionDto);
                if (result.Success == true)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
