using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.OrdeDePago;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.AdminFinancieros.Controllers
{
    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]
    public class OrdenDePagoController : Controller
    {

        private readonly IOrdenDePago _service;

        public OrdenDePagoController(IOrdenDePago service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View(new OrdenDePagoConsultaVM());
        }

        [HttpPost]
        public async Task<IActionResult> Index(OrdenDePagoConsultaVM model)
        {
            model.Detalles = await _service.Obtener(model.Folio);

            return View(model);
        }

        public async Task<IActionResult> ExportarExcel(string folio)
        {
            var archivo = await _service.GenerarExcel(folio);

            if (archivo == null)
            {
                return NotFound();
            }

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrdenPago_{folio}.xlsx");
        }


    }
}
