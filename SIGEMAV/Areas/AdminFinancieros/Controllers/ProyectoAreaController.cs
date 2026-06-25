using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{

    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]

    public class ProyectoAreaController : Controller
    {
        private readonly IProyectoAreaService _service;

        public ProyectoAreaController(
            IProyectoAreaService service)
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
            var model = new ProyectoAreaCrudVM();

            await CargarCombos(model);

            return View(model);
        }


        // ============================
        // POST CREAR
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ProyectoAreaCrudVM model)
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
            ProyectoAreaCrudVM model)
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
            ProyectoAreaCrudVM model)
        {
            // PROYECTOS
            var proyectos =
                await _service.ObtenerProyectos();

            model.Proyectos =
                proyectos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.IdProyecto.ToString(),

                        Text =
                            x.Proyecto,

                        Selected =
                            x.IdProyecto ==
                            model.IdProyecto
                    })

                .ToList();


            // AREAS
            var areas =
                await _service.ObtenerAreas();

            model.Areas =
                areas.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.IdArea.ToString(),

                        Text =
                            x.Area,

                        Selected =
                            x.IdArea ==
                            model.IdArea
                    })

                .ToList();
        }
    }
}

