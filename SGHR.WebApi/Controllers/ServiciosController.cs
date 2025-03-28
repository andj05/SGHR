using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Servicios;
using SGHR.WebApi.Models.Tarifas;

namespace SGHR.WebApi.Controllers
{
    public class ServiciosController : Controller
    {
        // GET: ServiciosController
        public async Task<IActionResult> Index()
        {
            List<ServiciosApiModel> servicios = new List<ServiciosApiModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress= new Uri("http://localhost:5187/api/");

                var reponse = await client.GetAsync("Servicios/GetServicios");

                if (reponse.IsSuccessStatusCode)
                {
                    servicios = await reponse.Content.ReadFromJsonAsync<List<ServiciosApiModel>>();
                }
            }
            return View(servicios);
        }

        // GET: ServiciosController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ServiciosApiModel servicio = new ServiciosApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Servicios/GetServiciosByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    servicio = await response.Content.ReadFromJsonAsync<ServiciosApiModel>();
                }
            }
            return View(servicio);
        }

        // GET: ServiciosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ServiciosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiciosApiModel servicio)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Servicios/SaveServicio", servicio);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar el servicio";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar el servicio";
                return View();
            }
        }

        // GET: ServiciosController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ServiciosApiModel servicio = new ServiciosApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Servicios/GetServiciosByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    servicio = await response.Content.ReadFromJsonAsync<ServiciosApiModel>() ?? new ServiciosApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de los servicios";
                    return View("Error");
                }
            }
            return View(servicio);
        }

        // POST: ServiciosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiciosApiModel serviciosApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Servicios/UpdateServicio/{id}", serviciosApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar el servicio: " + operationResult?.message;
                            return View(serviciosApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar el servicio: " + response.ReasonPhrase;
                        return View(serviciosApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizarel servicio: {ex.Message}";
                return View(serviciosApiModel);
            }
        }

        // GET: ServiciosController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            ServiciosApiModel servicios = new ServiciosApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Servicios/GetServiciosByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    servicios = await response.Content.ReadFromJsonAsync<ServiciosApiModel>() ?? new ServiciosApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al elimiar el servicio";
                    return View("Error");
                }
            }
            return View(servicios);
        }

        // POST: ServiciosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, ServiciosApiModel removeservicios)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Servicios/DeleteServicio/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error elimiar el servicio: ";
                            return View(removeservicios);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error elimiar el servicio: ";
                        return View(removeservicios);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error elimiar el servicio: {ex.Message}";
                return View(removeservicios);
            }
        }
    }
}
