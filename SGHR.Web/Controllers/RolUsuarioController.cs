using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Dtos.RolUsuario;
using SGHR.Application.Dtos.Tarifas;
using SGHR.Application.Interfaces;
using SGHR.Application.Services;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Web.Controllers
{
    public class RolUsuarioController : Controller
    {
        private readonly IRolUsuarioService rolUsuarioService;

        public RolUsuarioController(IRolUsuarioService rolUsuarioService)
        {
            this.rolUsuarioService = rolUsuarioService;
        }

        // GET: RolUsuarioController
        public async Task<IActionResult> Index()
        {
            var result = await rolUsuarioService.GetAll();
            if (result.Success == true)
            {
                List<RolUsuarioDto> rolUsuarios = (List<RolUsuarioDto>)result.Data;
                return View(rolUsuarios);
            }
            return View();
        }

        // GET: RolUsuarioController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await rolUsuarioService.GetById(id);

            if (result.Success == true)
            {
                var rolUsuariosEntity = (RolUsuario)result.Data;
                RolUsuarioDto rolUsuarioDto = RolUsuarioMapper.ToDto(rolUsuariosEntity);

                return View(rolUsuarioDto);
            }

            return View();
        }

        // GET: RolUsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolUsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SaveRolUsuarioDto saveRolUsuarioDto)
        {
            try
            {
                var result = await this.rolUsuarioService.Save(saveRolUsuarioDto);

                if (result.Success == true)
                    return RedirectToAction(nameof(Index));

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: RolUsuarioController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await rolUsuarioService.GetById(id);

            if (result.Success == true)
            {
                var rolUsuarioEntity = (RolUsuario)result.Data;
                RolUsuarioDto rolUsuarioDto = RolUsuarioMapper.ToDto(rolUsuarioEntity);

                return View(rolUsuarioDto);
            }

            return View();
        }

        // POST: RolUsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateRolUsuarioDto updateRolUsuarioDto)
        {
            try
            {
                var result = await this.rolUsuarioService.Update(updateRolUsuarioDto);
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
