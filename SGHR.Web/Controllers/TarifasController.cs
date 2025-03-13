using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Web.Controllers
{
    public class TarifasController : Controller
    {
        private readonly ITarifasService tarifasService;

        public TarifasController(ITarifasService tarifasService) 
        {
            this.tarifasService = tarifasService;
        }

        // GET: TarifasController
        public async Task <IActionResult> Index()
        {
            var result = await tarifasService.GetAll();
            if(result.Success == true) 
            {
                List<TarifasDto> tarifas = (List<TarifasDto>)result.Data;
                return View(tarifas);
            }
            return View();
        }

        // GET: TarifasController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await tarifasService.GetById(id);

            if (result.Success == true)
            {
                var tarifasEntity = (Tarifas)result.Data;
                TarifasDto tarifasDto = TarifasMapper.ToDto(tarifasEntity);

                return View(tarifasDto);
            }

            return View();
        }

        // GET: TarifasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TarifasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveTarifasDto saveTarifasDto)
        {
            try
            {
                var result = await this.tarifasService.Save(saveTarifasDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: TarifasController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await tarifasService.GetById(id);

            if (result.Success == true)
            {
                var tarifasEntity = (Tarifas)result.Data;
                TarifasDto tarifasDto = TarifasMapper.ToDto(tarifasEntity);

                return View(tarifasDto);
            }

            return View();
        }

        // POST: TarifasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTarifasDto updateTarifasDto)
        {
            try
            {
                var result = await this.tarifasService.Update(updateTarifasDto);
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
