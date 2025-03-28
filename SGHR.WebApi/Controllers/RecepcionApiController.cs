using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Recepcion;

namespace SGHR.WebApi.Controllers
{
    public class RecepcionApiController : Controller
    {
        // GET: RecepcionApiController
        public async Task<IActionResult> Index()
        {
            List<RecepcionModel> recepciones = new List<RecepcionModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Recepcion/GetRecepciones");
                if (response.IsSuccessStatusCode)
                {
                    recepciones = await response.Content.ReadFromJsonAsync<List<RecepcionModel>>();
                }
            }

            return View(recepciones);
        }

        // GET: RecepcionApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            RecepcionModel recepcion = new RecepcionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Recepcion/GetRecepcionById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    recepcion = await response.Content.ReadFromJsonAsync<RecepcionModel>();
                }
            }
            return View(recepcion);
        }

        // GET: RecepcionApiController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecepcionApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveRecepcionModel recepcion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Recepcion/GuardarRecepcion", recepcion);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar la recepcion";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar la recepcion";
                return View();
            }
        }

        // GET: RecepcionApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UpdateRecepcionModel recepcion = new UpdateRecepcionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Recepcion/GetRecepcionById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    recepcion = await response.Content.ReadFromJsonAsync<UpdateRecepcionModel>() ?? new UpdateRecepcionModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la recepcion";
                    return View("Error");
                }
            }
            return View(recepcion);
        }

        // PUT: RecepcionApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRecepcionModel recepcionModel)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Recepcion/ActualizarRecepcion/{id}", recepcionModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.Success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar la recepcion: ";
                            return View(recepcionModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar la recepcion: ";
                        return View(recepcionModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar la recepcion: {ex.Message}";
                return View(recepcionModel);
            }
        }

        // GET: RecepcionApiController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            RemoveRecepcionModel recepcion = new RemoveRecepcionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Recepcion/GetRecepcionById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    recepcion = await response.Content.ReadFromJsonAsync<RemoveRecepcionModel>() ?? new RemoveRecepcionModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la recepcion";
                    return View("Error");
                }
            }
            return View(recepcion);
        }

        // POST: RecepcionApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, RemoveRecepcionModel removeRecepcion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Recepcion/BorrarRecepcion/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.Success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar la recepcion: ";
                            return View(removeRecepcion);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar la recepcion: ";
                        return View(removeRecepcion);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar la recepcion: {ex.Message}";
                return View(removeRecepcion);
            }
        }
    }
}
