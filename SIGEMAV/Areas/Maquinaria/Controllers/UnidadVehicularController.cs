using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Implementations.Maquinaria;
using System.Drawing;
using System.Security.Claims;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    //nuevos cambios
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin,AreaOperativa")]
    public class UnidadVehicularController : Controller
    {

        private readonly MaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnidadVehicularController(MaquinariaService maquinariaService, BdSigeMavContext context, IHttpContextAccessor httpContextAccessor)
        {
            _maquinariaService = maquinariaService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<IActionResult> Index(int pagina = 1)
        //{
        //    const int registrosPorPagina = 10;

        //    var vehiculos = await _maquinariaService.ObtenerVehiculos(
        //        pagina,
        //        registrosPorPagina);

        //    return View(vehiculos);
        //}



        public async Task<IActionResult> Index(string? busqueda,int? marcaId,int? modeloId,int? anioDesde,int? anioHasta,int? areaId,int? municipioId,int pagina = 1)
        {
            //=====================================================
            // VALIDACIÓN DEL RANGO DE AÑOS
            //=====================================================

            if (anioDesde.HasValue &&
                anioHasta.HasValue &&
                anioDesde > anioHasta)
            {
                TempData["Error"] =
                    "El año inicial no puede ser mayor al año final.";

                // intercambiamos los valores para no romper la búsqueda
                (anioDesde, anioHasta) = (anioHasta, anioDesde);
            }





            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
            var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

            if (usuarioGuid.ToString() == "0bf13a21-fe5a-45f9-a584-1f6d338e1995")
            {
                areaId = 22;
            }

            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE









            var modelo = await _maquinariaService.ObtenerVehiculos(
                pagina: pagina,
                busqueda: busqueda,
                marcaId: marcaId,
                modeloId: modeloId,
                anioDesde: anioDesde,
                anioHasta: anioHasta,
                areaId: areaId,
                municipioId: municipioId);

            ViewBag.Busqueda = busqueda;
            ViewBag.MarcaId = marcaId;
            ViewBag.ModeloId = modeloId;
            ViewBag.AnioDesde = anioDesde;
            ViewBag.AnioHasta = anioHasta;
            ViewBag.AreaId = areaId;
            ViewBag.MunicipioId = municipioId;

            ViewBag.Marcas =
                await _maquinariaService.ObtenerMarcas();

            ViewBag.Modelos =
                marcaId.HasValue
                    ? await _maquinariaService.ObtenerModelosPorMarca(marcaId.Value)
                    : new List<SelectListItem>();

            ViewBag.Anios =
                await _maquinariaService.ObtenerAnios();

            ViewBag.Areas =
                await _maquinariaService.ObtenerAreas();

            ViewBag.Municipios =
                await _maquinariaService.ObtenerMunicipios();

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

            var resultado = await _maquinariaService.CrearVehiculo(dto);

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

