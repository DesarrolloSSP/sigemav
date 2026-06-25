using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace TuProyecto.Web.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class TipoCatalogoServicioController: Controller
    {
        private readonly IMaquinariaService _maquinariaService;

        public TipoCatalogoServicioController(IMaquinariaService maquinariaService)
        {
            _maquinariaService = maquinariaService;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            var model = await _maquinariaService.ObtenerIndexTipoCatalogoServicioAsync(pagina);

            return View(model);
        }




        [HttpGet]
        public IActionResult Create()
        {
            return View(new TipoCatalogoServicioCreateViewModel());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoCatalogoServicioCreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                var dto =
                    new TipoCatalogoServicioCreateDto
                    {
                        TipoCatalogoServicioNombre =
                            vm.TipoCatalogoServicioNombre
                    };

                await _maquinariaService.CrearTipoCatalogoServicioAsync(dto);

                TempData["Success"] =
                    "Tipo de catalogo registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(vm);
            }
        }



        

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerTipoCatalogoServicioEditarAsync(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TipoCatalogoServicioEditViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                var dto =
                    new TipoCatalogoServicioEditDto
                    {
                        TipoCatalogoServicioId =
                            vm.TipoCatalogoServicioId,

                        TipoCatalogoServicioNombre =
                            vm.TipoCatalogoServicioNombre
                    };

                await _maquinariaService.EditarTipoCatalogoServicioAsync(dto);

                TempData["Success"] =
                    "Tipo de catalogo actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return View(vm);
            }
        }





        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerTipoCatalogoEliminarAsync(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TipoCatalogoServicioDeleteViewModel vm)
        {
            try
            {
                await _maquinariaService.EliminarTipoCatalogoAsync(vm.TipoCatalogoServicioId);

                TempData["Success"] =
                    "Tipo de catalogo eliminado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }



    }
}