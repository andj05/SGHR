using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Pisos;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Web.Controllers
{
    public class PisoController : Controller
    {
        private readonly IPisosService pisosService;

        public PisoController(IPisosService pisosService) 
        {
            this.pisosService = pisosService;
        }

        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            var result = await pisosService.GetAll();
            if (result.Success == true)
            {
                List<PisosDto> pisos = (List<PisosDto>)result.Data;
                return View(pisos);
            }
            return View();
        }

        // GET: PisoController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await pisosService.GetById(id);

            if (result.Success == true)
            {
                var pisosEntity = (Piso)result.Data;
                PisosDto pisosDto = PisoMapper.ToDto(pisosEntity);

                return View(pisosDto);
            }

            return View();
        }

        // GET: PisoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SavePisosDto SavePisosDto)
        {
            try
            {
                var result = await this.pisosService.Save(SavePisosDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: PisoController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await pisosService.GetById(id);

            if (result.Success == true)
            {
                var pisosEntity = (Piso)result.Data;
                PisosDto tarifasDto = PisoMapper.ToDto(pisosEntity);

                return View(tarifasDto);
            }

            return View();
        }

        // POST: PisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePisosDto updatePisosDto)
        {
            try
            {
                var result = await this.pisosService.Update(updatePisosDto);
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
