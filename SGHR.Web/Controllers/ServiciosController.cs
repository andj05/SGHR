using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Interfaces;
using SGHR.Application.Dtos.Servicios;
using SGHR.Domain.Entities.Configuration;


namespace SGHR.Web.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IServiciosService serviciosService;

        public ServiciosController(IServiciosService serviciosService)
        {
            this.serviciosService = serviciosService;
        }
        // GET: ServiciosController
        public async Task<IActionResult> Index()
        {
            var result = await serviciosService.GetAll();
            if (result.Success == true)
            {
                List<ServiciosDto> servicios = (List<ServiciosDto>)result.Data;
                return View(servicios);
            }
            return View();
        }

        // GET: ServiciosController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await serviciosService.GetById(id);

            if (result.Success == true)
            {
                var serviciosEntity = (Servicios)result.Data;
                ServiciosDto serviciosDto = ServiciosMapper.ToDto(serviciosEntity);

                return View(serviciosDto);
            }

            return View();
        }

        // GET: ServiciosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServciosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveServiciosDto saveServiciosDto)
        {
            try
            {
                var result = await this.serviciosService.Save(saveServiciosDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: ServciosController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await serviciosService.GetById(id);

            if (result.Success == true)
            {
                var serviciosEntity = (Servicios)result.Data;
                ServiciosDto serviciosDto = ServiciosMapper.ToDto(serviciosEntity);

                return View(serviciosDto);
            }

            return View();
        }

        // POST: ServciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateServiciosDto updateServiciosDto)
        {
            try
            {
                var result = await this.serviciosService.Update(updateServiciosDto);
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
