using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Implementations;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;


namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]

    public class TallerController : Controller
    {

        private readonly IMaquinariaService _maquinariaService;

        private readonly ILogger<TallerConfiguracionController> _logger;

        public TallerController(IMaquinariaService maquinariaService, ILogger<TallerConfiguracionController> logger)
        {
            _maquinariaService = maquinariaService;
            _logger = logger;
        }




        // =========================================================
        // INDEX
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(
      string? busqueda,
      bool? activo,
      int pagina = 1)
        {
            try
            {
                // =================================================
                // 1. VALIDAR PÁGINA
                // =================================================

                if (pagina < 1)
                    pagina = 1;


                // =================================================
                // 2. OBTENER TALLERES
                // =================================================

                var resultado =
                    await _maquinariaService
                        .ObtenerTalleresAsync(
                            pagina,
                            10,
                            busqueda,
                            activo);


                // =================================================
                // 3. MAPEAR DTO → VIEWMODEL
                // =================================================

                var modelo =
                    new PaginacionViewModel<TallerViewModel>
                    {
                        Registros =
                            resultado.Registros
                                .Select(x =>
                                    new TallerViewModel
                                    {
                                        TallerId =
                                            x.TallerId,

                                        RazonSocial =
                                            x.RazonSocial,

                                        Rfc =
                                            x.Rfc,

                                        RecursoId =
                                            x.RecursoId,

                                        RecursoNombre =
                                            x.RecursoNombre,

                                        MontoMinimo =
                                            x.MontoMinimo,

                                        Contrato =
                                            x.Contrato,

                                        BancoId =
                                            x.BancoId,

                                        BancoNombre =
                                            x.BancoNombre,

                                        Fianza =
                                            x.Fianza,

                                        Clabe =
                                            x.Clabe,

                                        Ubicacion =
                                            x.Ubicacion,

                                        Activo =
                                            x.Activo,

                                        MontoMaximo =
                                            x.MontoMaximo
                                    })
                                .ToList(),

                        PaginaActual =
                                resultado.PaginaActual,

                        RegistrosPorPagina =
                                resultado.RegistrosPorPagina,

                        TotalRegistros =
                                resultado.TotalRegistros,

                        TotalPaginas =
                                resultado.TotalPaginas
                    };


                // =================================================
                // 4. CONSERVAR FILTROS
                // =================================================

                ViewData["Busqueda"] =
                    busqueda;

                ViewData["Activo"] =
                    activo;


                // =================================================
                // 5. RETORNAR VISTA
                // =================================================

                return View(modelo);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al consultar los talleres.";

                return View(
                    new PaginacionViewModel<TallerViewModel>());
            }
        }






        // GET: TallerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                // =====================================================
                // 1. OBTENER CATÁLOGOS
                // =====================================================

                var recursos =
                    await _maquinariaService
                        .ObtenerRecursosActivosAsync();

                var bancos =
                    await _maquinariaService
                        .ObtenerBancosActivosAsync();

                var objetivosMantenimiento =
                    await _maquinariaService
                        .ObtenerObjetivosMantenimientoActivosAsync();


                // =====================================================
                // 2. CONSTRUIR VIEWMODEL
                // =====================================================

                var modelo =
                    new TallerCreateViewModel
                    {
                        Activo = true,

                        Recursos =
                            recursos.Select(x =>
                                new SelectListItem
                                {
                                    Value =
                                        x.RecursoId.ToString(),

                                    Text =
                                        x.Nombre
                                }),

                        Bancos =
                            bancos.Select(x =>
                                new SelectListItem
                                {
                                    Value =
                                        x.BancoId.ToString(),

                                    Text =
                                        x.Nombre
                                }),

                        ObjetivosMantenimiento =
                            objetivosMantenimiento.Select(x =>
                                new SelectListItem
                                {
                                    Value =
                                        x.ObjetivoMantenimientoId.ToString(),

                                    Text =
                                        x.Nombre
                                })
                    };


                // =====================================================
                // 3. MOSTRAR FORMULARIO
                // =====================================================

                return View(modelo);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al cargar el formulario de registro del taller.";

                return RedirectToAction(
                    nameof(Index));
            }
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TallerCreateViewModel model)
        {
            try
            {
                // =====================================================
                // 1. NORMALIZAR DATOS DE ENTRADA
                // =====================================================

                model.RazonSocial =
                    model.RazonSocial?.Trim() ?? string.Empty;

                model.Rfc =
                    model.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

                model.Contrato =
                    model.Contrato?.Trim() ?? string.Empty;

                model.Fianza =
                    model.Fianza?.Trim() ?? string.Empty;

                model.Clabe =
                    model.Clabe?.Trim() ?? string.Empty;

                model.Ubicacion =
                    model.Ubicacion?.Trim() ?? string.Empty;


                // =====================================================
                // 2. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarCatalogosTallerAsync(model);

                    return View(model);
                }


                // =====================================================
                // 3. VALIDAR RFC
                // =====================================================

                if (model.Rfc.Length != 12 &&
                    model.Rfc.Length != 13)
                {
                    ModelState.AddModelError(
                        nameof(model.Rfc),
                        "El RFC debe contener 12 o 13 caracteres.");
                }


                // =====================================================
                // 4. VALIDAR FIANZA
                // =====================================================

                if (string.IsNullOrWhiteSpace(model.Fianza))
                {
                    ModelState.AddModelError(
                        nameof(model.Fianza),
                        "La fianza es obligatoria.");
                }


                // =====================================================
                // 5. VALIDAR CLABE
                // =====================================================

                if (string.IsNullOrWhiteSpace(model.Clabe))
                {
                    ModelState.AddModelError(
                        nameof(model.Clabe),
                        "La CLABE interbancaria es obligatoria.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.Clabe,
                    @"^\d{18}$"))
                {
                    ModelState.AddModelError(
                        nameof(model.Clabe),
                        "La CLABE debe contener exactamente 18 dígitos.");
                }


                // =====================================================
                // 6. VALIDAR MONTOS
                // =====================================================

                if (model.MontoMinimo.HasValue &&
                    model.MontoMaximo.HasValue &&
                    model.MontoMaximo.Value <
                    model.MontoMinimo.Value)
                {
                    ModelState.AddModelError(
                        nameof(model.MontoMaximo),
                        "El monto máximo no puede ser menor que el monto mínimo.");
                }


                // =====================================================
                // 7. VALIDAR PORCENTAJES
                // =====================================================

                if (model.PorcentajeAlerta.HasValue &&
                    model.PorcentajeCritico.HasValue)
                {
                    if (model.PorcentajeCritico.Value >=
                        model.PorcentajeAlerta.Value)
                    {
                        ModelState.AddModelError(
                            nameof(model.PorcentajeCritico),
                            "El porcentaje crítico debe ser menor que el porcentaje de alerta.");
                    }
                }


                // =====================================================
                // 8. CALCULAR MONTOS DE CONTROL PRESUPUESTAL
                // =====================================================

                if (model.MontoMaximo.HasValue &&
                    model.PorcentajeAlerta.HasValue)
                {
                    model.MontoAlerta =
                        Math.Round(
                            model.MontoMaximo.Value *
                            model.PorcentajeAlerta.Value /
                            100m,
                            2);
                }


                if (model.MontoMaximo.HasValue &&
                    model.PorcentajeCritico.HasValue)
                {
                    model.MontoCritico =
                        Math.Round(
                            model.MontoMaximo.Value *
                            model.PorcentajeCritico.Value /
                            100m,
                            2);
                }


                // =====================================================
                // 9. VALIDAR ERRORES ADICIONALES
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarCatalogosTallerAsync(model);

                    return View(model);
                }


                // =====================================================
                // 10. CONSTRUIR DTO
                // =====================================================

                var dto =
                    new TallerCreateDto
                    {
                        // -------------------------------------------------
                        // DATOS DEL TALLER / UBICACIÓN
                        // -------------------------------------------------

                        RazonSocial =
                            model.RazonSocial,

                        Rfc =
                            model.Rfc,

                        BancoId =
                            model.BancoId,

                        Fianza =
                            model.Fianza,

                        Clabe =
                            model.Clabe,

                        ObjetivoMantenimientoId =
                            model.ObjetivoMantenimientoId,

                        Ubicacion =
                            model.Ubicacion,

                        Activo =
                            model.Activo,


                        // -------------------------------------------------
                        // CONFIGURACIÓN PRESUPUESTAL
                        // -------------------------------------------------

                        RecursoId =
                            model.RecursoId,

                        Contrato =
                            model.Contrato,

                        MontoMinimo =
                            model.MontoMinimo!.Value,

                        MontoMaximo =
                            model.MontoMaximo!.Value,

                        PorcentajeAlerta =
                            model.PorcentajeAlerta!.Value,

                        MontoAlerta =
                            model.MontoAlerta!.Value,

                        PorcentajeCritico =
                            model.PorcentajeCritico!.Value,

                        MontoCritico =
                            model.MontoCritico!.Value
                    };


                // =====================================================
                // 11. DELEGAR CREACIÓN AL SERVICIO
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .CrearTallerAsync(dto);


                // =====================================================
                // 12. VALIDAR RESULTADO
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    await CargarCatalogosTallerAsync(model);

                    return View(model);
                }


                // =====================================================
                // 13. MENSAJE DE ÉXITO
                // =====================================================

                TempData["Success"] =
                    "El taller se registró correctamente.";


                // =====================================================
                // 14. REDIRECCIÓN
                // =====================================================

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception)
            {
                // =====================================================
                // ERROR NO CONTROLADO
                // =====================================================

                TempData["Error"] =
                    "Ocurrió un error al registrar el taller.";

                await CargarCatalogosTallerAsync(model);

                return View(model);
            }
        }








        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR ID
                // =====================================================

                if (id <= 0)
                {
                    TempData["Error"] =
                        "El taller especificado no es válido.";

                    return RedirectToAction(
                        nameof(Index));
                }


                // =====================================================
                // 2. OBTENER TALLER
                // =====================================================

                var taller =
                    await _maquinariaService
                        .ObtenerTallerParaEditarAsync(id);


                if (taller == null)
                {
                    TempData["Error"] =
                        "El taller especificado no existe.";

                    return RedirectToAction(
                        nameof(Index));
                }


                // =====================================================
                // 3. CONSTRUIR VIEWMODEL
                // =====================================================

                var model =
                    new TallerEditViewModel
                    {
                        TallerId =
                            taller.TallerId,

                        // -------------------------------------------------
                        // Datos del taller
                        // -------------------------------------------------

                        RazonSocial =
                            taller.RazonSocial,

                        Rfc =
                            taller.Rfc,

                        BancoId =
                            taller.BancoId,

                        Fianza =
                            taller.Fianza,

                        Clabe =
                            taller.Clabe,

                        ObjetivoMantenimientoId = (int)taller.ObjetivoMantenimientoId,

                        Ubicacion =
                            taller.Ubicacion,

                        Activo =
                            taller.Activo,


                        // -------------------------------------------------
                        // Configuración presupuestal
                        // SOLO LECTURA
                        // -------------------------------------------------

                        RecursoNombre =
                            taller.RecursoNombre,

                        Contrato =
                            taller.Contrato,

                        MontoMinimo =
                            taller.MontoMinimo,

                        MontoMaximo =
                            taller.MontoMaximo,

                        PorcentajeAlerta =
                            taller.PorcentajeAlerta,

                        MontoAlerta =
                            taller.MontoAlerta,

                        PorcentajeCritico =
                            taller.PorcentajeCritico,

                        MontoCritico =
                            taller.MontoCritico
                    };


                // =====================================================
                // 4. CARGAR CATÁLOGOS
                // =====================================================

                await CargarCatalogosTallerEditAsync(model);


                // =====================================================
                // 5. MOSTRAR VISTA
                // =====================================================

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al cargar la información del taller.";

                return RedirectToAction(
                    nameof(Index));
            }
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,TallerEditViewModel model)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR ID DE RUTA
                // =====================================================

                if (id <= 0)
                {
                    TempData["Error"] =
                        "El taller especificado no es válido.";

                    return RedirectToAction(
                        nameof(Index));
                }


                // =====================================================
                // 2. FORZAR ID DEL MODELO
                // =====================================================
                //
                // No confiamos en TallerId enviado por el formulario.
                // El identificador válido es el de la URL.
                // =====================================================

                model.TallerId = id;


                // =====================================================
                // 3. NORMALIZAR DATOS
                // =====================================================

                model.RazonSocial =
                    model.RazonSocial?.Trim() ?? string.Empty;

                model.Rfc =
                    model.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

                model.Fianza =
                    model.Fianza?.Trim() ?? string.Empty;

                model.Clabe =
                    model.Clabe?.Trim() ?? string.Empty;

                model.Ubicacion =
                    model.Ubicacion?.Trim() ?? string.Empty;


                // =====================================================
                // 4. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarCatalogosTallerEditAsync(model);

                    return View(model);
                }


                // =====================================================
                // 5. VALIDACIONES DE INTEGRIDAD
                // =====================================================

                // -----------------------------------------------------
                // RFC
                // -----------------------------------------------------

                if (model.Rfc.Length != 12 &&
                    model.Rfc.Length != 13)
                {
                    ModelState.AddModelError(
                        nameof(model.Rfc),
                        "El RFC debe contener 12 o 13 caracteres.");
                }


                // -----------------------------------------------------
                // FIANZA
                // -----------------------------------------------------

                if (string.IsNullOrWhiteSpace(model.Fianza))
                {
                    ModelState.AddModelError(
                        nameof(model.Fianza),
                        "La fianza es obligatoria.");
                }


                // -----------------------------------------------------
                // CLABE
                // -----------------------------------------------------

                if (string.IsNullOrWhiteSpace(model.Clabe))
                {
                    ModelState.AddModelError(
                        nameof(model.Clabe),
                        "La CLABE interbancaria es obligatoria.");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(
                    model.Clabe,
                    @"^\d{18}$"))
                {
                    ModelState.AddModelError(
                        nameof(model.Clabe),
                        "La CLABE debe contener exactamente 18 dígitos.");
                }


                // =====================================================
                // 6. SI EXISTE ERROR, RECARGAR FORMULARIO
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarCatalogosTallerEditAsync(model);

                    return View(model);
                }


                // =====================================================
                // 7. CONSTRUIR DTO
                // =====================================================
                //
                // IMPORTANTE:
                //
                // Aquí solamente enviamos datos propios del Taller.
                //
                // La configuración presupuestal NO se modifica
                // desde esta operación.
                // =====================================================

                var dto =
                    new TallerEditDto
                    {
                        TallerId =
                            id,

                        RazonSocial =
                            model.RazonSocial,

                        Rfc =
                            model.Rfc,

                        BancoId =
                            model.BancoId,

                        Fianza =
                            model.Fianza,

                        Clabe =
                            model.Clabe,

                        ObjetivoMantenimientoId =
                            model.ObjetivoMantenimientoId,

                        Ubicacion =
                            model.Ubicacion,

                        Activo =
                            model.Activo
                    };


                // =====================================================
                // 8. DELEGAR ACTUALIZACIÓN AL SERVICIO
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .ActualizarTallerAsync(dto);


                // =====================================================
                // 9. VALIDAR RESULTADO
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    await CargarCatalogosTallerEditAsync(model);

                    return View(model);
                }


                // =====================================================
                // 10. MENSAJE DE ÉXITO
                // =====================================================

                TempData["Success"] =
                    "El taller se actualizó correctamente.";


                // =====================================================
                // 11. REDIRECCIÓN
                // =====================================================

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al actualizar el taller.";

                await CargarCatalogosTallerEditAsync(model);

                return View(model);
            }
        }




        // GET: TallerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TallerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }










        private async Task CargarCatalogosTallerAsync(TallerCreateViewModel model)
        {
            // =====================================================
            // 1. RECURSOS
            // =====================================================

            var recursos =
                await _maquinariaService
                    .ObtenerRecursosActivosAsync();


            // =====================================================
            // 2. BANCOS
            // =====================================================

            var bancos =
                await _maquinariaService
                    .ObtenerBancosActivosAsync();


            // =====================================================
            // 3. OBJETIVOS DE MANTENIMIENTO
            // =====================================================

            var objetivosMantenimiento =
                await _maquinariaService
                    .ObtenerObjetivosMantenimientoActivosAsync();


            // =====================================================
            // 4. CONSTRUIR SELECTS
            // =====================================================

            model.Recursos =
                recursos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.RecursoId.ToString(),

                        Text =
                            x.Nombre
                    });


            model.Bancos =
                bancos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.BancoId.ToString(),

                        Text =
                            x.Nombre
                    });


            model.ObjetivosMantenimiento =
                objetivosMantenimiento.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.ObjetivoMantenimientoId.ToString(),

                        Text =
                            x.Nombre
                    });
        }


        // =========================================================
        // CARGAR CATÁLOGOS PARA EDITAR TALLER
        // =========================================================

        private async Task CargarCatalogosTallerEditAsync(TallerEditViewModel model)
        {
            var bancos =
                await _maquinariaService.ObtenerBancosActivosAsync();

            var objetivos =
                await _maquinariaService
                    .ObtenerObjetivosMantenimientoActivosAsync();


            // =====================================================
            // 1. CARGAR BANCOS ACTIVOS
            // =====================================================

            model.Bancos =
                bancos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.BancoId.ToString(),

                        Text =
                            x.Nombre
                    });


            // =====================================================
            // 2. CARGAR OBJETIVOS DE MANTENIMIENTO ACTIVOS
            // =====================================================

            model.ObjetivosMantenimiento =
                objetivos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.ObjetivoMantenimientoId.ToString(),

                        Text =
                            x.Nombre
                    });
        }












        // INDEX DE CONFIGURACION PRESUPUESTAL

        [HttpGet]
        public async Task<IActionResult> ConfiguracionPresupuestal(string? busqueda = null, bool? activo = null)
        {
            // var permiso = await _permisosService.ObtenerPermisosAsync("Configuración presupuestal", User);
            // if (!_permisosService.TienePermiso(permiso, "Leer"))
            // {
            //     return Redirect("~/Identity/Account/AccessDenied");
            // }

            // Se envían los parámetros recibidos desde el formulario GET al servicio
            var registros = await _maquinariaService.ObtenerConfiguracionesPresupuestalesAsync(busqueda, activo);

            return View("ConfiguracionPresupuestal", registros);
        }



        //CREATE
        [HttpGet]
        public async Task<IActionResult>CrearConfiguracionPresupuestal()
        {
            //var permiso =
            //    await _permisosService
            //        .ObtenerPermisosAsync(
            //            "Configuración presupuestal",
            //            User);

            //if (!_permisosService.TienePermiso(
            //    permiso,
            //    "Crear"))
            //{
            //    return Redirect(
            //        "~/Identity/Account/AccessDenied");
            //}

            var model =
                new TallerConfiguracionPresupuestalViewModel
                {
                    Activo = true
                };

            await CargarRecursosConfiguracionAsync(model);

            return View(
                "CrearConfiguracionPresupuestal",
                model);
        }



        //CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CrearConfiguracionPresupuestal(TallerConfiguracionPresupuestalViewModel model)
        {
            //var permiso =
            //    await _permisosService
            //        .ObtenerPermisosAsync(
            //            "Configuración presupuestal",
            //            User);

            //if (!_permisosService.TienePermiso(
            //    permiso,
            //    "Crear"))
            //{
            //    return Redirect(
            //        "~/Identity/Account/AccessDenied");
            //}


            model.Contrato =
                model.Contrato?.Trim() ?? string.Empty;


            if (!ModelState.IsValid)
            {
                await CargarRecursosConfiguracionAsync(model);

                return View(
                    "ConfiguracionPresupuestal/Create",
                    model);
            }


            var dto =
                new TallerConfiguracionPresupuestalCreateDto
                {
                    Contrato =
                        model.Contrato,

                    RecursoId =
                        model.RecursoId,

                    MontoMinimo =
                        model.MontoMinimo,

                    MontoMaximo =
                        model.MontoMaximo,

                    PorcentajeAlerta =
                        model.PorcentajeAlerta,

                    PorcentajeCritico =
                        model.PorcentajeCritico
                };


            var resultado =
                await _maquinariaService
                    .CrearConfiguracionPresupuestalAsync(dto);


            if (!resultado.Exito)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarRecursosConfiguracionAsync(model);

                return View(
                    "ConfiguracionPresupuestal/Create",
                    model);
            }


            TempData["Success"] =
                resultado.Mensaje;

            return RedirectToAction(
                nameof(ConfiguracionPresupuestal));
        }



        //EDITAR 
        [HttpGet]
        public async Task<IActionResult>EditarConfiguracionPresupuestal(int id)
        {

            // 1
            if (id <= 0)
            {
                TempData["Error"] =
                    "La configuración especificada no es válida.";

                return RedirectToAction(nameof(ConfiguracionPresupuestal));
            }


            //var permiso =
            //    await _permisosService
            //        .ObtenerPermisosAsync(
            //            "Configuración presupuestal",
            //            User);

            //if (!_permisosService.TienePermiso(
            //    permiso,
            //    "Editar"))
            //{
            //    return Redirect(
            //        "~/Identity/Account/AccessDenied");
            //}


            var model = await _maquinariaService.ObtenerConfiguracionPresupuestalParaEditarAsync(
                        id);


            if (model == null)
            {
                TempData["Error"] =
                    "La configuración especificada no existe.";

                return RedirectToAction(
                    nameof(ConfiguracionPresupuestal));
            }


            return View("EditarConfiguracionPresupuestal",model);
        }




        // configuracion presupuestal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarConfiguracionPresupuestal(int id,TallerConfiguracionPresupuestalViewModel model)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR ID
                // =====================================================

                if (id <= 0)
                {
                    TempData["Error"] =
                        "La configuración presupuestal especificada no es válida.";

                    return RedirectToAction(
                        nameof(ConfiguracionPresupuestal));
                }


                // =====================================================
                // 2. PERMISOS
                // =====================================================

                //var permiso =
                //    await _permisosService
                //        .ObtenerPermisosAsync(
                //            "Configuración presupuestal",
                //            User);

                //if (!_permisosService.TienePermiso(
                //    permiso,
                //    "Editar"))
                //{
                //    return Redirect(
                //        "~/Identity/Account/AccessDenied");
                //}


                // =====================================================
                // 3. EL ID DE RUTA ES LA FUENTE DE VERDAD
                // =====================================================

                model.TallerConfiguracionPresupuestalId = id;


                // =====================================================
                // 4. RECURSO NO SE EDITA
                // =====================================================
                //
                // El RecursoId pertenece a la configuración presupuestal
                // y es inmutable durante la edición.
                //
                // Si el ViewModel contiene RecursoId y viene como 0
                // porque no existe un campo editable en la vista,
                // no debemos permitir que esa validación bloquee el POST.
                //
                // El valor real se obtiene nuevamente desde BD en el GET
                // / servicio y no se confía en un valor enviado por el cliente.
                //
                ModelState.Remove(nameof(TallerConfiguracionPresupuestalViewModel.RecursoId));
                ModelState.Remove(nameof(TallerConfiguracionPresupuestalViewModel.Contrato));


                // =====================================================
                // 5. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarRecursosConfiguracionAsync(model);

                    return View(
                        "EditarConfiguracionPresupuestal",
                        model);
                }


                // =====================================================
                // 6. CONSTRUIR DTO
                // =====================================================

                var dto =
                    new TallerConfiguracionPresupuestalEditDto
                    {
                        TallerConfiguracionPresupuestalId =
                            id,

                        MontoMinimo =
                            model.MontoMinimo,

                        MontoMaximo =
                            model.MontoMaximo,

                        PorcentajeAlerta =
                            model.PorcentajeAlerta,

                        PorcentajeCritico =
                            model.PorcentajeCritico,

                        Activo =
                            model.Activo
                    };


                // =====================================================
                // 7. ACTUALIZAR CONFIGURACIÓN
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .ActualizarConfiguracionPresupuestalAsync(dto);


                // =====================================================
                // 8. MANEJAR ERROR DE NEGOCIO
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    await CargarRecursosConfiguracionAsync(model);

                    return View(
                        "EditarConfiguracionPresupuestal",
                        model);
                }


                // =====================================================
                // 9. ÉXITO
                // =====================================================

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(
                    nameof(ConfiguracionPresupuestal));
            }
            catch (Exception ex)
            {
                // =====================================================
                // 10. ERROR TÉCNICO
                // =====================================================

                _logger.LogError(
                    ex,
                    "Error al editar la configuración presupuestal {Id}.",
                    id);


                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error inesperado al actualizar la configuración presupuestal.");


                // =====================================================
                // 11. RECARGAR CATÁLOGOS
                // =====================================================

                await CargarRecursosConfiguracionAsync(model);


                return View(
                    "EditarConfiguracionPresupuestal",
                    model);
            }
        }







        //index de control presupuestal
        [HttpGet]
        public async Task<IActionResult> ControlPresupuestal(string? busqueda = null,short? ejercicio = null,bool? activo = null)
        {
            try
            {
                //var permiso =
                //    await _permisosService
                //        .ObtenerPermisosAsync(
                //            "Control presupuestal",
                //            User);

                //if (!_permisosService.TienePermiso(
                //        permiso,
                //        "Leer"))
                //{
                //    return Redirect(
                //        "~/Identity/Account/AccessDenied");
                //}


                var registros =
                    await _maquinariaService
                        .ObtenerControlesPresupuestalesAsync(
                            busqueda,
                            ejercicio,
                            activo);


                ViewBag.Busqueda =
                    busqueda;

                ViewBag.Ejercicio =
                    ejercicio;

                ViewBag.Activo =
                    activo;


                return View(
                    registros);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al cargar los controles presupuestales.";

                return RedirectToAction(nameof(Index));
            }
        }






        [HttpGet]
        public async Task<IActionResult>CrearControlPresupuestal()
        {
            try
            {
                //var permiso =
                //    await _permisosService
                //        .ObtenerPermisosAsync(
                //            "Control presupuestal",
                //            User);

                //if (!_permisosService.TienePermiso(
                //        permiso,
                //        "Crear"))
                //{
                //    return Redirect(
                //        "~/Identity/Account/AccessDenied");
                //}


                var model =
                    new TallerControlPresupuestalCreateViewModel
                    {
                        Ejercicio =
                            (short)DateTime.Now.Year,

                        FechaInicio =
                            new DateTime(
                                DateTime.Now.Year,
                                1,
                                1),

                        FechaFin =
                            new DateTime(
                                DateTime.Now.Year,
                                12,
                                31)
                    };


                await CargarConfiguracionesPresupuestalesControlAsync(model);


                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al cargar el formulario.";

                return RedirectToAction(
                    nameof(ControlPresupuestal));
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CrearControlPresupuestal(TallerControlPresupuestalCreateViewModel model)
        {
            try
            {
                //var permiso =
                //    await _permisosService
                //        .ObtenerPermisosAsync(
                //            "Control presupuestal",
                //            User);

                //if (!_permisosService.TienePermiso(
                //        permiso,
                //        "Crear"))
                //{
                //    return Redirect(
                //        "~/Identity/Account/AccessDenied");
                //}


                // =====================================================
                // NORMALIZAR
                // =====================================================

                model.FechaInicio =
                    model.FechaInicio.Date;

                model.FechaFin =
                    model.FechaFin.Date;


                // =====================================================
                // VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    await CargarConfiguracionesPresupuestalesControlAsync(
                        model);

                    return View(model);
                }


                // =====================================================
                // VALIDACIÓN ADICIONAL
                // =====================================================

                if (model.FechaInicio >
                    model.FechaFin)
                {
                    ModelState.AddModelError(
                        nameof(model.FechaInicio),
                        "La fecha de inicio no puede ser posterior a la fecha de fin.");
                }


                if (model.FechaInicio.Year !=
                        model.Ejercicio ||
                    model.FechaFin.Year !=
                        model.Ejercicio)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Las fechas de vigencia deben pertenecer al ejercicio seleccionado.");
                }


                if (!ModelState.IsValid)
                {
                    await CargarConfiguracionesPresupuestalesControlAsync(
                        model);

                    return View(model);
                }


                // =====================================================
                // DTO
                // =====================================================

                var dto =
                     new TallerControlPresupuestalCreateDto
                     {
                         TallerConfiguracionPresupuestalId = model.TallerConfiguracionPresupuestalId,
                         Ejercicio = model.Ejercicio,
                         PresupuestoAsignado = model.PresupuestoAsignado,
                         FechaInicio = DateOnly.FromDateTime(model.FechaInicio),
                         FechaFin = DateOnly.FromDateTime(model.FechaFin)
                     };


                // =====================================================
                // SERVICE
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .CrearControlPresupuestalAsync(dto);


                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    await CargarConfiguracionesPresupuestalesControlAsync(
                        model);

                    return View(model);
                }


                TempData["Success"] =
                    resultado.Mensaje;


                return RedirectToAction(
                    nameof(ControlPresupuestal));
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al crear el control presupuestal.";

                await CargarConfiguracionesPresupuestalesControlAsync(
                    model);

                return View(model);
            }
        }










        private async Task CargarConfiguracionesPresupuestalesControlAsync(TallerControlPresupuestalCreateViewModel model)
        {
            var configuraciones =  await _maquinariaService .ObtenerConfiguracionesPresupuestalesActivasAsync();


            model.ConfiguracionesPresupuestales =
                configuraciones.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.TallerConfiguracionPresupuestalId
                                .ToString(),

                        Text =
                            $"{x.Contrato} - {x.RecursoNombre} - Máx. {x.MontoMaximo:C}",

                        Selected =
                            x.TallerConfiguracionPresupuestalId ==
                            model.TallerConfiguracionPresupuestalId
                    });
        }








        [HttpGet]
        public async Task<IActionResult>EditarControlPresupuestal(int id)
        {
            try
            {
                //var permiso =
                //    await _permisosService
                //        .ObtenerPermisosAsync(
                //            "Control presupuestal",
                //            User);

                //if (!_permisosService.TienePermiso(
                //        permiso,
                //        "Editar"))
                //{
                //    return Redirect(
                //        "~/Identity/Account/AccessDenied");
                //}


                if (id <= 0)
                {
                    TempData["Error"] =
                        "El control presupuestal especificado no es válido.";

                    return RedirectToAction(
                        nameof(ControlPresupuestal));
                }


                var model =
                    await _maquinariaService
                        .ObtenerControlPresupuestalParaEditarAsync(
                            id);


                if (model == null)
                {
                    TempData["Error"] =
                        "El control presupuestal especificado no existe.";

                    return RedirectToAction(
                        nameof(ControlPresupuestal));
                }


                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Ocurrió un error al cargar el control presupuestal.";

                return RedirectToAction(
                    nameof(ControlPresupuestal));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarControlPresupuestal(int id,TallerControlPresupuestalEditViewModel model)
        {
            try
            {
                // =====================================================
                // 1. VALIDAR ID
                // =====================================================

                if (id <= 0)
                {
                    TempData["Error"] =
                        "El control presupuestal especificado no es válido.";

                    return RedirectToAction(
                        nameof(ControlPresupuestal));
                }


                // =====================================================
                // 2. ASIGNAR ID AUTORITATIVO
                // =====================================================

                model.IdControlTaller =
                    id;


                // =====================================================
                // 3. VALIDAR MODELSTATE
                // =====================================================

                if (!ModelState.IsValid)
                {
                    return View(model);
                }


                // =====================================================
                // 4. VALIDAR FECHAS
                // =====================================================

                if (model.FechaInicio >
                    model.FechaFin)
                {
                    ModelState.AddModelError(
                        nameof(model.FechaInicio),
                        "La fecha de inicio no puede ser posterior a la fecha de fin.");

                    return View(model);
                }


                // =====================================================
                // 5. VALIDAR EJERCICIO
                // =====================================================

                if (model.FechaInicio.Year !=
                        model.Ejercicio ||
                    model.FechaFin.Year !=
                        model.Ejercicio)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Las fechas de vigencia deben pertenecer al ejercicio del control.");

                    return View(model);
                }


                // =====================================================
                // 6. CREAR DTO
                // =====================================================

                var dto =
                    new TallerControlPresupuestalEditDto
                    {
                        IdControlTaller =
                            id,

                        PresupuestoAsignado =
                            model.PresupuestoAsignado,

                        FechaInicio =
                            model.FechaInicio,

                        FechaFin =
                            model.FechaFin,

                        Activo =
                            model.Activo
                    };


                // =====================================================
                // 7. ACTUALIZAR MEDIANTE SERVICE
                // =====================================================

                var resultado =
                    await _maquinariaService
                        .ActualizarControlPresupuestalAsync(
                            dto);


                // =====================================================
                // 8. VALIDAR RESPUESTA
                // =====================================================

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        resultado.Mensaje);

                    return View(model);
                }


                // =====================================================
                // 9. ÉXITO
                // =====================================================

                TempData["Success"] =
                    resultado.Mensaje;

                return RedirectToAction(
                    nameof(ControlPresupuestal));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error no controlado al actualizar control presupuestal {IdControlTaller}.",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error inesperado al actualizar el control presupuestal. Los cambios no fueron aplicados.");

                return View(model);
            }
        }








        private async Task CargarRecursosConfiguracionAsync(TallerConfiguracionPresupuestalViewModel model)
        {
            var recursos = await _maquinariaService.ObtenerRecursosActivosAsync();

            model.Recursos =
                recursos.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.RecursoId.ToString(),

                        Text =
                            x.Nombre
                    });
        }



















    }
}
