using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class MantenimientoController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;

        public MantenimientoController(
            IMaquinariaService maquinariaService)
        {
            _maquinariaService = maquinariaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int pagina = 1,
            int registrosPorPagina = 10)
        {
            var resultado = await _maquinariaService.ObtenerMantenimientos(
                    pagina,
                    registrosPorPagina);

            return View(resultado);
        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new MantenimientoCreateViewModel
            {
                UnidadesVehiculares = await _maquinariaService.ObtenerUnidadesVehicularesSelectList(),

                TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimiento(),

                Talleres = await _maquinariaService.ObtenerTalleres()
            };

            return View(vm);
        }







        [HttpGet]
        public async Task<IActionResult> Edit(int solicitudId)
        {
            var vm =
                await _maquinariaService.ObtenerMantenimientoEditar(solicitudId);

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MantenimientoEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _maquinariaService.EditarMantenimiento(dto);

            TempData["Success"] =
                "Mantenimiento actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(MantenimientoEditDto dto)
        {
            try
            {
                // 1. Guardar cambios de la pantalla
                await _maquinariaService.EditarMantenimiento(dto);

                // 2. Finalizar
                await _maquinariaService
                    .FinalizarMantenimiento(dto.MantenimientoId);

                TempData["Success"] =
                    "El mantenimiento fue finalizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message.Replace("WARNING|", "");

                return RedirectToAction(
                    nameof(Edit),
                    new
                    {
                        solicitudId = dto.SolicitudMantenimientoId
                    });
            }
        }




    }

}

