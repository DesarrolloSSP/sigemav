using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{

    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]
    


    public class ClaveAdministrativaController : Controller
    {
        private readonly IClaveAdministrativaService _service;

        public ClaveAdministrativaController(IClaveAdministrativaService service)
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
        public IActionResult Crear()
        {
            var model = new ClaveAdministrativaCrudVM();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ClaveAdministrativaCrudVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool guardado = await _service.Guardar(model);


            if (!guardado)
            {
                ModelState.AddModelError("ClaveAdmin",
                    "La clave administrativa ya existe.");

                return View(model);
            }

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

            return View(model);
        }


        // ============================
        // POST EDITAR
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ClaveAdministrativaCrudVM model)
        {
            if (!ModelState.IsValid)
            {
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

            TempData["Mensaje"] = "Registro desactivado correctamente";

            return RedirectToAction(nameof(Index));
        }


        // ============================
        // ACTIVAR
        // ============================
        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            await _service.Activar(id);

            TempData["Mensaje"] = "Registro activado correctamente";

            return RedirectToAction(nameof(Index));
        }


    }
}

