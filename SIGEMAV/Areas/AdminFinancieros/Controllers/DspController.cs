using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SIGEMAV.Models.ViewModels.Financieros.Disponibilidad;
using SIGEMAV.Models.ViewModels.Financieros.Dsp;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.Admin.Controllers
{

    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]
    public class DspController : Controller
    {
        private readonly IDspService _service;
        private readonly IConfiguration _configuration;

        public DspController(IDspService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }


        public async Task<IActionResult> Index(int estado = 1)
        {
            ViewBag.Estado = estado;

            var modelo = await _service.Obtener(estado);

            return View(modelo);
        }


        [HttpGet]
        public IActionResult Create()
        {
            DspVM model = new()
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DspVM model)
        {
            if (model.Archivo == null)
            {
                ModelState.AddModelError(
                    nameof(model.Archivo),
                    "Seleccione un archivo PDF.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string rutaArchivo = "";

            try
            {
                // =========================
                // GUARDAR PDF
                // =========================

                var repositorio = _configuration["Rutas:RepositorioArchivos"];

                var carpeta = Path.Combine(repositorio!, "DSP");

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                var nombreArchivo =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.Archivo!.FileName);

                var rutaCompleta =
                    Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(
                        rutaCompleta,
                        FileMode.Create))
                {
                    await model.Archivo.CopyToAsync(stream);
                }

                rutaArchivo = rutaCompleta;

                // =========================
                // GUARDAR REGISTRO
                // =========================

                bool resultado =
                    await _service.Guardar(
                        model,
                        rutaArchivo);

                if (!resultado)
                {
                    // Eliminar PDF huérfano
                    if (System.IO.File.Exists(rutaArchivo))
                    {
                        System.IO.File.Delete(rutaArchivo);
                    }

                    TempData["Error"] = $"El DSP '{model.NoDsp}' ya se encuentra registrado.";

                    return View(model);
                }

                TempData["Success"] = "DSP guardado correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Eliminar PDF si ocurrió algún error
                if (!string.IsNullOrEmpty(rutaArchivo) && System.IO.File.Exists(rutaArchivo))
                {
                    System.IO.File.Delete(rutaArchivo);
                }

                TempData["Error"] = $"Ocurrió un error al guardar el DSP. {ex.Message}";
                return View(model);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.ObtenerPorId(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DspEditarVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? rutaArchivo = null;

            try
            {
                if (model.Archivo != null)
                {
                    var repositorio = _configuration["Rutas:RepositorioArchivos"];

                    var carpeta = Path.Combine(repositorio!, "DSP");

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }

                    var nombreArchivo = Guid.NewGuid().ToString() +
                        Path.GetExtension(model.Archivo.FileName);

                    rutaArchivo = Path.Combine(carpeta, nombreArchivo);
                    using var stream =
                        new FileStream(
                            rutaArchivo,
                            FileMode.Create);

                    await model.Archivo.CopyToAsync(stream);
                }

                bool resultado = await _service.Editar(model, rutaArchivo);

                if (!resultado)
                {
                    // Eliminar PDF nuevo si no se pudo guardar
                    if (!string.IsNullOrEmpty(rutaArchivo) &&
                        System.IO.File.Exists(rutaArchivo))
                    {
                        System.IO.File.Delete(rutaArchivo);
                    }

                    TempData["Error"] = $"Ya existe un DSP con el número {model.NoDsp}.";

                    return View(model);
                }

                TempData["Success"] = "DSP actualizada correctamente.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(rutaArchivo) &&
                    System.IO.File.Exists(rutaArchivo))
                {
                    System.IO.File.Delete(rutaArchivo);
                }

                TempData["Error"] = $"Ocurrió un error al actualizar. {ex.Message}";

                return View(model);
            }
        }




        public async Task<IActionResult> VerPdf(int id)
        {
            var rutaArchivo = await _service.VerPdf(id);

            if (string.IsNullOrEmpty(rutaArchivo))
            {
                return NotFound();
            }

            var repositorio = _configuration["Rutas:RepositorioArchivos"];

            var rutaCompleta = Path.Combine(repositorio!, rutaArchivo);

            if (!System.IO.File.Exists(rutaCompleta))
            {
                return NotFound();
            }

            byte[] archivo = await System.IO.File.ReadAllBytesAsync(rutaCompleta);

            return File(archivo, "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            await _service.Activar(id);

            TempData["Mensaje"] = "Registro activado correctamente";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _service.Desactivar(id);

            TempData["Mensaje"] = "Registro desactivado correctamente";

            return RedirectToAction(nameof(Index));
        }


    }


}
