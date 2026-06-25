using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{
    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]
    public class ObjetoGastoController : Controller
    {
        private readonly IObjetoGastoCatalogoService _service;

        public ObjetoGastoController(
            IObjetoGastoCatalogoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.Obtener();

            return View(model);
        }


        public IActionResult Crear()
        {
            var model = new ObjetoGastoCrudVM();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Crear(
            ObjetoGastoCrudVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.Guardar(model);

            TempData["Mensaje"] =
                "Registro guardado correctamente";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var model = await _service.ObtenerPorId(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            ObjetoGastoCrudVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.Actualizar(model);

            TempData["Mensaje"] =
                "Registro actualizado correctamente";

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Desactivar(int id)
        {
            await _service.Desactivar(id);

            TempData["Mensaje"] =
                "Registro desactivado correctamente";

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Activar(int id)
        {
            await _service.Activar(id);

            TempData["Mensaje"] =
                "Registro activado correctamente";

            return RedirectToAction("Index");
        }
    }



}
