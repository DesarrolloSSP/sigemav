using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.ComponentModel.Design;
using System.Security.Claims;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;
using System.Security;
using SIGEMAV.Areas.Maquinaria.Controllers;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Areas.Maquinaria.Funciones;
using SIGEMAV.Models.Entities;


namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin,AreaOperativa")]
    public class SolicitudMantenimientoController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;
        private readonly ILogger<SolicitudMantenimientoController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public SolicitudMantenimientoController(IMaquinariaService maquinariaService, BdSigeMavContext context, ILogger<SolicitudMantenimientoController> logger, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _maquinariaService = maquinariaService;
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }



        


        [HttpGet]
        public async Task<IActionResult> Index(SolicitudMantenimientoFiltroViewModel filtro, int pagina = 1, int registrosPorPagina = 10)
        {
            filtro ??= new SolicitudMantenimientoFiltroViewModel();
            if (filtro.FechaInicio.HasValue && filtro.FechaFin.HasValue && filtro.FechaInicio > filtro.FechaFin)
            {
                ModelState.AddModelError("", "La fecha inicial no puede ser mayor que la fecha final.");
            }


            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
            var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

            if (usuarioGuid.ToString() == "0bf13a21-fe5a-45f9-a584-1f6d338e1995")
            {
                filtro.AreaSolicitanteId = 22;
                filtro.EsUsuarioDeAreaOperativa = true;
            }

            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
            ///
            var resultado = await _maquinariaService.ObtenerSolicitudesMantenimiento(filtro, pagina, registrosPorPagina);



            


            filtro.Areas = await _maquinariaService.ObtenerAreasSelectList(filtro.AreaSolicitanteId);
            filtro.Estatus = await _maquinariaService.ObtenerEstatusSelectList(filtro.EstatusSolicitudMantenimientoId);
            filtro.TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimientoSelectList(filtro.TipoMantenimientoId);
            // conservar filtro en la paginación
            resultado.Filtro = filtro;
            return View(resultado);
        }





        [HttpGet]
        public async Task<IActionResult> BuscarUnidades(string term)
        {
            var resultado = await _maquinariaService.BusquedaRapidaUnidades(term);

            return Json(resultado);
        }



        //[HttpGet]
        //public async Task<IActionResult> Create()
        //{




        //    var vm = new SolicitudMantenimientoCreateViewModel
        //    {
        //        Areas = await _maquinariaService.ObtenerAreas(),

        //        TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimiento(),

        //        Talleres = await _maquinariaService.ObtenerTalleres(),

        //        //Estatuses = await _maquinariaService.ObtenerEstatusSolicitudMantenimiento(),

        //        FechaSolicitud = DateTime.Now,

        //        ConceptosServicio = await _maquinariaService.ObtenerConceptosServicio(),



        //    };

        //    return View(vm);
        //}

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new SolicitudMantenimientoCreateViewModel
            {
                FechaSolicitud = DateTime.Now,

                AreaSolicitanteId = null,

                Areas =
                    await _maquinariaService.ObtenerAreas(),

                Talleres =
                    await _maquinariaService.ObtenerTalleres(),

                TiposMantenimiento =
                    await _maquinariaService.ObtenerTiposMantenimiento(),

                ConceptosServicio =
                    new List<SelectListItem>(),

                ObjetosGasto =
                    await _maquinariaService.ObtenerObjetosGasto(),

                EstadosOdometro = await _maquinariaService.ObtenerEstadosOdometro()
            };


            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerAreaUnidad(int unidadVehicularId)
        {
            var unidad = await _context.UnidadVehiculars
                .Include(x => x.Area)
                .FirstOrDefaultAsync(x =>
                    x.UnidadVehicularId == unidadVehicularId);

            if (unidad == null)
            {
                return NotFound();
            }

            return Json(new
            {
                areaId = unidad.AreaId
            });
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
        [RequestSizeLimit(15 * 1024 * 1024)]
        public async Task<IActionResult> Create(SolicitudMantenimientoCreateViewModel vm)
        {
            // ============================================================
            // VALIDAR SOPORTE DOCUMENTAL OBLIGATORIO
            // ============================================================
            bool tieneArchivosNuevos = vm.Archivos != null && vm.Archivos.Any(x => x != null && x.Length > 0);
            if (!tieneArchivosNuevos)
            {
                ModelState.AddModelError(nameof(vm.Archivos), "Debe adjuntar al menos un documento de soporte (PDF, JPG, PNG).");
            }

            // ============================================================
            // VALIDAR SERVICIOS
            // ============================================================
            if (vm.Servicios == null || !vm.Servicios.Any())
            {
                ModelState.AddModelError(string.Empty, "Debe capturar al menos un servicio.");
            }

            // ============================================================
            // VALIDACIÓN CRUZADA ODÓMETRO / KILOMETRAJE
            // ============================================================
            if (vm.EstadoOdometroId.HasValue && vm.KilometrajeActual.HasValue)
            {
                var estadoOdometro = vm.EstadoOdometroId.Value;
                var kilometraje = vm.KilometrajeActual.Value;

                if (estadoOdometro == 1 && kilometraje <= 0)
                {
                    ModelState.AddModelError(
                        nameof(vm.KilometrajeActual),
                        "Cuando el odómetro opera correctamente, el kilometraje debe ser mayor a cero.");
                }

                if (estadoOdometro != 1 && kilometraje > 0)
                {
                    ModelState.AddModelError(
                        nameof(vm.KilometrajeActual),
                        "Si el odómetro no opera correctamente, el kilometraje debe ser cero.");
                }
            }

            // ============================================================
            // VALIDAR RANGO DE KILOMETRAJE Y CONTRA HISTÓRICO DE BD
            // ============================================================
            if (vm.KilometrajeActual.HasValue)
            {
                if (vm.KilometrajeActual.Value < 0)
                {
                    ModelState.AddModelError(
                        nameof(vm.KilometrajeActual),
                        "El kilometraje no puede ser negativo.");
                }

                if (vm.KilometrajeActual.Value > 999999)
                {
                    ModelState.AddModelError(
                        nameof(vm.KilometrajeActual),
                        "El kilometraje debe estar entre 0 y 999999.");
                }

                // Validación histórica si la unidad está seleccionada y el odómetro opera correctamente
                if (vm.UnidadVehicularId.HasValue && vm.UnidadVehicularId.Value > 0 && vm.EstadoOdometroId == 1)
                {
                    var ultimoKmConocido = await _maquinariaService.ObtenerUltimoKilometrajeUnidadAsync(vm.UnidadVehicularId.Value);
                    if (ultimoKmConocido > 0 && vm.KilometrajeActual.Value < ultimoKmConocido)
                    {
                        ModelState.AddModelError(
                            nameof(vm.KilometrajeActual),
                            $"El kilometraje capturado ({vm.KilometrajeActual.Value}) no puede ser menor al último registrado en el sistema para esta unidad ({ultimoKmConocido}).");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarCatalogos(vm);

                if (vm.UnidadVehicularId.HasValue && vm.UnidadVehicularId.Value > 0)
                {
                    vm.UnidadVehicularTexto = await _maquinariaService.ObtenerTextoUnidadVehicular(vm.UnidadVehicularId.Value);
                }

                return View(vm);
            }

            try
            {
                var serviciosDto = vm.Servicios
                    .Select(x => new SolicitudMantenimientoDetalleCreateDto
                    {
                        ConceptoServicioId = x.ConceptoServicioId,
                        Cantidad = x.Cantidad,
                        CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                        CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                        IdObjetoGastoManoObra = x.IdObjetoGastoManoObra,
                        IdObjetoGastoRefacciones = x.IdObjetoGastoRefacciones,
                        Observaciones = x.Observaciones
                    })
                    .ToList();

                var importeCotizacion = serviciosDto.Sum(x => x.Cantidad * (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones));

                var dto = new SolicitudMantenimientoCreateDto
                {
                    Folio = vm.Folio,
                    NumeroOficio = vm.NumeroOficio!,
                    FechaSolicitud = vm.FechaSolicitud,
                    AreaSolicitanteId = vm.AreaSolicitanteId!.Value,
                    UnidadVehicularId = vm.UnidadVehicularId!.Value,
                    TallerId = vm.TallerId!.Value,
                    TallerConfiguracionPresupuestalId = vm.TallerConfiguracionPresupuestalId!.Value,
                    TipoMantenimientoId = vm.TipoMantenimientoId!.Value,
                    KilometrajeActual = vm.KilometrajeActual!.Value,
                    EstadoOdometroId = vm.EstadoOdometroId!.Value,
                    Observaciones = vm.Observaciones,
                    ImporteCotizacion = importeCotizacion,
                    EstatusSolicitudMantenimientoId = vm.EstatusSolicitudMantenimientoId.HasValue ? vm.EstatusSolicitudMantenimientoId.Value : 0,
                    Servicios = serviciosDto
                };

                var resultado = await _maquinariaService.CrearSolicitudMantenimiento(dto, vm.Archivos);

                TempData["Success"] = $"La solicitud con folio {resultado.Folio} fue registrada exitosamente y su oficio fue generado.";
                TempData["AbrirOficioPdf"] = resultado.SolicitudMantenimientoId;
                TempData.Keep("AbrirOficioPdf");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarCatalogos(vm);

                if (vm.UnidadVehicularId.HasValue && vm.UnidadVehicularId.Value > 0)
                {
                    vm.UnidadVehicularTexto = await _maquinariaService.ObtenerTextoUnidadVehicular(vm.UnidadVehicularId.Value);
                }

                return View(vm);
            }
        }



        [HttpGet]
        public async Task<IActionResult> ObtenerConfiguracionesPorTaller(int tallerId)
        {
            // 1. Consultar de forma segura desde la BD
            var configuracionesRaw = await _context.TallerConfiguracions
                .AsNoTracking()
                .Where(tc => tc.TallerId == tallerId && tc.Activo && tc.TallerConfiguracionPresupuestal.Activo)
                .Select(tc => new
                {
                    Id = tc.TallerConfiguracionPresupuestalId,
                    Contrato = tc.TallerConfiguracionPresupuestal.Contrato,
                    RecursoNombre = tc.TallerConfiguracionPresupuestal.Recurso.Nombre,
                    MontoMaximo = tc.TallerConfiguracionPresupuestal.MontoMaximo
                })
                .ToListAsync();

            // 2. Limpieza forzosa en memoria: Agrupar y filtrar estrictamente duplicados por ID (Value)
            var configuracionesUnicas = configuracionesRaw
                .GroupBy(x => x.Id)
                .Select(g => g.First()) // Si hay duplicados en crudo, nos quedamos estrictamente con el primero
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"Contrato: {x.Contrato} | Recurso: {x.RecursoNombre} (Monto Máx: {x.MontoMaximo:C})"
                })
                .OrderBy(x => x.Text)
                .ToList();

            return Json(configuracionesUnicas);
        }



        private async Task CargarCatalogos(SolicitudMantenimientoCreateViewModel vm)
        {
            vm.ConceptosServicio = new List<SelectListItem>();

            vm.Areas = await _maquinariaService.ObtenerAreas();
            vm.TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimiento();
            vm.Talleres = await _maquinariaService.ObtenerTalleres();
            vm.Estatuses = await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();
            vm.ObjetosGasto = await _maquinariaService.ObtenerObjetosGasto();
            vm.EstadosOdometro = await _maquinariaService.ObtenerEstadosOdometro();

            // REINICIALIZACIÓN FORZOSA: 
            // Si quieres limpiar el taller y su configuración tras un error de validación para evitar duplicidad:
            vm.TallerId = null;
            vm.TallerConfiguracionPresupuestalId = null;
            vm.ConfiguracionesTaller = new List<SelectListItem>();
        }



        //replicar todas las validaciones del Create en el Edit. cambio de unidad, select de la configuracion presupuestal... 
        //limpiar tabla de servicios al cambiar la unidad



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _maquinariaService.ObtenerSolicitudMantenimientoEdit(id);

                vm.EstadosOdometro = await _maquinariaService.ObtenerEstadosOdometro();

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }








        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(SolicitudMantenimientoEditViewModel vm,IEnumerable<IFormFile>? archivos){
        //    try
        //    {
        //        // ============================================
        //        // RECUPERAR REGISTRO ACTUAL
        //        // ============================================

        //        var solicitudActual =
        //            await _maquinariaService
        //                .ObtenerSolicitudMantenimientoPorId(
        //                    vm.SolicitudMantenimientoId);

        //        if (solicitudActual == null)
        //        {
        //            TempData["Error"] =
        //                "La solicitud no fue encontrada.";

        //            return RedirectToAction(nameof(Index));
        //        }


        //        // ============================================
        //        // CAMPOS PROTEGIDOS
        //        // ============================================

        //        vm.Folio =
        //            solicitudActual.Folio;

        //        vm.AreaSolicitanteId =
        //            solicitudActual.AreaSolicitanteId;

        //        vm.UnidadVehicularId =
        //            solicitudActual.UnidadVehicularId;

        //        vm.EstatusSolicitudMantenimientoId =
        //            solicitudActual.EstatusSolicitudMantenimientoId;

        //        vm.FechaRegistro =
        //            solicitudActual.FechaCreacion;


        //        // ============================================
        //        // VALIDAR SERVICIOS
        //        // ============================================

        //        if (vm.Servicios == null ||
        //            !vm.Servicios.Any())
        //        {
        //            ModelState.AddModelError(
        //                string.Empty,
        //                "Debe capturar al menos un servicio.");
        //        }


        //        // ============================================
        //        // ERROR DE VALIDACIÓN
        //        // ============================================

        //        if (!ModelState.IsValid)
        //        {
        //            await PrepararEditViewModel(vm);

        //            return View(vm);
        //        }


        //        // ============================================
        //        // MAPEAR SERVICIOS
        //        // ============================================

        //        var serviciosDto =
        //            vm.Servicios
        //                .Select(x =>
        //                    new SolicitudMantenimientoDetalleCreateDto
        //                    {
        //                        ConceptoServicioId =
        //                            x.ConceptoServicioId,

        //                        Cantidad =
        //                            x.Cantidad,

        //                        CostoEstimadoManoObra =
        //                            x.CostoEstimadoManoObra,

        //                        CostoEstimadoRefacciones =
        //                            x.CostoEstimadoRefacciones,

        //                        IdObjetoGastoManoObra =
        //                            x.IdObjetoGastoManoObra,

        //                        IdObjetoGastoRefacciones =
        //                            x.IdObjetoGastoRefacciones,

        //                        Observaciones =
        //                            x.Observaciones
        //                    })
        //                .ToList();


        //        // ============================================
        //        // DTO
        //        // ============================================

        //        var dto =
        //            new SolicitudMantenimientoEditDto
        //            {
        //                SolicitudMantenimientoId =
        //                    vm.SolicitudMantenimientoId,

        //                Folio =
        //                    vm.Folio,

        //                NumeroOficio =
        //                    vm.NumeroOficio?.Trim(),

        //                FechaSolicitud =
        //                    vm.FechaSolicitud,

        //                AreaSolicitanteId =
        //                    vm.AreaSolicitanteId,

        //                UnidadVehicularId =
        //                    vm.UnidadVehicularId,

        //                TallerId =
        //                    vm.TallerId,

        //                TipoMantenimientoId =
        //                    vm.TipoMantenimientoId,

        //                KilometrajeActual =
        //                    vm.KilometrajeActual,

        //                EstadoOdometroId =
        //                    vm.EstadoOdometroId,

        //                Observaciones =
        //                    vm.Observaciones?.Trim(),

        //                ImporteCotizacion =
        //                    vm.ImporteCotizacion,

        //                EstatusSolicitudMantenimientoId =
        //                    vm.EstatusSolicitudMantenimientoId,

        //                Servicios =
        //                    serviciosDto
        //            };


        //        // ============================================
        //        // ACTUALIZAR
        //        // ============================================

        //        await _maquinariaService
        //            .EditarSolicitudMantenimiento(
        //                dto,
        //                archivos);


        //        TempData["Success"] =
        //            "La solicitud fue actualizada correctamente.";

        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        foreach (var mensaje in
        //                 ex.Message.Split(
        //                     new[] { "\r\n", "\n" },
        //                     StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            ModelState.AddModelError(
        //                string.Empty,
        //                mensaje);
        //        }


        //        await PrepararEditViewModel(vm);

        //        return View(vm);
        //    }
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SolicitudMantenimientoEditViewModel vm, IEnumerable<IFormFile>? archivos)
        {
            try
            {
                // ============================================
                // RECUPERAR REGISTRO ACTUAL
                // ============================================
                var solicitudActual = await _maquinariaService.ObtenerSolicitudMantenimientoPorId(vm.SolicitudMantenimientoId);

                if (solicitudActual == null)
                {
                    TempData["Error"] = "La solicitud no fue encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                // ============================================
                // VALIDAR SERVICIOS Y CAMPOS OBLIGATORIOS
                // ============================================
                if (vm.Servicios == null || !vm.Servicios.Any())
                {
                    ModelState.AddModelError(string.Empty, "Debe capturar al menos un servicio.");
                }

                if (!vm.AreaSolicitanteId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.AreaSolicitanteId), "Falta el área solicitante.");
                }

                if (!vm.UnidadVehicularId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.UnidadVehicularId), "Falta la unidad vehicular.");
                }

                if (!vm.TallerId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.TallerId), "Debe seleccionar un taller.");
                }

                if (!vm.TallerConfiguracionPresupuestalId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.TallerConfiguracionPresupuestalId), "Debe seleccionar el contrato/configuración presupuestal del taller.");
                }

                if (!vm.TipoMantenimientoId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.TipoMantenimientoId), "Falta el tipo de mantenimiento.");
                }

                if (!vm.EstatusSolicitudMantenimientoId.HasValue)
                {
                    ModelState.AddModelError(nameof(vm.EstatusSolicitudMantenimientoId), "Falta el estatus de mantenimiento.");
                }

                // ============================================
                // VALIDACIÓN CRUZADA ODÓMETRO
                // ============================================
                if (vm.EstadoOdometroId.HasValue && vm.KilometrajeActual.HasValue)
                {
                    if (vm.EstadoOdometroId.Value == 1 && vm.KilometrajeActual.Value <= 0)
                    {
                        ModelState.AddModelError(nameof(vm.KilometrajeActual), "Cuando el odómetro opera correctamente, el kilometraje debe ser mayor a cero.");
                    }

                    if (vm.EstadoOdometroId.Value != 1 && vm.KilometrajeActual.Value > 0)
                    {
                        ModelState.AddModelError(nameof(vm.KilometrajeActual), "Si el odómetro no opera correctamente, el kilometraje debe ser cero.");
                    }
                }

                // ============================================
                // PROTEGER CAMPOS NO EDITABLES
                // ============================================
                vm.Folio = solicitudActual.Folio;
                vm.AreaSolicitanteId = solicitudActual.AreaSolicitanteId;
                vm.UnidadVehicularId = solicitudActual.UnidadVehicularId;
                vm.EstatusSolicitudMantenimientoId = solicitudActual.EstatusSolicitudMantenimientoId;
                vm.FechaRegistro = solicitudActual.FechaCreacion;

                // ============================================
                // MODELSTATE INVÁLIDO
                // ============================================
                if (!ModelState.IsValid)
                {
                    await CargarCatalogosEdit(vm, solicitudActual.UnidadVehicularId, solicitudActual.UnidadVehicularTexto);

                    vm.Documentos = await _maquinariaService.ObtenerDocumentosSolicitudMantenimiento(vm.SolicitudMantenimientoId);
                    vm.TieneDocumentos = vm.Documentos.Any();
                    vm.TotalDocumentos = vm.Documentos.Count;

                    return View(vm);
                }

                // ============================================
                // MAPEAR SERVICIOS
                // ============================================
                var serviciosDto = vm.Servicios
                    .Select(x => new SolicitudMantenimientoDetalleCreateDto
                    {
                        ConceptoServicioId = x.ConceptoServicioId,
                        Cantidad = x.Cantidad,
                        CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                        CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                        IdObjetoGastoManoObra = x.IdObjetoGastoManoObra,
                        IdObjetoGastoRefacciones = x.IdObjetoGastoRefacciones,
                        Observaciones = x.Observaciones
                    })
                    .ToList();

                // ============================================
                // DTO PRINCIPAL EDIT
                // ============================================
                var dto = new SolicitudMantenimientoEditDto
                {
                    SolicitudMantenimientoId = vm.SolicitudMantenimientoId,
                    Folio = vm.Folio,
                    NumeroOficio = vm.NumeroOficio?.Trim(),
                    FechaSolicitud = vm.FechaSolicitud,
                    AreaSolicitanteId = vm.AreaSolicitanteId.Value,
                    UnidadVehicularId = vm.UnidadVehicularId.Value,
                    TallerId = vm.TallerId.Value,
                    TallerConfiguracionPresupuestalId = vm.TallerConfiguracionPresupuestalId.Value, // <--- INTEGRADO
                    TipoMantenimientoId = vm.TipoMantenimientoId.Value,
                    KilometrajeActual = vm.KilometrajeActual.Value,
                    EstadoOdometroId = vm.EstadoOdometroId.Value,
                    Observaciones = vm.Observaciones?.Trim(),
                    ImporteCotizacion = vm.ImporteCotizacion,
                    EstatusSolicitudMantenimientoId = vm.EstatusSolicitudMantenimientoId.Value,
                    Servicios = serviciosDto
                };

                // ============================================
                // ACTUALIZAR
                // ============================================
                await _maquinariaService.EditarSolicitudMantenimiento(dto, archivos);

                TempData["Success"] = "La solicitud fue actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var solicitudActual = await _maquinariaService.ObtenerSolicitudMantenimientoPorId(vm.SolicitudMantenimientoId);

                if (solicitudActual != null)
                {
                    vm.Folio = solicitudActual.Folio;
                    vm.AreaSolicitanteId = solicitudActual.AreaSolicitanteId;
                    vm.UnidadVehicularId = solicitudActual.UnidadVehicularId;
                    vm.EstatusSolicitudMantenimientoId = solicitudActual.EstatusSolicitudMantenimientoId;
                    vm.FechaRegistro = solicitudActual.FechaCreacion;

                    await CargarCatalogosEdit(vm, solicitudActual.UnidadVehicularId, solicitudActual.UnidadVehicularTexto);
                }
                else
                {
                    vm.Areas = await _maquinariaService.ObtenerAreas();
                    vm.TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimiento();
                    vm.Talleres = await _maquinariaService.ObtenerTalleres();
                    vm.Estatuses = await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();
                    vm.EstadosOdometro = await _maquinariaService.ObtenerEstadosOdometro();
                    vm.ObjetosGasto = await _maquinariaService.ObtenerObjetosGasto();
                }

                var mensajes = ex.Message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var msg in mensajes)
                {
                    ModelState.AddModelError(string.Empty, msg);
                }

                return View(vm);
            }
        }


   


        private async Task CargarCatalogosEdit(SolicitudMantenimientoEditViewModel vm,int unidadVehicularId,string unidadVehicularTexto)
        {
            vm.Areas =
                await _maquinariaService.ObtenerAreas();

            vm.UnidadesVehiculares =
                new List<SelectListItem>
                {
            new SelectListItem
            {
                Value = unidadVehicularId.ToString(),
                Text = unidadVehicularTexto,
                Selected = true
            }
                };

            vm.TiposMantenimiento =
                await _maquinariaService
                    .ObtenerTiposMantenimiento();

            vm.Talleres =
                await _maquinariaService
                    .ObtenerTalleres();

            vm.Estatuses =
                await _maquinariaService
                    .ObtenerEstatusSolicitudMantenimiento();

            vm.EstadosOdometro =
                await _maquinariaService
                    .ObtenerEstadosOdometro();

            vm.ConceptosServicio =
                await _maquinariaService
                    .ObtenerConceptosServicioFiltrados(
                        unidadVehicularId);

            vm.ObjetosGasto =
                await _maquinariaService
                    .ObtenerObjetosGasto();
        }





        private async Task PrepararEditViewModel(SolicitudMantenimientoEditViewModel vm)
        {
            // ============================================================
            // CATÁLOGOS
            // ============================================================

            vm.Areas =
                await _maquinariaService
                    .ObtenerAreas();


            vm.TiposMantenimiento =
                await _maquinariaService
                    .ObtenerTiposMantenimiento();


            vm.Talleres =
                await _maquinariaService
                    .ObtenerTalleres();


            vm.Estatuses =
                await _maquinariaService
                    .ObtenerEstatusSolicitudMantenimiento();


            vm.EstadosOdometro =
                await _maquinariaService
                    .ObtenerEstadosOdometro();


            // ============================================================
            // UNIDAD VEHICULAR
            // ============================================================

            var solicitudActual =
                await _maquinariaService
                    .ObtenerSolicitudMantenimientoPorId(
                        vm.SolicitudMantenimientoId);


            if (solicitudActual != null)
            {
                vm.UnidadesVehiculares =
                    new List<SelectListItem>
                    {
                new SelectListItem
                {
                    Value =
                        vm.UnidadVehicularId.ToString(),

                    Text =
                        solicitudActual.UnidadVehicularTexto,

                    Selected = true
                }
                    };
            }
            else
            {
                vm.UnidadesVehiculares =
                    new List<SelectListItem>();
            }


            // ============================================================
            // CONCEPTOS DE SERVICIO
            // ============================================================

            vm.ConceptosServicio =
                vm.TipoCatalogoServicioId > 0
                    ? await _maquinariaService
                        .ObtenerConceptosServicio(
                            vm.TipoCatalogoServicioId)
                    : new List<SelectListItem>();


            // ============================================================
            // OBJETOS DE GASTO
            // ============================================================

            vm.ObjetosGasto =
                await _maquinariaService
                    .ObtenerObjetosGasto();


            // ============================================================
            // DOCUMENTOS EXISTENTES
            // ============================================================

            vm.Documentos =await _maquinariaService.ObtenerDocumentosSolicitudMantenimiento(
                        vm.SolicitudMantenimientoId);

            vm.Documentos ??=
                new List<SolicitudMantenimientoDocumentoViewModel>();


            vm.TotalDocumentos =
                vm.Documentos.Count;


            vm.TieneDocumentos =
                vm.Documentos.Any();


            // ============================================================
            // RECONSTRUIR TEXTOS DE SERVICIOS
            // ============================================================

            PrepararTextosServicios(vm);
        }








        private void PrepararTextosServicios(SolicitudMantenimientoEditViewModel vm)
        {
            if (vm.Servicios == null ||
                !vm.Servicios.Any())
            {
                return;
            }


            // ============================================================
            // CONCEPTOS
            // ============================================================

            var conceptos =
                vm.ConceptosServicio
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(
                        x => x.Value,
                        x => x.Text);


            // ============================================================
            // OBJETOS DE GASTO
            // ============================================================

            var objetosGasto =
                vm.ObjetosGasto
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(
                        x => x.Value,
                        x => x.Text);


            // ============================================================
            // SERVICIOS
            // ============================================================

            foreach (var servicio in vm.Servicios)
            {
                // --------------------------------------------------------
                // CONCEPTO
                // --------------------------------------------------------

                if (conceptos.TryGetValue(
                        servicio.ConceptoServicioId.ToString(),
                        out var concepto))
                {
                    servicio.ConceptoServicio =
                        concepto;
                }


                // --------------------------------------------------------
                // PARTIDA MANO DE OBRA
                // --------------------------------------------------------

                if (servicio.IdObjetoGastoManoObra.HasValue)
                {
                    if (objetosGasto.TryGetValue(
                            servicio.IdObjetoGastoManoObra.Value.ToString(),
                            out var partidaManoObra))
                    {
                        servicio.ObjetoGastoManoObraTexto =
                            partidaManoObra;
                    }
                }


                // --------------------------------------------------------
                // PARTIDA REFACCIONES
                // --------------------------------------------------------

                if (servicio.IdObjetoGastoRefacciones.HasValue)
                {
                    if (objetosGasto.TryGetValue(
                            servicio.IdObjetoGastoRefacciones.Value.ToString(),
                            out var partidaRefacciones))
                    {
                        servicio.ObjetoGastoRefaccionesTexto =
                            partidaRefacciones;
                    }
                }
            }
        }







        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var vm = await _maquinariaService
                .ObtenerDetalleSolicitud(id);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);

        }



        //[HttpGet]
        //public async Task<IActionResult>AgregarConcepto(int id)
        //{
        //    var vm = new AgregarConceptoViewModel
        //    {
        //        SolicitudMantenimientoId = id,

        //        ConceptosServicio =await _maquinariaService.ObtenerConceptosServicio()
        //    };

        //    return View(vm);
        //}


        //[HttpGet]
        //public async Task<JsonResult> ObtenerCostoConcepto(int solicitudId, int conceptoServicioId)
        //{
        //    var costo = await _maquinariaService.ObtenerCostoConcepto(solicitudId, conceptoServicioId);

        //    return Json(costo);
        //}


        [HttpGet]
        public async Task<JsonResult> ObtenerCostoConceptoCreate(int unidadVehicularId, int conceptoServicioId)
        {
            var costo =
                await _maquinariaService.ObtenerCostoConceptoCreate(unidadVehicularId, conceptoServicioId);

            return Json(costo);
        }




        [HttpGet]
        public async Task<IActionResult> AgregarConcepto(int id)
        {
            var solicitud =
                await _maquinariaService.ObtenerSolicitudMantenimientoPorId(id);

            if (solicitud == null)
            {
                TempData["Error"] = "Solicitud no encontrada.";
                return RedirectToAction("Index");
            }

            var vm = new AgregarConceptoViewModel
            {
                SolicitudMantenimientoId = id,

                ConceptosServicio = solicitud.TipoCatalogoServicioId > 0
                        ? await _maquinariaService.ObtenerConceptosServicio(
                            solicitud.TipoCatalogoServicioId)
                        : new List<SelectListItem>(),

                ConceptosYaRegistrados =
                    await _maquinariaService.ObtenerConceptosYaRegistrados(id)
            };

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarConcepto(AgregarConceptoViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var solicitud =
                        await _maquinariaService.ObtenerSolicitudMantenimientoPorId(vm.SolicitudMantenimientoId);

                    vm.ConceptosServicio =
                        solicitud?.TipoCatalogoServicioId > 0
                            ? await _maquinariaService.ObtenerConceptosServicio(
                                solicitud.TipoCatalogoServicioId)
                            : new List<SelectListItem>();

                    vm.ConceptosYaRegistrados =
                        await _maquinariaService.ObtenerConceptosYaRegistrados(vm.SolicitudMantenimientoId);

                    return View(vm);
                }

                var dto =
                    new SolicitudMantenimientoDetalleCreateDto
                    {
                        SolicitudMantenimientoId = vm.SolicitudMantenimientoId,
                        ConceptoServicioId = vm.ConceptoServicioId,
                        Cantidad = vm.Cantidad,
                        Observaciones = vm.Observaciones
                    };

                await _maquinariaService.AgregarConceptoSolicitud(dto);

                TempData["Success"] =
                    "Concepto agregado correctamente.";

                return RedirectToAction("Detalle", new { id = vm.SolicitudMantenimientoId });
            }
            catch (Exception ex)
            {
                var solicitud = await _maquinariaService.ObtenerSolicitudMantenimientoPorId(vm.SolicitudMantenimientoId);

                vm.ConceptosServicio = solicitud?.TipoCatalogoServicioId > 0
                        ? await _maquinariaService.ObtenerConceptosServicio(
                            solicitud.TipoCatalogoServicioId)
                        : new List<SelectListItem>();

                vm.ConceptosYaRegistrados = await _maquinariaService.ObtenerConceptosYaRegistrados(vm.SolicitudMantenimientoId);

                ViewBag.Error = ex.Message;

                return View(vm);
            }
        }



        // =====================================================
        // 1. GET: REVISIÓN TÉCNICA DE SOLICITUD
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Revisar(int id)
        {
            if (id <= 0)
            {
                TempData["Warning"] = "La solicitud de mantenimiento no es válida.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var model = await _maquinariaService.ObtenerRevisionSolicitud(id);
                return View(model);
            }
            catch (Exception ex)
            {
                var mensaje = ex.Message ?? string.Empty;
                var esWarning = mensaje.StartsWith("WARNING|", StringComparison.OrdinalIgnoreCase);
                var esInfo = mensaje.StartsWith("INFO|", StringComparison.OrdinalIgnoreCase);

                if (esWarning || esInfo)
                {
                    mensaje = mensaje.Substring(mensaje.IndexOf('|') + 1).Trim();
                    TempData[esWarning ? "Warning" : "Info"] = string.IsNullOrWhiteSpace(mensaje)
                        ? "No fue posible consultar la revisión técnica."
                        : mensaje;
                }
                else
                {
                    _logger.LogError(ex, "Error no controlado al obtener revisión técnica para la solicitud {SolicitudId}", id);
                    TempData["Error"] = "No fue posible cargar la información de la revisión técnica.";
                }

                return RedirectToAction(nameof(Index));
            }
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revisar(RevisionSolicitudPostViewModel vm, bool aprobada)
        {
            // =====================================================
            // 1. VALIDAR IDENTIFICADOR
            // =====================================================
            if (vm == null || vm.SolicitudMantenimientoId <= 0)
            {
                TempData["Warning"] = "La solicitud de mantenimiento no es válida.";
                return RedirectToAction(nameof(Index));
            }

            // =====================================================
            // 2. VALIDACIONES DE NEGOCIO
            // =====================================================
            if (!aprobada)
            {
                if (string.IsNullOrWhiteSpace(vm.Observaciones))
                {
                    ModelState.AddModelError(
                        nameof(vm.Observaciones),
                        "Debe indicar las observaciones que justifican el rechazo.");
                }

                if (!vm.TipoRechazoSolicitudId.HasValue || vm.TipoRechazoSolicitudId.Value <= 0)
                {
                    ModelState.AddModelError(
                        nameof(vm.TipoRechazoSolicitudId),
                        "Seleccione el tipo de rechazo.");
                }
            }

            // =====================================================
            // 3. VALIDAR MODELSTATE
            // =====================================================
            if (!ModelState.IsValid)
            {
                return await RegresarRevisionConErroresAsync(vm);
            }

            // =====================================================
            // 4. CONSTRUIR DTO
            // =====================================================
            var dto = new RevisionSolicitudDto
            {
                SolicitudMantenimientoId = vm.SolicitudMantenimientoId,
                Aprobada = aprobada,
                Observaciones = string.IsNullOrWhiteSpace(vm.Observaciones)
                    ? null
                    : vm.Observaciones.Trim(),
                TipoRechazoSolicitudId = aprobada ? null : vm.TipoRechazoSolicitudId
            };

            // =====================================================
            // 5. PROCESAR REVISIÓN TÉCNICA
            // =====================================================
            try
            {
                await _maquinariaService.RevisarSolicitud(dto);

                if (aprobada)
                {
                    TempData["Success"] = "La revisión técnica fue autorizada correctamente.";

                    // NOTA: Se removió 'TempData["AbrirDictamen"]' porque el DTA ya fue emitido y firmado previamente.
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] =
                    "La solicitud fue rechazada y se procesó la liberación presupuestal correspondiente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // =============================================
                // 5.1 MENSAJES CONTROLADOS DE NEGOCIO Y LOGS
                // =============================================
                var mensaje = ex.Message ?? string.Empty;

                if (mensaje.StartsWith("WARNING|", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        mensaje.Substring("WARNING|".Length).Trim());
                }
                else if (mensaje.StartsWith("INFO|", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        mensaje.Substring("INFO|".Length).Trim());
                }
                else
                {
                    _logger.LogError(
                        ex,
                        "Error al procesar la revisión técnica de la solicitud {SolicitudId}.",
                        vm.SolicitudMantenimientoId);

                    ModelState.AddModelError(
                        string.Empty,
                        "No fue posible completar la revisión técnica. Verifique el estado de la solicitud y vuelva a intentarlo.");
                }

                // =============================================
                // 5.2 RECUPERAR VISTA Y CAPTURA DEL USUARIO
                // =============================================
                return await RegresarRevisionConErroresAsync(vm);
            }
        }



        [HttpGet]
        public async Task<IActionResult> EmitirDta(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "La solicitud de mantenimiento no es válida.";
                return RedirectToAction(nameof(Revisar), new { id });
            }

            try
            {
                // 1. Obtener la ruta base desde el appsettings.json
                var rutaBase = _configuration["Storage:DocumentosRuta"];
                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    TempData["Error"] = "La ruta de almacenamiento de documentos no está configurada en el servidor.";
                    return RedirectToAction(nameof(Revisar), new { id });
                }

                // 2. Buscar la solicitud para obtener su folio y validar que exista
                var solicitud = await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == id && x.Activo);

                if (solicitud == null)
                {
                    TempData["Error"] = "La solicitud de mantenimiento no existe.";
                    return RedirectToAction(nameof(Index));
                }

                // 3. Generar los bytes del documento Word fresco con los datos más actualizados
                byte[] wordBytes = await _maquinariaService.GenerarDictamenTecnicoWordAsync(id);

                if (wordBytes == null || wordBytes.Length == 0)
                {
                    TempData["Error"] = "No fue posible generar el documento DTA para esta solicitud.";
                    return RedirectToAction(nameof(Revisar), new { id });
                }

                // 4. Crear la carpeta secuencial segura basada en el folio
                var carpetaFolioSegura = string.IsNullOrWhiteSpace(solicitud.Folio)
                    ? $"Solicitud_{solicitud.SolicitudMantenimientoId}"
                    : solicitud.Folio.Replace('/', '_');

                var carpetaSolicitud = Path.Combine(rutaBase, carpetaFolioSegura);

                if (!Directory.Exists(carpetaSolicitud))
                {
                    Directory.CreateDirectory(carpetaSolicitud);
                }

                // 5. CONTROL DE DUPLICADOS: Buscar si ya existía un DTA emitido anteriormente
                var documentoAnterior = await _context.SolicitudMantenimientoDocumentos
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == id && x.TipoDocumento == "DICTAMEN_TECNICO" && x.Activo);

                if (documentoAnterior != null && !string.IsNullOrEmpty(documentoAnterior.RutaArchivo))
                {
                    // Intentar borrar el archivo físico viejo de la red para liberar espacio
                    var rutaFisicaAnterior = Path.Combine(rutaBase, documentoAnterior.RutaArchivo.Replace("/", "\\").TrimStart('\\', '/'));
                    if (System.IO.File.Exists(rutaFisicaAnterior))
                    {
                        try
                        {
                            System.IO.File.Delete(rutaFisicaAnterior);
                        }
                        catch
                        {
                            // Evitar que falle el flujo si el archivo estuviera bloqueado temporalmente
                        }
                    }

                    // Marcar el registro anterior como inactivo
                    documentoAnterior.Activo = false;
                }

                // 6. Definir nombre físico único y ruta completa para la nueva versión
                string nombreArchivoFisico = $"DTA_EMITIDO_{id}_{DateTime.Now:yyyyMMddHHmmss}.docx";
                string rutaCompleta = Path.Combine(carpetaSolicitud, nombreArchivoFisico);

                // 7. Guardar el nuevo archivo físicamente en la red
                await System.IO.File.WriteAllBytesAsync(rutaCompleta, wordBytes);

                // 8. Calcular el Hash SHA256 del nuevo archivo
                string hashArchivo;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    hashArchivo = Convert.ToHexString(sha256.ComputeHash(wordBytes));
                }

                // 9. Registrar el nuevo documento activo en la base de datos
                var rutaRelativaParaBd = Path.Combine(carpetaFolioSegura, nombreArchivoFisico).Replace("\\", "/");

                var documentoEntidad = new SolicitudMantenimientoDocumento
                {
                    SolicitudMantenimientoId = id,
                    TipoDocumento = "DICTAMEN_TECNICO",
                    NombreArchivo = $"DTA_{solicitud.Folio?.Replace('/', '_') ?? id.ToString()}.docx",
                    RutaArchivo = rutaRelativaParaBd,
                    FechaCarga = DateTime.Now,
                    UsuarioCargaId = 1, // Ajustar según el usuario actual logueado si aplica
                    Activo = true,
                    HashArchivo = hashArchivo
                };

                _context.SolicitudMantenimientoDocumentos.Add(documentoEntidad);
                await _context.SaveChangesAsync();

                // 10. Retornar el archivo Word actualizado directamente para su descarga en el navegador
                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", documentoEntidad.NombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al emitir y actualizar el DTA para la solicitud {SolicitudId}.", id);
                TempData["Error"] = "Ocurrió un error al intentar emitir el documento DTA: " + ex.Message;
                return RedirectToAction(nameof(Revisar), new { id });
            }
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarDtaFirmado(int solicitudId, IFormFile dtaFile)
        {
            try
            {
                if (solicitudId <= 0)
                    return Json(new { success = false, message = "El ID de la solicitud no es válido." });

                if (dtaFile == null || dtaFile.Length == 0)
                    return Json(new { success = false, message = "No se ha seleccionado ningún archivo PDF válido." });

                // 1. REFORZAMIENTO: Validación de Tamaño Máximo (2 MB)
                const long tamanoMaximoBytes = 2 * 1024 * 1024; // 2 MB
                if (dtaFile.Length > tamanoMaximoBytes)
                {
                    return Json(new { success = false, message = "El archivo excede el tamaño máximo permitido de 2 MB." });
                }

                // 2. REFORZAMIENTO: Validación de Extensión estricta
                var extension = Path.GetExtension(dtaFile.FileName).ToLowerInvariant();
                if (extension != ".pdf")
                {
                    return Json(new { success = false, message = "Solo se permiten archivos con formato PDF." });
                }

                // 3. REFORZAMIENTO: Validación por Estructura Interna (Magic Numbers para PDF)
                using (var streamValidacion = dtaFile.OpenReadStream())
                {
                    var headerBytes = new byte[5];
                    int bytesRead = await streamValidacion.ReadAsync(headerBytes, 0, 5);

                    if (bytesRead < 5)
                    {
                        return Json(new { success = false, message = "El archivo está incompleto o dañado." });
                    }

                    var headerString = System.Text.Encoding.ASCII.GetString(headerBytes);
                    if (!headerString.StartsWith("%PDF-"))
                    {
                        return Json(new { success = false, message = "La estructura del archivo no corresponde a un documento PDF válido." });
                    }
                }

                var solicitud = await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId && x.Activo);

                if (solicitud == null)
                    return Json(new { success = false, message = "La solicitud de mantenimiento no existe." });

                var rutaBaseServidor = _configuration["Storage:DocumentosRuta"];

                if (string.IsNullOrEmpty(rutaBaseServidor))
                    return Json(new { success = false, message = "La ruta de almacenamiento no está configurada." });

                var nombreCarpeta = string.IsNullOrWhiteSpace(solicitud.Folio)
                    ? $"Solicitud_{solicitud.SolicitudMantenimientoId}"
                    : solicitud.Folio.Replace("/", "_");

                var carpetaDestino = Path.Combine(rutaBaseServidor, nombreCarpeta);

                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                // 4. CONTROL DE DUPLICADOS: Buscar si ya existe un DTA firmado previo para esta solicitud
                var documentoAnterior = await _context.SolicitudMantenimientoDocumentos
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId && x.TipoDocumento == "DTA_FIRMADO" && x.Activo);

                if (documentoAnterior != null && !string.IsNullOrEmpty(documentoAnterior.RutaArchivo))
                {
                    // Intentar borrar el archivo físico anterior de la red para liberar espacio
                    var rutaFisicaAnterior = Path.Combine(rutaBaseServidor, documentoAnterior.RutaArchivo.Replace("/", "\\").TrimStart('\\', '/'));
                    if (System.IO.File.Exists(rutaFisicaAnterior))
                    {
                        try
                        {
                            System.IO.File.Delete(rutaFisicaAnterior);
                        }
                        catch
                        {
                            // Opcional: Manejar o registrar si el archivo estuviera bloqueado temporalmente, sin detener el flujo
                        }
                    }

                    // Marcar el registro anterior como inactivo
                    documentoAnterior.Activo = false;
                }

                // 5. Guardar el nuevo archivo físicamente
                var nombreLimpioOriginal = Path.GetFileName(dtaFile.FileName);
                var nombreUnicoArchivo = $"{Guid.NewGuid():N}.pdf";
                var rutaFisicaCompleta = Path.Combine(carpetaDestino, nombreUnicoArchivo);

                using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                {
                    await dtaFile.CopyToAsync(stream);
                }

                var rutaRelativaParaBd = Path.Combine(nombreCarpeta, nombreUnicoArchivo).Replace("\\", "/");

                // 6. Registrar el nuevo documento activo en la base de datos
                var documentoNuevo = new SolicitudMantenimientoDocumento
                {
                    SolicitudMantenimientoId = solicitudId,
                    TipoDocumento = "DTA_FIRMADO",
                    NombreArchivo = nombreLimpioOriginal,
                    RutaArchivo = rutaRelativaParaBd,
                    FechaCarga = DateTime.Now,
                    Activo = true
                };

                _context.SolicitudMantenimientoDocumentos.Add(documentoNuevo);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno de seguridad al procesar el archivo: " + ex.Message });
            }
        }







        private async Task<IActionResult> RegresarRevisionConErroresAsync(RevisionSolicitudPostViewModel vm)
        {
            try
            {
                // Recuperar datos de consulta desde la base de datos.
                var modelo =
                    await _maquinariaService.ObtenerRevisionSolicitud(vm.SolicitudMantenimientoId);

                // Restaurar únicamente los valores capturados por el usuario.
                modelo.Observaciones = vm.Observaciones ?? string.Empty;
                modelo.TipoRechazoSolicitudId = vm.TipoRechazoSolicitudId;
                return View("Revisar", modelo);
            }
            catch (Exception ex)
            {
                // Registrar ex con ILogger si está disponible.
                TempData["Error"] =
                    "No fue posible recuperar la solicitud para mostrar los errores. " +
                    "Vuelva a abrir la revisión.";

                return RedirectToAction(nameof(Index));
            }
        }





        //SELECCIONAR UNIDAD EN EL FORMULARIO
        [HttpGet]
        public async Task<JsonResult> BuscarUnidadesVehiculares(string term)
        {
            var resultado = await _maquinariaService.BuscarUnidadesVehiculares(term);

            return Json(resultado);
        }




        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(int id)
        {
            try
            {
                // 1. Buscar el documento en la base de datos
                var documento = await _context.SolicitudMantenimientoDocumentos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoDocumentoId == id && x.Activo);

                if (documento == null || string.IsNullOrEmpty(documento.RutaArchivo))
                {
                    return NotFound("El documento solicitado no existe o no está disponible en la base de datos.");
                }

                // 2. Obtener la ruta base desde appsettings.json
                var rutaBaseServidor = _configuration["Storage:DocumentosRuta"];

                if (string.IsNullOrEmpty(rutaBaseServidor))
                {
                    return NotFound("La ruta de almacenamiento de documentos no está configurada en el servidor.");
                }

                // 3. Construir la ruta física completa de forma robusta
                string rutaFisica;

                // Si por alguna razón la ruta en BD ya viene con la IP o es totalmente calificada, se usa directamente normalizada
                if (documento.RutaArchivo.Contains("10.8.3.61") || Path.IsPathFullyQualified(documento.RutaArchivo))
                {
                    rutaFisica = documento.RutaArchivo.Replace("/", "\\");
                }
                else
                {
                    // Si es una ruta relativa con carpetas secuenciales (ej. "SM_067_2026/archivo.pdf"), se combina con la base UNC
                    var rutaRelativaLimpia = documento.RutaArchivo.TrimStart('~', '/', '\\');
                    rutaFisica = Path.Combine(rutaBaseServidor, rutaRelativaLimpia);
                }

                // 4. Validar existencia física en el servidor de red
                if (!System.IO.File.Exists(rutaFisica))
                {
                    return NotFound($"El archivo físico no fue encontrado en el servidor de red: {rutaFisica}");
                }

                // 5. Leer los bytes del archivo
                var bytes = await System.IO.File.ReadAllBytesAsync(rutaFisica);

                // 6. Determinar dinámicamente el ContentType (MIME type) según la extensión real del archivo
                var extension = Path.GetExtension(rutaFisica).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".doc" => "application/msword",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".xls" => "application/vnd.ms-excel",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".xml" => "application/xml",
                    _ => "application/octet-stream"
                };

                // 7. Retornar el archivo para su descarga
                return File(bytes, contentType, documento.NombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error interno al intentar descargar el documento: " + ex.Message);
            }
        }





        // GET: DSPController
        [HttpGet]
        public async Task<IActionResult> DSP(int id)
        {

            try
            {
                var vm = await _maquinariaService.ObtenerDSP(id);
                return View(vm);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("WARNING|"))
                {
                    TempData["Warning"] = ex.Message.Replace("WARNING|", "");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    if (ex.Message.StartsWith("INFO|"))
                    {
                        TempData["Info"] = ex.Message.Replace("INFO|", "");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // No mostrar excepciones técnicas directamente.
                        TempData["Error"] =
                            ex.Message.StartsWith("ERROR|")
                                ? ex.Message.Substring("ERROR|".Length)
                                : "Ocurrió un error al consultar la información presupuestal.";
                    }
                }
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DSP(DSPViewModel vm)
        {
            // =====================================================
            // 1. VALIDAR MODELO RECIBIDO
            // =====================================================
            if (vm == null)
            {
                TempData["Warning"] = "La información recibida no es válida.";
                return RedirectToAction(nameof(Index));
            }

            if (vm.SolicitudMantenimientoId <= 0)
            {
                TempData["Warning"] = "La solicitud de mantenimiento no es válida.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // =====================================================
                // 2. RECUPERAR INFORMACIÓN PRESUPUESTAL DEL SERVIDOR
                // =====================================================
                var vmOriginal = await _maquinariaService.ObtenerDSP(vm.SolicitudMantenimientoId);

                if (vmOriginal == null)
                {
                    TempData["Warning"] = "No fue posible recuperar la información actual de la solicitud.";
                    return RedirectToAction(nameof(Index));
                }

                // =====================================================
                // 3. CONSERVAR OBSERVACIONES EDITABLES
                // =====================================================
                vmOriginal.Observaciones = vm.Observaciones;

                // =====================================================
                // 4. VALIDAR MODELSTATE
                // =====================================================
                if (!ModelState.IsValid)
                {
                    return View(vmOriginal);
                }

                // =====================================================
                // 5. VALIDAR PARTIDAS DEL SERVIDOR
                // =====================================================
                if (vmOriginal.PartidasSolicitud == null || !vmOriginal.PartidasSolicitud.Any())
                {
                    TempData["Warning"] = "La solicitud no contiene partidas presupuestales para validar.";
                    return View(vmOriginal);
                }

                if (vmOriginal.PartidasSolicitud.Any(x => x.IdObjetoGasto <= 0 || x.ImporteSolicitado <= 0))
                {
                    TempData["Warning"] = "La solicitud contiene partidas presupuestales inválidas. Verifique sus detalles antes de continuar.";
                    return View(vmOriginal);
                }

                // =====================================================
                // 6. CONSTRUIR DTO CON DATOS DEL SERVIDOR
                // =====================================================
                var dto = new DSPDto
                {
                    SolicitudMantenimientoId = vmOriginal.SolicitudMantenimientoId,
                    Observaciones = vmOriginal.Observaciones,
                    IdDispPresupuestal = 0, // Se conserva por compatibilidad
                    Partidas = vmOriginal.PartidasSolicitud
                        .GroupBy(x => x.IdObjetoGasto)
                        .Select(g => new DSPPartidaDto
                        {
                            IdObjetoGasto = g.Key,
                            ImporteSolicitado = g.Sum(x => x.ImporteSolicitado),
                            ClaveObjetoGasto = g.Select(x => x.ClaveObjetoGasto)
                                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                        })
                        .ToList()
                };

                // =====================================================
                // 7. PROCESAR OPERACIÓN PRESUPUESTAL
                // =====================================================
                var resultado = await _maquinariaService.ProcesarDSP(dto);

                // =====================================================
                // 8. MANEJAR RESULTADO FALLIDO
                // =====================================================
                if (resultado == null || !resultado.Exitoso)
                {
                    var mensaje = resultado?.Warnings != null && resultado.Warnings.Any()
                        ? string.Join(Environment.NewLine, resultado.Warnings)
                        : "No fue posible procesar la validación presupuestal de la solicitud.";

                    TempData["Warning"] = mensaje;
                    return View(vmOriginal);
                }

                // =====================================================
                // 9. OPERACIÓN EXITOSA CON O SIN ADVERTENCIAS
                // =====================================================
                if (resultado.Warnings != null && resultado.Warnings.Any())
                {
                    TempData["Warning"] = string.Join(Environment.NewLine, resultado.Warnings);
                }
                else
                {
                    TempData["Success"] = "DSP autorizada y validada presupuestalmente correctamente.";
                }

                // =====================================================
                // 10. REDIRECCIÓN
                // =====================================================
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // =====================================================
                // 11. CLASIFICAR MENSAJE CONTROLADO Y REGISTRAR LOG
                // =====================================================
                var mensaje = ex.Message ?? string.Empty;

                var esWarning = mensaje.StartsWith("WARNING|", StringComparison.OrdinalIgnoreCase);
                var esInfo = mensaje.StartsWith("INFO|", StringComparison.OrdinalIgnoreCase);

                if (esWarning || esInfo)
                {
                    mensaje = mensaje.Substring(mensaje.IndexOf('|') + 1).Trim();
                    TempData[esWarning ? "Warning" : "Info"] = string.IsNullOrWhiteSpace(mensaje)
                        ? "No fue posible completar la operación."
                        : mensaje;
                }
                else
                {
                    _logger.LogError(ex, "Error al procesar la validación DSP para la solicitud {SolicitudId}", vm.SolicitudMantenimientoId);
                    TempData["Error"] = "No fue posible procesar la validación presupuestal de la solicitud.";
                }

                // =====================================================
                // 12. RECUPERAR LA VISTA ACTUALIZADA
                // =====================================================
                try
                {
                    var vmOriginal = await _maquinariaService.ObtenerDSP(vm.SolicitudMantenimientoId);
                    if (vmOriginal != null)
                    {
                        vmOriginal.Observaciones = vm.Observaciones;
                        return View(vmOriginal);
                    }
                }
                catch
                {
                    // Evitar que excepciones secundarias al recargar la vista sobrescriban el mensaje original.
                }

                return RedirectToAction(nameof(Index));
            }
        }







        [HttpGet]
        public async Task<IActionResult> IngresarATaller(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService
                        .ObtenerSolicitudParaIngresoTaller(id);

                if (vm == null)
                {
                    TempData["Error"] =
                        "La solicitud no fue encontrada.";

                    return RedirectToAction(nameof(Index));
                }

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
        public async Task<IActionResult> IngresarATaller(MantenimientoCreateViewModel vm)
        {
            try
            {
                if (vm.SolicitudMantenimientoId <= 0)
                {
                    TempData["Error"] =
                        "Solicitud inválida.";

                    return RedirectToAction(nameof(Index));
                }

                await _maquinariaService.IngresarSolicitudATaller(vm.SolicitudMantenimientoId);

                TempData["Success"] =
                    "La solicitud fue ingresada al taller correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }




        // OBTIENE CONCEPTOS POR UNIDAD SELECCIONADA
        [HttpGet]
        public async Task<IActionResult> ObtenerConceptosPorUnidad(int unidadVehicularId)
        {
            var resultado = await _maquinariaService.ObtenerConceptosServicioFiltrados(unidadVehicularId);
            return Json(resultado);
        }




        [HttpGet]
        public async Task<IActionResult> ObtenerConceptosServicioPorTipo(int tipoCatalogoServicioId)
        {
            var data = await _maquinariaService
                .ObtenerConceptosPorTipo(tipoCatalogoServicioId);

            return Json(data);
        }




        [HttpPost]
        public async Task<IActionResult> ValidarServiciosHistoricos(int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId)
        {
            var resultado =
                await _maquinariaService.ValidarServiciosHistoricos(
                    unidadVehicularId,
                    conceptos,
                    tipoMantenimientoId);

            return Json(resultado);
        }




        [HttpPost]
        public async Task<IActionResult> ValidarServiciosEdit(int solicitudExcluirId, int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId)
        {
            var resultado =
                await _maquinariaService.ValidarServiciosHistoricos(
                    unidadVehicularId,
                    conceptos,
                    tipoMantenimientoId,
                    solicitudExcluirId);

            return Json(new
            {
                tieneRecurrencia = resultado.TieneRecurrencia,
                mensajes = resultado.Mensajes
            });
        }






        //[HttpGet]
        //public async Task<IActionResult> ImprimirDictamenSolicitud(int solicitudId)
        //{
        //    try
        //    {
        //        // Llamamos directamente al servicio que construye el PDF con QuestPDF
        //        byte[] pdfBytes = await _maquinariaService.GenerarDictamenTecnicoPdfAsync(solicitudId);

        //        // Omitimos el tercer parámetro (nombre de archivo) para que el navegador 
        //        // abra el visor embebido de PDF en una nueva pestaña en lugar de forzar la descarga.
        //        return File(pdfBytes, "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al generar el dictamen técnico en PDF para la solicitud {SolicitudId}", solicitudId);

        //        TempData["Error"] = $"No se pudo obtener el PDF del dictamen técnico: {ex.Message}";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> ImprimirDictamenSolicitud(int solicitudId)
        {
            try
            {
                // =====================================================
                // 1. GENERAR DOCUMENTO WORD
                // =====================================================

                byte[] wordBytes = await _maquinariaService.GenerarDictamenTecnicoWordAsync(solicitudId);

                // =====================================================
                // 2. DESCARGAR ARCHIVO DOCX
                // =====================================================

                string nombreArchivo = $"Dictamen_Tecnico_{solicitudId}.docx";

                return File(
                    wordBytes,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    nombreArchivo
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al generar el dictamen técnico en Word para la solicitud {SolicitudId}",
                    solicitudId
                );

                TempData["Error"] =
                    "No fue posible generar el documento Word del dictamen técnico. " +
                    "El problema ha sido registrado para su revisión.";

                return RedirectToAction(nameof(Index));
            }
        }







        [HttpGet]
        public async Task<IActionResult> VerDocumento(int id)
        {
            var doc = await _context.SolicitudMantenimientoDocumentos
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.SolicitudMantenimientoDocumentoId == id && d.Activo);

            if (doc == null)
                return NotFound("El documento especificado no existe o no está activo.");

            // Construir la ruta física completa de forma segura en el servidor
            string rutaFisica = Path.IsPathFullyQualified(doc.RutaArchivo)
                ? doc.RutaArchivo
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.RutaArchivo.TrimStart('~', '/', '\\'));

            if (!System.IO.File.Exists(rutaFisica))
                return NotFound("El archivo físico no fue encontrado en el servidor.");

            // Tipo MIME específico para documentos de Word (.docx)
            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            // Sirve el archivo
            return PhysicalFile(rutaFisica, contentType, doc.NombreArchivo);
        }





        /// <summary>
        /// Forza la descarga directa del documento adjunto a la computadora del usuario
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> DescargarDocumento(int id)
        //{
        //    var doc = await _context.SolicitudMantenimientoDocumentos
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(d => d.SolicitudMantenimientoDocumentoId == id && d.Activo);

        //    if (doc == null)
        //        return NotFound("El documento especificado no existe o no está activo.");

        //    string rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.RutaArchivo.TrimStart('~', '/'));

        //    if (!System.IO.File.Exists(rutaFisica))
        //        return NotFound("El archivo físico no fue encontrado en el servidor.");

        //    string contentType = ObtenerContentType(rutaFisica);

        //    // PhysicalFile con el parámetro fileDownloadName obliga al navegador a abrir la ventana de descarga
        //    return PhysicalFile(rutaFisica, contentType, fileDownloadName: doc.NombreArchivo);
        //}

        #region Helper
        private string ObtenerContentType(string rutaArchivo)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(rutaArchivo, out string? contentType))
            {
                contentType = "application/octet-stream"; // Genérico si no identifica la extensión
            }
            return contentType;
        }
        #endregion






        [HttpGet]
        public async Task<IActionResult> GenerarYGuardarDictamen(int solicitudId)
        {
            try
            {
                // 1. Consultar la solicitud de mantenimiento con todas sus relaciones requeridas
                var solicitud = await _context.SolicitudMantenimientos
                    .Include(s => s.AreaSolicitante)
                    .Include(s => s.EstadoOdometro)
                    .Include(s => s.EstatusSolicitudMantenimiento)
                    .Include(s => s.TipoMantenimiento)
                    .Include(s => s.Taller)
                    .Include(s => s.UnidadVehicular)
                        .ThenInclude(u => u.Marca)
                    .Include(s => s.UnidadVehicular)
                        .ThenInclude(u => u.Modelo)
                    .Include(s => s.UnidadVehicular)
                        .ThenInclude(u => u.Anio)
                    .Include(s => s.UnidadVehicular)
                        .ThenInclude(u => u.Cilindro)
                    .Include(s => s.SolicitudMantenimientoDetalles)
                        .ThenInclude(d => d.ConceptoServicio)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SolicitudMantenimientoId == solicitudId && s.Activo == true);

                if (solicitud == null)
                {
                    _logger.LogWarning("No se encontró la solicitud activa con ID {SolicitudId} para generar el dictamen técnico.", solicitudId);
                    TempData["Error"] = "No se encontró la solicitud de mantenimiento especificada.";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Generar el PDF del dictamen técnico a través de tu servicio de reportes (QuestPDF)
                byte[] pdfBytes = await _maquinariaService.GenerarDictamenTecnicoPdfAsync(solicitudId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    throw new Exception("El flujo de bytes generado para el dictamen técnico está vacío.");
                }

                // 3. Configurar la ruta física de almacenamiento desde appsettings.json
                var rutaBase = _configuration["Storage:DocumentosRuta"];
                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    throw new Exception("No se configuró la ruta de almacenamiento de documentos en el sistema.");
                }

                // Limpiar el formato del folio para evitar problemas con las barras diagonales en las rutas de Windows (ej. SM_001_2026)
                var carpetaFolioSegura = solicitud.Folio.Replace('/', '_');
                var carpetaSolicitud = Path.Combine(rutaBase, carpetaFolioSegura);

                if (!Directory.Exists(carpetaSolicitud))
                {
                    Directory.CreateDirectory(carpetaSolicitud);
                }

                string nombreArchivoFisico = $"DICTAMEN_{solicitudId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaCompleta = Path.Combine(carpetaSolicitud, nombreArchivoFisico);

                // 4. Guardar el archivo físicamente en el disco
                await System.IO.File.WriteAllBytesAsync(rutaCompleta, pdfBytes);

                // 5. Calcular el Hash SHA256 del documento para control de integridad
                string hashArchivo;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    hashArchivo = Convert.ToHexString(sha256.ComputeHash(pdfBytes));
                }

                // 6. Registrar el documento en la tabla SolicitudMantenimientoDocumento
                var documentoEntidad = new SolicitudMantenimientoDocumento
                {
                    SolicitudMantenimientoId = solicitudId,
                    TipoDocumento = "DICTAMEN_TECNICO",
                    NombreArchivo = $"Dictamen_Tecnico_{solicitud.NumeroOficio.Replace('/', '_')}.pdf",
                    RutaArchivo = rutaCompleta,
                    FechaCarga = DateTime.Now,
                    UsuarioCargaId = 1, // Puedes adaptarlo para capturar el ID del usuario actual de la sesión si aplica
                    Activo = true,
                    HashArchivo = hashArchivo
                };

                _context.SolicitudMantenimientoDocumentos.Add(documentoEntidad);
                await _context.SaveChangesAsync();

                TempData["Success"] = "El dictamen técnico se generó, guardó y registró exitosamente en los documentos de la solicitud.";

                // 7. Retornar el archivo PDF para descarga/visualización inmediata en el navegador
                return File(pdfBytes, "application/pdf", $"Dictamen_{solicitud.Folio.Replace('/', '_')}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar, guardar y registrar el dictamen técnico para la solicitud ID {SolicitudId}", solicitudId);
                TempData["Error"] = $"No se pudo generar o guardar el dictamen técnico: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }








        [HttpGet]
        public async Task<IActionResult> DescargarOficioSolicitud(int solicitudId)
        {
            try
            {
                // 1. Obtenemos los bytes del documento Word (.docx) compatible universalmente
                byte[] wordBytes = await _maquinariaService.GenerarOficioSolicitudWordAsync(solicitudId);

                var solicitud = await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId);

                var nombreArchivo = solicitud != null ? $"Oficio_{solicitud.Folio.Replace('/', '_')}.docx" : "Oficio_Solicitud.docx";

                // Tipo MIME oficial de Word OpenXML
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                return File(wordBytes, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo obtener el documento del oficio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }




        [HttpGet]
        public async Task<IActionResult> ObtenerUltimoKilometraje(int unidadVehicularId)
        {
            try
            {
                var ultimoKm = await _maquinariaService.ObtenerUltimoKilometrajeUnidadAsync(unidadVehicularId);
                return Json(new { ultimoKilometraje = ultimoKm });
            }
            catch
            {
                return Json(new { ultimoKilometraje = 0 });
            }
        }




    }
}
