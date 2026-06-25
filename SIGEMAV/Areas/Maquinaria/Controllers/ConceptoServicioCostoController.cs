using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class ConceptoServicioCostoController : Controller
    {

        private readonly IMaquinariaService _maquinariaService;

        public ConceptoServicioCostoController(IMaquinariaService maquinariaService)
        {

            _maquinariaService = maquinariaService;
        }


        public async Task<IActionResult> Index(int pagina = 1)
        {
            var vm = await _maquinariaService.ObtenerIndexConceptoServicioCostoAsync(pagina);

            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ConceptoServicioCostoCreateViewModel
                {
                    ConceptosServicio =
                        await _maquinariaService
                            .ObtenerConceptosServicioSelect(),

                    Marcas =
                        await _maquinariaService.ObtenerMarcas(),

                    Modelos =
                        new List<SelectListItem>(),

                    Anios =
                        await _maquinariaService.ObtenerAnios(),

                    Transmisiones =
                        await _maquinariaService.ObtenerTransmision(),

                    Cilindros =
                        await _maquinariaService.ObtenerCilindro()
                };

            return View(vm);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConceptoServicioCostoCreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarCombosConceptoServicioCosto(vm);

                    return View(vm);
                }

                // VALIDAR FECHA INICIO

                if (vm.FechaInicioVigencia == null)
                {
                    TempData["Error"] =
                        "Debe seleccionar la fecha de inicio de vigencia.";

                    await CargarCombosConceptoServicioCosto(vm);

                    return View(vm);
                }

                // VALIDAR FECHAS

                if (vm.FechaFinVigencia.HasValue && vm.FechaFinVigencia < vm.FechaInicioVigencia)

                {
                    TempData["Error"] =
                        "La fecha fin de vigencia no puede ser menor a la fecha inicio.";

                    await CargarCombosConceptoServicioCosto(vm);

                    return View(vm);
                }

                var dto =
                    new ConceptoServicioCostoCreateDto
                    {
                        ConceptoServicioId =
                            vm.ConceptoServicioId,

                        MarcaId =
                            vm.MarcaId,

                        ModeloId =
                            vm.ModeloId,

                        AnioId =
                            vm.AnioId,

                        TransmisionId =
                            vm.TransmisionId,

                        CilindroId =
                            vm.CilindroId,

                        CostoManoObra =
                            vm.CostoManoObra,

                        CostoRefacciones =
                            vm.CostoRefacciones,

                        FechaInicioVigencia = vm.FechaInicioVigencia,

                        FechaFinVigencia = vm.FechaFinVigencia,

                        Observaciones =
                            vm.Observaciones
                    };

                var resultado =
                    await _maquinariaService
                        .CrearConceptoServicioCosto(dto);

                if (!resultado.Exitoso)
                {
                    TempData["Error"] =
                        resultado.Mensaje;

                    await CargarCombosConceptoServicioCosto(vm);

                    return View(vm);
                }

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                await CargarCombosConceptoServicioCosto(vm);

                return View(vm);
            }
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerConceptoServicioCostoEdit(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ConceptoServicioCostoEditViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarCombos(vm);

                    return View(vm);
                }

                // VALIDAR FECHAS

                if (
                    vm.FechaFinVigencia.HasValue
                    &&
                    vm.FechaFinVigencia.Value
                        < vm.FechaInicioVigencia
                )
                {
                    TempData["Error"] =
                        "La fecha fin de vigencia no puede ser menor a la fecha inicio.";

                    await CargarCombos(vm);

                    return View(vm);
                }

                var dto =
                    new ConceptoServicioCostoEditDto
                    {
                        ConceptoServicioCostoId =
                            vm.ConceptoServicioCostoId,

                        ConceptoServicioId =
                            vm.ConceptoServicioId,

                        MarcaId =
                            vm.MarcaId,

                        ModeloId =
                            vm.ModeloId,

                        AnioId =
                            vm.AnioId,

                        FechaInicioVigencia =
                            vm.FechaInicioVigencia,

                        FechaFinVigencia =
                            vm.FechaFinVigencia,

                        TransmisionId =
                            vm.TransmisionId,

                        CilindroId =
                            vm.CilindroId,

                        CostoManoObra =
                            vm.CostoManoObra,

                        CostoRefacciones =
                            vm.CostoRefacciones,

                        Observaciones =
                            vm.Observaciones
                    };

                await _maquinariaService
                    .EditarConceptoServicioCosto(dto);

                TempData["Success"] =
                    "Costo de servicio actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await CargarCombos(vm);

                TempData["Error"] =
                    ex.Message;

                return View(vm);
            }
        }









        private async Task CargarCombosConceptoServicioCosto(ConceptoServicioCostoCreateViewModel vm)
        {
            vm.ConceptosServicio =await _maquinariaService.ObtenerConceptosServicioSelect();

            vm.Marcas =
                await _maquinariaService.ObtenerMarcas();

            vm.Modelos =
                vm.MarcaId > 0
                    ? await _maquinariaService
                        .ObtenerModelosPorMarca(vm.MarcaId)
                    : new List<SelectListItem>();

            vm.Anios =
                await _maquinariaService.ObtenerAnios();

            vm.Transmisiones =
                await _maquinariaService.ObtenerTransmision();

            vm.Cilindros =
                await _maquinariaService.ObtenerCilindro();
        }




        [HttpGet]
        public async Task<JsonResult> ObtenerModelosPorMarca(int marcaId)
        {
            var modelos = await _maquinariaService.ObtenerModelosPorMarca(marcaId);

            return Json(modelos);
        }



        private async Task CargarCombos(ConceptoServicioCostoEditViewModel vm)
        {
            if (vm.TipoCatalogoServicioId > 0)
            {
                vm.ConceptosServicio =
                    await _maquinariaService.ObtenerConceptosServicio(
                        vm.TipoCatalogoServicioId);
            }
            else
            {
                vm.ConceptosServicio = new List<SelectListItem>();
            }

            vm.Marcas =
                await _maquinariaService
                    .ObtenerMarcas();

            vm.Modelos =
                vm.MarcaId > 0
                    ? await _maquinariaService
                        .ObtenerModelosPorMarca(vm.MarcaId)
                    : new List<SelectListItem>();

            vm.Transmisiones =
                await _maquinariaService
                    .ObtenerTransmision();

            vm.Cilindros =
                await _maquinariaService
                    .ObtenerCilindro();

            vm.Anios =
                await _maquinariaService
                    .ObtenerAnios();
        }






        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerConceptoServicioCostoDelete(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ConceptoServicioCostoDeleteViewModel vm)
        {
            try
            {
                await _maquinariaService
                    .EliminarConceptoServicioCosto(
                        vm.ConceptoServicioCostoId);

                TempData["Success"] =
                    "Costo de servicio eliminado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
             


    }
}
