using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.Security.Claims;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{

    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin,AreaOperativa")]
    public class FacturacionController : Controller
    {

        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;
        private readonly IWebHostEnvironment _environment;

        private readonly ILogger<SolicitudMantenimientoController> _logger;



        public FacturacionController(IMaquinariaService maquinariaService, BdSigeMavContext context, IWebHostEnvironment environment, ILogger<SolicitudMantenimientoController> logger)
        {
            _maquinariaService = maquinariaService;
            _context = context;
            _environment = environment;
            _logger = logger;
        }



        [HttpGet]
        public async Task<IActionResult> Index(string? filtroUnidad,string? filtroTaller,DateTime? fechaDesde,DateTime? fechaHasta)
        {
            try
            {
                var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var vm = await _maquinariaService.ObtenerDashboardFacturacionAsync(
                    filtroUnidad,
                    filtroTaller,
                    fechaDesde,
                    fechaHasta);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista principal de facturación.");
                TempData["Error"] = "Ocurrió un error al cargar el panel de facturación. Intente nuevamente.";
                return View(new FacturaIndexViewModel());
            }
        }




        // =====================================================
        // PREPARAR VISTA
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Create(int? mantenimientoId)
        {
            try
            {
                if (!mantenimientoId.HasValue || mantenimientoId <= 0)
                {
                    TempData["Error"] =
                        "Debe seleccionar un mantenimiento para facturar.";

                    return RedirectToAction("Index", "Mantenimiento");
                }

                var vm = await _maquinariaService.PrepararVistaAsync(mantenimientoId.Value);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Mantenimiento");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaMantenimientoViewModel vm, IFormFile xmlFactura, IFormFile pdfFactura)
        {
            try
            {
                // -----------------------------------
                // 1. LIMPIAR MODELSTATE VISUAL
                // -----------------------------------
                ModelState.Remove(nameof(vm.UUID));
                ModelState.Remove(nameof(vm.Serie));
                ModelState.Remove(nameof(vm.Folio));
                ModelState.Remove(nameof(vm.RFCEmisor));
                ModelState.Remove(nameof(vm.RFCReceptor));
                ModelState.Remove(nameof(vm.RazonSocial));
                ModelState.Remove(nameof(vm.MetodoPago));
                ModelState.Remove(nameof(vm.FormaPago));
                ModelState.Remove(nameof(vm.UsoCFDI));
                ModelState.Remove(nameof(vm.Moneda));
                ModelState.Remove(nameof(vm.TipoComprobante));
                ModelState.Remove(nameof(vm.Subtotal));
                ModelState.Remove(nameof(vm.IVA));
                ModelState.Remove(nameof(vm.Total));
                ModelState.Remove(nameof(vm.Detalles));

                // -----------------------------------
                // 2. VALIDACIÓN DE ARCHIVOS
                // -----------------------------------
                if (xmlFactura == null || xmlFactura.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo XML válido.");
                }

                if (pdfFactura == null || pdfFactura.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo PDF válido.");
                }

                // -----------------------------------
                // 3. REVALIDACIÓN DE FORMULARIO
                // -----------------------------------
                if (!ModelState.IsValid)
                {
                    var vista = await _maquinariaService.PrepararVistaAsync(vm.MantenimientoId);
                    vista.FechaRecepcion = vm.FechaRecepcion;
                    vista.Observaciones = string.IsNullOrWhiteSpace(vm.Observaciones) ? null : vm.Observaciones.Trim();
                    return View(vista);
                }

                // -----------------------------------
                // 4. REGISTRO Y VALIDACIÓN
                // -----------------------------------
                var dto = new FacturaMantenimientoCreateDto
                {
                    MantenimientoId = vm.MantenimientoId,
                    FechaRecepcion = vm.FechaRecepcion,
                    Observaciones = vm.Observaciones
                };

                var sat = await _maquinariaService.RegistrarFacturaAsync(dto, xmlFactura, pdfFactura);

                if (sat.ConsultaExitosa)
                {
                    TempData["Success"] = $"Factura registrada y validada exitosamente ante el SAT ({sat.Estado}).";
                }
                else
                {
                    TempData["Warning"] = "Factura registrada correctamente. La validación ante el SAT quedó pendiente debido a indisponibilidad del servicio.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // -----------------------------------
                // CONTROL Y FORMATO DE EXCEPCIONES
                // -----------------------------------
                string mensaje = ex.Message ?? "Ocurrió un error inesperado al procesar la factura.";

                if (mensaje.StartsWith("WARNING|"))
                {
                    TempData["Warning"] = mensaje.Replace("WARNING|", "");
                }
                else if (mensaje.StartsWith("INFO|"))
                {
                    TempData["Info"] = mensaje.Replace("INFO|", "");
                }
                else if (mensaje.StartsWith("SUCCESS|"))
                {
                    TempData["Success"] = mensaje.Replace("SUCCESS|", "");
                }
                else
                {
                    TempData["Error"] = mensaje.Replace("ERROR|", "");
                }

                // -----------------------------------
                // RECARGAR VISTA Y RETENER VALORES
                // -----------------------------------
                var vista = await _maquinariaService.PrepararVistaAsync(vm.MantenimientoId);
                vista.FechaRecepcion = vm.FechaRecepcion;
                vista.Observaciones = string.IsNullOrWhiteSpace(vm.Observaciones) ? null : vm.Observaciones.Trim();

                return View(vista);
            }
        }





        public async Task<IActionResult> Details(int id)
        {
            var factura =
                await _context
                    .FacturaMantenimientos

                    .Include(x => x.Taller)

                    .Include(x =>
                        x.FacturaMantenimientoDetalles)

                    .Include(x =>
                        x.FacturaMantenimientoDocumentos)

                    .AsNoTracking()

                    .FirstOrDefaultAsync(x =>
                        x.FacturaMantenimientoId == id);

            if (factura == null)
            {
                TempData["Warning"] =
                    "Factura no encontrada.";

                return RedirectToAction(
                    nameof(Index));
            }

            //-------------------------------------------------
            // CATÁLOGO CONCEPTOS
            //-------------------------------------------------

            var conceptosCatalogo =
                await _context
                    .ConceptoServicios
                    .AsNoTracking()
                    .ToDictionaryAsync(
                        x => x.ConceptoServicioId,
                        x => x.ConceptoServicioNombre);

            //-------------------------------------------------
            // MAPEO
            //-------------------------------------------------

            var vm =
                new FacturaDetalleViewModel
                {
                    FacturaMantenimientoId =
                        factura.FacturaMantenimientoId,

                    UUID =
                        factura.Uuid,

                    Serie =
                        factura.Serie,

                    Folio =
                        factura.Folio,

                    Taller =
                        factura.Taller.RazonSocial,

                    FechaFactura =
                        factura.FechaFactura,

                    FechaRecepcion =
                        DateOnly.FromDateTime(
                            factura.FechaRecepcion),

                    RFCEmisor =
                        factura.Rfcemisor,

                    RFCReceptor =
                        factura.Rfcreceptor,

                    RazonSocial =
                        factura.RazonSocial,

                    MetodoPago =
                        factura.MetodoPago,

                    FormaPago =
                        factura.FormaPago,

                    UsoCFDI =
                        factura.UsoCfdi,

                    Moneda =
                        factura.Moneda,

                    Subtotal =
                        factura.Subtotal ?? 0,

                    IVA =
                        factura.Iva ?? 0,

                    Retencion =
                        factura.Retencion ?? 0,

                    Total =
                        factura.Total ?? 0,

                    Observaciones =
                        factura.Observaciones,

                    //-------------------------------------------------
                    // DETALLES
                    //-------------------------------------------------

                    Conceptos =
                        factura
                        .FacturaMantenimientoDetalles
                        .Select(x =>
                            new FacturaDetalleConceptoVm
                            {
                                Concepto =
                                    conceptosCatalogo
                                        .ContainsKey(
                                            x.ConceptoServicioId ?? 0)

                                    ?

                                    conceptosCatalogo[
                                        x.ConceptoServicioId.Value]

                                    :

                                    "Sin concepto",

                                Cantidad =
                                    x.Cantidad ?? 0,

                                CostoManoObra =
                                    x.CostoManoObra ?? 0,

                                CostoRefacciones =
                                    x.CostoRefacciones ?? 0,

                                Subtotal =
                                    x.Subtotal ?? 0,

                                IVA =
                                    x.Iva ?? 0,

                                TotalLinea =
                                    x.TotalLinea ?? 0
                            })
                        .ToList(),

                    //-------------------------------------------------
                    // DOCUMENTOS
                    //-------------------------------------------------

                    Documentos =
                        factura
                        .FacturaMantenimientoDocumentos
                        .Where(x =>
                            x.Activo == true)

                        .Select(x =>
                            new FacturaDocumentoVm
                            {
                                FacturaDocumentoId =
                                    x.FacturaDocumentoId,

                                TipoDocumento =
                                    x.TipoDocumento,

                                NombreArchivo =
                                    x.NombreArchivo,

                                RutaArchivo =
                                    x.RutaArchivo
                            })

                        .ToList()
                };

            return View(vm);
        }






        public async Task<IActionResult>VerDocumento(int id)
        {
            var doc =
                await _context
                .FacturaMantenimientoDocumentos
                .FirstOrDefaultAsync(x =>
                    x.FacturaDocumentoId == id);

            if (doc == null)
                return NotFound();

            var ruta =
                Path.Combine(_environment.WebRootPath,
                    doc.RutaArchivo);

            if (!System.IO.File.Exists(ruta))
                return NotFound();

            var bytes =
                await System.IO.File
                .ReadAllBytesAsync(ruta);

            var mime =
                doc.TipoDocumento == "PDF"
                ? "application/pdf"
                : "text/xml";

            return File(
                bytes,
                mime,
                doc.NombreArchivo);
        }



        //CARGAR


        //. ValidarFacturaAsync(CFDI + duplicados + cruce SAT)
        // . PrepararVistaAsync(con mantenimiento + partidas + presupuesto disponible)
        // . RegistrarFacturaAsync(con transacción + cruce presupuestal)




    }
}
