using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Implementations;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]


    public class ConceptoServicioController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        public ConceptoServicioController(IMaquinariaService maquinariaService)
        {
            _maquinariaService = maquinariaService;
        }
        
        public async Task<IActionResult> Index(int pagina = 1)
        {
            var vm = await _maquinariaService.ObtenerIndexConceptoServicioAsync(pagina);

            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm =
                new ConceptoServicioCreateViewModel
                {
                    TiposCatalogo =
                        await _maquinariaService
                            .ObtenerTiposCatalogoServicio()
                };

            return View(vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConceptoServicioCreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    vm.TiposCatalogo =
                        await _maquinariaService
                            .ObtenerTiposCatalogoServicio();

                    return View(vm);
                }

                var dto =
                    new ConceptoServicioCreateDto
                    {
                        TipoCatalogoServicioId =
                            (int)vm.TipoCatalogoServicioId,

                        ConceptoServicioNombre =
                            vm.ConceptoServicioNombre,

                        ConceptoServicioDescripcion =
                            vm.ConceptoServicioDescripcion
                    };

                await _maquinariaService
                    .CrearConceptoServicioAsync(dto);

                TempData["Success"] =
                    "Concepto de servicio registrado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                vm.TiposCatalogo =
                    await _maquinariaService
                        .ObtenerTiposCatalogoServicio();

                TempData["Error"] = ex.Message;

                return View(vm);
            }
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _maquinariaService.ObtenerConceptoServicioEdit(id);

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
        public async Task<IActionResult> Edit(ConceptoServicioEditViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    vm.TiposCatalogoServicio =
                        await _maquinariaService.ObtenerTiposCatalogoServicioSelect();

                    return View(vm);
                }

                var dto =
                    new ConceptoServicioEditDto
                    {
                        ConceptoServicioId =
                            vm.ConceptoServicioId,

                        TipoCatalogoServicioId =
                            vm.TipoCatalogoServicioId,

                        ConceptoServicioNombre =
                            vm.ConceptoServicioNombre,

                        ConceptoServicioDescripcion =
                            vm.ConceptoServicioDescripcion
                    };

                var resultado =
                    await _maquinariaService
                        .EditarConceptoServicio(dto);

                if (!resultado.Exitoso)
                {
                    TempData["Error"] =
                        resultado.Mensaje;

                    vm.TiposCatalogoServicio =
                        await _maquinariaService
                            .ObtenerTiposCatalogoServicioSelect();

                    return View(vm);
                }

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                vm.TiposCatalogoServicio =
                    await _maquinariaService
                        .ObtenerTiposCatalogoServicioSelect();

                return View(vm);
            }
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService
                        .ObtenerConceptoServicioDelete(id);

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
        public async Task<IActionResult> Delete(ConceptoServicioDeleteViewModel vm)
        {
            try
            {
                var resultado =
                    await _maquinariaService.EliminarConceptoServicio(vm.ConceptoServicioId);

                if (!resultado.Exitoso)
                {
                    TempData["Error"] =
                        resultado.Mensaje;

                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] =
                    resultado.Mensaje;

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
