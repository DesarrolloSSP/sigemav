using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Implementations.Maquinaria;
using System.Drawing;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    //nuevos cambios
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class UnidadVehicularController : Controller
    {

        private readonly MaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;

        public UnidadVehicularController(MaquinariaService maquinariaService, BdSigeMavContext context)
        {
            _maquinariaService = maquinariaService;
            _context = context;
        }

        //public async Task<IActionResult> Index(int pagina = 1)
        //{
        //    const int registrosPorPagina = 10;

        //    var vehiculos = await _maquinariaService.ObtenerVehiculos(
        //        pagina,
        //        registrosPorPagina);

        //    return View(vehiculos);
        //}



        public async Task<IActionResult> Index(
       string? busqueda,
       int? marcaId,
       int? modeloId,
       int? areaId,
       int? municipioId,
       int pagina = 1)
        {
            var modelo =await _maquinariaService.ObtenerVehiculos(pagina: pagina,busqueda: busqueda,marcaId: marcaId,modeloId: modeloId,areaId: areaId,municipioId:municipioId);

            ViewBag.Busqueda =
                busqueda;

            ViewBag.MarcaId =
                marcaId;

            ViewBag.ModeloId =
                modeloId;

            ViewBag.AreaId =
                areaId;

            ViewBag.MunicipioId =
                municipioId;

            ViewBag.Marcas =
                await _maquinariaService
                    .ObtenerMarcas();

            ViewBag.Modelos =
                marcaId.HasValue

                    ? await _maquinariaService
                        .ObtenerModelosPorMarca(
                            marcaId.Value)

                    : new List<SelectListItem>();

            ViewBag.Areas =
                await _maquinariaService
                    .ObtenerAreas();

            ViewBag.Municipios =
                await _maquinariaService
                    .ObtenerMunicipios();

            return View(modelo);
        }





        //[HttpGet]
        //public async Task<JsonResult> ObtenerModelosPorMarca(int marcaId)
        //{
        //    var modelos =
        //        await _maquinariaService
        //            .ObtenerModelosPorMarca(
        //                marcaId);

        //    return Json(modelos);
        //}





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new UnidadVehicularCreateViewModel
            {
            };

            await CargarCombos(vm);

            return View(vm);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnidadVehicularCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(vm);

                return View(vm);
            }

            var dto = new UnidadVehicularCreateDto
            {
                NumeroSerie = vm.NumeroSerie,
                PlacaActual = vm.PlacaActual,
                NumeroEconomico = vm.NumeroEconomico,

                MarcaId = (int)vm.IdMarca,
                ModeloId = (int)vm.IdModelo,
                AnioId = (int)vm.IdAnio,
                ColorId = (int)vm.IdColor,
                MunicipioId = (int)vm.IdMunicipio,
                AreaId = (int)vm.IdArea,
                TransmisionId = (int)vm.IdTransmision,
                CilindroId = (int)vm.IdCilindro
            };

            var resultado =
                await _maquinariaService
                    .CrearVehiculo(dto);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCombos(vm);

                return View(vm);
            }

            TempData["Success"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }







        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vehiculo = await _maquinariaService.ObtenerVehiculoEditar(id);

            if (vehiculo == null)
            {
                return NotFound();
            }

            //CargarCombos(vehiculo).Wait();
            await CargarCombos(vehiculo);


            return View(vehiculo);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnidadVehicularCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(vm);

                return View(vm);
            }

            var dto = new UnidadVehicularEditDto
            {
                IdUnidadVehicular =
                    (int)vm.IdUnidadVehicular,

                NumeroSerie = vm.NumeroSerie,
                PlacaActual = vm.PlacaActual,
                NumeroEconomico = vm.NumeroEconomico,

                IdMarca = (int)vm.IdMarca,
                IdModelo = (int)vm.IdModelo,
                IdAnio = (int)vm.IdAnio,
                IdColor = (int)vm.IdColor,
                IdMunicipio = (int)vm.IdMunicipio,
                IdArea = (int)vm.IdArea,
                IdTransmision = (int)vm.IdTransmision,
                IdCilindro = (int)vm.IdCilindro
            };

            var resultado =
                await _maquinariaService
                    .EditarVehiculo(dto);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCombos(vm);

                return View(vm);
            }

            TempData["Success"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }





        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerUnidadVehicularDetails(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }






        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _maquinariaService.ObtenerEliminarAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UnidadVehicularEliminarViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new UnidadVehicularEliminarDto
            {
                UnidadVehicularId = model.UnidadVehicularId
            };

            var eliminado = await _maquinariaService.EliminarAsync(dto);

            if (!eliminado)
            {
                TempData["Error"] =
                    "No fue posible eliminar la unidad vehicular.";

                return View(model);
            }

            TempData["Success"] =
                "Unidad vehicular eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }






        private async Task CargarCombos(UnidadVehicularCreateViewModel model)
        {
            model.Marcas = await _maquinariaService.ObtenerMarcas();

            model.Modelos = model.IdMarca.HasValue
                ? await _maquinariaService.ObtenerModelosPorMarca(
                    model.IdMarca.Value)
                : new List<SelectListItem>();

            model.Anios = await _maquinariaService.ObtenerAnios();

            model.Colores = await _maquinariaService.ObtenerColores();

            model.Municipios = await _maquinariaService.ObtenerMunicipios();

            model.Areas = await _maquinariaService.ObtenerAreas();

            model.Transmisiones = await _maquinariaService.ObtenerTransmision();

            model.Cilindros = await _maquinariaService.ObtenerCilindro();
        }










        [HttpGet]
        public async Task<JsonResult> ObtenerModelosPorMarca(int marcaId)
        {
            var modelos =
                await _maquinariaService
                    .ObtenerModelosPorMarca(marcaId);

            return Json(
                modelos.Select(x => new
                {
                    value = x.Value,
                    text = x.Text
                }));
        }









    }


}

