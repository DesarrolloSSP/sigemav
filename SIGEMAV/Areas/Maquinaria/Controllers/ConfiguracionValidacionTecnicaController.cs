using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;


namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class ConfiguracionValidacionTecnicaController : Controller
    {
        public readonly IMaquinariaService _maquinariaService;
        public readonly BdSigeMavContext _context;

        public ConfiguracionValidacionTecnicaController(IMaquinariaService service, BdSigeMavContext context)
        {
            _maquinariaService = service;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _maquinariaService.ObtenerConfiguracionesValidacion();

            var vm = new ConfiguracionValidacionTecnicaIndexViewModel
            {
                Items = data.Select(x => new ConfiguracionValidacionTecnicaIndexViewModel.Item
                {
                    ConfiguracionValidacionTecnicaId = x.ConfiguracionValidacionTecnicaId,
                    TipoCatalogoServicioNombre = x.TipoCatalogoServicioNombre,
                    ConceptoServicioNombre = x.ConceptoServicioNombre,
                    ValidarSolicitudesAbiertas = x.ValidarSolicitudesAbiertas,
                    ValidarRecurrencia = x.ValidarRecurrencia,
                    DiasRecurrencia = x.DiasRecurrencia,
                    EstatusSolicitudMantenimientoNombre = x.EstatusSolicitudMantenimientoNombre,
                    Activo = x.Activo
                }).ToList()
            };

            return View(vm);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ConfiguracionValidacionTecnicaCreateViewModel
            {
                TiposCatalogoServicio =
                    await _maquinariaService.ObtenerTiposCatalogoServicio(),

                EstatusDisponibles =
                    await _maquinariaService.ObtenerEstatusSolicitudMantenimiento(),

                ConceptosServicio = new List<SelectListItem>()
            };

            return View(vm);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguracionValidacionTecnicaCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TiposCatalogoServicio = await _maquinariaService.ObtenerTiposCatalogoServicio();
                vm.EstatusDisponibles = await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();
                vm.ConceptosServicio = new List<SelectListItem>();

                return View(vm);
            }

            var dto = new ConfiguracionValidacionTecnicaCreateDto
            {
                TipoCatalogoServicioId = vm.TipoCatalogoServicioId ?? 0,
                ConceptoServicioId = vm.ConceptoServicioId ?? 0,
                ValidarSolicitudesAbiertas = vm.ValidarSolicitudesAbiertas,
                EstatusPermitidos = vm.EstatusPermitidos,
                ValidarRecurrencia = vm.ValidarRecurrencia,
                DiasRecurrencia = vm.DiasRecurrencia,
                Activo = vm.Activo
            };

            var resultado = await _maquinariaService.CrearConfiguracion(dto);

            if (!resultado.Exitoso)
            {
                TempData["SweetAlertMessage"] = resultado.Mensaje;
                TempData["SweetAlertType"] = "error";

                vm.TiposCatalogoServicio = await _maquinariaService.ObtenerTiposCatalogoServicio();
                vm.EstatusDisponibles = await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();
                vm.ConceptosServicio = new List<SelectListItem>();

                return View(vm);
            }

            TempData["SweetAlertMessage"] = resultado.Mensaje;
            TempData["SweetAlertType"] = "success";

            return RedirectToAction(nameof(Index));
        }






        public async Task<IActionResult> Edit(int? id)
        {
            var vm = new ConfiguracionValidacionTecnicaViewModel();

            vm.TiposCatalogoServicio =
                await _maquinariaService.ObtenerTiposCatalogoServicio();

            vm.EstatusDisponibles =
                await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();

            vm.EstatusPermitidos = new List<int>();

            if (!id.HasValue)
            {
                vm.ConceptosServicio =
                    new List<SelectListItem>();

                return View(vm);
            }

            var entidad =
                await _context.ConfiguracionValidacionTecnicas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ConfiguracionValidacionTecnicaId == id.Value);

            if (entidad == null)
                return NotFound();

            vm.ConfiguracionValidacionTecnicaId = entidad.ConfiguracionValidacionTecnicaId;
            vm.TipoCatalogoServicioId = entidad.TipoCatalogoServicioId;
            vm.ConceptoServicioId = entidad.ConceptoServicioId;

            vm.ValidarSolicitudesAbiertas = entidad.ValidarSolicitudesAbiertas;
            vm.ValidarRecurrencia = entidad.ValidarRecurrencia;
            vm.DiasRecurrencia = entidad.DiasRecurrencia;
            vm.Activo = entidad.Activo;

            // IMPORTANTE: cargar seleccionados
            vm.EstatusPermitidos =
                await _context.ConfiguracionValidacionEstatuses
                    .AsNoTracking()
                    .Where(x =>
                        x.ConfiguracionValidacionTecnicaId == id.Value
                        && x.Activo)
                    .Select(x => x.EstatusSolicitudMantenimientoId)
                    .ToListAsync();

            vm.ConceptosServicio =
                await _maquinariaService
                    .ObtenerConceptosPorTipo((int)entidad.TipoCatalogoServicioId);

            return View(vm);
        }






        [HttpPost]
        public async Task<IActionResult> Guardar(ConfiguracionValidacionTecnicaViewModel vm)
        {
            var resultado =
                await _maquinariaService
                    .GuardarConfiguracion(vm);

            if (!resultado.Exitoso)
            {
                TempData["SweetAlertMessage"] =
                    resultado.Mensaje;

                TempData["SweetAlertType"] =
                    "error";

                //----------------------------------
                // RECONSTRUIR VIEWMODEL
                //----------------------------------

                vm.TiposCatalogoServicio =
                    await _maquinariaService
                        .ObtenerTiposCatalogoServicio();

                vm.EstatusDisponibles =
                    await _maquinariaService
                        .ObtenerEstatusSolicitudMantenimiento();

                vm.EstatusPermitidos
                    ??= new List<int>();

                vm.ConceptosServicio =
                    vm.TipoCatalogoServicioId > 0
                        ? await _maquinariaService
                            .ObtenerConceptosPorTipo(
                                vm.TipoCatalogoServicioId.Value)

                        : new List<SelectListItem>();

                return View(
                    "Edit",
                    vm);
            }

            TempData["SweetAlertMessage"] =
                resultado.Mensaje;

            TempData["SweetAlertType"] =
                "success";

            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerConceptosPorTipo(int tipoCatalogoServicioId)
        {
            var data =
                await _maquinariaService.ObtenerConceptosPorTipo(tipoCatalogoServicioId);

            return new JsonResult(data);
        }



    }
}
