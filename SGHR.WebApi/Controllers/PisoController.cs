using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Piso;

namespace SGHR.WebApi.Controllers
{
    public class PisoController : Controller
    {
        // GET: PisoController
        public async Task<IActionResult> Index()
        {
            List<PisoApiModel> pisos = new List<PisoApiModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync("Piso/GetPisos");

                if (response.IsSuccessStatusCode)
                {
                    pisos = await response.Content.ReadFromJsonAsync<List<PisoApiModel>>();
                }
            }
            return View(pisos);
        }

        // GET: PisoController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            PisoApiModel piso = new PisoApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Piso/GetPisoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    piso = await response.Content.ReadFromJsonAsync<PisoApiModel>();
                }
            }
            return View(piso);
        }

        // GET: PisoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PisoApiModel piso)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Piso/SavePiso", piso);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar el piso";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar el piso";
                return View();
            }
        }

        // GET: PisoController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            PisoApiModel piso = new PisoApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Piso/GetPisoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    piso = await response.Content.ReadFromJsonAsync<PisoApiModel>() ?? new PisoApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles del piso";
                    return View("Error");
                }
            }
            return View(piso);
        }

        // POST: PisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PisoApiModel pisoApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Piso/UpdatePiso/{id}", pisoApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar el piso: " + operationResult?.message;
                            return View(pisoApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar el piso: " + response.ReasonPhrase;
                        return View(pisoApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar el piso: {ex.Message}";
                return View(pisoApiModel);
            }
        }

        // GET: PisoController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            PisoApiModel piso = new PisoApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Piso/GetPisoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    piso = await response.Content.ReadFromJsonAsync<PisoApiModel>() ?? new PisoApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al eliminar el piso";
                    return View("Error");
                }
            }
            return View(piso);
        }

        // POST: PisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, PisoApiModel removepiso)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Piso/DeletePiso/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar el piso: ";
                            return View(removepiso);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar el piso: ";
                        return View(removepiso);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar el piso: {ex.Message}";
                return View(removepiso);
            }
        }
    }
}
