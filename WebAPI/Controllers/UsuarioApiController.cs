using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebAPI.Models;
using WebAPI.Models.Cliente;
using WebAPI.Models.Usuario;

namespace WebAPI.Controllers
{
    public class UsuarioApiController : Controller
    {

        // GET: UsuarioApiController1
        public async Task<IActionResult> Index()
        {
            List<UsuarioModel> usuarios = new List<UsuarioModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Usuario/GetUsuarios");
                if (response.IsSuccessStatusCode)
                {
                    usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioModel>>();
                }
                else
                {
                    TempData["Error"] = "Error al obtener la lista de usuarios";
                }
            }
            return View(usuarios);
        }

        // GET: UsuarioApiController1/Details/5
        public async Task<IActionResult> Details(int id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Usuario/GetUsuarioByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                }
                else
                {
                    TempData["Error"] = "Error al obtener los detalles del usuario";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usuario);
        }

        // GET: UsuarioApiController1/Deleted
        public async Task<IActionResult> Deleted()
        {
            List<UsuarioModel> usuarios = new List<UsuarioModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Usuario/GetDeletedUsuarios");
                if (response.IsSuccessStatusCode)
                {
                    usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioModel>>();
                }
            }
            return View(usuarios);
        }

        // GET: UsuarioApiController1/DeletedDetails/5
        public async Task<IActionResult> DeletedDetails(int id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Usuario/GetDeletedUsuarioByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                }
                else
                {
                    TempData["Error"] = "Error al obtener los detalles del usuario";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usuario);
        }

        // GET: UsuarioApiController1/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: UsuarioApiController1/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var content = JsonContent.Create(login);
                    var response = await client.PostAsync("Usuario/Login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoginResponseModel>();
                        return RedirectToAction("Details", new { id = result.Usuario.IdUsuario });
                    }
                }
            }
            return View(login);
        }

        // GET: UsuarioApiController/Create
        public IActionResult Create()
        {
            return View(new UsuarioModel { FechaCreacion = DateTime.Now });
        }

        // POST: UsuarioApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioModel usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");

                    var saveDto = new
                    {
                        NombreCompleto = usuario.NombreCompleto,
                        Correo = usuario.Correo,
                        Clave = usuario.Clave,
                        IdRolUsuario = 1, // Rol por defecto
                        ChangeUser = 1 // Usuario por defecto
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(saveDto),
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync("Usuario/SaveUsuario", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Usuario creado correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al crear el usuario: {errorContent}";
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(usuario);
            }
        }


        // GET: UsuarioApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Usuario/GetUsuarioByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el usuario";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usuario);
        }


        // POST: UsuarioApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioModel usuario)
        {
            try
            {
                if (usuario.IdUsuario <= 0)
                {
                    usuario.IdUsuario = id;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");

                    if (string.IsNullOrEmpty(usuario.Clave))
                    {
                        var getResponse = await client.GetAsync($"Usuario/GetUsuarioByID/{id}");
                        if (getResponse.IsSuccessStatusCode)
                        {
                            var existingUser = await getResponse.Content.ReadFromJsonAsync<UsuarioModel>();
                            usuario.Clave = existingUser.Clave;
                        }
                        else
                        {
                            TempData["Error"] = "No se pudo recuperar la información del usuario";
                            return View(usuario);
                        }
                    }

                    var updateDto = new
                    {
                        IdUsuario = id,
                        NombreCompleto = usuario.NombreCompleto,
                        Correo = usuario.Correo,
                        Clave = usuario.Clave,
                        IdRolUsuario = 1,
                        Estado = true,
                        ChangeUser = 1,
                        ChangeDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                    };

                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(updateDto);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    System.Diagnostics.Debug.WriteLine($"Enviando actualización: {jsonContent}");

                    var response = await client.PutAsync($"Usuario/UpdateUsuario/{id}", content);
                    System.Diagnostics.Debug.WriteLine($"Respuesta: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Usuario actualizado correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al actualizar el usuario: {errorContent}";
                    return View(usuario);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Excepción: {ex.Message}");
                return View(usuario);
            }
        }


        private async Task<UsuarioModel> GetExistingUserById(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Usuario/GetUsuarioByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioModel>();
                }
                return null;
            }
        }

        // GET: UsuarioApiController/Restore/5
        public async Task<IActionResult> Restore(int id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Usuario/GetDeletedUsuarioByID/{id}");

                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                    return View(usuario);
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el usuario eliminado";
                    return RedirectToAction(nameof(Deleted));
                }
            }
        }

        // POST: UsuarioApiController/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");

                    var content = new StringContent("{}", Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"Usuario/RestoreUsuario/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Usuario restaurado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al restaurar el usuario: {errorContent}";
                        return RedirectToAction(nameof(Deleted));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al restaurar el usuario: {ex.Message}";
                return RedirectToAction(nameof(Deleted));
            }
        }

        // GET: UsuarioApiController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Usuario/GetUsuarioByID/{id}");

                if (response.IsSuccessStatusCode)
                {
                    usuario = await response.Content.ReadFromJsonAsync<UsuarioModel>();
                    return View(usuario);
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el usuario";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // POST: UsuarioApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");

                    var response = await client.DeleteAsync($"Usuario/DeleteUsuario/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Usuario eliminado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al eliminar el usuario: {errorContent}";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
