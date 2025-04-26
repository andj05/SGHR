using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Categorias;
using SGHR.Application.Interfaces;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Web.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly ICategoriasService categoriasService;

        public CategoriaController(ICategoriasService categoriasService) 
        {
            this.categoriasService = categoriasService;
        }

        // GET: CategoriaController
        public async Task<IActionResult> Index()
        {
            var result = await categoriasService.GetAll();
            if (result.Success == true)
            {
                List<CategoriasDto> categorias = (List<CategoriasDto>)result.Data;
                return View(categorias);
            }
            return View();
        }

        // GET: CategoriaController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await categoriasService.GetById(id);

            if (result.Success == true)
            {
                var categoriasEntity = (Categoria)result.Data;
                CategoriasDto categoriasDto = CategoriaMapper.ToDto(categoriasEntity);

                return View(categoriasDto);
            }

            return View();
        }

        // GET: CategoriaController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveCategoriasDto saveCategoriasDto)
        {
            try
            {
                var result = await this.categoriasService.Save(saveCategoriasDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoriaController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await categoriasService.GetById(id);

            if (result.Success == true)
            {
                var categoriasEntity = (Categoria)result.Data;
                CategoriasDto categoriasDto = CategoriaMapper.ToDto(categoriasEntity);

                return View(categoriasDto);
            }

            return View();
        }

        // POST: CategoriaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoriasDto updateCategoriasDto)
        {
            try
            {
                var result = await this.categoriasService.Update(updateCategoriasDto);
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
