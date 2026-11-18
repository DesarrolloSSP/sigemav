using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{

    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]

    public class AreaController : Controller
    {
        private readonly IMaquinariaService _service;

        public AreaController(IMaquinariaService service)
        {
            _service = service;
        }




        [HttpGet]
        public async Task<IActionResult> Areas()
        {
            try
            {
                var areas = await _service.ObtenerAreasAsync();
                var vm = new AreaIndexViewModel { Areas = areas };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de áreas: " + ex.Message;
                return View(new AreaIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AreaCrear(string areaNombre, bool activoFinancieros)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(areaNombre))
                {
                    TempData["Error"] = "El nombre del área es obligatorio.";
                    return RedirectToAction(nameof(Areas));
                }

                var dto = new AreaDto
                {
                    AreaNombre = areaNombre.Trim(),
                    ActivoFinancieros = activoFinancieros
                };

                await _service.CrearAreaAsync(dto);
                TempData["Success"] = "Área registrada correctamente.";
                return RedirectToAction(nameof(Areas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el área: " + ex.Message;
                return RedirectToAction(nameof(Areas));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AreaEditar(int areaId, string areaNombre, bool activo, bool activoFinancieros)
        {
            try
            {
                if (areaId <= 0 || string.IsNullOrWhiteSpace(areaNombre))
                {
                    TempData["Error"] = "Los datos del área no son válidos.";
                    return RedirectToAction(nameof(Areas));
                }

                var dto = new AreaDto
                {
                    AreaId = areaId,
                    AreaNombre = areaNombre.Trim(),
                    Activo = activo,
                    ActivoFinancieros = activoFinancieros
                };

                await _service.ActualizarAreaAsync(dto);
                TempData["Success"] = "Área actualizada correctamente.";
                return RedirectToAction(nameof(Areas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el área: " + ex.Message;
                return RedirectToAction(nameof(Areas));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarArea(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de área no válido.";
                    return RedirectToAction(nameof(Areas));
                }

                await _service.EliminarAreaAsync(id);
                TempData["Success"] = "Área desactivada correctamente.";
                return RedirectToAction(nameof(Areas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Areas));
            }
        }
    }
}
