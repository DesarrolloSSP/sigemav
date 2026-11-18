using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{

    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]


    public class MunicipioController : Controller

    {

        private readonly IMaquinariaService _service;


        public MunicipioController(IMaquinariaService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> Municipios()
        {
            try
            {
                var municipios = await _service.ObtenerMunicipiosAsync();
                var vm = new MunicipioIndexViewModel { Municipios = municipios };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de municipios: " + ex.Message;
                return View(new MunicipioIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MunicipioCrear(MunicipioDto dto)
        {
            try
            {
                await _service.CrearMunicipioAsync(dto);
                TempData["Success"] = "Municipio registrado correctamente.";
                return RedirectToAction(nameof(Municipios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el municipio: " + ex.Message;
                return RedirectToAction(nameof(Municipios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MunicipioEditar(MunicipioDto dto)
        {
            try
            {
                if (dto.MunicipioId <= 0)
                {
                    TempData["Error"] = "Identificador de municipio no válido.";
                    return RedirectToAction(nameof(Municipios));
                }

                await _service.ActualizarMunicipioAsync(dto);
                TempData["Success"] = "Municipio actualizado correctamente.";
                return RedirectToAction(nameof(Municipios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el municipio: " + ex.Message;
                return RedirectToAction(nameof(Municipios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarMunicipio(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de municipio no válido.";
                    return RedirectToAction(nameof(Municipios));
                }

                await _service.EliminarMunicipioAsync(id);
                TempData["Success"] = "Municipio eliminado correctamente.";
                return RedirectToAction(nameof(Municipios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Municipios));
            }
        }
    }
}
