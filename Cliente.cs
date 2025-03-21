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
            if (result.Success == true)
            {
                List<ClienteDto> clienteList = (List<ClienteDto>)result.Data;
                return View(clienteList);
            }
            return View();
        }

        // GET: ClienteAdmController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await clientesService.GetById(id);
            if (result.Success == true)
            {
                var clienteEntity = (ClienteDto)result.Data;
                return View(clienteEntity);
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
        public async Task<ActionResult> Create(SaveClienteDto saveClienteDto)
        {
            try
            {
                var result = await this.clientesService.Save(saveClienteDto);
                if (result.Success == true)
                    return RedirectToAction(nameof(Index));
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: ClienteAdmController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await clientesService.GetById(id);
            if (result.Success == true)
            {
                var clienteEntity = (ClienteDto)result.Data;
                return View(clienteEntity);
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
                var result = await this.clientesService.Update(updateClienteDto);
                if (result.Success == true)
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
