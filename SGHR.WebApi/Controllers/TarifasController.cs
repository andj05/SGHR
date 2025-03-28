using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Tarifas;

namespace SGHR.WebApi.Controllers
{
    public class TarifasController : Controller
    {
        // GET: TarifasController
        public async Task<IActionResult> Index()
        {
            List<TarifasApiModel> tarifas = new List<TarifasApiModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync("Tarifas/GetTarifas");
                if (response.IsSuccessStatusCode)
                {
                    tarifas = await response.Content.ReadFromJsonAsync<List<TarifasApiModel>>();
                }
            }
            return View(tarifas);
        }

        // GET: TarifasController/Details/5
        public async Task <IActionResult> Details(int id)
        {
            TarifasApiModel tarifa = new TarifasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Tarifas/GetTarifasByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    tarifa = await response.Content.ReadFromJsonAsync<TarifasApiModel>();
                }
            }
            return View(tarifa);
        }

        // GET: TarifasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TarifasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TarifasApiModel tarifas)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Tarifas/SaveTarifas", tarifas);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar la tarifa";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar la tarifa";
                return View();
            }
        }

        // GET: TarifasController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            TarifasApiModel tarifas = new TarifasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Tarifas/GetTarifasByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    tarifas = await response.Content.ReadFromJsonAsync<TarifasApiModel>() ?? new TarifasApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la Tarifa";
                    return View("Error");
                }
            }
            return View(tarifas);
        }

        // POST: TarifasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TarifasApiModel tarifasApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Tarifas/UpdateTarifa/{id}", tarifasApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar la tarifa: " + operationResult?.message;
                            return View(tarifasApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar la tarifa: " + response.ReasonPhrase;
                        return View(tarifasApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar la tarifa: {ex.Message}";
                return View(tarifasApiModel);
            }
        }


        // GET: TarifasController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            TarifasApiModel tarifas = new TarifasApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Tarifas/GetTarifasByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    tarifas = await response.Content.ReadFromJsonAsync<TarifasApiModel>() ?? new TarifasApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al elimiar la Tarifa";
                    return View("Error");
                }
            }
            return View(tarifas);
        }

        // POST: TarifasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, TarifasApiModel removetarifas)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Tarifas/DeleteTarifa/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar la tarifa: ";
                            return View(removetarifas);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar la tarifa: ";
                        return View(removetarifas);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar la tarifa: {ex.Message}";
                return View(removetarifas);
            }
        }
    }
}
