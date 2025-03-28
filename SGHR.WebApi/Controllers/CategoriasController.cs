using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Categorias;

namespace SGHR.WebApi.Controllers
{
    public class CategoriasController : Controller
    {
        // GET: CategoriasController
        public async Task<IActionResult> Index()
        {
            List<CategoriasApiModel> categorias = new List<CategoriasApiModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync("Categoria/GetCategoria");

                if (response.IsSuccessStatusCode)
                {
                    categorias = await response.Content.ReadFromJsonAsync<List<CategoriasApiModel>>();
                }
            }
            return View(categorias);
        }

        // GET: CategoriasController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            CategoriasApiModel categoria = new CategoriasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Categoria/GetCategoriaByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    categoria = await response.Content.ReadFromJsonAsync<CategoriasApiModel>();
                }
            }
            return View(categoria);
        }

        // GET: CategoriasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriasApiModel categoria)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Categoria/SaveCategoria", categoria);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar la categoría";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar la categoría";
                return View();
            }
        }

        // GET: CategoriasController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            CategoriasApiModel categoria = new CategoriasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Categoria/GetCategoriaByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    categoria = await response.Content.ReadFromJsonAsync<CategoriasApiModel>() ?? new CategoriasApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la categoría";
                    return View("Error");
                }
            }
            return View(categoria);
        }

        // POST: CategoriasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriasApiModel categoriaApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Categoria/UpdateCategoria/{id}", categoriaApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar la categoría: " + operationResult?.message;
                            return View(categoriaApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar la categoría: " + response.ReasonPhrase;
                        return View(categoriaApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar la categoría: {ex.Message}";
                return View(categoriaApiModel);
            }
        }

        // GET: CategoriasController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            CategoriasApiModel categoria = new CategoriasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Categoria/GetCategoriaByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    categoria = await response.Content.ReadFromJsonAsync<CategoriasApiModel>() ?? new CategoriasApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al eliminar la categoría";
                    return View("Error");
                }
            }
            return View(categoria);
        }

        // POST: CategoriasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CategoriasApiModel removeCategoria)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Categoria/DeleteCategoria/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar la categoría: ";
                            return View(removeCategoria);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar la categoría: ";
                        return View(removeCategoria);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar la categoría: {ex.Message}";
                return View(removeCategoria);
            }
        }
    }
}


