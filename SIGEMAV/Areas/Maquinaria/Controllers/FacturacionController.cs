using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.Diagnostics.Eventing.Reader;
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
        public async Task<IActionResult> Index(string? filtroUnidad, string? filtroTaller, string? filtroArea, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            try
            {
                var user = HttpContext.User;

                // Extraer roles del usuario autenticado
                bool esAdminMaquinaria = user.IsInRole("AdminMaquinaria");
                bool esAreaOperativa = user.IsInRole("AreaOperativa");

                // Área por defecto del usuario
                string areaUsuario = "OFICIALÍA MAYOR";

                var vm = await _maquinariaService.ObtenerDashboardFacturacionAsync(
                    filtroUnidad,
                    filtroTaller,
                    filtroArea,
                    fechaDesde,
                    fechaHasta,
                    esAdminMaquinaria,
                    esAreaOperativa,
                    areaUsuario);

                // Enviamos el filtroArea actual para que el SelectList marque la opción seleccionada
                ViewBag.Areas = await ObtenerAreas(filtroArea);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista principal de facturación.");
                TempData["Error"] = "Ocurrió un error al cargar el panel de facturación. Intente nuevamente.";
                return View(new FacturaIndexViewModel());
            }
        }




        public async Task<List<SelectListItem>> ObtenerAreas(string? selectedValue = null)
        {


            //OBETENR EL CHIPOTE 


            var user = HttpContext.User;

            // Extraer roles del usuario autenticado
            bool esAdminMaquinaria = user.IsInRole("AdminMaquinaria");
            bool esAreaOperativa = user.IsInRole("AreaOperativa");

            // Área por defecto del usuario
            string areaUsuario = "OFICIALÍA MAYOR";


            if (esAreaOperativa)
            {
                
                    return await _context.Areas
                        .AsNoTracking()
                        .Where(x => x.Activo && x.AreaNombre == "OFICIALÍA MAYOR")
                        .OrderBy(x => x.AreaNombre)
                        .Select(x => new SelectListItem
                        {
                            Value = x.AreaId.ToString(),
                            Text = x.AreaNombre,
                            
                            Selected = !string.IsNullOrEmpty(selectedValue) && x.AreaId.ToString() == selectedValue
                        })
                        .ToListAsync();
                
            }
            else 
            {
                return await _context.Areas
                       .AsNoTracking()
                       .Where(x => x.Activo )
                       .OrderBy(x => x.AreaNombre)
                       .Select(x => new SelectListItem
                       {
                           Value = x.AreaId.ToString(),
                           Text = x.AreaNombre,
                           
                           Selected = !string.IsNullOrEmpty(selectedValue) && x.AreaId.ToString() == selectedValue
                       })
                       .ToListAsync();

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
                // CONTROL Y TRADUCCIÓN DE MENSAJES DE ERROR
                // -----------------------------------
                string mensajeOriginal = ex.InnerException?.Message ?? ex.Message ?? string.Empty;
                string mensajeLimpio;

                // 1. Identificar si es un error por índice único de UUID duplicado en SQL Server
                if (mensajeOriginal.Contains("UX_Factura_UUID", StringComparison.OrdinalIgnoreCase) ||
                    mensajeOriginal.Contains("Violation of UNIQUE KEY constraint", StringComparison.OrdinalIgnoreCase))
                {
                    mensajeLimpio = "El comprobante fiscal (UUID) que intenta registrar ya se encuentra dado de alta en el sistema.";
                    TempData["Warning"] = mensajeLimpio;
                }
                // 2. Identificar si viene de una regla de negocio lanzada explícitamente con prefijo
                else if (ex.Message.StartsWith("WARNING|"))
                {
                    TempData["Warning"] = ex.Message.Replace("WARNING|", "");
                }
                else if (ex.Message.StartsWith("INFO|"))
                {
                    TempData["Info"] = ex.Message.Replace("INFO|", "");
                }
                else if (ex.Message.StartsWith("SUCCESS|"))
                {
                    TempData["Success"] = ex.Message.Replace("SUCCESS|", "");
                }
                else
                {
                    // 3. Fallback genérico amigable en español en lugar del mensaje de EF en inglés
                    TempData["Error"] = "Ocurrió un error al procesar y guardar la factura. Verifique los datos e intente nuevamente.";
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
