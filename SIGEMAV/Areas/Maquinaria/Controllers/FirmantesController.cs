using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")] 
    public class FirmantesController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;
        private readonly ILogger<SolicitudMantenimientoController> _logger;

        public FirmantesController(IMaquinariaService maquinariaService, BdSigeMavContext context, ILogger<SolicitudMantenimientoController> logger)
        {
            _maquinariaService = maquinariaService;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? areaFiltroId)
        {
            var firmantes = await _maquinariaService.ObtenerTodosAsync(areaFiltroId);
            var areas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.AreaNombre)
                .AsNoTracking()
                .ToListAsync();

            var viewModel = new FirmanteIndexViewModel
            {
                Firmantes = firmantes,
                AreaFiltroId = areaFiltroId,
                Areas = new SelectList(areas, "AreaId", "AreaNombre", areaFiltroId)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var viewModel = new FirmanteFormViewModel
            {
                Firmante = new FirmanteDto { Activo = true },
                Areas = await CargarAreasSelectList(),
                ClavesPuesto = CargarClavesPuestoSelectList()
            };

            return View("Formulario", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var dto = await _maquinariaService.ObtenerPorIdAsync(id);
            if (dto == null)
            {
                TempData["Error"] = "No se encontró el firmante especificado.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new FirmanteFormViewModel
            {
                Firmante = dto,
                Areas = await CargarAreasSelectList(dto.AreaId),
                ClavesPuesto = CargarClavesPuestoSelectList(dto.ClavePuesto)
            };

            return View("Formulario", viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(FirmanteFormViewModel model)
        {
            // Limpiar claves del SelectList del ModelState por si acaso
            ModelState.Remove("Areas");
            ModelState.Remove("ClavesPuesto");
            ModelState.Remove("Firmante.AreaNombre");

            if (!ModelState.IsValid)
            {
                // Recargar dropdowns si la validación de Nombre, Cargo o ClavePuesto falla
                model.Areas = await CargarAreasSelectList(model.Firmante.AreaId);
                model.ClavesPuesto = CargarClavesPuestoSelectList(model.Firmante.ClavePuesto);
                return View("Formulario", model);
            }

            var resultado = await _maquinariaService.GuardarFirmanteAsync(model.Firmante);
            if (resultado)
            {
                TempData["Success"] = "La información del firmante se guardó correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Ocurrió un error al intentar guardar el firmante.";
            model.Areas = await CargarAreasSelectList(model.Firmante.AreaId);
            model.ClavesPuesto = CargarClavesPuestoSelectList(model.Firmante.ClavePuesto);
            return View("Formulario", model);
        }


        [HttpPost]
        public async Task<IActionResult> CambiarEstatus(int id)
        {
            var resultado = await _maquinariaService.CambiarEstatusAsync(id);
            if (resultado)
                return Json(new { success = true, message = "Estatus actualizado correctamente." });

            return Json(new { success = false, message = "No se pudo cambiar el estatus." });
        }

        #region Métodos Auxiliares
        private async Task<SelectList> CargarAreasSelectList(int? seleccionado = null)
        {
            var listadoAreas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.AreaNombre)
                .Select(a => new { a.AreaId, a.AreaNombre })
                .AsNoTracking()
                .ToListAsync();

            return new SelectList(listadoAreas, "AreaId", "AreaNombre", seleccionado);
        }

        private SelectList CargarClavesPuestoSelectList(string? seleccionado = null)
        {
            var puestos = new List<SelectListItem>
            {
                new SelectListItem { Value = "SOLICITANTE", Text = "SOLICITANTE DEL ÁREA USUARIA" },
                new SelectListItem { Value = "SUPERVISOR", Text = "SUPERVISA, VERIFICA Y VALIDA REPARACIÓN" },
                new SelectListItem { Value = "SUBDIRECTOR_MAQUINARIA", Text = "REVISIÓN DE SOLICITUD (SUBDIRECTOR DE MAQUINARIA)" },
                new SelectListItem { Value = "DIRECTOR_RECURSOS", Text = "Vo. Bo. (DIRECTOR DE RECURSOS MATERIALES)" },
                new SelectListItem { Value = "AUTORIZA", Text = "AUTORIZA (JEFE DE ÁREA / TITULAR)" }
            };

            return new SelectList(puestos, "Value", "Text", seleccionado);
        }
        #endregion
    }
}
