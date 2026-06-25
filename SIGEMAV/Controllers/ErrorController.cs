using Microsoft.AspNetCore.Mvc;

namespace SIGEMAV.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    [Route("Error/403")]
    public IActionResult Error403()
    {
        Response.StatusCode = 403;
        return View("403");
    }

    [Route("Error/404")]
    public IActionResult Error404()
    {
        Response.StatusCode = 404;
        return View("404");
    }

    [Route("Error/500")]
    public IActionResult Error500()
    {
        Response.StatusCode = 500;
        return View("500");
    }
}