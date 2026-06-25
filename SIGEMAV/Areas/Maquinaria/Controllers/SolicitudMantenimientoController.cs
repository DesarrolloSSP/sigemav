using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.ComponentModel.Design;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class SolicitudMantenimientoController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;
        

        public SolicitudMantenimientoController(IMaquinariaService maquinariaService, BdSigeMavContext context)
        {
            _maquinariaService = maquinariaService;
            _context = context;
        }






        [HttpGet]
        public async Task<IActionResult> Index(int pagina = 1, int registrosPorPagina = 10)
        {
            var resultado = await _maquinariaService.ObtenerSolicitudesMantenimiento(pagina, registrosPorPagina);

            return View(resultado);

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

                Areas = await _maquinariaService.ObtenerAreas(),
                Talleres = await _maquinariaService.ObtenerTalleres(),
                TiposMantenimiento = await _maquinariaService.ObtenerTiposMantenimiento(),

                ConceptosServicio = new List<SelectListItem>(),

                ObjetosGasto = await _maquinariaService.ObtenerObjetosGasto()
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
            if (
                vm.Servicios == null
                ||
                !vm.Servicios.Any())
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe capturar al menos un servicio.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCatalogos(vm);

                if (vm.UnidadVehicularId > 0)
                {
                    vm.UnidadVehicularTexto =
                        await _maquinariaService
                            .ObtenerTextoUnidadVehicular(
                                vm.UnidadVehicularId);
                }

                return View(vm);
            }

            if (!vm.AreaSolicitanteId.HasValue)
            {
                ModelState.AddModelError(nameof(vm.AreaSolicitanteId),
                    "Debe seleccionar una unidad vehicular válida.");

                await CargarCatalogos(vm);

                return View(vm);
            }

            try
            {
                // MAPEAR SERVICIOS

                var serviciosDto =

                    vm.Servicios

                    .Select(x =>

                        new SolicitudMantenimientoDetalleCreateDto
                        {
                            ConceptoServicioId =
                                x.ConceptoServicioId,

                            Cantidad =
                                x.Cantidad,

                            CostoEstimadoManoObra =
                                x.CostoEstimadoManoObra,

                            CostoEstimadoRefacciones =
                                x.CostoEstimadoRefacciones,

                            IdObjetoGastoManoObra =
                                x.IdObjetoGastoManoObra,

                            IdObjetoGastoRefacciones =
                                x.IdObjetoGastoRefacciones,

                            Observaciones =
                                x.Observaciones
                        })

                    .ToList();

                // =================================================
                // DTO PRINCIPAL
                // =================================================

                var dto =
                    new SolicitudMantenimientoCreateDto
                    {
                        Folio =
                            vm.Folio,

                        NumeroOficio =
                            vm.NumeroOficio,

                        FechaSolicitud =
                            vm.FechaSolicitud,

                        AreaSolicitanteId =
                            vm.AreaSolicitanteId.Value,

                        UnidadVehicularId =
                            vm.UnidadVehicularId,

                        TallerId =
                            vm.TallerId,

                        TipoMantenimientoId =
                            vm.TipoMantenimientoId,

                        KilometrajeActual =
                            vm.KilometrajeActual,

                        MotivoSolicitud =
                            vm.MotivoSolicitud,

                        DiagnosticoInicial =
                            vm.DiagnosticoInicial,

                        Observaciones =
                            vm.Observaciones,

                        ImporteCotizacion =
                            serviciosDto.Sum(x =>

                                x.CostoEstimadoManoObra
                                +
                                x.CostoEstimadoRefacciones),

                        EstatusSolicitudMantenimientoId =
                            vm.EstatusSolicitudMantenimientoId,

                        Servicios =
                            serviciosDto
                    };

                // CREAR SOLICITUD
                // incluye validacion historica
                await _maquinariaService.CrearSolicitudMantenimiento(dto,vm.Archivos);

                TempData["Success"] =
                    "La solicitud fue registrada correctamente.";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);

                await CargarCatalogos(vm);

                if (vm.UnidadVehicularId > 0)
                {
                    vm.UnidadVehicularTexto =
                        await _maquinariaService
                            .ObtenerTextoUnidadVehicular(
                                vm.UnidadVehicularId);
                }

                return View(vm);
            }
        }




        private async Task CargarCatalogos(SolicitudMantenimientoCreateViewModel vm)
        {
            vm.ConceptosServicio = new List<SelectListItem>();

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

            vm.ObjetosGasto =
                await _maquinariaService
                    .ObtenerObjetosGasto();
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _maquinariaService.ObtenerSolicitudMantenimientoEdit(id);



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
        public async Task<IActionResult> Edit(SolicitudMantenimientoEditViewModel vm, IEnumerable<IFormFile>? archivos)
        {
            try
            {
                // ============================================
                // RECUPERAR REGISTRO ACTUAL
                // ============================================

                var solicitudActual =
                    await _maquinariaService
                        .ObtenerSolicitudMantenimientoPorId(
                            vm.SolicitudMantenimientoId);

                if (solicitudActual == null)
                {
                    TempData["Error"] =
                        "La solicitud no fue encontrada.";

                    return RedirectToAction(nameof(Index));
                }

                // ============================================
                // PROTEGER CAMPOS NO EDITABLES
                // ============================================

                vm.Folio =
                    solicitudActual.Folio;

                vm.AreaSolicitanteId =
                    solicitudActual.AreaSolicitanteId;

                vm.UnidadVehicularId =
                    solicitudActual.UnidadVehicularId;

                vm.EstatusSolicitudMantenimientoId =
                    solicitudActual.EstatusSolicitudMantenimientoId;

                vm.FechaRegistro =
                    solicitudActual.FechaCreacion;

                // ============================================
                // VALIDAR SERVICIOS
                // ============================================

                if (vm.Servicios == null || !vm.Servicios.Any())
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Debe capturar al menos un servicio.");
                }

                // ============================================
                // VALIDACIONES MODELSTATE
                // ============================================

                if (!ModelState.IsValid)
                {
                    vm.Areas =
                        await _maquinariaService.ObtenerAreas();

                    vm.UnidadesVehiculares =
                        new List<SelectListItem>
                        {
                    new SelectListItem
                    {
                        Value =
                            solicitudActual.UnidadVehicularId
                                .ToString(),

                        Text =
                            solicitudActual.UnidadVehicularTexto,

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

                    vm.ConceptosServicio =
                    vm.TipoCatalogoServicioId > 0
                        ? await _maquinariaService.ObtenerConceptosServicio(vm.TipoCatalogoServicioId)
                        : new List<SelectListItem>();



                    return View(vm);
                }

                // ============================================
                // MAPEAR SERVICIOS
                // ============================================

                var serviciosDto =
                    vm.Servicios
                        .Select(x =>
                            new SolicitudMantenimientoDetalleCreateDto
                            {
                                ConceptoServicioId =
                                    x.ConceptoServicioId,

                                Cantidad =
                                    x.Cantidad,

                                CostoEstimadoManoObra =
                                    x.CostoEstimadoManoObra,

                                CostoEstimadoRefacciones =
                                    x.CostoEstimadoRefacciones,

                                IdObjetoGastoManoObra =
                                        x.IdObjetoGastoManoObra,

                                IdObjetoGastoRefacciones =
                                        x.IdObjetoGastoRefacciones,

                                Observaciones =
                                    x.Observaciones
                            })
                        .ToList();

                // ============================================
                // DTO PRINCIPAL
                // ============================================

                var dto =
                    new SolicitudMantenimientoEditDto
                    {
                        SolicitudMantenimientoId =
                            vm.SolicitudMantenimientoId,

                        Folio =
                            vm.Folio,

                        NumeroOficio =
                            vm.NumeroOficio?.Trim(),

                        FechaSolicitud =
                            vm.FechaSolicitud,

                        AreaSolicitanteId =
                            vm.AreaSolicitanteId,

                        UnidadVehicularId =
                            vm.UnidadVehicularId,

                        TallerId =
                            vm.TallerId,

                        TipoMantenimientoId =
                            vm.TipoMantenimientoId,

                        KilometrajeActual =
                            vm.KilometrajeActual,

                        MotivoSolicitud =
                            vm.MotivoSolicitud?.Trim(),

                        DiagnosticoInicial =
                            vm.DiagnosticoInicial?.Trim(),

                        Observaciones =
                            vm.Observaciones?.Trim(),

                        ImporteCotizacion =
                            vm.ImporteCotizacion,

                        EstatusSolicitudMantenimientoId =
                            vm.EstatusSolicitudMantenimientoId,

                        // ====================================
                        // SERVICIOS
                        // ====================================

                        Servicios =
                            serviciosDto
                    };

                // ============================================
                // ACTUALIZAR
                // ============================================

                //var resultadoHistorico = await _maquinariaService.ValidarServiciosHistoricos(dto.UnidadVehicularId,conceptosServicio,dto.TipoMantenimientoId);

                //if (resultadoHistorico.TieneDuplicadosActivos)
                //{
                //    foreach (var msg in resultadoHistorico.Mensajes)
                //    {
                //        ModelState.AddModelError(string.Empty, msg);
                //    }

                //    vm.ValidacionHistorica = resultadoHistorico;
                //    return View(vm);
                //}





                await _maquinariaService.EditarSolicitudMantenimiento(
                        dto,
                        archivos);

                TempData["Success"] =
                    "La solicitud fue actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var solicitudActual = await _maquinariaService.ObtenerSolicitudMantenimientoPorId(vm.SolicitudMantenimientoId);

                vm.Areas = await _maquinariaService.ObtenerAreas();

                vm.UnidadesVehiculares =
                    new List<SelectListItem>
                    {
            new SelectListItem
            {
                Value = vm.UnidadVehicularId.ToString(),
                Text = solicitudActual?.UnidadVehicularTexto ?? "Unidad seleccionada",
                Selected = true
            }
                    };

                vm.TiposMantenimiento =
                    await _maquinariaService.ObtenerTiposMantenimiento();

                vm.Talleres =
                    await _maquinariaService.ObtenerTalleres();

                vm.Estatuses =
                    await _maquinariaService.ObtenerEstatusSolicitudMantenimiento();

                vm.ConceptosServicio =
                    vm.TipoCatalogoServicioId > 0
                        ? await _maquinariaService.ObtenerConceptosServicio(vm.TipoCatalogoServicioId)
                        : new List<SelectListItem>();

                
                var mensajes = ex.Message
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var msg in mensajes)
                {
                    ModelState.AddModelError(string.Empty, msg);
                }

                return View(vm);
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




        public async Task<IActionResult> Revisar(int id)
        {
            try
            {
                var model =
                    await _maquinariaService.ObtenerRevisionSolicitud(id);

                return View(model);
            }
            catch (Exception ex)
            {

                //recibe mensajes personalizados

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
                }

                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }



        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Revisar(RevisionSolicitudViewModel vm, bool aprobada)
        {
            try
            {
                var dto = new RevisionSolicitudDto
                {
                    SolicitudMantenimientoId =
                        vm.SolicitudMantenimientoId,

                    Aprobada =
                        aprobada,

                    TipoRechazoSolicitudId =
                        aprobada
                            ? null
                            : vm.TipoRechazoSolicitudId,

                    Observaciones =
                        vm.Observaciones
                };

                await _maquinariaService.RevisarSolicitud(dto);

                TempData["Success"] = aprobada ? "Solicitud revisada correctamente." : "Solicitud rechazada.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // =========================================
                // RECARGAR MODELO COMPLETO
                // =========================================

                var modelCompleto =
                    await _maquinariaService
                        .ObtenerRevisionSolicitud(
                            vm.SolicitudMantenimientoId);

                // =========================================
                // PRESERVAR CAPTURA USUARIO
                // =========================================

                modelCompleto.Observaciones =
                    vm.Observaciones;

                modelCompleto.TipoRechazoSolicitudId =
                    vm.TipoRechazoSolicitudId;

                //ViewBag.Error = ex.Message;
                //recibe mensajes personalizados

                if (ex.Message.StartsWith("WARNING|"))
                {
                    TempData["Warning"] = ex.Message.Replace("WARNING|", "");

                    return View(modelCompleto);
                }
                else
                {
                    if (ex.Message.StartsWith("INFO|"))
                    {
                        TempData["Info"] = ex.Message.Replace("INFO|", "");

                        return View(modelCompleto);
                    }
                    else
                    {
                        TempData["Error"] = ex.Message;
                    }
                }



                return View(modelCompleto);
            }
        }






        [HttpGet]
        public async Task<JsonResult> BuscarUnidadesVehiculares(string term)
        {
            var resultado = await _maquinariaService.BuscarUnidadesVehiculares(term);

            return Json(resultado);
        }




        [HttpGet]
        public async Task<IActionResult> DescargarDocumento(int id)
        {
            var documento = await _context.SolicitudMantenimientoDocumentos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.SolicitudMantenimientoDocumentoId == id
                        &&
                        x.Activo);

            if (documento == null)
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(documento.RutaArchivo))
            {
                return NotFound();
            }

            var bytes =
                await System.IO.File.ReadAllBytesAsync(
                    documento.RutaArchivo);

            var contentType =
                "application/octet-stream";

            return File(
                bytes,
                contentType,
                documento.NombreArchivo);
        }





        // GET: DSPController
        [HttpGet]
        public async Task<IActionResult> DSP(int id)
        {

            try
            {
                var vm =
                    await _maquinariaService.ObtenerDSP(id);

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
                        TempData["Error"] = ex.Message;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DSP(DSPViewModel vm)
        {
            try
            {

                var cintador = vm.PartidasDSP.Count;


                var vmOriginal =
                    await _maquinariaService
                        .ObtenerDSP(vm.SolicitudMantenimientoId);

                if (!ModelState.IsValid)
                {
                    vmOriginal.Observaciones =
                        vm.Observaciones;

                    return View(vmOriginal);
                }

                await _maquinariaService.ProcesarDSP(
                     new DSPDto
                     {
                         SolicitudMantenimientoId =
                             vm.SolicitudMantenimientoId,

                         Observaciones =
                             vm.Observaciones,

                         Partidas =
                             vm.PartidasSolicitud
                                 .Select(x => new DSPPartidaDto
                                 {
                                     IdObjetoGasto =
                                         x.IdObjetoGasto,

                                     ImporteSolicitado =
                                         x.ImporteSolicitado,

                                     ClaveObjetoGasto =
                                         x.ClaveObjetoGasto
                                 })
                                 .ToList()
                     });

                TempData["Success"] =
                    "DSP autorizado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var vmOriginal =
                    await _maquinariaService
                        .ObtenerDSP(vm.SolicitudMantenimientoId);

                vmOriginal.Observaciones =
                    vm.Observaciones;

                if (ex.Message.StartsWith("WARNING|"))
                {
                    TempData["Warning"] =
                        ex.Message.Replace("WARNING|", "");

                    return View(vmOriginal);
                }

                if (ex.Message.StartsWith("INFO|"))
                {
                    TempData["Info"] =
                        ex.Message.Replace("INFO|", "");

                    return View(vmOriginal);
                }

                TempData["Error"] = ex.Message;

                return View(vmOriginal);
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

                await _maquinariaService
                    .IngresarSolicitudATaller(
                        vm.SolicitudMantenimientoId);

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


        [HttpGet]
        public async Task<IActionResult>ObtenerConceptosServicioPorUnidad(int unidadVehicularId)
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
        public async Task<IActionResult> ValidarServiciosHistoricos(int unidadVehicularId,List<int> conceptos,int tipoMantenimientoId)
        {
            var resultado =
                await _maquinariaService.ValidarServiciosHistoricos(
                    unidadVehicularId,
                    conceptos,
                    tipoMantenimientoId);

            return Json(resultado);
        }




        [HttpPost]
        public async Task<IActionResult> ValidarServiciosEdit(int unidadVehicularId,List<int> conceptos,int tipoMantenimientoId)
        {
            var resultado =
                await _maquinariaService.ValidarServiciosHistoricos(
                    unidadVehicularId,
                    conceptos,
                    tipoMantenimientoId);

            return Json(resultado);
        }



    }
}
