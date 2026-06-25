using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models;
using SIGEMAV.Services.Interfaces.SIA;

namespace SIGEMAV.Controllers
{


    
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISiaService _sicaService;

        public HomeController(ILogger<HomeController> logger, ISiaService sicaService)
        {
            _logger = logger;
            _sicaService = sicaService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> PruebaSica()
        {
            var resultado =
                await _sicaService
                    .ValidarUsuarioAsync("JMORALESL", "JM180312");

            return Json(resultado);

        }




    }










}
