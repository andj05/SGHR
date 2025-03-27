using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.Habitacion;
using System.Net.Http.Json;

namespace SGHR.WebApi.Controllers

    //finalizado Habitacion sin optimizar, seguir con recepcion. 
    //Aunque actualizar tenga [HttpPut] y borrar tenga [HttpDelete] en la capa API,
    //al declarar entre brackets aqui en el web controller usar [HttpPost]
{
    public class HabitacionApiController : Controller
    {
        // GET: HabitacionController
        public async Task<IActionResult> Index()
        {
            List<HabitacionModel> habitaciones = new List<HabitacionModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Habitacion/GetHabitacion");
                if (response.IsSuccessStatusCode)
                {
                    habitaciones = await response.Content.ReadFromJsonAsync<List<HabitacionModel>>();
                }
            }

            return View(habitaciones);
        }

        // GET: HabitacionController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            HabitacionModel habitacion = new HabitacionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Habitacion/GetHabitacionById?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    habitacion = await response.Content.ReadFromJsonAsync<HabitacionModel>();
                }
            }
            return View(habitacion);
        }

        // GET: HabitacionController/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: HabitacionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveHabitacionModel habitacion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("Habitacion/GuardarHabitacion", habitacion);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar la habitación";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar la habitación";
                return View();
            }
        }

        // GET: HabitacionController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UpdateHabitacionModel habitacion = new UpdateHabitacionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Habitacion/GetHabitacionById?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    habitacion = await response.Content.ReadFromJsonAsync<UpdateHabitacionModel>() ?? new UpdateHabitacionModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la habitación";
                    return View("Error");
                }
            }
            return View(habitacion);
        }

        // PUT: HabitacionController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateHabitacionModel habitacionModel)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"Habitacion/ActualizarHabitacion/{id}", habitacionModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.Success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar la habitación: ";
                            return View(habitacionModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar la habitación: ";
                        return View(habitacionModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar la habitación: {ex.Message}";
                return View(habitacionModel);
            }
        }

        // GET: HabitacionController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            RemoveHabitacionModel habitacion = new RemoveHabitacionModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Habitacion/GetHabitacionById?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    habitacion = await response.Content.ReadFromJsonAsync<RemoveHabitacionModel>() ?? new RemoveHabitacionModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles de la habitación";
                    return View("Error");
                }
            }
            return View(habitacion);
        }

        // DELETE: HabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, RemoveHabitacionModel removeHabitacion)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"Habitacion/BorrarHabitacion/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.Success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar la habitación: ";
                            return View(removeHabitacion);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar la habitación: ";
                        return View(removeHabitacion);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar la habitación: {ex.Message}";
                return View(removeHabitacion);
            }
        }
    }
    }
