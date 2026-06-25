using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{
    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]
    public class OdpController : Controller
    {

        private readonly IOdpService _OdpService;

        public OdpController(IOdpService dpService)
        {
            _OdpService = dpService;
        }


        public async Task<IActionResult> Index()
        {
            var model = await _OdpService.Obtener();

            return View(model);
        }


      

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _OdpService.InicializarFormulario();

            return View(model);
        }


        // =========================
        // CREATE POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdenPagoVM model)
        {
            try
            {
                // =========================
                // VALIDAR DETALLE
                // =========================

                if (model.Detalles == null ||
                    !model.Detalles.Any())
                {
                    TempData["error"] =
                        "Debe agregar al menos una partida.";

                    model =
                        await _OdpService.InicializarFormulario(model);

                    return View(model);
                }

                // =========================
                // VALIDAR MODELSTATE
                // =========================

                if (!ModelState.IsValid)
                {
                    model =
                        await _OdpService
                            .InicializarFormulario(model);

                    return View(model);
                }

                // =========================
                // GUARDAR
                // =========================

                bool respuesta =
                    await _OdpService.Guardar(model);

                if (respuesta)
                {
                    TempData["success"] =
                        "La orden de pago fue registrada correctamente.";

                    return RedirectToAction(nameof(Index));
                }

                TempData["error"] =
                    "Ocurrió un error al guardar.";

                model =
                    await _OdpService
                        .InicializarFormulario(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                model =
                    await _OdpService
                        .InicializarFormulario(model);

                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model =
                await _OdpService.ObtenerPorId(id);

            if (model == null)
            {
                return NotFound();
            }

            model =
                await _OdpService
                    .InicializarFormulario(model);

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            OrdenPagoVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model =
                        await _OdpService
                            .InicializarFormulario(model);

                    return View(model);
                }

                bool respuesta =
                    await _OdpService
                        .Editar(model);

                if (respuesta)
                {
                    TempData["success"] =
                        "La orden fue actualizada correctamente.";

                    return RedirectToAction(nameof(Index));
                }

                TempData["error"] =
                    "Ocurrió un error al actualizar.";

                model =
                    await _OdpService
                        .InicializarFormulario(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                model =
                    await _OdpService
                        .InicializarFormulario(model);

                return View(model);
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                bool respuesta =
                    await _OdpService.Cancelar(id);

                if (respuesta)
                {
                    TempData["success"] =
                        "La orden fue cancelada correctamente.";
                }
                else
                {
                    TempData["error"] =
                        "No fue posible cancelar la orden.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(int id)
        {
            try
            {
                bool respuesta = await _OdpService.Reactivar(id);

                if (respuesta)
                {
                    TempData["success"] =
                        "La orden fue reactivada correctamente.";
                }
                else
                {
                    TempData["error"] = "No fue posible reactivar la orden.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ExportarExcel(int id)
        {
            var archivo =
                await _OdpService
                    .GenerarExcel(id);

            if (archivo == null)
            {
                return NotFound();
            }

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrdenPago_{id}.xlsx");
        }

    }
}






