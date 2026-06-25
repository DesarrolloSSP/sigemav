using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Login;
using SIGEMAV.Services.Interfaces.SIA;

namespace SIGEMAV.Areas.Account.Controllers
{


    [Area("Account")]
    
    public class AccountController : Controller
    {
        private readonly ISiaService _sicaService;

        public AccountController(ISiaService sicaService)
        {
            _sicaService = sicaService;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado =
                await _sicaService
                    .ValidarUsuarioAsync(
                        model.UserName,
                        model.Password);

            //foreach (var rol in resultado.Usuario.Roles)//descomentar
            //{
            //    Console.WriteLine($"ROL RECIBIDO: {rol}");
            //}

            //if (resultado == null ||
            //    !resultado.Success ||
            //    resultado.Usuario == null)
            //{
            //    ModelState.AddModelError(
            //        "",
            //        "Usuario o contraseña incorrectos.");

            //    return View(model);
            //}

            if (resultado == null)
            {
                ModelState.AddModelError(
                    "",
                    "No fue posible comunicarse con SIA.");

                return View(model);
            }

            if (!resultado.Success)
            {
                ModelState.AddModelError(
                    "",
                    resultado.Mensaje ??
                    "Acceso denegado.");

                return View(model);
            }

            if (resultado.Usuario == null)
            {
                ModelState.AddModelError(
                    "",
                    "No se recibió información del usuario.");

                return View(model);
            }

            var usuario = resultado.Usuario;

            if (!usuario.Activo)
            {
                ModelState.AddModelError(
                    "",
                    "El usuario se encuentra inactivo.");

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    usuario.UserName),

                new Claim(
                    "NombreCompleto",
                    usuario.NombreCompleto)
            };

            //Descomentar
            TempData["Roles"] = string.Join(", ", usuario.Roles);
            foreach (var rol in usuario.Roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        rol));
            }

            var identity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var principal =
                new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent =
                        model.RememberMe
                });

            TempData["LoginInfo"] =
                $"Bienvenido {usuario.NombreCompleto}";

            return RedirectToAction(
                "Index",
                "Home",
                new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(
                nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }




}
