using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{

    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]

    public class ProyectoController : Controller
    {

        private readonly IProyectoService _service;

        public ProyectoController(
            IProyectoService service)
        {
            _service = service;
        }


        // ============================
        // INDEX
        // ============================
        public async Task<IActionResult> Index()
        {
            var model = await _service.Obtener();

            return View(model);
        }


        // ============================
        // GET CREAR
        // ============================
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var model = new ProyectoCrudVM();

            await CargarCombos(model);

            return View(model);
        }


        // ============================
        // POST CREAR
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ProyectoCrudVM model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(model);

                return View(model);
            }

            await _service.Guardar(model);

            TempData["Mensaje"] =
                "Registro guardado correctamente";

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // GET EDITAR
        // ============================
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var model = await _service.ObtenerPorId(id);

            if (model == null)
            {
                return NotFound();
            }

            await CargarCombos(model);

            return View(model);
        }


        // ============================
        // POST EDITAR
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            ProyectoCrudVM model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(model);

                return View(model);
            }

            await _service.Actualizar(model);

            TempData["Mensaje"] =
                "Registro actualizado correctamente";

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // DESACTIVAR
        // ============================
        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _service.Desactivar(id);

            TempData["Mensaje"] =
                "Registro desactivado correctamente";

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // ACTIVAR
        // ============================
        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            await _service.Activar(id);

            TempData["Mensaje"] =
                "Registro activado correctamente";

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // CARGAR COMBOS
        // ============================
        private async Task CargarCombos(
            ProyectoCrudVM model)
        {
            var claves =
                await _service
                    .ObtenerClavesAdministrativas();

            model.ClavesAdministrativas =
                claves.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.IdClaveAdmin.ToString(),

                        Text =
                            x.ClaveAdmin,

                        Selected =
                            x.IdClaveAdmin ==
                            model.IdClaveAdmin
                    })

                .ToList();
        }
    }
}

