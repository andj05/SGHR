using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.WebApi.Models;
using SGHR.WebApi.Models.RolUsuario;
using SGHR.WebApi.Models.Tarifas;

namespace SGHR.WebApi.Controllers
{
    public class RolUsuarioController : Controller
    {
        // GET: RolUsuarioController
        public async Task<IActionResult> Index()
        {
            List<RolUsuarioApiModel> rolUsuario = new List<RolUsuarioApiModel>();
            using (var client = new HttpClient())
            { 
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync("RolUsuario/GetRolUsuario");

                if (response.IsSuccessStatusCode) 
                { 
                    rolUsuario = await response.Content.ReadFromJsonAsync<List<RolUsuarioApiModel>>();
                }
            }
                return View(rolUsuario);
        }

        // GET: RolUsuarioController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            RolUsuarioApiModel rolUsuario = new RolUsuarioApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"RolUsuario/GetRolByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    rolUsuario = await response.Content.ReadFromJsonAsync<RolUsuarioApiModel>();
                }
            }
            return View(rolUsuario);
        }

        // GET: RolUsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolUsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolUsuarioApiModel rolUsuario)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PostAsJsonAsync("RolUsuario/SaveRolUsuario", rolUsuario);
                    if (response.IsSuccessStatusCode)
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                    else
                    {
                        ViewBag.Message = "Error al guardar el Rol del Usuario";
                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al guardar el Rol del Usuario";
                return View();
            }
        }

        // GET: RolUsuarioController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            RolUsuarioApiModel rolUsuario = new RolUsuarioApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"RolUsuario/GetRolByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    rolUsuario = await response.Content.ReadFromJsonAsync<RolUsuarioApiModel>() ?? new RolUsuarioApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al obtener los detalles del rol del usuario";
                    return View("Error");
                }
            }
            return View(rolUsuario);
        }

        // POST: RolUsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RolUsuarioApiModel rolUsuarioApiModel)
        {
            OperationResult? operationResult = null;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsJsonAsync($"RolUsuario/UpdateRol/{id}", rolUsuarioApiModel);
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al actualizar rol del usuario: " + operationResult?.message;
                            return View(rolUsuarioApiModel);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al actualizar rol del usuario: " + response.ReasonPhrase;
                        return View(rolUsuarioApiModel);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al actualizar rol del usuario: {ex.Message}";
                return View(rolUsuarioApiModel);
            }
        }

        // GET: RolUsuarioController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            RolUsuarioApiModel rolUsuario = new RolUsuarioApiModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"RolUsuario/GetRolByID?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    rolUsuario = await response.Content.ReadFromJsonAsync<RolUsuarioApiModel>() ?? new RolUsuarioApiModel();
                }
                else
                {
                    ViewBag.Message = "Error al elimiar el rol Usuario";
                    return View("Error");
                }
            }
            return View(rolUsuario);
        }

        // POST: RolUsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, RolUsuarioApiModel removeRolUsuario)
        {
            OperationResult operationResult = new OperationResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.DeleteAsync($"RolUsuario/DeleteRolUsuario/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        operationResult = await response.Content.ReadFromJsonAsync<OperationResult>();
                        if (operationResult != null && operationResult.success)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ViewBag.Message = "Error al eliminar el rol usuario: ";
                            return View(removeRolUsuario);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Error al eliminar el rol usuario: ";
                        return View(removeRolUsuario);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al eliminar rol usuario: {ex.Message}";
                return View(removeRolUsuario);
            }
        }
    }
}
