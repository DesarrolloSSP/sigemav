using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;

using SIGEMAV.Models.ViewModels.Financieros.Disponibilidad;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Areas.Admin.Controllers
{


    [Area("AdminFinancieros")]
    [Authorize(Roles = "AdminFinancieros,Sysadmin")]

    public class DispPresupuestalController : Controller
    {
        private readonly IDispService _dispService;

        private readonly IConfiguration _configuration;

        public DispPresupuestalController(IDispService objetoGastoService, IConfiguration config)
        {
            _dispService = objetoGastoService;
            _configuration = config;
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var model = new DispCrearVM();

            await CargarCombos(model);

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Crear(DispCrearVM model)
        {
            await CargarCombos(model);

            return View(model);
        }

        private async Task CargarCombos(DispCrearVM model)
        {
            // OBJETO GASTO
            var objetoGasto = await _dispService.ObtenerTipoGasto();

            model.ObjetosGasto = objetoGasto
                .Select(x => new SelectListItem
                {
                    Value = x.IdObjetoGasto.ToString(),
                    Text = x.ClaveObjGasto,
                    Selected = x.IdObjetoGasto == model.IdObjetoGasto
                }).ToList();


            // DSP
            var objetoDsp = await _dispService.ObtenerListadoDsp();

            model.ListadoDsp = objetoDsp
                .Select(x => new SelectListItem
                {
                    Value = x.IdDsp.ToString(),
                    Text = x.NoDsp,
                    Selected = x.IdDsp == model.IDsp
                }).ToList();



            // DESCRIPCIÓN
            if (model.IdObjetoGasto.HasValue)
            {
                var descripcion = objetoGasto.FirstOrDefault(x => x.IdObjetoGasto == model.IdObjetoGasto);

                if (descripcion != null)
                {
                    model.DescripcionObjetoGasto = descripcion.Descripcion;

                    ModelState.Remove(nameof(model.DescripcionObjetoGasto));
                }
            }

            // CLAVE ADMINISTRATIVA
            var claveAdmin = await _dispService.ObtenerClaveAdministrativa();

            model.ClavesAdministrativas = claveAdmin
                .Select(x => new SelectListItem
                {
                    Value = x.IdClaveAdmin.ToString(),
                    Text = x.ClaveAdmin,
                    Selected = x.IdClaveAdmin == model.IdClaveAdmin
                }).ToList();

            // PROYECTOS
            if (model.IdClaveAdmin.HasValue)
            {
                var proyectos = await _dispService.ObtenerProyecto(model.IdClaveAdmin.Value);

                model.Proyectos = proyectos
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdProyecto.ToString(),
                        Text = x.ClaveProyecto,
                        Selected = x.IdProyecto == model.IdProyecto
                    }).ToList();
            }




            if (model.IdProyecto.HasValue)
            {
                var areas = await _dispService.ObtenerAreasPorProyecto(model.IdProyecto.Value);

                model.AreasUsuarias =
                    areas.Select(x =>
                        new SelectListItem
                        {
                            Value = x.AreaId.ToString(),
                            Text = x.AreaNombre
                        })

                    .ToList();
            }




        }








        [HttpPost]
        public async Task<IActionResult> Guardar(DispCrearVM model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(model);

                return View("Crear", model);
            }

            bool resultado =
                await _dispService.GuardarDisp(model);

            if (!resultado)
            {
                ModelState.AddModelError(
                    "",
                    "Ya existe una disponibilidad presupuestal con los mismos datos.");

                await CargarCombos(model);

                return View("Crear", model);
            }

            TempData["Success"] =
                "Disponibilidad presupuestal registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Index()
        {
            var model = await _dispService
                .ObtenerDsp();

            return View(model);
        }




        private async Task CargarCombos(DispEditarVM model)
        {
            // OBJETO GASTO
            var objetoGasto = await _dispService.ObtenerTipoGasto();

            model.ObjetosGasto = objetoGasto.Select(x =>
                    new SelectListItem
                    {
                        Value = x.IdObjetoGasto.ToString(),

                        Text = x.ClaveObjGasto,

                        Selected = x.IdObjetoGasto == model.IdObjetoGasto
                    })

                .ToList();



            // OBJETO DSP         
            var objetoDsp = await _dispService.ObtenerListadoDsp();

            model.ListadoDsp = objetoDsp.Select(x => new SelectListItem
            {
                Value = x.IdDsp.ToString(),
                Text = x.NoDsp,
                Selected = x.IdDsp == model.IDsp

            }).ToList();


            // DESCRIPCIÓN
            if (model.IdObjetoGasto.HasValue)
            {
                var descripcion =
                    objetoGasto.FirstOrDefault(x =>
                        x.IdObjetoGasto ==
                        model.IdObjetoGasto);

                if (descripcion != null)
                {
                    model.DescripcionObjetoGasto =
                        descripcion.Descripcion;
                }
            }


            // CLAVES ADMIN
            var claveAdmin =
                await _dispService.ObtenerClaveAdministrativa();

            model.ClavesAdministrativas =
                claveAdmin.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.IdClaveAdmin.ToString(),

                        Text =
                            x.ClaveAdmin,

                        Selected =
                            x.IdClaveAdmin ==
                            model.IdClaveAdmin
                    })

                .ToList();


            // PROYECTOS
            if (model.IdClaveAdmin.HasValue)
            {
                var proyectos =
                    await _dispService
                        .ObtenerProyecto(
                            model.IdClaveAdmin.Value);

                model.Proyectos =
                    proyectos.Select(x =>
                        new SelectListItem
                        {
                            Value =
                                x.IdProyecto.ToString(),

                            Text =
                                x.ClaveProyecto,

                            Selected =
                                x.IdProyecto ==
                                model.IdProyecto
                        })

                    .ToList();
            }


            // AREAS
            if (model.IdProyecto.HasValue)
            {
                var areas =
                    await _dispService
                        .ObtenerAreasPorProyecto(
                            model.IdProyecto.Value);

                model.AreasUsuarias =
                    areas.Select(x =>
                        new SelectListItem
                        {
                            Value =
                                x.AreaId.ToString(),

                            Text =
                                x.AreaNombre,

                            Selected =
                                x.AreaId ==
                                model.IdArea
                        })

                    .ToList();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var model = await _dispService.ObtenerPorId(id);

            if (model == null)
            {
                return NotFound();
            }


            await CargarCombos(model);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(DispEditarVM model, string recargar)
        {
            // =====================================
            // RECARGA CASCADAS
            // =====================================
            if (recargar == "true")
            {
                await CargarCombos(model);

                return View(model);
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos(model);

                return View(model);
            }


            string? rutaArchivo = null;


        
            await _dispService
                .ActualizarDsp(model, rutaArchivo);


            TempData["Mensaje"] =
                "DSP actualizado correctamente";


            
            return RedirectToAction("Index", "DispPresupuestal", new { area = "Admin" });
        }



        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            await _dispService
                .Activar(id);

            TempData["Mensaje"] =
                "DSP activado correctamente";


            return RedirectToAction(
                "Index",
                "DispPresupuestal",
                new { area = "Admin" });
        }

        // =====================================
        [HttpGet]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _dispService
                .Desactivar(id);

            TempData["Mensaje"] =
                "DSP desactivado correctamente";


            return RedirectToAction(
                "Index",
                "DispPresupuestal",
                new { area = "Admin" });
        }




      

    }


}

