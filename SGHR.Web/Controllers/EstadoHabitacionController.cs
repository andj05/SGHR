using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.EstadoHabitacion;
using SGHR.Application.Interfaces;


namespace SGHR.Web.Controllers
{
    public class EstadoHabitacionController : Controller
    {
        private readonly IEstadoHabitacionService estadoHabitacionService;

        public EstadoHabitacionController(IEstadoHabitacionService estadoHabitacionService)
        {
            this.estadoHabitacionService = estadoHabitacionService;
        }

        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            var result = await estadoHabitacionService.GetAll();
            if (result.Success == true)
            {
                List<EstadoHabitacionDto> estadoHabitacion = (List<EstadoHabitacionDto>)result.Data;
                return View(estadoHabitacion);
            }
            return View();
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await estadoHabitacionService.GetById(id);

            if (result.Success == true)
            {
                var estadoHabitacionEntity = (EstadoHabitacion)result.Data;
                EstadoHabitacionDto estadoHabitacionDto = EstadoHabitacionMapper.ToDto(estadoHabitacionEntity);

                return View(estadoHabitacionDto);
            }

            return View();
        }

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoHabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveEstadoHabitacionDto saveEstadoHabitacionDto)
        {
            try
            {
                var result = await this.estadoHabitacionService.Save(saveEstadoHabitacionDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoHabitacionController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await estadoHabitacionService.GetById(id);

            if (result.Success == true)
            {
                var estadoHabitacionEntity = (EstadoHabitacion)result.Data;
                EstadoHabitacionDto estadoHabitacionDto = EstadoHabitacionMapper.ToDto(estadoHabitacionEntity);

                return View(estadoHabitacionDto);
            }

            return View();
        }

        // POST: EstadoHabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEstadoHabitacionDto updateEstadoHabitacionDto)
        {
            try
            {
                var result = await this.estadoHabitacionService.Update(updateEstadoHabitacionDto);
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
