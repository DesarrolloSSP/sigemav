using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]


    public class TallerConfiguracionController : Controller
    {


        private readonly IMaquinariaService _maquinariaService;
        private readonly ILogger<TallerConfiguracionController> _logger;

        public TallerConfiguracionController(IMaquinariaService maquinariaService, ILogger<TallerConfiguracionController> logger)
        {
            _maquinariaService = maquinariaService;
            _logger = logger;
        }




        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var configuraciones =
                    await _maquinariaService
                        .ObtenerConfiguracionesTallerAsync();

                return View(configuraciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar las configuraciones de talleres.");

                TempData["Error"] =
                    "Ocurrió un error al cargar las configuraciones de talleres.";

                return View(new List<TallerConfiguracionDto>());
            }
        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model =
                    new TallerConfiguracionCreateViewModel
                    {
                        Activo = true
                    };

                await _maquinariaService
                    .CargarCatalogosTallerConfiguracionAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar formulario de creación de configuración de taller.");

                TempData["Error"] =
                    "No fue posible cargar el formulario de configuración del taller.";

                return RedirectToAction(nameof(Index));
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TallerConfiguracionCreateViewModel model)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    try
                    {
                        await _maquinariaService
                            .CargarCatalogosTallerConfiguracionAsync(model);
                    }
                    catch (Exception exCatalogos)
                    {
                        _logger.LogError(
                            exCatalogos,
                            "Error al recargar catálogos de configuración de taller después de una validación inválida.");

                        ModelState.AddModelError(
                            string.Empty,
                            "No fue posible cargar nuevamente los catálogos del formulario.");
                    }

                    return View(model);
                }


                // =====================================================
                // 2. CREAR DTO
                // =====================================================

                var dto =
                    new TallerConfiguracionCreateDto
                    {
                        TallerId =
                            model.TallerId,

                        TallerConfiguracionPresupuestalId =
                            model.TallerConfiguracionPresupuestalId,

                        Activo =
                            model.Activo
                    };


                // =====================================================
                // 3. EJECUTAR SERVICIO
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .CrearTallerConfiguracionAsync(
                            dto,
                            User);


                // =====================================================
                // 4. VALIDAR RESPUESTA DEL SERVICIO
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    try
                    {
                        await _maquinariaService
                            .CargarCatalogosTallerConfiguracionAsync(model);
                    }
                    catch (Exception exCatalogos)
                    {
                        _logger.LogError(
                            exCatalogos,
                            "Error al recargar catálogos de configuración de taller después de un error de negocio.");

                        ModelState.AddModelError(
                            string.Empty,
                            "No fue posible cargar nuevamente los catálogos del formulario.");
                    }

                    return View(model);
                }


                // =====================================================
                // 5. ÉXITO
                // =====================================================

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                // =====================================================
                // 6. ERROR NO CONTROLADO
                // =====================================================

                _logger.LogError(
                    ex,
                    "Error no controlado al crear configuración de taller.");

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al crear la configuración del taller. Intente nuevamente.");

                try
                {
                    await _maquinariaService
                        .CargarCatalogosTallerConfiguracionAsync(model);
                }
                catch (Exception exCatalogos)
                {
                    _logger.LogError(
                        exCatalogos,
                        "Error al recargar catálogos después de una excepción al crear configuración de taller.");

                    ModelState.AddModelError(
                        string.Empty,
                        "No fue posible cargar nuevamente los catálogos del formulario.");
                }

                return View(model);
            }
        }






        [HttpGet]
        public async Task<IActionResult> Edit(
     int tallerId,
     int configuracionId)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR IDS
                // =====================================================

                if (tallerId <= 0 ||
                    configuracionId <= 0)
                {
                    TempData["Error"] =
                        "La asociación del taller no es válida.";

                    return RedirectToAction(nameof(Index));
                }


                // =====================================================
                // 2. OBTENER ASOCIACIÓN
                // =====================================================

                var model =
                    await _maquinariaService
                        .ObtenerTallerConfiguracionParaEditarAsync(
                            tallerId,
                            configuracionId);


                if (model == null)
                {
                    TempData["Error"] =
                        "No se encontró la configuración asociada al taller.";

                    return RedirectToAction(nameof(Index));
                }


                // =====================================================
                // 3. CARGAR CONFIGURACIONES PRESUPUESTALES
                // =====================================================

                await _maquinariaService
                    .CargarConfiguracionesPresupuestalesParaEditarAsync(
                        model);


                // =====================================================
                // 4. MOSTRAR VISTA
                // =====================================================

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar edición de configuración del taller {TallerId} con configuración {ConfiguracionId}.",
                    tallerId,
                    configuracionId);

                TempData["Error"] =
                    "Ocurrió un error al cargar la configuración del taller.";

                return RedirectToAction(nameof(Index));
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TallerConfiguracionEditViewModel model)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    try
                    {
                        await _maquinariaService
                            .CargarConfiguracionesPresupuestalesParaEditarAsync(
                                model);
                    }
                    catch (Exception exCatalogos)
                    {
                        _logger.LogError(
                            exCatalogos,
                            "Error al recargar configuraciones presupuestales para editar taller {TallerId}.",
                            model.TallerId);

                        ModelState.AddModelError(
                            string.Empty,
                            "No fue posible cargar nuevamente las configuraciones presupuestales.");
                    }

                    return View(model);
                }


                // =====================================================
                // 2. VALIDAR TALLER
                // =====================================================

                if (model.TallerId <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.TallerId),
                        "El taller no es válido.");

                    await RecargarConfiguracionesAsync(model);

                    return View(model);
                }


                // =====================================================
                // 3. VALIDAR CONFIGURACIÓN ORIGINAL
                // =====================================================

                if (model.ConfiguracionPresupuestalOriginalId <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.ConfiguracionPresupuestalOriginalId),
                        "La configuración presupuestal original no es válida.");

                    await RecargarConfiguracionesAsync(model);

                    return View(model);
                }


                // =====================================================
                // 4. VALIDAR CONFIGURACIÓN DESTINO
                // =====================================================

                if (model.TallerConfiguracionPresupuestalId <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.TallerConfiguracionPresupuestalId),
                        "Debe seleccionar una configuración presupuestal.");

                    await RecargarConfiguracionesAsync(model);

                    return View(model);
                }


                // =====================================================
                // 5. CREAR DTO
                // =====================================================

                var dto =
                    new TallerConfiguracionEditDto
                    {
                        TallerId =
                            model.TallerId,

                        ConfiguracionPresupuestalOriginalId =
                            model.ConfiguracionPresupuestalOriginalId,

                        TallerConfiguracionPresupuestalId =
                            model.TallerConfiguracionPresupuestalId,

                        Activo =
                            model.Activo
                    };


                // =====================================================
                // 6. ACTUALIZAR
                // =====================================================
                var resultado = await _maquinariaService.EditarTallerConfiguracionAsync(dto);

                // =====================================================
                // 7. VALIDAR RESPUESTA
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    await RecargarConfiguracionesAsync(model);

                    return View(model);
                }


                // =====================================================
                // 8. ÉXITO
                // =====================================================

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error no controlado al editar configuración del taller {TallerId}.",
                    model.TallerId);

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al actualizar la configuración del taller. Intente nuevamente.");

                await RecargarConfiguracionesAsync(model);

                return View(model);
            }
        }





        private async Task RecargarConfiguracionesAsync(TallerConfiguracionEditViewModel model)
        {
            try
            {
                await _maquinariaService
                    .CargarConfiguracionesPresupuestalesParaEditarAsync(
                        model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al recargar configuraciones presupuestales para taller {TallerId}.",
                    model.TallerId);

                ModelState.AddModelError(
                    string.Empty,
                    "No fue posible cargar nuevamente las configuraciones presupuestales.");
            }
        }







    }
}
