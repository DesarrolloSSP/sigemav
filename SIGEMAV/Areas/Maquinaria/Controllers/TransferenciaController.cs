using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Implementations;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]

    public class TransferenciaController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;


        public TransferenciaController(IMaquinariaService maquinariaService, BdSigeMavContext context)
        {
            _maquinariaService = maquinariaService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var modelo = await _maquinariaService.ObtenerTransferencias();
            return View(modelo);
        }

        // GET: TransferenciaController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService
                        .ObtenerTransferenciaDetalle(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TransferenciaController/Create
        public async Task<IActionResult> Create()
        {
            var vm =
                await _maquinariaService.ObtenerTransferenciaCreate();

            return View(vm);
        }

        //POST: TransferenciaController/Create
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransferenciaUnidadCreateViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    vm.Areas =
                        await _maquinariaService.ObtenerAreas();

                    vm.UnidadesVehiculares =
                        await _maquinariaService
                            .ObtenerUnidadesVehicularesSelectList();

                    return View(vm);
                }

                var dto =
                    new TransferenciaUnidadCreateDto
                    {
                        UnidadVehicularId =
                            vm.UnidadVehicularId,

                        AreaOrigenId =
                            vm.AreaOrigenId,

                        AreaDestinoId =
                            vm.AreaDestinoId,

                        FechaTransferencia =
                            vm.FechaTransferencia,

                        MotivoTransferencia =
                            vm.MotivoTransferencia,

                        Observaciones =
                            vm.Observaciones
                    };

                await _maquinariaService.CrearTransferenciaUnidad(dto, vm.Documento);

                TempData["Success"] =
                    "La transferencia fue registrada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                vm.Areas =
                    await _maquinariaService.ObtenerAreas();

                vm.UnidadesVehiculares =
                    await _maquinariaService
                        .ObtenerUnidadesVehicularesSelectList();

                TempData["Error"] =
                    ex.Message;

                return View(vm);
            }
        }

        // GET: TransferenciaController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TransferenciaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: TransferenciaController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TransferenciaController/Delete/5
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



   





        [HttpGet]
        public async Task<IActionResult> ObtenerAreaPorUnidad(
         int unidadVehicularId)
        {
            var unidad =
                await _context.UnidadVehiculars
                    .AsNoTracking()
                    .Include(x => x.Area)
                    .FirstOrDefaultAsync(x =>
                        x.UnidadVehicularId ==
                            unidadVehicularId
                        &&
                        x.Activo);

            if (unidad == null)
            {
                return Json(new
                {
                    success = false,
                    message = "La unidad no fue encontrada."
                });
            }

            return Json(new
            {
                success = true,

                areaId =
                    unidad.AreaId,

                areaNombre =
                    unidad.Area != null
                        ? unidad.Area.AreaNombre
                        : string.Empty
            });
        }





        [HttpGet]
        public async Task<JsonResult> BuscarUnidadesVehiculares(string term)
        {
            var resultado = await _maquinariaService.BuscarUnidadesVehiculares(term);

            return Json(resultado);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Autorizar(TransferenciaUnidadAutorizarDto dto,IFormFile? documento)
        {
            try
            {
                await _maquinariaService
                    .AutorizarTransferencia(
                        dto,
                        documento);

                TempData["Success"] =
                    "La transferencia fue autorizada.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(TransferenciaUnidadRechazarDto dto)
        {
            try
            {
                await _maquinariaService
                    .RechazarTransferencia(dto);

                TempData["Success"] =
                    "La transferencia fue rechazada correctamente.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(TransferenciaUnidadCancelarDto dto)
        {
            try
            {
                await _maquinariaService.CancelarTransferencia(dto);

                TempData["Success"] =
                    "La transferencia fue cancelada.";

                return RedirectToAction(
                   nameof(Details),
                   new { id = dto.TransferenciaUnidadId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
        }



        // GET: TransferenciaUnidad/Entregar/5

        [HttpGet]
        public async Task<IActionResult> Entregar(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerTransferenciaParaEntrega(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }






        // ================================================
        // POST: TransferenciaUnidad/Entregar
        // ================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entregar(TransferenciaUnidadEntregarViewModel vm,
            IFormFile documento)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                await _maquinariaService
                    .EntregarTransferencia(
                        new TransferenciaUnidadEntregarDto
                        {
                            TransferenciaUnidadId =
                                vm.TransferenciaUnidadId,

                            UsuarioEntregaId =
                                vm.UsuarioEntregaId,

                            ObservacionesEntrega =
                                vm.ObservacionesEntrega
                        },
                        documento);

                TempData["Success"] =
                    "La unidad fue entregada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;

                return View(vm);
            }
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Recibir(TransferenciaUnidadRecibirDto dto,IFormFile? documento)
        //{
        //    try
        //    {
        //        await _maquinariaService.RecibirTransferencia(dto,documento);

        //        TempData["Success"] =
        //            "La unidad fue recibida.";

        //        return RedirectToAction(
        //             nameof(Details),
        //             new { id = dto.TransferenciaUnidadId });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Message;

        //        return RedirectToAction(
        //             nameof(Details),
        //             new { id = dto.TransferenciaUnidadId });
        //    }
        //}





        [HttpGet]
        public async Task<IActionResult> Recepcionar(int id)
        {
            try
            {
                var vm =
                    await _maquinariaService.ObtenerRecepcionTransferencia(id);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recepcionar(TransferenciaUnidadRecepcionarDto dto,IFormFile documento)
        {
            try
            {
                await _maquinariaService
                    .RecepcionarTransferencia(dto, documento);

                TempData["Success"] =
                    "La unidad fue recepcionada correctamente.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = dto.TransferenciaUnidadId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    nameof(Recepcionar),
                    new { id = dto.TransferenciaUnidadId });
            }
        }



        [HttpGet]
        public async Task<IActionResult> DescargarDocumentoSeguimiento(int id)
        {
            try
            {
                // =========================================
                // OBTENER SEGUIMIENTO
                // =========================================

                var seguimiento =
                    await _context.TransferenciaUnidadSeguimientos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadSeguimientoId == id
                            &&
                            x.Activo);

                if (seguimiento == null)
                {
                    TempData["Warning"] =
                        "El documento solicitado no existe.";

                    return RedirectToAction(nameof(Index));
                }

                // =========================================
                // VALIDAR RUTA
                // =========================================

                if (string.IsNullOrWhiteSpace(seguimiento.DocumentoRuta))
                {
                    TempData["Warning"] =
                        "El seguimiento no contiene documento.";

                    return RedirectToAction(
                        nameof(Details),
                        new
                        {
                            id = seguimiento.TransferenciaUnidadId
                        });
                }

                // =========================================
                // VALIDAR EXISTENCIA FÍSICA
                // =========================================

                if (!System.IO.File.Exists(seguimiento.DocumentoRuta))
                {
                    TempData["Warning"] =
                        "El archivo físico no fue encontrado.";

                    return RedirectToAction(
                        nameof(Details),
                        new
                        {
                            id = seguimiento.TransferenciaUnidadId
                        });
                }

                // =========================================
                // LEER ARCHIVO
                // =========================================

                var bytes =
                    await System.IO.File.ReadAllBytesAsync(
                        seguimiento.DocumentoRuta);

                // =========================================
                // CONTENT TYPE
                // =========================================

                var provider =
                    new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(
                        seguimiento.DocumentoNombreOriginal ??
                        seguimiento.DocumentoRuta,
                        out var contentType))
                {
                    contentType =
                        "application/octet-stream";
                }

                // =========================================
                // NOMBRE DESCARGA
                // =========================================

                var nombreDescarga =
                    string.IsNullOrWhiteSpace(
                        seguimiento.DocumentoNombreOriginal)
                    ?
                    Path.GetFileName(
                        seguimiento.DocumentoRuta)
                    :
                    seguimiento.DocumentoNombreOriginal;

                // =========================================
                // RETORNAR ARCHIVO
                // =========================================

                return File(
                    bytes,
                    contentType,
                    nombreDescarga);
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
