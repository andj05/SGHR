using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Recepcion;
using SGHR.Application.Interfaces;

namespace SGHR.Web.Controllers
{
    public class RecepcionController : Controller
    {
        private readonly IRecepcionService _recepcionService;
        public RecepcionController(IRecepcionService recepcionService)
        {
            _recepcionService = recepcionService;
        }

        // GET: RecepcionController
        public async Task<IActionResult> Index()
        {
            var result = await _recepcionService.GetAll();
            if (result.Success == true)
            {
                List<RecepcionDto> recepciones = (List<RecepcionDto>)result.Data;
                return View(recepciones);
            }
            return View();
        }

        // GET: RecepcionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _recepcionService.GetById(id);
            if (result.Success == true)
            {
                RecepcionDto recepcion = (RecepcionDto)result.Data;
                return View(recepcion);
            }
            return View();
        }

        // GET: RecepcionController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: RecepcionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveRecepcionDto saveRecepcionDto)
        {
            try
            {
                var result = await _recepcionService.Save(saveRecepcionDto);
                if (result.Success == true)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: RecepcionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _recepcionService.GetById(id);
            if (result.Success == true)
            {
                RecepcionDto recepcion = (RecepcionDto)result.Data;
                return View(recepcion);
            }
            return View();
        }

        // POST: RecepcionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateRecepcionDto updateRecepcionDto)
        {
            try
            {
                var result = await _recepcionService.Update(updateRecepcionDto);
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
