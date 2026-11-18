using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class CatalogoVehicularController : Controller
    {


        private readonly IMaquinariaService _service;

        public CatalogoVehicularController(IMaquinariaService service)
        {
            _service = service;
        }


        // ================= MARCAS =================
        [HttpGet]
        public async Task<IActionResult> Marcas()
        {
            var marcas = await _service.ObtenerMarcasAsync();
            var vm = new MarcaIndexViewModel { Marcas = marcas };
            return View(vm);
        }

        //[HttpGet]
        //public IActionResult MarcaCrear()
        //{
        //    var vm = new MarcaFormViewModel();
        //    return View(vm);
        //}

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MarcaCrear(string marcaNombre)
{
    try
    {
        if (string.IsNullOrWhiteSpace(marcaNombre))
        {
            TempData["Error"] = "El nombre de la marca es obligatorio.";
            return RedirectToAction(nameof(Marcas));
        }

        // Creamos el DTO de forma limpia para enviarlo al servicio
        var nuevaMarcaDto = new MarcaCreateUpdateDto
        {
            MarcaNombre = marcaNombre.Trim(),
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        await _service.CrearMarcaAsync(nuevaMarcaDto);
        TempData["Success"] = "Marca registrada correctamente.";
        return RedirectToAction(nameof(Marcas));
    }
    catch (Exception ex)
    {
        TempData["Error"] = "Error al registrar la marca: " + ex.Message;
        return RedirectToAction(nameof(Marcas));
    }
}






        //[HttpGet]
        //public async Task<IActionResult> MarcaEditar(int id)
        //{
        //    var dto = await _service.ObtenerMarcaPorIdParaEditarAsync(id);
        //    if (dto == null)
        //    {
        //        TempData["Warning"] = "La marca solicitada no existe.";
        //        return RedirectToAction(nameof(Marcas));
        //    }

        //    var vm = new MarcaFormViewModel { Marca = dto };
        //    return View(vm);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcaEditar(int marcaId, string marcaNombre, bool activo)
        {
            try
            {
                if (marcaId <= 0 || string.IsNullOrWhiteSpace(marcaNombre))
                {
                    TempData["Error"] = "Los datos de la marca no son válidos.";
                    return RedirectToAction(nameof(Marcas));
                }

                var marcaDto = new MarcaCreateUpdateDto
                {
                    MarcaId = marcaId,
                    MarcaNombre = marcaNombre.Trim(),
                    Activo = activo
                };

                await _service.ActualizarMarcaAsync(marcaDto);
                TempData["Success"] = "Marca actualizada correctamente.";
                return RedirectToAction(nameof(Marcas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la marca: " + ex.Message;
                return RedirectToAction(nameof(Marcas));
            }
        }



        [HttpPost]
        public async Task<IActionResult> MarcaToggleStatus(int id)
        {
            try
            {
                await _service.CambiarEstadoMarcaAsync(id);
                TempData["Success"] = "Estado de la marca actualizado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Marcas));
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarMarca(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de marca no válido.";
                    return RedirectToAction(nameof(Marcas));
                }

                // Llamas al método de tu servicio que cambia el estado a inactivo o elimina según tu lógica
                await _service.EliminarMarcaAsync(id); // O DesactivarMarcaAsync(id)

                TempData["Success"] = "Marca desactivada correctamente.";
                return RedirectToAction(nameof(Marcas));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Marcas));
            }
        }



        // ================= MODELOS =================
        [HttpGet]
        public async Task<IActionResult> Modelos()
        {
            //var modelos = await _service.ObtenerModelosAsync();
            //var vm = new ModeloIndexViewModel { Modelos = modelos };
            //return View(vm);

            // Llama al servicio que llena tanto la tabla como el SelectList para los modales
            var viewModel = await _service.ObtenerModeloIndexViewModelAsync();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ModeloCrear()
        {




            var vm = new ModeloFormViewModel
            {
                MarcasSelectList = await _service.ObtenerMarcasSelectListAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModeloCrear(int marcaId, string modeloNombre)
        {
            try
            {
                if (marcaId <= 0 || string.IsNullOrWhiteSpace(modeloNombre))
                {
                    TempData["Error"] = "Por favor, verifique los datos ingresados para el modelo.";
                    return RedirectToAction(nameof(Modelos));
                }

                var modeloDto = new ModeloCreateUpdateDto
                {
                    MarcaId = marcaId,
                    ModeloNombre = modeloNombre.Trim(),
                    Activo = true
                };

                await _service.CrearModeloAsync(modeloDto);
                TempData["Success"] = "Modelo registrado correctamente.";
                return RedirectToAction(nameof(Modelos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el modelo: " + ex.Message;
                return RedirectToAction(nameof(Modelos));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModeloEditar(int modeloId, int marcaId, string modeloNombre, bool activo)
        {
            try
            {
                if (modeloId <= 0 || marcaId <= 0 || string.IsNullOrWhiteSpace(modeloNombre))
                {
                    TempData["Error"] = "Los datos del modelo no son válidos.";
                    return RedirectToAction(nameof(Modelos));
                }

                var modeloDto = new ModeloCreateUpdateDto
                {
                    ModeloId = modeloId,
                    MarcaId = marcaId,
                    ModeloNombre = modeloNombre.Trim(),
                    Activo = activo
                };

                await _service.ActualizarModeloAsync(modeloDto);
                TempData["Success"] = "Modelo actualizado correctamente.";
                return RedirectToAction(nameof(Modelos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el modelo: " + ex.Message;
                return RedirectToAction(nameof(Modelos));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ModeloEditar(int id)
        {
            var dto = await _service.ObtenerModeloPorIdParaEditarAsync(id);
            if (dto == null)
            {
                TempData["Warning"] = "El modelo solicitado no existe.";
                return RedirectToAction(nameof(Modelos));
            }

            var vm = new ModeloFormViewModel
            {
                Modelo = dto,
                MarcasSelectList = await _service.ObtenerMarcasSelectListAsync()
            };
            return View(vm);
        }

       

        [HttpPost]
        public async Task<IActionResult> ModeloToggleStatus(int id)
        {
            try
            {
                await _service.CambiarEstadoModeloAsync(id);
                TempData["Success"] = "Estado del modelo actualizado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Modelos));
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarModelo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de modelo no válido.";
                    return RedirectToAction(nameof(Modelos));
                }

                await _service.EliminarModeloAsync(id);

                TempData["Success"] = "Modelo desactivado correctamente.";
                return RedirectToAction(nameof(Modelos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Modelos));
            }
        }





        ///================= AÑOS =================
        [HttpGet]
        public async Task<IActionResult> Anios()
        {
            try
            {
                var anios = await _service.ObtenerAniosAsync();
                var vm = new AnioIndexViewModel { Anios = anios };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de años: " + ex.Message;
                return View(new AnioIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnioCrear(string anioDescripcion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(anioDescripcion))
                {
                    TempData["Error"] = "La descripción del año es obligatoria.";
                    return RedirectToAction(nameof(Anios));
                }

                var dto = new AnioDto
                {
                    AnioDescripcion = anioDescripcion.Trim()
                };

                await _service.CrearAnioAsync(dto);
                TempData["Success"] = "Año registrado correctamente.";
                return RedirectToAction(nameof(Anios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el año: " + ex.Message;
                return RedirectToAction(nameof(Anios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnioEditar(int anioId, string anioDescripcion, bool activo)
        {
            try
            {
                if (anioId <= 0 || string.IsNullOrWhiteSpace(anioDescripcion))
                {
                    TempData["Error"] = "Los datos del año no son válidos.";
                    return RedirectToAction(nameof(Anios));
                }

                var dto = new AnioDto
                {
                    AnioId = anioId,
                    AnioDescripcion = anioDescripcion.Trim(),
                    Activo = activo
                };

                await _service.ActualizarAnioAsync(dto);
                TempData["Success"] = "Año actualizado correctamente.";
                return RedirectToAction(nameof(Anios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el año: " + ex.Message;
                return RedirectToAction(nameof(Anios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAnio(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de año no válido.";
                    return RedirectToAction(nameof(Anios));
                }

                await _service.EliminarAnioAsync(id);
                TempData["Success"] = "Año desactivado correctamente.";
                return RedirectToAction(nameof(Anios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Anios));
            }
        }


        //================= AÑOS =================






        //================= TRANSMISION =================

        [HttpGet]
        public async Task<IActionResult> Transmisiones()
        {
            try
            {
                var transmisiones = await _service.ObtenerTransmisionesAsync();
                var vm = new TransmisionIndexViewModel { Transmisiones = transmisiones };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de transmisiones: " + ex.Message;
                return View(new TransmisionIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransmisionCrear(string transmisionNombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transmisionNombre))
                {
                    TempData["Error"] = "El nombre de la transmisión es obligatorio.";
                    return RedirectToAction(nameof(Transmisiones));
                }

                var dto = new TransmisionDto
                {
                    TransmisionNombre = transmisionNombre.Trim()
                };

                await _service.CrearTransmisionAsync(dto);
                TempData["Success"] = "Transmisión registrada correctamente.";
                return RedirectToAction(nameof(Transmisiones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar la transmisión: " + ex.Message;
                return RedirectToAction(nameof(Transmisiones));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransmisionEditar(int transmisionId, string transmisionNombre, bool activo)
        {
            try
            {
                if (transmisionId <= 0 || string.IsNullOrWhiteSpace(transmisionNombre))
                {
                    TempData["Error"] = "Los datos de la transmisión no son válidos.";
                    return RedirectToAction(nameof(Transmisiones));
                }

                var dto = new TransmisionDto
                {
                    TransmisionId = transmisionId,
                    TransmisionNombre = transmisionNombre.Trim(),
                    Activo = activo
                };

                await _service.ActualizarTransmisionAsync(dto);
                TempData["Success"] = "Transmisión actualizada correctamente.";
                return RedirectToAction(nameof(Transmisiones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar la transmisión: " + ex.Message;
                return RedirectToAction(nameof(Transmisiones));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTransmision(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de transmisión no válido.";
                    return RedirectToAction(nameof(Transmisiones));
                }

                await _service.EliminarTransmisionAsync(id);
                TempData["Success"] = "Transmisión desactivada correctamente.";
                return RedirectToAction(nameof(Transmisiones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Transmisiones));
            }
        }

        //================= TRANSMISION =================




        //================= CILINDRO =================

        [HttpGet]
        public async Task<IActionResult> Cilindros()
        {
            try
            {
                var cilindros = await _service.ObtenerCilindrosAsync();
                var vm = new CilindroIndexViewModel { Cilindros = cilindros };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de cilindros: " + ex.Message;
                return View(new CilindroIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CilindroCrear(string cilindroNombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cilindroNombre))
                {
                    TempData["Error"] = "El nombre del cilindro es obligatorio.";
                    return RedirectToAction(nameof(Cilindros));
                }

                var dto = new CilindroDto
                {
                    CilindroNombre = cilindroNombre.Trim()
                };

                await _service.CrearCilindroAsync(dto);
                TempData["Success"] = "Cilindro registrado correctamente.";
                return RedirectToAction(nameof(Cilindros));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el cilindro: " + ex.Message;
                return RedirectToAction(nameof(Cilindros));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CilindroEditar(int cilindroId, string cilindroNombre, bool activo)
        {
            try
            {
                if (cilindroId <= 0 || string.IsNullOrWhiteSpace(cilindroNombre))
                {
                    TempData["Error"] = "Los datos del cilindro no son válidos.";
                    return RedirectToAction(nameof(Cilindros));
                }

                var dto = new CilindroDto
                {
                    CilindroId = cilindroId,
                    CilindroNombre = cilindroNombre.Trim(),
                    Activo = activo
                };

                await _service.ActualizarCilindroAsync(dto);
                TempData["Success"] = "Cilindro actualizado correctamente.";
                return RedirectToAction(nameof(Cilindros));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el cilindro: " + ex.Message;
                return RedirectToAction(nameof(Cilindros));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCilindro(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de cilindro no válido.";
                    return RedirectToAction(nameof(Cilindros));
                }

                await _service.EliminarCilindroAsync(id);
                TempData["Success"] = "Cilindro desactivado correctamente.";
                return RedirectToAction(nameof(Cilindros));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Cilindros));
            }
        }



        //================= CILINDRO =================





        //================= COLOR =================
        [HttpGet]
        public async Task<IActionResult> Colores()
        {
            try
            {
                var colores = await _service.ObtenerColoresAsync();
                var vm = new ColorIndexViewModel { Colores = colores };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el catálogo de colores: " + ex.Message;
                return View(new ColorIndexViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ColorCrear(string colorNombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(colorNombre))
                {
                    TempData["Error"] = "El nombre del color es obligatorio.";
                    return RedirectToAction(nameof(Colores));
                }

                var dto = new ColorDto
                {
                    ColorNombre = colorNombre.Trim()
                };

                await _service.CrearColorAsync(dto);
                TempData["Success"] = "Color registrado correctamente.";
                return RedirectToAction(nameof(Colores));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar el color: " + ex.Message;
                return RedirectToAction(nameof(Colores));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ColorEditar(int colorId, string colorNombre, bool activo)
        {
            try
            {
                if (colorId <= 0 || string.IsNullOrWhiteSpace(colorNombre))
                {
                    TempData["Error"] = "Los datos del color no son válidos.";
                    return RedirectToAction(nameof(Colores));
                }

                var dto = new ColorDto
                {
                    ColorId = colorId,
                    ColorNombre = colorNombre.Trim(),
                    Activo = activo
                };

                await _service.ActualizarColorAsync(dto);
                TempData["Success"] = "Color actualizado correctamente.";
                return RedirectToAction(nameof(Colores));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el color: " + ex.Message;
                return RedirectToAction(nameof(Colores));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarColor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Identificador de color no válido.";
                    return RedirectToAction(nameof(Colores));
                }

                await _service.EliminarColorAsync(id);
                TempData["Success"] = "Color desactivado correctamente.";
                return RedirectToAction(nameof(Colores));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la solicitud: " + ex.Message;
                return RedirectToAction(nameof(Colores));
            }
        }


        //================= COLOR =================










    }
}
