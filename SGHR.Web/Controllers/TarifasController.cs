using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;

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
        public async Task<IActionResult> Index()
        {
            var result = await tarifasService.GetAll();
            if (result.Success != true)
            {
                List<TarifasDto> tarifasList = (List<TarifasDto>)result.Data;

                return View(tarifasList);
            }
            //tarifas
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
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TarifasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TarifasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TarifasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TarifasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
