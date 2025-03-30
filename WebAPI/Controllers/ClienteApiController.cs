using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebAPI.Models;
using WebAPI.Models.Cliente;

namespace WebAPI.Controllers
{
    public class ClienteApiController : Controller
    {
        // GET: ClienteApiController
        public async Task<IActionResult> Index()
        {
            List<ClienteModel> clientes = new List<ClienteModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Cliente/GetClientes");

                if (response.IsSuccessStatusCode)
                {
                    clientes = await response.Content.ReadFromJsonAsync<List<ClienteModel>>();
                }
            }
            return View(clientes);
        }

        // GET: ClienteApiController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ClienteModel cliente = new ClienteModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Cliente/GetClienteByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                }
            }
            return View(cliente);
        }

        // GET: ClienteApiController/Deleted
        public async Task<IActionResult> Deleted()
        {
            List<ClienteModel> clientes = new List<ClienteModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync("Cliente/GetDeletedClientes");

                if (response.IsSuccessStatusCode)
                {
                    clientes = await response.Content.ReadFromJsonAsync<List<ClienteModel>>();
                }
            }
            return View(clientes);
        }

        // GET: ClienteApiController/DeletedDetails/5
        public async Task<IActionResult> DeletedDetails(int id)
        {
            ClienteModel cliente = new ClienteModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Cliente/GetDeletedClienteByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                }
            }
            return View(cliente);
        }

        // GET: ClienteApiController/Create
        public IActionResult Create()
        {
            return View(new ClienteModel { Estado = true });
        }

        // POST: ClienteApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteModel cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(cliente);
                }

                // Configurar valores predeterminados
                cliente.FechaCreacion = DateTime.Now;
                cliente.ModifyUser = 1;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var saveDto = new SaveClienteDto
                    {
                        TipoDocumento = cliente.TipoDocumento,
                        Documento = cliente.Documento,
                        NombreCompleto = cliente.NombreCompleto,
                        Correo = cliente.Correo,
                        Clave = cliente.Clave,
                        Telefono = cliente.Telefono,
                        Nacionalidad = cliente.Nacionalidad,
                        ChangeUser = cliente.ModifyUser
                    };

                    var response = await client.PostAsJsonAsync("Cliente/SaveCliente", saveDto);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cliente creado correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al crear el cliente: {errorContent}";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(cliente);
            }
        }

        // GET: ClienteApiController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            ClienteModel cliente = new ClienteModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Cliente/GetClienteByID/{id}");
                if (response.IsSuccessStatusCode)
                {
                    cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el cliente";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(cliente);
        }

        // POST: ClienteApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteModel cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(cliente);
                }

                if (cliente.IdCliente != id && cliente.IdCliente > 0)
                {
                    id = cliente.IdCliente;
                }
                else if (cliente.IdCliente <= 0)
                {
                    cliente.IdCliente = id;
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");

                    var checkResponse = await client.GetAsync($"Cliente/GetClienteByID/{id}");
                    if (!checkResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "El cliente no existe o ya fue eliminado";
                        return RedirectToAction(nameof(Index));
                    }

                    var existingCliente = await checkResponse.Content.ReadFromJsonAsync<ClienteModel>();

                    var jsonObject = new
                    {
                        IdCliente = id,
                        TipoDocumento = cliente.TipoDocumento,
                        Documento = cliente.Documento,
                        NombreCompleto = cliente.NombreCompleto,
                        Correo = cliente.Correo,
                        Clave = cliente.Clave ?? existingCliente.Clave,
                        Telefono = cliente.Telefono,
                        Nacionalidad = cliente.Nacionalidad,
                        Estado = cliente.Estado,
                        ChangeUser = 1,
                        ChangeDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                    };

                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(jsonObject);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PutAsync($"Cliente/UpdateCliente/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cliente actualizado correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Error al actualizar el cliente: {errorContent}";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error inesperado: {ex.Message}";
                return View(cliente);
            }
        }


        // GET: ClienteApiController/Restore/5
        public async Task<IActionResult> Restore(int id)
        {
            ClienteModel cliente = new ClienteModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");
                var response = await client.GetAsync($"Cliente/GetDeletedClienteByID/{id}");

                if (response.IsSuccessStatusCode)
                {
                    cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                    return View(cliente);
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el cliente eliminado";
                    return RedirectToAction(nameof(Deleted));
                }
            }
        }

        // POST: ClienteApiController/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var response = await client.PutAsync($"Cliente/RestoreCliente/{id}", null);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cliente restaurado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al restaurar el cliente: {errorContent}";
                        return RedirectToAction(nameof(Deleted));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al restaurar el cliente: {ex.Message}";
                return RedirectToAction(nameof(Deleted));
            }
        }

        // GET: ClienteController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            ClienteModel cliente = new ClienteModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5187/api/");

                var response = await client.GetAsync($"Cliente/GetClienteByID/{id}");

                if (response.IsSuccessStatusCode)
                {
                    cliente = await response.Content.ReadFromJsonAsync<ClienteModel>();
                    return View(cliente);
                }
                else
                {
                    TempData["Error"] = "No se pudo encontrar el cliente";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:5187/api/");
                    var removeDto = new RemoveClienteDto
                    {
                        IdCliente = id,
                        ChangeUser = 1
                    };

                    var response = await client.DeleteAsync($"Cliente/DeleteCliente/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "Cliente eliminado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error al eliminar el cliente: {errorContent}";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }

}
