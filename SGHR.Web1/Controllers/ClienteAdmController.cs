using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.Cliente;
using SGHR.Application.Intefaces;

namespace SGHR.Web.Controllers
{
    public class ClienteAdmController : Controller
    {
        private readonly IClientesService clientesService;

        public ClienteAdmController(IClientesService clientesService)
        {
            this.clientesService = clientesService;
        }

        // GET: ClienteAdmController
        public async Task<IActionResult> Index()
        {
            var result = await clientesService.GetAll();
            if (result.Success)
            {
                List<ClienteDto> clienteList = (List<ClienteDto>)result.Data;
                return View(clienteList);
            }
            return View();
        }

        // GET: ClienteAdmController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await clientesService.GetById(id);
            if (result.Success)
            {
                ClienteDto cliente = (ClienteDto)result.Data;
                return View(cliente);
            }
            return View();
        }

        // GET: ClienteAdmController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClienteAdmController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaveClienteDto saveClienteDto)
        {
            try
            {
                var result = await clientesService.Save(saveClienteDto);
                if (result.Success)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteAdmController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await clientesService.GetById(id);
            if (result.Success)
            {
                ClienteDto cliente = (ClienteDto)result.Data;
                return View(cliente);
            }
            return View();
        }

        // POST: ClienteAdmController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateClienteDto updateClienteDto)
        {
            try
            {
                var result = await clientesService.Update(updateClienteDto);
                if (result.Success)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteAdmController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = await clientesService.GetById(id);
            if (result.Success)
            {
                ClienteDto cliente = (ClienteDto)result.Data;
                return View(cliente);
            }
            return View();
        }

        // POST: ClienteAdmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RemoveClienteDto removeClienteDto)
        {
            try
            {
                var result = await clientesService.Remove(removeClienteDto);
                if (result.Success)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
