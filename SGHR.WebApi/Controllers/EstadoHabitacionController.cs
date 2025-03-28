using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.EstadoHabitacion;

namespace SGHR.WebApi.Controllers
{
    public class EstadoHabitacionController : Controller
    {
        // GET: EstadoHabitacionController
        public async Task<IActionResult> Index()
        {
            List<EstadoHabitacionApiModel> estadoHabitacion = new List<EstadoHabitacionApiModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync("EstadoHabitacion/GetEstadoHabitacion");

                if (response.IsSuccessStatusCode)
                {
                    estadoHabitacion = await response.Content.ReadFromJsonAsync<List<EstadoHabitacionApiModel>>();
                }
            }
            return View(estadoHabitacion);
        }

        // GET: EstadoHabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            EstadoHabitacionApiModel estadoHabitacion = new EstadoHabitacionApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"EstadoHabitacion/GetEstadoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    estadoHabitacion = await response.Content.ReadFromJsonAsync<EstadoHabitacionApiModel>();
                }
            }
            return View(estadoHabitacion);
        }

        // GET: EstadoHabitacionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoHabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EstadoHabitacionApiModel estadoHabitacion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("EstadoHabitacion/SaveEstadoHabitacion", estadoHabitacion);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar el estado de habitación";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar el estado de habitación";
                return View();
            }
        }

        // GET: EstadoHabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            EstadoHabitacionApiModel estadoHabitacion = new EstadoHabitacionApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"EstadoHabitacion/GetEstadoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    estadoHabitacion = await response.Content.ReadFromJsonAsync<EstadoHabitacionApiModel>() ?? new EstadoHabitacionApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles del estado de habitación";
                    return View("Error");
                }
            }
            return View(estadoHabitacion);
        }

        // POST: EstadoHabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EstadoHabitacionApiModel estadoHabitacionApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"EstadoHabitacion/UpdateEstadoHabitacion/{id}", estadoHabitacionApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar el estado de habitación: " + operationResult?.message;
                            return View(estadoHabitacionApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar el estado de habitación: " + response.ReasonPhrase;
                        return View(estadoHabitacionApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar el estado de habitación: {ex.Message}";
                return View(estadoHabitacionApiModel);
            }
        }

        // GET: EstadoHabitacionController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            EstadoHabitacionApiModel estadoHabitacion = new EstadoHabitacionApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"EstadoHabitacion/GetEstadoByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    estadoHabitacion = await response.Content.ReadFromJsonAsync<EstadoHabitacionApiModel>() ?? new EstadoHabitacionApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al eliminar el estado de habitación";
                    return View("Error");
                }
            }
            return View(estadoHabitacion);
        }

        // POST: EstadoHabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, EstadoHabitacionApiModel removeEstadoHabitacion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"EstadoHabitacion/DeleteEstado/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar el estado de habitación: ";
                            return View(removeEstadoHabitacion);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar el estado de habitación: ";
                        return View(removeEstadoHabitacion);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar el estado de habitación: {ex.Message}";
                return View(removeEstadoHabitacion);
            }
        }
    }
}

