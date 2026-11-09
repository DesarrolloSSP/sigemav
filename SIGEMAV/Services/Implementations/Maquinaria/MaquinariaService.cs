using AspNetCoreGeneratedDocument;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SIGEMAV.Areas.Maquinaria.Controllers;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Areas.Maquinaria.Funciones;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;
using static SIGEMAV.Services.Implementations.Maquinaria.MaquinariaService;
using static SIGEMAV.Services.Interfaces.Maquinaria.IMaquinariaService;
using static System.Net.WebRequestMethods;
using Colors = QuestPDF.Helpers.Colors;
using File = System.IO.File;
using QPdf = QuestPDF.Fluent.Document;





namespace SIGEMAV.Services.Implementations.Maquinaria
{
    public class MaquinariaService : IMaquinariaService
    {
        private readonly BdSigeMavContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _http;
        private readonly ILogger<TallerConfiguracionController> _logger;
        private readonly IWebHostEnvironment _env;

        public MaquinariaService(BdSigeMavContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, HttpClient http,
            ILogger<TallerConfiguracionController> logger,
            IWebHostEnvironment env 
            )
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _http = http;
            _logger = logger;
            _env = env;
        }

        public async Task<PaginacionViewModel<UnidadVehicularViewModel>> ObtenerVehiculos(
     int pagina = 1,
     int registrosPorPagina = 10,
     string? busqueda = null,
     int? marcaId = null,
     int? modeloId = null,
     int? anioDesde = null,
     int? anioHasta = null,
     int? areaId = null,
     int? municipioId = null)
        {
            var query = _context.UnidadVehiculars
                .AsNoTracking()
                .Where(x => x.Activo);

            //=====================================
            // BÚSQUEDA GENERAL
            //=====================================

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                query = query.Where(x =>
                    x.NumeroSerie.Contains(busqueda)
                    || x.NumeroEconomico.Contains(busqueda)
                    || x.PlacaActual.Contains(busqueda)
                    || x.Marca.MarcaNombre.Contains(busqueda)
                    || x.Modelo.ModeloNombre.Contains(busqueda));
            }

            //=====================================
            // FILTROS
            //=====================================

            if (marcaId.HasValue)
            {
                query = query.Where(x =>
                    x.MarcaId == marcaId.Value);
            }

            if (modeloId.HasValue)
            {
                query = query.Where(x =>
                    x.ModeloId == modeloId.Value);
            }

            if (anioDesde.HasValue)
            {
                query = query.Where(x =>
                    x.AnioId >= anioDesde.Value);
            }

            if (anioHasta.HasValue)
            {
                query = query.Where(x =>
                    x.AnioId <= anioHasta.Value);
            }

            if (areaId.HasValue)
            {
                query = query.Where(x =>
                    x.AreaId == areaId.Value);
            }

            if (municipioId.HasValue)
            {
                query = query.Where(x =>
                    x.MunicipioId == municipioId.Value);
            }

            //=====================================
            // TOTAL
            //=====================================

            var totalRegistros =
                await query.CountAsync();

            //=====================================
            // REGISTROS
            //=====================================

            var registros = await query

                .OrderBy(x => x.UnidadVehicularId)

                .Skip((pagina - 1) * registrosPorPagina)

                .Take(registrosPorPagina)

                .Select(x => new UnidadVehicularViewModel
                {
                    IdUnidadVehicular =
                        x.UnidadVehicularId,

                    NumeroSerie =
                        x.NumeroSerie,

                    NumeroEconomico =
                        x.NumeroEconomico,

                    PlacaActual =
                        x.PlacaActual,

                    MarcaNombre =
                        x.Marca.MarcaNombre,

                    ModeloNombre =
                        x.Modelo.ModeloNombre,

                    AnioNombre =
                        x.Anio.AnioDescripcion,

                    ColorNombre =
                        x.Color.ColorNombre,

                    MunicipioNombre =
                        x.Municipio.Nombre,

                    AreaNombre =
                        x.Area.AreaNombre,

                    CilindroNombre =
                        x.Cilindro.CilindroNombre,

                    TransmisionNombre =
                        x.Transmision.TransmisionNombre,

                    IdMarca =
                        x.MarcaId,

                    IdModelo =
                        x.ModeloId,

                    IdAnio =
                        x.AnioId,

                    IdColor =
                        x.ColorId,

                    IdMunicipio =
                        x.MunicipioId,

                    IdArea =
                        x.AreaId,

                    IdCilindro =
                        x.CilindroId,

                    IdTransmision =
                        x.TransmisionId,

                    Activo =
                        x.Activo
                })

                .ToListAsync();

            return new PaginacionViewModel<UnidadVehicularViewModel>
            {
                Registros = registros,

                PaginaActual = pagina,

                RegistrosPorPagina = registrosPorPagina,

                TotalRegistros = totalRegistros,

                TotalPaginas =
                    (int)Math.Ceiling(
                        totalRegistros /
                        (double)registrosPorPagina)
            };
        }






        public async Task<List<SelectListItem>> ObtenerMarcas()
        {
            return await _context.Marcas
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.MarcaNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.MarcaId.ToString(),
                    Text = x.MarcaNombre
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerModelosPorMarca(int marcaId)
        {
            return await _context.Modelos
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .Where(x => x.MarcaId == marcaId)
                .OrderBy(x => x.ModeloNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.ModeloId.ToString(),
                    Text = x.ModeloNombre
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerAnios()
        {
            return await _context.Anios
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderByDescending(x => x.AnioDescripcion)
                .Select(x => new SelectListItem
                {
                    Value = x.AnioId.ToString(),
                    Text = x.AnioDescripcion
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerColores()
        {
            return await _context.Colors
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.ColorNombre)
                .Where(x => x.Activo == true)
                .Select(x => new SelectListItem
                {
                    Value = x.ColorId.ToString(),
                    Text = x.ColorNombre
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerMunicipios()
        {
            return await _context.Municipios
                .AsNoTracking()
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem
                {
                    Value = x.MunicipioId.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();
        }





        public async Task<List<SelectListItem>> ObtenerCilindro()
        {
            return await _context.Cilindros
                .AsNoTracking()
                .OrderBy(x => x.CilindroNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.CilindroId.ToString(),
                    Text = x.CilindroNombre
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerTransmision()
        {
            return await _context.Transmisions
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.TransmisionNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.TransmisionId.ToString(),
                    Text = x.TransmisionNombre
                })
                .ToListAsync();
        }




        public async Task<ResultadoOperacionDto> CrearVehiculo(UnidadVehicularCreateDto dto)
        {
            try
            {
                // =========================================================
                // NORMALIZACIÓN
                // =========================================================

                dto.NumeroSerie =
                    dto.NumeroSerie?.Trim();

                dto.PlacaActual =
                    dto.PlacaActual?.Trim().ToUpper();

                dto.NumeroEconomico =
                    dto.NumeroEconomico?.Trim();


                // =========================================================
                // NÚMERO DE SERIE
                // Siempre debe ser único
                // =========================================================

                var existeNumeroSerie =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroSerie == dto.NumeroSerie);

                if (existeNumeroSerie)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ya existe un vehículo con el mismo número de serie."
                    };
                }


                // =========================================================
                // PLACA
                //
                // "ALTA" significa que todavía no tiene placa asignada.
                // Por lo tanto, ALTA puede repetirse.
                // Cualquier placa real debe ser única.
                // =========================================================

                if (dto.PlacaActual != "SIN PLACA")
                {
                    var existePlaca = await _context.UnidadVehiculars
                            .AnyAsync(x =>
                                x.PlacaActual == dto.PlacaActual);

                    if (existePlaca)
                    {
                        return new ResultadoOperacionDto
                        {
                            Exitoso = false,
                            Mensaje =
                                "Ya existe un vehículo con la misma placa."
                        };
                    }
                }


                // =========================================================
                // NÚMERO ECONÓMICO
                // Actualmente NO se valida unicidad.
                // =========================================================

                var entidad = new UnidadVehicular
                {
                    NumeroSerie = dto.NumeroSerie,
                    PlacaActual = dto.PlacaActual,
                    NumeroEconomico = dto.NumeroEconomico,

                    MarcaId = dto.MarcaId,
                    ModeloId = dto.ModeloId,
                    AnioId = dto.AnioId,
                    ColorId = dto.ColorId,
                    MunicipioId = dto.MunicipioId,
                    AreaId = dto.AreaId,
                    TransmisionId = dto.TransmisionId,
                    CilindroId = dto.CilindroId,

                    Activo = true,

                    FechaCreacion = DateTime.Now
                };

                _context.UnidadVehiculars.Add(entidad);

                await _context.SaveChangesAsync();

                return new ResultadoOperacionDto
                {
                    Exitoso = true,
                    Mensaje =
                        "Vehículo registrado correctamente."
                };
            }
            catch
            {
                return new ResultadoOperacionDto
                {
                    Exitoso = false,
                    Mensaje =
                        "Ocurrió un error al registrar el vehículo."
                };
            }
        }



        public async Task<UnidadVehicularCreateViewModel> ObtenerVehiculoEditar(int id)
        {
#pragma warning disable CS8603 // Posible tipo de valor devuelto de referencia nulo
            return await _context.UnidadVehiculars
                .AsNoTracking()
                .Where(x => x.UnidadVehicularId == id)
                .Select(x => new UnidadVehicularCreateViewModel
                {
                    IdUnidadVehicular = x.UnidadVehicularId,

                    NumeroSerie = x.NumeroSerie,

                    PlacaActual = x.PlacaActual,

                    NumeroEconomico = x.NumeroEconomico,

                    IdMarca = x.MarcaId,

                    IdModelo = x.ModeloId,

                    IdAnio = x.AnioId,

                    IdColor = x.ColorId,

                    IdMunicipio = x.MunicipioId,

                    IdArea = x.AreaId,

                    IdTransmision = x.TransmisionId,

                    IdCilindro = x.CilindroId
                })
                .FirstOrDefaultAsync();
#pragma warning restore CS8603 // Posible tipo de valor devuelto de referencia nulo
        }





        public async Task<ResultadoOperacionDto> EditarVehiculo(
     UnidadVehicularEditDto dto)
        {
            try
            {
                // =========================================================
                // BUSCAR VEHÍCULO
                // =========================================================

                var vehiculo =
                    await _context.UnidadVehiculars
                        .FirstOrDefaultAsync(x =>
                            x.UnidadVehicularId ==
                            dto.IdUnidadVehicular);

                if (vehiculo == null)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Vehículo no encontrado."
                    };
                }


                // =========================================================
                // NORMALIZACIÓN
                // =========================================================

                dto.NumeroSerie =
                    dto.NumeroSerie?.Trim().ToUpper();

                dto.PlacaActual =
                    dto.PlacaActual?.Trim().ToUpper();

                dto.NumeroEconomico =
                    dto.NumeroEconomico?.Trim();


                // =========================================================
                // NÚMERO DE SERIE
                //
                // Siempre debe ser único.
                // Se excluye el vehículo que estamos editando.
                // =========================================================

                var serieDuplicada =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroSerie ==
                                dto.NumeroSerie
                            &&
                            x.UnidadVehicularId !=
                                dto.IdUnidadVehicular);

                if (serieDuplicada)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ya existe un vehículo con el mismo número de serie."
                    };
                }


                // =========================================================
                // PLACA
                //
                // ALTA puede repetirse.
                // Cualquier placa real debe ser única.
                // =========================================================

                if (dto.PlacaActual != "ALTA")
                {
                    var placaDuplicada =
                        await _context.UnidadVehiculars
                            .AnyAsync(x =>
                                x.PlacaActual ==
                                    dto.PlacaActual
                                &&
                                x.UnidadVehicularId !=
                                    dto.IdUnidadVehicular);

                    if (placaDuplicada)
                    {
                        return new ResultadoOperacionDto
                        {
                            Exitoso = false,
                            Mensaje =
                                "Ya existe un vehículo con la misma placa."
                        };
                    }
                }


                // =========================================================
                // NÚMERO ECONÓMICO
                //
                // Actualmente no se aplica restricción de unicidad.
                // =========================================================

                var economicoDuplicado =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroEconomico ==
                                dto.NumeroEconomico
                            &&
                            x.UnidadVehicularId !=
                                dto.IdUnidadVehicular);

                // Actualmente NO se utiliza.
                // Se conserva la validación por si posteriormente
                // se decide hacer único el número económico.


                // =========================================================
                // ACTUALIZAR ENTIDAD
                // =========================================================

                vehiculo.NumeroSerie =
                    dto.NumeroSerie;

                vehiculo.PlacaActual =
                    dto.PlacaActual;

                vehiculo.NumeroEconomico =
                    dto.NumeroEconomico;

                vehiculo.MarcaId =
                    dto.IdMarca;

                vehiculo.ModeloId =
                    dto.IdModelo;

                vehiculo.AnioId =
                    dto.IdAnio;

                vehiculo.ColorId =
                    dto.IdColor;

                vehiculo.MunicipioId =
                    dto.IdMunicipio;

                vehiculo.AreaId =
                    dto.IdArea;

                vehiculo.TransmisionId =
                    dto.IdTransmision;

                vehiculo.CilindroId =
                    dto.IdCilindro;


                // =========================================================
                // GUARDAR
                // =========================================================

                await _context.SaveChangesAsync();


                return new ResultadoOperacionDto
                {
                    Exitoso = true,
                    Mensaje =
                        "Vehículo actualizado correctamente."
                };
            }
            catch
            {
                return new ResultadoOperacionDto
                {
                    Exitoso = false,
                    Mensaje =
                        "Ocurrió un error al actualizar el vehículo."
                };
            }
        }




        public async Task<PaginacionViewModel<MantenimientoViewModel>> ObtenerMantenimientos(int pagina = 1, int registrosPorPagina = 10)
        {
            var query = _context.Mantenimientos
                .AsNoTracking();

            var totalRegistros = await query.CountAsync();

            var registros = await query
                .OrderByDescending(x => x.MantenimientoId)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(x => new MantenimientoViewModel
                {
                    MantenimientoId = x.MantenimientoId,

                    //Folio = "SM/"
                    //         + x.MantenimientoId.ToString().PadLeft(4, '0')
                    //         + "/"
                    //         + x.FechaIngreso.Year,

                    Folio = x.Folio,

                    NumeroEconomico = x.UnidadVehicular.NumeroEconomico,

                    PlacaActual = x.UnidadVehicular.PlacaActual,

                    MarcaNombre = x.UnidadVehicular.Marca.MarcaNombre,

                    ModeloNombre = x.UnidadVehicular.Modelo.ModeloNombre,

                    TipoMantenimiento = x.TipoMantenimiento.Nombre,

                    TallerNombre = x.Taller.RazonSocial,

                    FechaIngreso = x.FechaIngreso,

                    FechaSalida = x.FechaSalida,

                    KilometrajeEntrada = x.KilometrajeEntrada,

                    Activo = x.Activo
                })
                .ToListAsync();

            return new PaginacionViewModel<MantenimientoViewModel>
            {
                Registros = registros,

                PaginaActual = pagina,

                RegistrosPorPagina = registrosPorPagina,

                TotalRegistros = totalRegistros,

                TotalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)registrosPorPagina)
            };
        }



        public async Task<List<SelectListItem>> ObtenerUnidadesVehicularesSelectList()
        {
            return await _context.UnidadVehiculars
                .AsNoTracking()
                .OrderBy(x => x.NumeroEconomico)
                .Select(x => new SelectListItem
                {
                    Value = x.UnidadVehicularId.ToString(),

                    Text =
                        x.NumeroEconomico
                        + " - "
                        + x.NumeroSerie
                        + " - "
                        + x.PlacaActual
                        + " - "
                        + x.Marca.MarcaNombre
                        + " "
                        + x.Modelo.ModeloNombre
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> ObtenerTiposMantenimiento()
        {
            return await _context.TipoMantenimientos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem
                {
                    Value = x.TipoMantenimientoId.ToString(),

                    Text = x.Nombre
                })
                .ToListAsync();
        }


        //public async Task<List<SelectListItem>> ObtenerTalleres()
        //{
        //    return await _context.Tallers
        //        .AsNoTracking()
        //        .Where(x => x.Activo)
        //        .OrderBy(x => x.RazonSocial)
        //        .Select(x => new SelectListItem
        //        {
        //            Value =
        //                x.TallerId.ToString(),

        //            Text =
        //                x.RazonSocial
        //                + " -- "
        //                + x.Ubicacion.Trim()

        //            // Si posteriormente se requiere mostrar
        //            // el recurso presupuestal:
        //            //
        //            // Text =
        //            //     x.RazonSocial
        //            //     + " -- "
        //            //     + (
        //            //         x.TallerConfiguracions
        //            //             .Where(tc =>
        //            //                 tc.Activo &&
        //            //                 tc.TallerConfiguracionPresupuestal.Activo)
        //            //             .Select(tc =>
        //            //                 tc.TallerConfiguracionPresupuestal
        //            //                     .Recurso
        //            //                     .Nombre)
        //            //             .FirstOrDefault()
        //            //         ?? "SIN RECURSO"
        //            //       ).Trim()
        //        })
        //        .ToListAsync();
        //}

        //AHUEVO QUE QUIEREN QUE SE MUESTRE SIN LA UBICACION - PARA QUE DEJEN DE MAMAR , EL MÉTODO DE ARRIBA PERMITE MOSTRAR UBICACIONES
        public async Task<List<SelectListItem>> ObtenerTalleres()
        {
            return await _context.Tallers
                .AsNoTracking()
                .Where(x => x.Activo)
                .GroupBy(x => x.RazonSocial)
                .Select(g => new SelectListItem
                {
                    // Tomamos el ID del primer registro que coincida con esa Razón Social
                    Value = g.OrderBy(x => x.TallerId).Select(x => x.TallerId).First().ToString(),
                    Text = g.Key // g.Key representa la Razón Social agrupada, evitando duplicados
                })
                .OrderBy(x => x.Text)
                .ToListAsync();
        }



        public async Task<List<SelectListItem>> ObtenerAreas()
        {

            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
            var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

            if (usuarioGuid.ToString() == "0bf13a21-fe5a-45f9-a584-1f6d338e1995")
            {
                return await _context.Areas
              .AsNoTracking()
              .Where(x => x.Activo && x.AreaId == 22) 
              .OrderBy(x => x.AreaNombre)
              .Select(x => new SelectListItem
              {
                  Value = x.AreaId.ToString(),
                  Text = x.AreaNombre
              })
              .ToListAsync();
            }

            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE el siguiente CODIGO DEBE ESTAR SOLO EN ESTE MÉTODO
            else
            {
                return await _context.Areas
             .AsNoTracking()
             .Where(x => x.Activo)
             .OrderBy(x => x.AreaNombre)
             .Select(x => new SelectListItem
             {
                 Value = x.AreaId.ToString(),
                 Text = x.AreaNombre
             })
             .ToListAsync();
            }

         
        }



        //public async Task<PaginacionViewModel<SolicitudMantenimientoViewModel>> ObtenerSolicitudesMantenimiento(int pagina = 1, int registrosPorPagina = 10)
        public async Task<PaginacionViewModel<SolicitudMantenimientoViewModel>> ObtenerSolicitudesMantenimiento(SolicitudMantenimientoFiltroViewModel filtro, int pagina = 1, int registrosPorPagina = 10)
        {
            
            var query = _context.SolicitudMantenimientos
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(filtro.Folio))
            {
                query = query.Where(x =>
                    x.Folio.Contains(filtro.Folio));
            }

            if (!string.IsNullOrWhiteSpace(filtro.NumeroOficio))
            {
                query = query.Where(x =>
                    x.NumeroOficio.Contains(filtro.NumeroOficio));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Unidad))
            {
                string unidad = filtro.Unidad.Trim();

                query = query.Where(x =>

                    x.UnidadVehicular.NumeroEconomico.Contains(unidad)

                    ||

                    x.UnidadVehicular.NumeroSerie.Contains(unidad)

                    ||

                    x.UnidadVehicular.PlacaActual.Contains(unidad)

                    ||

                    x.UnidadVehicular.Marca.MarcaNombre.Contains(unidad)

                    ||

                    x.UnidadVehicular.Modelo.ModeloNombre.Contains(unidad)
                );
            }

            if (filtro.AreaSolicitanteId.HasValue)
            {
                query = query.Where(x =>
                    x.AreaSolicitanteId ==
                    filtro.AreaSolicitanteId.Value);
            }

            if (filtro.EstatusSolicitudMantenimientoId.HasValue)
            {
                query = query.Where(x =>
                    x.EstatusSolicitudMantenimientoId ==
                    filtro.EstatusSolicitudMantenimientoId.Value);
            }

            if (filtro.TipoMantenimientoId.HasValue)
            {
                query = query.Where(x =>
                    x.TipoMantenimientoId ==
                    filtro.TipoMantenimientoId.Value);
            }

            //DateOnly fechaInicio = filtro.FechaInicio ?? DateOnly.MinValue;


            if (filtro.FechaInicio.HasValue)
            {
                var fechaInicio = filtro.FechaInicio.Value.ToDateTime(TimeOnly.MinValue);

                query = query.Where(x =>
                    x.FechaSolicitud >= fechaInicio);
            }

            if (filtro.FechaFin.HasValue)
            {
                var fechaFin = filtro.FechaFin.Value.ToDateTime(TimeOnly.MaxValue);

                query = query.Where(x =>
                    x.FechaSolicitud <= fechaFin);
            }




            var totalRegistros = await query.CountAsync();




            var kpi = new SolicitudMantenimientoKpiViewModel
            {
                TotalSolicitudes = await query.CountAsync(),

                PendientesDSP = await query.CountAsync(x =>
                    x.EstatusSolicitudMantenimientoId == 2),

                EnRevision = await query.CountAsync(x =>
                    x.EstatusSolicitudMantenimientoId == 4),

                Autorizadas = await query.CountAsync(x =>
                    x.EstatusSolicitudMantenimientoId == 5),

                EnMantenimiento = await query.CountAsync(x =>
                    x.EstatusSolicitudMantenimientoId == 7),

                Finalizadas = await query.CountAsync(x =>
                    x.EstatusSolicitudMantenimientoId == 8),

                // Se proyecta el cálculo del IVA sobre la suma total directamente en la base de datos
                ImporteCotizado = (await query
                 .Where(x => x.ImporteCotizacion.HasValue)
                 .SumAsync(x => x.ImporteCotizacion) ?? 0m) * 1.16m
            };



            var registros = await query
                .OrderByDescending(x => x.SolicitudMantenimientoId)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .Select(x => new SolicitudMantenimientoViewModel
                {
                    SolicitudMantenimientoId = x.SolicitudMantenimientoId,

                    Folio = x.Folio,

                    NumeroOficio = x.NumeroOficio,
                    FechaSolicitud = x.FechaSolicitud,
                    AreaSolicitante = x.AreaSolicitante.AreaNombre,
                    NumeroEconomico = x.UnidadVehicular.NumeroEconomico,
                    NumeroSerie = x.UnidadVehicular.NumeroSerie,
                    PlacaActual = x.UnidadVehicular.PlacaActual,
                    MarcaNombre = x.UnidadVehicular.Marca.MarcaNombre,
                    ModeloNombre = x.UnidadVehicular.Modelo.ModeloNombre,
                    TipoMantenimiento = x.TipoMantenimiento.Nombre,
                    TallerNombre = x.Taller != null ? x.Taller.RazonSocial : "Pendiente",
                    ImporteCotizacion = x.ImporteCotizacion,
                    Estatus = x.EstatusSolicitudMantenimiento.Nombre,
                    ColorEstatus = x.EstatusSolicitudMantenimiento.Color,
                    TotalConceptos = x.SolicitudMantenimientoDetalles.Count(),
                    EstatusSolicitudMantenimientoId = x.EstatusSolicitudMantenimientoId,

                    Activo = x.Activo
                })
                .ToListAsync();




            //---------------------------------------------------------
            // CARGAR FILTROS
            //---------------------------------------------------------

            filtro.Areas = await _context.Areas
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.AreaNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.AreaId.ToString(),
                    Text = x.AreaNombre
                })
                .ToListAsync();

            filtro.Estatus = await _context.EstatusSolicitudMantenimientos
                .AsNoTracking()
                .OrderBy(x => x.EstatusSolicitudMantenimientoId)
                .Select(x => new SelectListItem
                {
                    Value = x.EstatusSolicitudMantenimientoId.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();

            filtro.TiposMantenimiento = await _context.TipoMantenimientos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem
                {
                    Value = x.TipoMantenimientoId.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();

            filtro.Unidades = await _context.UnidadVehiculars
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.NumeroEconomico)
                .Select(x => new SelectListItem
                {
                    Value = x.UnidadVehicularId.ToString(),
                    Text = x.NumeroEconomico + " - " + x.PlacaActual
                })
                .ToListAsync();




            return new PaginacionViewModel<SolicitudMantenimientoViewModel>
            {
                Registros = registros,

                PaginaActual = pagina,

                RegistrosPorPagina = registrosPorPagina,

                TotalRegistros = totalRegistros,

                TotalConceptos = registros.Sum(r => r.TotalConceptos),

                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina),

                Filtro = filtro,

                Kpi = kpi

            };


        }




        public async Task<List<SelectListItem>> ObtenerAreasSelectList(int? seleccionado)
        {

            var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

            // ES CHIPOTE ??   SI SE LOGUE ASIGNARLE EL ÁREA OM 
            if (usuarioGuid.HasValue && usuarioGuid.ToString() == "0bf13a21-fe5a-45f9-a584-1f6d338e1995")
            {
                return await _context.Areas
                .Where(x => x.Activo && x.AreaId == 22)
                .OrderBy(x => x.AreaNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.AreaId.ToString(),
                    Text = x.AreaNombre,
                    Selected = seleccionado.HasValue && x.AreaId == seleccionado.Value
                })
                .ToListAsync();
            }
            else
            {





                //////////////////////////////////ESTA SECCION DEBE ESTAR FUERA DEL IF UNICAMENTE.
                return await _context.Areas
                .Where(x => x.Activo)
                .OrderBy(x => x.AreaNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.AreaId.ToString(),
                    Text = x.AreaNombre,
                    Selected = seleccionado.HasValue &&
                               x.AreaId == seleccionado.Value
                })
                .ToListAsync();
            }

        }




        public async Task<List<SelectListItem>> ObtenerEstatusSelectList(int? seleccionado)
        {
            return await _context.EstatusSolicitudMantenimientos
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem
                {
                    Value = x.EstatusSolicitudMantenimientoId.ToString(),
                    Text = x.Nombre,
                    Selected = seleccionado.HasValue &&
                               x.EstatusSolicitudMantenimientoId == seleccionado.Value
                })
                .ToListAsync();
        }



        public async Task<List<SelectListItem>> ObtenerTiposMantenimientoSelectList(int? seleccionado)
        {
            return await _context.TipoMantenimientos
                .OrderBy(x => x.Nombre)
                .Where(x => x.Activo == true)
                .Select(x => new SelectListItem
                {
                    Value = x.TipoMantenimientoId.ToString(),
                    Text = x.Nombre,
                    Selected = seleccionado.HasValue &&
                               x.TipoMantenimientoId == seleccionado.Value
                })
                .ToListAsync();
        }





        public async Task<List<Select2ItemViewModel>> BusquedaRapidaUnidades(string? term)
        {
            term ??= "";

            term = term.Trim();

            return await _context.UnidadVehiculars
                .AsNoTracking()
                .Where(x => x.Activo)
                .Where(x =>

                    x.NumeroEconomico.Contains(term)

                    ||

                    x.NumeroSerie.Contains(term)

                    ||

                    x.PlacaActual.Contains(term)

                    ||

                    x.Marca.MarcaNombre.Contains(term)

                    ||

                    x.Modelo.ModeloNombre.Contains(term)

                )
                .OrderBy(x => x.NumeroEconomico)
                .Take(20)
                .Select(x => new Select2ItemViewModel
                {
                    id = x.UnidadVehicularId,

                    text =
                        x.NumeroEconomico +
                        " | " +
                        x.PlacaActual +
                        " | " +
                        x.Marca.MarcaNombre +
                        " " +
                        x.Modelo.ModeloNombre +
                        " | " +
                        x.NumeroSerie
                })
                .ToListAsync();
        }








        public async Task<List<SelectListItem>> ObtenerEstatusSolicitudMantenimiento()
        {
            return await _context
                .EstatusSolicitudMantenimientos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Orden)
                .Select(x => new SelectListItem
                {
                    Value = x.EstatusSolicitudMantenimientoId
                        .ToString(),

                    Text = x.Nombre
                })
                .ToListAsync();
        }



        public async Task<ResultadoCreacionSolicitudDto> CrearSolicitudMantenimiento(SolicitudMantenimientoCreateDto dto, IEnumerable<IFormFile>? archivos)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var archivosGuardados = new List<string>();
            try
            {
                // =====================================================
                // VALIDACIONES GENERALES
                // =====================================================
                if (dto == null)
                {
                    throw new Exception("La información de la solicitud es inválida.");
                }

                if (string.IsNullOrWhiteSpace(dto.NumeroOficio))
                {
                    throw new Exception("Debe capturar el número de oficio.");
                }

                if (dto.UnidadVehicularId <= 0)
                {
                    throw new Exception("Debe seleccionar una unidad vehicular.");
                }

                if (dto.TipoMantenimientoId <= 0)
                {
                    throw new Exception("Debe seleccionar un tipo de mantenimiento.");
                }

                if (dto.TallerId <= 0)
                {
                    throw new Exception("Debe seleccionar un taller.");
                }

                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    throw new Exception("Debe seleccionar una configuración/contrato presupuestal del taller.");
                }




                // =========================================
                // VALIDAR KILOMETRAJE
                // =========================================
                if (dto.KilometrajeActual < 0)
                {
                    throw new Exception("El kilometraje no puede ser negativo.");
                }

                if (dto.KilometrajeActual > 999999)
                {
                    throw new Exception("El kilometraje debe estar entre 0 y 999999.");
                }

                if (dto.EstadoOdometroId == 1 && dto.KilometrajeActual <= 0)
                {
                    throw new Exception("Cuando el odómetro opera correctamente, el kilometraje debe ser mayor a cero.");
                }

                if (dto.EstadoOdometroId != 1 && dto.KilometrajeActual > 0)
                {
                    throw new Exception("Si el odómetro no opera correctamente, el kilometraje debe ser cero.");
                }


                // Validación cruzada con el último kilometraje histórico registrado en BD
                if (dto.EstadoOdometroId == 1)
                {
                    var ultimoKmHistorico = await ObtenerUltimoKilometrajeUnidadAsync(dto.UnidadVehicularId);
                    if (ultimoKmHistorico > 0 && dto.KilometrajeActual < ultimoKmHistorico)
                    {
                        throw new Exception($"El kilometraje capturado ({dto.KilometrajeActual}) no puede ser menor al último registrado en el sistema para esta unidad ({ultimoKmHistorico}).");
                    }
                }

                // =====================================================
                // PROTECCIÓN EN SERVIDOR: VALIDAR CONFIGURACIÓN Y TALLER
                // =====================================================
                var existeConfiguracionValida = await (
                    from tc in _context.TallerConfiguracions
                    join tcp in _context.TallerConfiguracionPresupuestals
                        on tc.TallerConfiguracionPresupuestalId equals tcp.TallerConfiguracionPresupuestalId
                    where tc.TallerId == dto.TallerId
                          && tc.TallerConfiguracionPresupuestalId == dto.TallerConfiguracionPresupuestalId
                          && tc.Activo
                          && tcp.Activo
                    select tcp
                ).AnyAsync();

                if (!existeConfiguracionValida)
                {
                    throw new Exception("La configuración/contrato presupuestal seleccionado no pertenece al taller o no se encuentra activo.");
                }

                if (dto.AreaSolicitanteId <= 0)
                {
                    throw new Exception("Debe seleccionar un área solicitante.");
                }


                if (dto.Servicios == null || !dto.Servicios.Any())
                {
                    throw new Exception("Debe capturar al menos un servicio.");
                }

                // =====================================================
                // VALIDAR SERVICIOS
                // =====================================================
                foreach (var servicio in dto.Servicios)
                {
                    if (servicio.ConceptoServicioId <= 0)
                    {
                        throw new Exception("Existe un servicio inválido.");
                    }

                    if (servicio.Cantidad <= 0)
                    {
                        throw new Exception("La cantidad del servicio debe ser mayor a cero.");
                    }

                    if (servicio.CostoEstimadoManoObra < 0)
                    {
                        throw new Exception("El costo estimado de mano de obra es inválido.");
                    }

                    if (servicio.CostoEstimadoRefacciones < 0)
                    {
                        throw new Exception("El costo estimado de refacciones es inválido.");
                    }
                }

               




                // =====================================================
                // GENERAR FOLIO ÚNICO (REFORZADO)
                // =====================================================
                var fechaActual = DateTime.Now;
                var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;
                var anioActual = fechaActual.Year;
                var prefijoFolio = "SM/";
                var sufijoFolio = $"/{anioActual}";

                // 1. Obtener todos los folios creados en el año actual con patrón SM/XXX/AAAA
                var foliosDelAnio = await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .Where(x => x.Folio.StartsWith(prefijoFolio) && x.Folio.EndsWith(sufijoFolio))
                    .Select(x => x.Folio)
                    .ToListAsync();

                int consecutivoMaximo = 0;

                // 2. Analizar numéricamente el consecutivo central
                foreach (var f in foliosDelAnio)
                {
                    var partes = f.Split('/');
                    if (partes.Length == 3 && int.TryParse(partes[1], out int numActual))
                    {
                        if (numActual > consecutivoMaximo)
                        {
                            consecutivoMaximo = numActual;
                        }
                    }
                }

                int nuevoConsecutivo = consecutivoMaximo + 1;
                var folio = $"SM/{nuevoConsecutivo:D3}/{anioActual}";

                if (string.IsNullOrWhiteSpace(folio))
                {
                    throw new Exception("No fue posible generar un folio único.");
                }


                // =====================================================
                // VALIDAR ARCHIVOS
                // =====================================================
                const int maxArchivos = 5;
                const long tamanioMaximoArchivo = 2 * 1024 * 1024;

                var extensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png" };
                var contentTypesPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "application/pdf", "image/jpeg", "image/png" };
                var provider = new FileExtensionContentTypeProvider();

                var archivosValidados = new List<(byte[] Bytes, string Extension, string NombreOriginal, string NombreFisico, string HashArchivo)>();

                if (archivos != null)
                {
                    var archivosLista = archivos
                        .Where(x => x != null && x.Length > 0 && !string.IsNullOrWhiteSpace(x.FileName))
                        .GroupBy(x => new { Nombre = x.FileName.Trim().ToUpperInvariant(), Tamanio = x.Length })
                        .Select(x => x.First())
                        .ToList();

                    if (archivosLista.Count > maxArchivos)
                    {
                        throw new Exception($"Solo se permiten {maxArchivos} archivos.");
                    }

                    foreach (var archivo in archivosLista)
                    {
                        if (archivo.Length > tamanioMaximoArchivo)
                        {
                            throw new Exception($"El archivo '{archivo.FileName}' excede el tamaño máximo permitido de 2 MB.");
                        }

                        var nombreOriginal = Path.GetFileName(archivo.FileName?.Trim());

                        if (string.IsNullOrWhiteSpace(nombreOriginal))
                        {
                            throw new Exception("Nombre de archivo inválido.");
                        }

                        if (nombreOriginal.Length > 150)
                        {
                            throw new Exception("Nombre de archivo demasiado largo.");
                        }

                        var extension = Path.GetExtension(nombreOriginal).ToLowerInvariant();

                        if (string.IsNullOrWhiteSpace(extension) || !extensionesPermitidas.Contains(extension))
                        {
                            throw new Exception($"El archivo '{nombreOriginal}' tiene una extensión no permitida.");
                        }

                        if (!provider.TryGetContentType(nombreOriginal, out _))
                        {
                            throw new Exception($"No fue posible validar el tipo del archivo '{nombreOriginal}'.");
                        }

                        var contentTypeCliente = archivo.ContentType?.Trim().ToLowerInvariant();

                        if (string.IsNullOrWhiteSpace(contentTypeCliente) || !contentTypesPermitidos.Contains(contentTypeCliente))
                        {
                            throw new Exception($"El archivo '{nombreOriginal}' tiene un tipo MIME no permitido.");
                        }

                        using var memoryStream = new MemoryStream();
                        await archivo.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();

                        if (bytes.Length == 0)
                        {
                            throw new Exception($"El archivo '{nombreOriginal}' está vacío.");
                        }

                        if (!ArchivoValidoPorFirma(extension, bytes))
                        {
                            throw new Exception($"El contenido real del archivo '{nombreOriginal}' no coincide con su extensión.");
                        }

                        string hashArchivo;
                        using (var sha256 = SHA256.Create())
                        {
                            var hashBytes = sha256.ComputeHash(bytes);
                            hashArchivo = Convert.ToHexString(hashBytes);
                        }

                        nombreOriginal = nombreOriginal.Trim();

                        var duplicadoEnCarga = archivosValidados.Any(x =>
                            x.HashArchivo == hashArchivo &&
                            string.Equals(x.NombreOriginal, nombreOriginal, StringComparison.OrdinalIgnoreCase)
                        );

                        if (duplicadoEnCarga)
                        {
                            continue;
                        }

                        var nombreFisico = $"{Guid.NewGuid():N}{extension}";

                        archivosValidados.Add((
                            Bytes: bytes,
                            Extension: extension,
                            NombreOriginal: nombreOriginal,
                            NombreFisico: nombreFisico,
                            HashArchivo: hashArchivo
                        ));
                    }
                }

                // REFORZAMIENTO OBLIGATORIO: Validar que exista al menos un archivo válido adjunto
                if (!archivosValidados.Any())
                {
                    throw new Exception("Debe adjuntar al menos un documento de soporte válido para registrar la solicitud de mantenimiento.");
                }





                // VALIDACIÓN HISTÓRICA CENTRAL
                ResultadoValidacionHistoricaDto resultadoHistorico = new();

                var conceptosServicio = dto.Servicios
                    .Where(x => x.ConceptoServicioId > 0)
                    .Select(x => x.ConceptoServicioId)
                    .Distinct()
                    .ToList();

                if (conceptosServicio.Any())
                {
                    resultadoHistorico = await ValidarServiciosHistoricos(
                        dto.UnidadVehicularId,
                        conceptosServicio,
                        dto.TipoMantenimientoId.Value);

                    dto.ValidacionHistorica = resultadoHistorico;

                    if (resultadoHistorico.TieneDuplicadosActivos)
                    {
                        throw new Exception(string.Join(Environment.NewLine, resultadoHistorico.Mensajes));
                    }
                }

                // =====================================================
                // CREAR ENTIDAD SOLICITUD MANTENIMIENTO
                // =====================================================
                var entidad = new SolicitudMantenimiento
                {
                    Folio = folio,
                    NumeroOficio = dto.NumeroOficio.Trim(),
                    FechaSolicitud = dto.FechaSolicitud,
                    AreaSolicitanteId = dto.AreaSolicitanteId,
                    UnidadVehicularId = dto.UnidadVehicularId,
                    TallerId = dto.TallerId,
                    TallerConfiguracionPresupuestalId = dto.TallerConfiguracionPresupuestalId,
                    TipoMantenimientoId = dto.TipoMantenimientoId.Value,
                    KilometrajeActual = dto.KilometrajeActual.Value,
                    Observaciones = dto.Observaciones,
                    ImporteCotizacion = dto.Servicios.Sum(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad),
                    EstatusSolicitudMantenimientoId = 2,
                    UsuarioSolicitaId = usuarioGuid,
                    Activo = true,
                    FechaCreacion = fechaActual
                };

                _context.SolicitudMantenimientos.Add(entidad);

                // Intento de guardado inicial para asegurar el folio en BD
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Captura colisión del índice único UQ_SolicitudMantenimiento_Folio
                    if (ex.InnerException?.Message.Contains("UQ_SolicitudMantenimiento_Folio") == true ||
                        ex.InnerException?.Message.Contains("2601") == true ||
                        ex.InnerException?.Message.Contains("2627") == true)
                    {
                        throw new Exception($"El folio '{folio}' ya fue asignado en otra operación simultánea. Por favor, intente guardar nuevamente.");
                    }

                    throw;
                }

                // =====================================================
                // CREAR DETALLES DE SERVICIOS
                // =====================================================
                foreach (var servicio in dto.Servicios)
                {
                    if (servicio.IdObjetoGastoManoObra.HasValue &&
                        servicio.IdObjetoGastoRefacciones.HasValue &&
                        servicio.IdObjetoGastoManoObra == servicio.IdObjetoGastoRefacciones)
                    {
                        throw new Exception("La partida de mano de obra y la de refacciones no pueden ser la misma.");
                    }

                    if (servicio.CostoEstimadoManoObra > 0 && !servicio.IdObjetoGastoManoObra.HasValue)
                    {
                        throw new Exception("Debe seleccionar una partida para mano de obra.");
                    }

                    if (servicio.CostoEstimadoRefacciones > 0 && !servicio.IdObjetoGastoRefacciones.HasValue)
                    {
                        throw new Exception("Debe seleccionar una partida para refacciones.");
                    }

                    var detalle = new SolicitudMantenimientoDetalle
                    {
                        SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                        ConceptoServicioId = servicio.ConceptoServicioId,
                        Cantidad = (int)servicio.Cantidad,
                        CostoEstimadoManoObra = servicio.CostoEstimadoManoObra,
                        CostoEstimadoRefacciones = servicio.CostoEstimadoRefacciones,
                        IdObjetoGastoManoObra = servicio.IdObjetoGastoManoObra,
                        IdObjetoGastoRefacciones = servicio.IdObjetoGastoRefacciones,
                        Observaciones = servicio.Observaciones,
                        FechaCreacion = fechaActual,
                        Activo = true
                    };

                    _context.SolicitudMantenimientoDetalles.Add(detalle);
                }

                await _context.SaveChangesAsync();

                // =====================================================
                // GUARDAR DOCUMENTOS DE SOPORTE (ARCHIVOS ADJUNTOS)
                // =====================================================
                var rutaBase = _configuration["Storage:DocumentosRuta"];

                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    throw new Exception("No se configuró la ruta de almacenamiento.");
                }

                // Limpiar formato del folio para nombre de carpeta seguro en disco (ej. SM_001_2026)
                var carpetaFolioSegura = folio.Replace('/', '_');
                var carpetaSolicitud = Path.Combine(rutaBase, carpetaFolioSegura);

                if (!Directory.Exists(carpetaSolicitud))
                {
                    Directory.CreateDirectory(carpetaSolicitud);
                }

                if (archivosValidados.Any())
                {
                    foreach (var archivo in archivosValidados)
                    {
                        var rutaCompleta = Path.Combine(carpetaSolicitud, archivo.NombreFisico);

                        await File.WriteAllBytesAsync(rutaCompleta, archivo.Bytes);
                        archivosGuardados.Add(rutaCompleta);

                        var documento = new SolicitudMantenimientoDocumento
                        {
                            SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                            TipoDocumento = "SOPORTE_INICIAL",
                            NombreArchivo = archivo.NombreOriginal,
                            RutaArchivo = rutaCompleta,
                            FechaCarga = DateTime.Now,
                            UsuarioCargaId = 1,
                            Activo = true,
                            HashArchivo = archivo.HashArchivo
                        };

                        _context.SolicitudMantenimientoDocumentos.Add(documento);
                    }

                    await _context.SaveChangesAsync();
                }

                // =====================================================
                // GENERACIÓN Y GUARDADO AUTOMÁTICO DEL OFICIO SOLICITUD DE MANTENIMIENTO PDF (QUESTPDF)
                // =====================================================
                // NOTA: Si deseas que un fallo en el PDF detenga la transacción, 
                // remueve el try-catch interno o lanza la excepción.
                byte[] pdfBytes = await GenerarOficioSolicitudPdfAsync(entidad.SolicitudMantenimientoId);

                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    string nombreOficioFisico = $"OFICIO_{entidad.SolicitudMantenimientoId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    string rutaOficioCompleta = Path.Combine(carpetaSolicitud, nombreOficioFisico);

                    await File.WriteAllBytesAsync(rutaOficioCompleta, pdfBytes);
                    archivosGuardados.Add(rutaOficioCompleta);

                    string hashOficio;
                    using (var sha256 = SHA256.Create())
                    {
                        hashOficio = Convert.ToHexString(sha256.ComputeHash(pdfBytes));
                    }

                    var docOficio = new SolicitudMantenimientoDocumento
                    {
                        SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                        TipoDocumento = "OFICIO_SOLICITUD",
                        NombreArchivo = $"Oficio_Solicitud_{entidad.NumeroOficio.Replace('/', '_')}.pdf",
                        RutaArchivo = rutaOficioCompleta,
                        FechaCarga = DateTime.Now,
                        UsuarioCargaId = 1,
                        Activo = true,
                        HashArchivo = hashOficio
                    };

                    _context.SolicitudMantenimientoDocumentos.Add(docOficio);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new ResultadoCreacionSolicitudDto
                {
                    Correcto = true,
                    SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                    Folio = folio,
                    ValidacionHistorica = resultadoHistorico
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                LimpiarArchivos(archivosGuardados);

                if (ex.InnerException?.Message.Contains("UQ_SolicitudMantenimiento_Folio") == true ||
                    ex.InnerException?.Message.Contains("2601") == true ||
                    ex.InnerException?.Message.Contains("2627") == true)
                {
                    throw new Exception("No se pudo guardar la solicitud porque el folio generado colisionó con otra transacción activa. Por favor, reintente la operación.");
                }

                throw;
            }
            catch
            {
                await transaction.RollbackAsync();
                LimpiarArchivos(archivosGuardados);
                throw;
            }
        }






        // Método auxiliar de limpieza de archivos para mantener el catch limpio
        private void LimpiarArchivos(List<string> rutas)
        {
            foreach (var ruta in rutas)
            {
                try
                {
                    if (File.Exists(ruta))
                    {
                        File.Delete(ruta);
                    }
                }
                catch
                {
                    // LOG
                }
            }
        }





        public async Task<List<SelectListItem>> ObtenerEstadosOdometro()
        {
            return await _context.EstadoOdometros
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Descripcion)
                .Select(x => new SelectListItem
                {
                    Value = x.EstadoOdometroId.ToString(),
                    Text = x.Descripcion
                })
                .ToListAsync();
        }





        public async Task<List<SolicitudMantenimientoDocumentoViewModel>> ObtenerDocumentosSolicitudMantenimiento(int solicitudMantenimientoId)
        {
            return await _context.SolicitudMantenimientoDocumentos
                .AsNoTracking()
                .Where(x =>
                    x.SolicitudMantenimientoId == solicitudMantenimientoId &&
                    x.Activo)
                .OrderByDescending(x => x.FechaCarga)
                .Select(x => new SolicitudMantenimientoDocumentoViewModel
                {
                    SolicitudMantenimientoDocumentoId =
                        x.SolicitudMantenimientoDocumentoId,

                    TipoDocumento =
                        x.TipoDocumento,

                    NombreArchivo =
                        x.NombreArchivo,

                    FechaCarga =
                        x.FechaCarga
                })
                .ToListAsync();
        }







        public async Task AutorizarSolicitudMantenimiento(int solicitudId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
                var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;
                var solicitud =
                    await _context.SolicitudMantenimientos
                        .Include(x => x.SolicitudMantenimientoDetalles)
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId == solicitudId
                            &&
                            x.Activo);

                if (solicitud == null)
                {
                    throw new Exception(
                        "La solicitud no existe.");
                }

                // =========================================
                // ACTUALIZAR ESTATUS
                // =========================================

                solicitud.EstatusSolicitudMantenimientoId = 2;

                // =========================================
                // CREAR MANTENIMIENTO
                // =========================================

                var mantenimiento =
                    new Mantenimiento
                    {
                        UnidadVehicularId =
                            solicitud.UnidadVehicularId,

                        TipoMantenimientoId =
                            solicitud.TipoMantenimientoId,

                        TallerId = (int)solicitud.TallerId,

                        FechaIngreso =
                            DateTime.Now,

                        KilometrajeEntrada =
                            solicitud.KilometrajeActual,

                        MotivoMantenimiento =
                            solicitud.MotivoSolicitud,

                        Diagnostico =
                            solicitud.DiagnosticoInicial,

                        Observaciones =
                            solicitud.Observaciones,

                        DiasFueraServicio = EF.Functions.DateDiffDay(solicitud.FechaCreacion, DateTime.Now),


                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId = usuarioGuid,

                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId
                    };

                _context.Mantenimientos.Add(mantenimiento);

                await _context.SaveChangesAsync();

                // =========================================
                // CREAR DETALLES
                // =========================================

                foreach (var item in solicitud.SolicitudMantenimientoDetalles)
                {
                    var detalle =
                        new MantenimientoDetalle
                        {
                            MantenimientoId =
                                mantenimiento.MantenimientoId,

                            ConceptoServicioId =
                                item.ConceptoServicioId,

                            Cantidad =
                                item.Cantidad,

                            CostoUnitarioManoObra = item.CostoEstimadoManoObra,

                            CostoUnitarioRefacciones = item.CostoEstimadoRefacciones,

                            SubtotalManoObra = (item.CostoEstimadoManoObra)
                                * item.Cantidad,

                            SubtotalRefacciones =
                                (item.CostoEstimadoRefacciones)
                                * item.Cantidad,

                            TotalLinea =
                                (
                                    (item.CostoEstimadoManoObra)
                                    +
                                    (item.CostoEstimadoRefacciones)
                                ) * item.Cantidad,

                            Observaciones =
                                item.Observaciones,

                            FechaCreacion =
                                DateTime.Now
                        };

                    _context
                        .MantenimientoDetalles
                        .Add(detalle);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }








        // VALIDAR FIRMA BINARIA
        private bool ArchivoValidoPorFirma(
            string extension,
            byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                return false;
            }

            extension =
                extension.ToLowerInvariant();

            // PDF
            if (extension == ".pdf")
            {
                return
                    bytes[0] == 0x25 &&
                    bytes[1] == 0x50 &&
                    bytes[2] == 0x44 &&
                    bytes[3] == 0x46;
            }

            // PNG
            if (extension == ".png")
            {
                return
                    bytes[0] == 0x89 &&
                    bytes[1] == 0x50 &&
                    bytes[2] == 0x4E &&
                    bytes[3] == 0x47;
            }

            // JPG / JPEG
            if (
                extension == ".jpg"
                ||
                extension == ".jpeg"
            )
            {
                return
                    bytes[0] == 0xFF &&
                    bytes[1] == 0xD8 &&
                    bytes[2] == 0xFF;
            }

            return false;
        }







        public async Task<SolicitudMantenimientoDetallePageViewModel> ObtenerDetalleSolicitud(int solicitudId)
        {
            var solicitud = await _context.SolicitudMantenimientos
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == solicitudId)
                .Select(x => new SolicitudMantenimientoDetallePageViewModel
                {
                    // =====================================================
                    // SOLICITUD
                    // =====================================================
                    SolicitudMantenimientoId = x.SolicitudMantenimientoId,
                    Folio = x.Folio,

                    // =====================================================
                    // VEHÍCULO
                    // =====================================================
                    Vehiculo = x.UnidadVehicular.Marca.MarcaNombre + " " +
                               x.UnidadVehicular.Modelo.ModeloNombre + " - " +
                               x.UnidadVehicular.PlacaActual + " - " +
                               x.UnidadVehicular.NumeroSerie + " - " +
                               x.UnidadVehicular.NumeroEconomico,

                    // =====================================================
                    // TALLER
                    // =====================================================
                    Taller = x.Taller != null
                        ? x.Taller.RazonSocial + " - " + (x.Taller.TallerConfiguracions
                            .Where(tc => tc.Activo && tc.TallerConfiguracionPresupuestal.Activo)
                            .Select(tc => tc.TallerConfiguracionPresupuestal.Recurso.Nombre)
                            .FirstOrDefault() ?? "SIN RECURSO")
                        : "SIN TALLER",

                    // =====================================================
                    // ESTATUS Y TOTALES
                    // =====================================================
                    Estatus = x.EstatusSolicitudMantenimiento.Nombre,
                    TotalConceptos = x.SolicitudMantenimientoDetalles.Count(),

                    // =====================================================
                    // DETALLES (PROYECTANDO BASE E IVA 16%)
                    // =====================================================
                    Detalles = x.SolicitudMantenimientoDetalles
                        .Select(d => new SolicitudMantenimientoDetalleViewModel
                        {
                            SolicitudMantenimientoDetalleId = d.SolicitudMantenimientoDetalleId,
                            ConceptoServicioId = d.ConceptoServicioId,
                            ConceptoServicio = d.ConceptoServicio.ConceptoServicioNombre,
                            Cantidad = d.Cantidad,

                            // Precios base originales
                            CostoEstimadoManoObra = d.CostoEstimadoManoObra,
                            CostoEstimadoRefacciones = d.CostoEstimadoRefacciones,

                            // Precios con IVA (16%) orientados a la tabla de detalles
                            CostoManoObraConIva = d.CostoEstimadoManoObra * 1.16m,
                            CostoRefaccionesConIva = d.CostoEstimadoRefacciones * 1.16m,

                            Observaciones = d.Observaciones
                        })
                        .ToList(),

                    // =====================================================
                    // DOCUMENTOS ADJUNTOS
                    // =====================================================
                    Documentos = x.SolicitudMantenimientoDocumentos
                        .Where(d => d.Activo)
                        .OrderByDescending(d => d.FechaCarga)
                        .Select(d => new SolicitudMantenimientoDocumentoViewModel
                        {
                            SolicitudMantenimientoDocumentoId = d.SolicitudMantenimientoDocumentoId,
                            TipoDocumento = d.TipoDocumento,
                            NombreArchivo = d.NombreArchivo,
                            RutaArchivo = d.RutaArchivo,
                            FechaCarga = d.FechaCarga
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return solicitud!;
        }



        public async Task<List<SelectListItem>> ObtenerConceptosServicio(int tipoCatalogoServicioId)
        {
            return await _context.ConceptoServicios
                .AsNoTracking()
                .Where(x =>
                    x.Activo &&
                    x.TipoCatalogoServicioId == tipoCatalogoServicioId)
                .OrderBy(x => x.ConceptoServicioNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.ConceptoServicioId.ToString(),
                    Text = x.ConceptoServicioNombre
                })
                .ToListAsync();
        }


        // <summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////        
        // </summary>
        // <param name=" Filtrados "></param>
        // <returns></returns>

        public async Task<List<SelectListItem>> ObtenerConceptosServicioFiltrados(int unidadVehicularId)
        {
            var unidad =
                await _context.UnidadVehiculars
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.UnidadVehicularId ==
                        unidadVehicularId);

            if (unidad == null)
            {
                return new List<SelectListItem>();
            }

            //var conceptos =await _context
            //        .ConceptoServicioCostos
            //        .AsNoTracking()
            //        .Where(x => x.Activo &&
            //            x.MarcaId == unidad.MarcaId &&
            //            x.ModeloId == unidad.ModeloId &&
            //            x.AnioId == unidad.AnioId &&
            //            x.TransmisionId == unidad.TransmisionId &&
            //            x.CilindroId == unidad.CilindroId &&
            //            x.ConceptoServicio.Activo
            //        )
            //        .GroupBy(x =>
            //            new
            //            {
            //                x.ConceptoServicioId,
            //                x.ConceptoServicio.ConceptoServicioNombre
            //            })
            //        .Select(x =>
            //            new SelectListItem
            //            {
            //                Value =
            //                    x.Key.ConceptoServicioId
            //                        .ToString(),
            //                Text =
            //                    x.Key.ConceptoServicioNombre
            //            })
            //        .OrderBy(x => x.Text)
            //        .ToListAsync();




            //VALIDACION DEL CATALOGO DE CONCEPTOS CON LA UNIDAD SELECCIONADA
            var conceptos =
                await _context.ConceptoServicioCostos
                    .AsNoTracking()
                    .Where(x =>
                        x.Activo == true && x.ConceptoServicio.Activo == true
                        &&
                        x.MarcaId == unidad.MarcaId
                        &&
                        x.ModeloId == unidad.ModeloId &&
                        x.AnioId == unidad.AnioId
                        //&&
                        //x.TransmisionId == unidad.TransmisionId
                        //&&
                        //x.CilindroId == unidad.CilindroId
                        )
                    .Select(x => new
                    {
                        x.ConceptoServicioId,
                        x.ConceptoServicio.ConceptoServicioNombre
                    })
                    .Distinct()
                    .OrderBy(x => x.ConceptoServicioNombre)
                    .Select(x => new SelectListItem
                    {
                        Value = x.ConceptoServicioId.ToString(),
                        Text = x.ConceptoServicioNombre
                    })
                    .ToListAsync();
            return conceptos;
        }




        public async Task<ConceptoServicioCostoDto> ObtenerCostoConcepto(int solicitudId, int conceptoServicioId)
        {
            // SOLICITUD + UNIDAD

            var solicitud = await _context
                .SolicitudMantenimientos
                .Include(x => x.UnidadVehicular)
                .FirstOrDefaultAsync(x =>
                    x.SolicitudMantenimientoId
                        == solicitudId);

            if (solicitud == null)
            {
                throw new Exception(
                    "Solicitud no encontrada.");
            }

            var unidad = solicitud.UnidadVehicular;

            if (unidad == null)
            {
                throw new Exception(
                    "Unidad vehicular no encontrada.");
            }

            // BUSCAR MATRIZ

            var costo = await _context.ConceptoServicioCostos.AsNoTracking()
                .FirstOrDefaultAsync(x =>

                    x.ConceptoServicioId == conceptoServicioId
                    &&
                    x.MarcaId == unidad.MarcaId
                    &&
                    x.ModeloId == unidad.ModeloId
                    &&
                    x.AnioId == unidad.AnioId
                    &&
                    x.TransmisionId == unidad.TransmisionId
                    &&
                    x.CilindroId == unidad.CilindroId
                    &&
                    x.Activo
                );

            // SI NO EXISTE MATRIZ

            if (costo == null)
            {
                return new ConceptoServicioCostoDto
                {
                    CostoManoObra = 0,
                    CostoRefacciones = 0
                };
            }

            return new ConceptoServicioCostoDto
            {
                CostoManoObra =
                    costo.CostoManoObra,

                CostoRefacciones =
                    costo.CostoRefacciones
            };
        }





        public async Task<ConceptoServicioCostoDto> ObtenerCostoConceptoCreate(int unidadVehicularId, int conceptoServicioId)
        {
            // OBTENER UNIDAD
            var unidad = await _context.UnidadVehiculars.AsNoTracking().FirstOrDefaultAsync(x => x.UnidadVehicularId == unidadVehicularId);

            if (unidad == null)
            {
                throw new Exception(
                    "Unidad vehicular no encontrada.");
            }

            // ============================================
            // BUSCAR MATRIZ DE COSTOS
            // ============================================

            var costo =
                await _context
                    .ConceptoServicioCostos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ConceptoServicioId == conceptoServicioId

                        &&

                        x.MarcaId
                            == unidad.MarcaId
                        &&
                        x.ModeloId
                            == unidad.ModeloId

                        &&

                        x.AnioId
                            == unidad.AnioId

                        &&

                        x.TransmisionId
                            == unidad.TransmisionId

                        &&

                        x.CilindroId
                            == unidad.CilindroId

                        &&

                        x.Activo
                    );

            // ============================================
            // SI NO EXISTE MATRIZ
            // ============================================

            if (costo == null)
            {
                return new ConceptoServicioCostoDto
                {
                    CostoManoObra = 0,
                    CostoRefacciones = 0
                };
            }

            // ============================================
            // RESPUESTA
            // ============================================

            return new ConceptoServicioCostoDto
            {
                CostoManoObra = costo.CostoManoObra,

                CostoRefacciones = costo.CostoRefacciones,

                //IdObjetoGastoManoObra = costo.IdObjetoGastoManoObra,

                //IdObjetoGastoRefacciones = costo.IdObjetoGastoRefacciones
            };
        }





        public async Task AgregarConceptoSolicitud(SolicitudMantenimientoDetalleCreateDto dto)
        {
            // VALIDAR SOLICITUD

            var solicitud = await _context
                .SolicitudMantenimientos
                .Include(x => x.UnidadVehicular)
                .FirstOrDefaultAsync(x =>
                    x.SolicitudMantenimientoId
                        == dto.SolicitudMantenimientoId);

            if (solicitud == null)
            {
                throw new Exception(
                    "La solicitud de mantenimiento no existe.");
            }

            // VALIDAR CONCEPTO DUPLICADO

            var existeConcepto = await _context
                .SolicitudMantenimientoDetalles
                .AnyAsync(x =>

                    x.SolicitudMantenimientoId
                        == dto.SolicitudMantenimientoId

                    &&

                    x.ConceptoServicioId
                        == dto.ConceptoServicioId
                );

            if (existeConcepto)
            {
                throw new Exception("El concepto ya fue agregado a la solicitud.");
            }

            // OBTENER COSTOS OFICIALES DESDE MATRIZ

            var costos = await ObtenerCostoConcepto(
                dto.SolicitudMantenimientoId,
                dto.ConceptoServicioId);

            // CREAR DETALLE

            var detalle = new SolicitudMantenimientoDetalle
            {
                SolicitudMantenimientoId =
                    dto.SolicitudMantenimientoId,

                ConceptoServicioId =
                    dto.ConceptoServicioId,

                Cantidad = (int)dto.Cantidad,

                // IMPORTANTE:
                // USAR COSTOS OFICIALES
                // NO LOS DEL FRONTEND

                CostoEstimadoManoObra =
                    costos.CostoManoObra,

                CostoEstimadoRefacciones =
                    costos.CostoRefacciones,

                Observaciones =
                    dto.Observaciones,

                FechaCreacion =
                    DateTime.Now
            };

            _context
                .SolicitudMantenimientoDetalles
                .Add(detalle);

            await _context.SaveChangesAsync();

            // RECALCULAR IMPORTE TOTAL

            solicitud.ImporteCotizacion =
                await _context
                    .SolicitudMantenimientoDetalles
                    .Where(x =>

                        x.SolicitudMantenimientoId
                            == solicitud.SolicitudMantenimientoId
                    )
                    .SumAsync(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad
                    );

            // CAMBIO AUTOMÁTICO DE ESTATUS

            if (solicitud.EstatusSolicitudMantenimientoId == 1)
            {
                solicitud.EstatusSolicitudMantenimientoId = 2;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<int>> ObtenerConceptosYaRegistrados(int solicitudId)
        {
            return await _context
                .SolicitudMantenimientoDetalles
                .Where(x =>
                    x.SolicitudMantenimientoId
                        == solicitudId)
                .Select(x => x.ConceptoServicioId)
                .ToListAsync();
        }





        // =====================================================
        // OBTENER REVISIÓN TÉCNICA
        // =====================================================
        public async Task<RevisionSolicitudViewModel> ObtenerRevisionSolicitud(int solicitudId)
        {
            var solicitud = await _context.SolicitudMantenimientos
                .AsNoTracking()
                .Include(x => x.AreaSolicitante)
                .Include(x => x.UnidadVehicular)
                    .ThenInclude(x => x.Marca)
                .Include(x => x.UnidadVehicular)
                    .ThenInclude(x => x.Modelo)
                .Include(x => x.Taller)
                .Include(x => x.EstadoOdometro)
                .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId);

            if (solicitud == null)
            {
                throw new InvalidOperationException("INFO|La solicitud de mantenimiento no existe.");
            }

            if (solicitud.EstatusSolicitudMantenimientoId < 4)
            {
                throw new InvalidOperationException("WARNING|La solicitud no cuenta con validación presupuestal (DSP).");
            }

            if (solicitud.EstatusSolicitudMantenimientoId == 8 || solicitud.EstatusSolicitudMantenimientoId == 9)
            {
                throw new InvalidOperationException("INFO|La solicitud ya ha sido finalizada o cancelada.");
            }

            // Historial base de mantenimientos concluidos (Estatus 5)
            var historialBase = await _context.MantenimientoDetalles
                .AsNoTracking()
                .Include(x => x.Mantenimiento)
                .Include(x => x.ConceptoServicio)
                .Where(x => x.Mantenimiento.UnidadVehicularId == solicitud.UnidadVehicularId &&
                            x.Mantenimiento.EstatusMantenimientoId == 5)
                .ToListAsync();

            var now = DateTime.Now;

            var conceptosSolicitados = await _context.SolicitudMantenimientoDetalles
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == solicitudId)
                .Select(x => x.ConceptoServicioId)
                .ToListAsync();

            var conceptosSolicitadosSet = conceptosSolicitados.ToHashSet();
            var historialPorConcepto = historialBase.GroupBy(x => x.ConceptoServicioId).ToDictionary(g => g.Key);

            var serviciosSolicitadosConHistorial = conceptosSolicitadosSet.Count(id => historialPorConcepto.ContainsKey(id));

            var recurrenciasSolicitud = conceptosSolicitadosSet.Count(id =>
                historialPorConcepto.TryGetValue(id, out var grupo) && grupo.Count() > 1);

            var serviciosRecientes = conceptosSolicitadosSet.Count(id =>
                historialPorConcepto.TryGetValue(id, out var grupo) &&
                grupo.Max(x => x.Mantenimiento.FechaEjecucion ?? x.Mantenimiento.FechaCreacion) >= now.AddDays(-30));

            var costoHistoricoSolicitado = historialBase
                .Where(x => conceptosSolicitadosSet.Contains(x.ConceptoServicioId))
                .Sum(x => (decimal)(x.TotalLinea ?? (x.SubtotalManoObra + x.SubtotalRefacciones)));

            var conceptosConAntecedente = historialBase
                .Where(x => conceptosSolicitadosSet.Contains(x.ConceptoServicioId))
                .GroupBy(x => new { x.ConceptoServicioId, x.ConceptoServicio.ConceptoServicioNombre })
                .Select(g => g.Key.ConceptoServicioNombre)
                .OrderBy(x => x)
                .ToList();

            var historialTecnico = historialBase
             .OrderByDescending(x => x.Mantenimiento.FechaEjecucion ?? x.Mantenimiento.FechaCreacion)
             .Select(x =>
             {
                 var fecha = x.Mantenimiento.FechaEjecucion ?? x.Mantenimiento.FechaCreacion;
                 var veces = historialPorConcepto[x.ConceptoServicioId].Count();

                 var diasDesdeUltimo = (now.Date - fecha.Date).Days;
                 return new HistorialTecnicoUnidadViewModel
                 {
                     Fecha = fecha,
                     Folio = x.Mantenimiento.MantenimientoId.ToString(),
                     ConceptoServicioId = x.ConceptoServicioId,
                     Concepto = x.ConceptoServicio.ConceptoServicioNombre,
                     Kilometraje = x.Mantenimiento.KilometrajeSalida ?? 0,
                     Costo = (decimal)(x.TotalLinea ?? (x.SubtotalManoObra + x.SubtotalRefacciones)),
                     DiasDesdeUltimo = diasDesdeUltimo < 0 ? 0 : diasDesdeUltimo,
                     EsRecurrencia = veces > 1,
                     EsConceptoSolicitado = conceptosSolicitadosSet.Contains(x.ConceptoServicioId),
                     NumeroEjecuciones = veces
                 };
             })
             .ToList();

            var resumenHistorico = new RevisionResumenHistoricoDto
            {
                TotalServicios = historialTecnico.Count,
                UltimaFecha = historialTecnico.FirstOrDefault()?.Fecha,
                CostoAcumulado = historialTecnico.Sum(x => x.Costo),
                ServiciosSolicitadosConHistorial = serviciosSolicitadosConHistorial,
                TotalRecurrenciasSolicitadas = recurrenciasSolicitud,
                ConceptosConAntecedente = conceptosConAntecedente,
                ServiciosRecientes = serviciosRecientes,
                CostoHistoricoConceptosSolicitados = costoHistoricoSolicitado
            };

            // Proyección de conceptos con soporte de IVA (16%)
            var conceptos = await _context.SolicitudMantenimientoDetalles
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == solicitudId)
                .Select(x => new SolicitudMantenimientoDetalleViewModel
                {
                    SolicitudMantenimientoDetalleId = x.SolicitudMantenimientoDetalleId,
                    ConceptoServicioId = x.ConceptoServicioId,
                    ConceptoServicioNombre = x.ConceptoServicio.ConceptoServicioNombre,
                    Cantidad = x.Cantidad,
                    CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                    CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                    CostoManoObraConIva = x.CostoEstimadoManoObra * 1.16m,
                    CostoRefaccionesConIva = x.CostoEstimadoRefacciones * 1.16m,
                    Observaciones = x.Observaciones
                })
                .ToListAsync();

            if (!conceptos.Any())
            {
                throw new InvalidOperationException("WARNING|La solicitud no contiene conceptos o servicios registrados.");
            }

            bool yaAprobadaDSP = solicitud.EstatusSolicitudMantenimientoId == 4;
            bool yaAprobada = solicitud.EstatusSolicitudMantenimientoId == 5;
            string? mensajeEstatus = yaAprobada ? "La solicitud ya fue autorizada técnicamente." : null;

            return new RevisionSolicitudViewModel
            {
                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                Folio = solicitud.Folio,
                AreaSolicitante = solicitud.AreaSolicitante?.AreaNombre ?? string.Empty,
                Marca = solicitud.UnidadVehicular?.Marca?.MarcaNombre ?? string.Empty,
                Modelo = solicitud.UnidadVehicular?.Modelo?.ModeloNombre ?? string.Empty,
                NumeroSerie = solicitud.UnidadVehicular?.NumeroSerie ?? string.Empty,
                PlacaActual = solicitud.UnidadVehicular?.PlacaActual ?? string.Empty,
                NumeroEconomico = solicitud.UnidadVehicular?.NumeroEconomico ?? string.Empty,
                Taller = solicitud.Taller?.RazonSocial ?? string.Empty,
                ImporteCotizacion = (decimal)solicitud.ImporteCotizacion,
                Conceptos = conceptos,
                TiposRechazo = await ObtenerTiposRechazo(),
                YaProcesada = yaAprobada,
                yaAprobadaDSP = yaAprobadaDSP,
                EstatusSolicitudMantenimientoId = solicitud.EstatusSolicitudMantenimientoId,
                MensajeEstatus = mensajeEstatus,
                HistorialTecnicoUnidad = historialTecnico,
                ResumenHistorico = resumenHistorico,
                Kilometraje = solicitud.KilometrajeActual,
                EstadoOdometroId = solicitud.EstadoOdometroId,
                EstadoOdometroDescripcion = solicitud.EstadoOdometro?.Descripcion ?? string.Empty
            };
        }




        public async Task RevisarSolicitud(RevisionSolicitudDto dto)
        {
            // =====================================================
            // 1. VALIDACIONES INICIALES
            // =====================================================
            if (dto == null)
            {
                throw new InvalidOperationException("WARNING|La información de la revisión no es válida.");
            }

            if (dto.SolicitudMantenimientoId <= 0)
            {
                throw new InvalidOperationException("WARNING|La solicitud de mantenimiento no es válida.");
            }

            if (!dto.Aprobada && (!dto.TipoRechazoSolicitudId.HasValue || dto.TipoRechazoSolicitudId.Value <= 0))
            {
                throw new InvalidOperationException("WARNING|Debe indicar el tipo de rechazo.");
            }

            var usuarioGuid = Guid.TryParse(
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var guid) ? guid : (Guid?)null;

            if (!usuarioGuid.HasValue)
            {
                throw new InvalidOperationException("WARNING|No fue posible identificar al usuario que realiza la revisión técnica.");
            }

            var fechaActual = DateTime.Now;

            await using var transaction = await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

            try
            {
                // =====================================================
                // 2. OBTENER SOLICITUD Y VALIDAR EXISTENCIA
                // =====================================================
                var solicitud = await _context.SolicitudMantenimientos
                    .Include(x => x.SolicitudMantenimientoDetalles)
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == dto.SolicitudMantenimientoId);

                if (solicitud == null)
                {
                    throw new InvalidOperationException("WARNING|La solicitud de mantenimiento no fue encontrada.");
                }

                var estatusActual = solicitud.EstatusSolicitudMantenimientoId;

                // =====================================================
                // 3. VALIDAR ESTADO PERMITIDO
                // =====================================================
                if (estatusActual == 8)
                {
                    throw new InvalidOperationException("INFO|La solicitud ya está finalizada y no puede modificarse.");
                }

                if (estatusActual == 9)
                {
                    throw new InvalidOperationException("INFO|La solicitud se encuentra cancelada.");
                }

                if (estatusActual != 4 && estatusActual != 6)
                {
                    if (estatusActual == 5)
                    {
                        throw new InvalidOperationException("WARNING|La solicitud ya fue aprobada técnicamente.");
                    }

                    throw new InvalidOperationException("WARNING|La solicitud no se encuentra en un estado válido para revisión técnica. Debe contar con DSP validada o estar rechazada para su reaprobación.");
                }

                // =====================================================
                // 4. VALIDAR CONCEPTOS
                // =====================================================
                if (solicitud.SolicitudMantenimientoDetalles == null || !solicitud.SolicitudMantenimientoDetalles.Any())
                {
                    throw new InvalidOperationException("WARNING|La solicitud no tiene conceptos registrados.");
                }

                // =====================================================
                // 5. OBTENER MOVIMIENTOS HISTÓRICOS DSP
                // =====================================================
                var movimientosDSP = await _context.MovimientoPresupuestals
                    .Where(x => x.SolicitudMantenimientoId == solicitud.SolicitudMantenimientoId && x.Activo == true)
                    .ToListAsync();

                if (!movimientosDSP.Any(x => x.TipoMovimiento == "COMPROMETIDO"))
                {
                    throw new InvalidOperationException("WARNING|No existen movimientos de compromiso DSP asociados a la solicitud.");
                }

                if (movimientosDSP.Any(x => x.TipoMovimiento == "DEVENGADO" && (x.Importe ?? 0m) > 0m))
                {
                    throw new InvalidOperationException("WARNING|La solicitud tiene importes devengados. No puede rechazarse o recomprometerse automáticamente; requiere regularización presupuestal.");
                }

                // =====================================================
                // 6. OBTENER MOVIMIENTOS HISTÓRICOS DEL TALLER
                // =====================================================
                var movimientosTaller = await _context.TallerMovimientoPresupuestos
                    .Where(x => x.SolicitudMantenimientoId == solicitud.SolicitudMantenimientoId && x.Activo)
                    .ToListAsync();

                // =====================================================
                // 7. APROBAR O REAPROBAR
                // =====================================================
                if (dto.Aprobada)
                {
                    if (estatusActual == 4)
                    {
                        solicitud.EstatusSolicitudMantenimientoId = 5;
                    }
                    else if (estatusActual == 6)
                    {
                        var compromisosPorDSP = movimientosDSP
                            .Where(x => x.TipoMovimiento == "COMPROMETIDO" || x.TipoMovimiento == "DESCOMPROMETIDO")
                            .GroupBy(x => x.IdDispPresupuestal)
                            .Select(g => new
                            {
                                IdDispPresupuestal = g.Key,
                                Comprometido = g.Where(x => x.TipoMovimiento == "COMPROMETIDO").Sum(x => x.Importe ?? 0m),
                                Descomprometido = g.Where(x => x.TipoMovimiento == "DESCOMPROMETIDO").Sum(x => x.Importe ?? 0m)
                            })
                            .ToList();

                        if (!compromisosPorDSP.Any())
                        {
                            throw new InvalidOperationException("WARNING|No existen movimientos DSP para reconstruir los compromisos de la solicitud rechazada.");
                        }

                        foreach (var item in compromisosPorDSP)
                        {
                            if (!item.IdDispPresupuestal.HasValue)
                            {
                                throw new InvalidOperationException("WARNING|Existe un movimiento DSP sin disponibilidad presupuestal asociada.");
                            }

                            if (item.Comprometido < 0m || item.Descomprometido < 0m || item.Descomprometido > item.Comprometido)
                            {
                                throw new InvalidOperationException("WARNING|Los movimientos históricos DSP presentan una inconsistencia de compromisos y descompromisos.");
                            }

                            var compromisoVigente = item.Comprometido - item.Descomprometido;

                            if (compromisoVigente != 0m)
                            {
                                throw new InvalidOperationException("WARNING|La solicitud rechazada conserva compromisos DSP vigentes. Debe regularizarse antes de reaprobar.");
                            }
                        }

                        var idsDSP = compromisosPorDSP
                            .Where(x => x.Descomprometido > 0m)
                            .Select(x => x.IdDispPresupuestal!.Value)
                            .Distinct()
                            .ToList();

                        var dsps = await _context.DispPresupuestals
                            .Where(x => idsDSP.Contains(x.IdDispPresupuestal))
                            .ToListAsync();

                        if (dsps.Count != idsDSP.Count)
                        {
                            throw new InvalidOperationException("WARNING|Una o más DSP originales ya no existen. No es posible restaurar sus compromisos.");
                        }

                        var dspPorId = dsps.ToDictionary(x => x.IdDispPresupuestal);
                        var movimientosDeTodasLasDSP = await _context.MovimientoPresupuestals
                            .Where(x => idsDSP.Contains((int)x.IdDispPresupuestal) && x.Activo == true)
                            .ToListAsync();

                        foreach (var item in compromisosPorDSP)
                        {
                            if (!item.IdDispPresupuestal.HasValue || item.Descomprometido <= 0m)
                            {
                                continue;
                            }

                            var idDSP = item.IdDispPresupuestal.Value;
                            var dsp = dspPorId[idDSP];

                            if (!dsp.Activo)
                            {
                                throw new InvalidOperationException($"WARNING|La DSP {dsp.IdObjetoGasto} se encuentra inactiva y no puede recomprometerse.");
                            }

                            var movimientosDeLaDSP = movimientosDeTodasLasDSP
                                .Where(x => x.IdDispPresupuestal == idDSP)
                                .ToList();

                            var comprometido = movimientosDeLaDSP.Where(x => x.TipoMovimiento == "COMPROMETIDO").Sum(x => x.Importe ?? 0m);
                            var descomprometido = movimientosDeLaDSP.Where(x => x.TipoMovimiento == "DESCOMPROMETIDO").Sum(x => x.Importe ?? 0m);
                            var devengado = movimientosDeLaDSP.Where(x => x.TipoMovimiento == "DEVENGADO").Sum(x => x.Importe ?? 0m);

                            var comprometidoNeto = comprometido - descomprometido;
                            var disponible = dsp.Importe - comprometidoNeto - devengado;

                            if (item.Descomprometido > disponible)
                            {
                                throw new InvalidOperationException($"WARNING|La DSP {dsp.IdObjetoGasto} ya no tiene suficiencia para restaurar el compromiso. Disponible: {disponible:C}; requerido: {item.Descomprometido:C}.");
                            }

                            var disponiblePosterior = disponible - item.Descomprometido;
                            var limite20 = dsp.Importe * 0.20m;

                            if (disponiblePosterior <= limite20)
                            {
                                throw new InvalidOperationException($"WARNING|La reaprobación dejaría la DSP {dsp.IdObjetoGasto} en o por debajo del umbral del 20 %. Disponible proyectado: {disponiblePosterior:C}.");
                            }

                            _context.MovimientoPresupuestals.Add(new MovimientoPresupuestal
                            {
                                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                                IdDispPresupuestal = idDSP,
                                TipoMovimiento = "COMPROMETIDO",
                                Importe = item.Descomprometido,
                                Observaciones = $"Recompromiso por reaprobación técnica. {dto.Observaciones}",
                                FechaMovimiento = fechaActual,
                                FechaCaptura = fechaActual,
                                UsuarioCaptura = usuarioGuid,
                                Activo = true
                            });
                        }

                        var movimientosTallerPorControl = movimientosTaller
                            .Where(x => x.TipoMovimiento == "COMPROMETIDO" || x.TipoMovimiento == "DESCOMPROMETIDO")
                            .GroupBy(x => x.IdControlTaller)
                            .Select(g => new
                            {
                                IdControlTaller = g.Key,
                                Comprometido = g.Where(x => x.TipoMovimiento == "COMPROMETIDO").Sum(x => x.Importe),
                                Descomprometido = g.Where(x => x.TipoMovimiento == "DESCOMPROMETIDO").Sum(x => x.Importe)
                            })
                            .ToList();

                        var idsControlTaller = movimientosTallerPorControl.Select(x => x.IdControlTaller).Distinct().ToList();
                        var controlesTaller = await _context.TallerControlPresupuestals
                            .Where(x => idsControlTaller.Contains(x.IdControlTaller))
                            .ToListAsync();

                        var controlesPorId = controlesTaller.ToDictionary(x => x.IdControlTaller);
                        foreach (var item in movimientosTallerPorControl)
                        {
                            var compromisoVigente = item.Comprometido - item.Descomprometido;
                            if (compromisoVigente != 0m || item.Descomprometido <= 0m)
                            {
                                continue;
                            }

                            var control = controlesPorId[item.IdControlTaller];
                            var disponible = control.PresupuestoAsignado - control.PresupuestoComprometido - control.PresupuestoEjercido;

                            if (item.Descomprometido > disponible)
                            {
                                throw new InvalidOperationException($"WARNING|El taller ya no tiene disponibilidad suficiente para la reaprobación. Disponible: {disponible:C}; requerido: {item.Descomprometido:C}.");
                            }

                            control.PresupuestoComprometido += item.Descomprometido;
                            control.PresupuestoDisponible = control.PresupuestoAsignado - control.PresupuestoComprometido - control.PresupuestoEjercido;
                            control.FechaActualizacion = fechaActual;

                            _context.TallerMovimientoPresupuestos.Add(new TallerMovimientoPresupuesto
                            {
                                TallerId = solicitud.TallerId ?? throw new InvalidOperationException("WARNING|La solicitud no tiene un taller asignado."),
                                IdControlTaller = item.IdControlTaller,
                                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                                TipoMovimiento = "COMPROMETIDO",
                                Importe = item.Descomprometido,
                                FechaMovimiento = fechaActual,
                                UsuarioCaptura = usuarioGuid,
                                Activo = true
                            });
                        }

                        solicitud.EstatusSolicitudMantenimientoId = 5;
                    }
                }
                else
                {
                    if (estatusActual != 4)
                    {
                        throw new InvalidOperationException("WARNING|Solo puede rechazarse una solicitud con DSP validada y pendiente de revisión técnica.");
                    }

                    var compromisosVigentesDSP = movimientosDSP
                        .Where(x => x.TipoMovimiento == "COMPROMETIDO" || x.TipoMovimiento == "DESCOMPROMETIDO")
                        .GroupBy(x => x.IdDispPresupuestal)
                        .Select(g => new
                        {
                            IdDispPresupuestal = g.Key,
                            Comprometido = g.Where(x => x.TipoMovimiento == "COMPROMETIDO").Sum(x => x.Importe ?? 0m),
                            Descomprometido = g.Where(x => x.TipoMovimiento == "DESCOMPROMETIDO").Sum(x => x.Importe ?? 0m)
                        })
                        .ToList();

                    foreach (var item in compromisosVigentesDSP)
                    {
                        var importeVigente = item.Comprometido - item.Descomprometido;
                        if (importeVigente <= 0m) continue;

                        _context.MovimientoPresupuestals.Add(new MovimientoPresupuestal
                        {
                            SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                            IdDispPresupuestal = item.IdDispPresupuestal,
                            TipoMovimiento = "DESCOMPROMETIDO",
                            Importe = importeVigente,
                            Observaciones = $"Liberación por rechazo técnico. {dto.Observaciones}",
                            FechaMovimiento = fechaActual,
                            FechaCaptura = fechaActual,
                            UsuarioCaptura = usuarioGuid,
                            Activo = true
                        });
                    }

                    var compromisosTaller = movimientosTaller
                        .Where(x => x.TipoMovimiento == "COMPROMETIDO" || x.TipoMovimiento == "DESCOMPROMETIDO")
                        .GroupBy(x => x.IdControlTaller)
                        .Select(g => new
                        {
                            IdControlTaller = g.Key,
                            Comprometido = g.Where(x => x.TipoMovimiento == "COMPROMETIDO").Sum(x => x.Importe),
                            Descomprometido = g.Where(x => x.TipoMovimiento == "DESCOMPROMETIDO").Sum(x => x.Importe)
                        })
                        .ToList();

                    var idsControl = compromisosTaller.Select(x => x.IdControlTaller).Distinct().ToList();
                    var controles = await _context.TallerControlPresupuestals
                        .Where(x => idsControl.Contains(x.IdControlTaller))
                        .ToListAsync();

                    var controlesPorId = controles.ToDictionary(x => x.IdControlTaller);

                    foreach (var item in compromisosTaller)
                    {
                        var importeVigente = item.Comprometido - item.Descomprometido;
                        if (importeVigente <= 0m) continue;

                        var control = controlesPorId[item.IdControlTaller];
                        control.PresupuestoComprometido -= importeVigente;
                        control.PresupuestoDisponible = control.PresupuestoAsignado - control.PresupuestoComprometido - control.PresupuestoEjercido;
                        control.FechaActualizacion = fechaActual;

                        _context.TallerMovimientoPresupuestos.Add(new TallerMovimientoPresupuesto
                        {
                            TallerId = solicitud.TallerId ?? throw new InvalidOperationException("WARNING|La solicitud no tiene un taller asignado."),
                            IdControlTaller = item.IdControlTaller,
                            SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                            TipoMovimiento = "DESCOMPROMETIDO",
                            Importe = importeVigente,
                            FechaMovimiento = fechaActual,
                            UsuarioCaptura = usuarioGuid,
                            Activo = true
                        });
                    }

                    solicitud.EstatusSolicitudMantenimientoId = 6;
                }

                _context.SolicitudMantenimientoSeguimientos.Add(new SolicitudMantenimientoSeguimiento
                {
                    SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                    EstatusSolicitudMantenimientoId = solicitud.EstatusSolicitudMantenimientoId,
                    Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones)
                        ? (dto.Aprobada ? "Revisión técnica aprobada." : "Solicitud rechazada y presupuesto liberado.")
                        : dto.Observaciones.Trim(),
                    TipoRechazoSolicitudId = dto.Aprobada ? null : dto.TipoRechazoSolicitudId,
                    FechaMovimiento = fechaActual,
                    UsuarioId = usuarioGuid
                });

                // =====================================================
                // 10. GUARDAR Y CONFIRMAR TRANSACCIÓN EXITOSAMENTE
                // =====================================================
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Error al revertir la revisión técnica de la solicitud {SolicitudId}.", dto.SolicitudMantenimientoId);
                }

                var mensaje = ex.Message ?? string.Empty;

                if (mensaje.StartsWith("WARNING|", StringComparison.OrdinalIgnoreCase) ||
                    mensaje.StartsWith("INFO|", StringComparison.OrdinalIgnoreCase))
                {
                    throw;
                }

                _logger.LogError(ex, "Error técnico no controlado al revisar la solicitud {SolicitudId}.", dto.SolicitudMantenimientoId);

                throw new InvalidOperationException("ERROR|No fue posible completar la revisión técnica y la operación presupuestal.");
            }

            // =========================================================================
            // 11. GENERACIÓN Y GUARDADO DEL PDF FUERA DE LA TRANSACCIÓN (AISLADO)
            // =========================================================================
            if (dto.Aprobada)
            {
                try
                {
                    byte[] pdfBytes = await GenerarDictamenTecnicoPdfAsync(dto.SolicitudMantenimientoId);

                    if (pdfBytes != null && pdfBytes.Length > 0)
                    {
                        var rutaBase = _configuration["Storage:DocumentosRuta"];
                        if (!string.IsNullOrWhiteSpace(rutaBase))
                        {
                            var solFolio = await _context.SolicitudMantenimientos
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == dto.SolicitudMantenimientoId);

                            if (solFolio != null)
                            {
                                var carpetaFolioSegura = solFolio.Folio.Replace('/', '_');
                                var carpetaSolicitud = Path.Combine(rutaBase, carpetaFolioSegura);

                                if (!Directory.Exists(carpetaSolicitud))
                                {
                                    Directory.CreateDirectory(carpetaSolicitud);
                                }

                                string nombreArchivoFisico = $"DICTAMEN_{dto.SolicitudMantenimientoId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                                string rutaCompleta = Path.Combine(carpetaSolicitud, nombreArchivoFisico);

                                await File.WriteAllBytesAsync(rutaCompleta, pdfBytes);

                                string hashArchivo;
                                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                                {
                                    hashArchivo = Convert.ToHexString(sha256.ComputeHash(pdfBytes));
                                }

                                var documentoEntidad = new SolicitudMantenimientoDocumento
                                {
                                    SolicitudMantenimientoId = dto.SolicitudMantenimientoId,
                                    TipoDocumento = "DICTAMEN_TECNICO",
                                    NombreArchivo = $"Dictamen_Tecnico_{solFolio.NumeroOficio?.Replace('/', '_') ?? "SIN_OFICIO"}.pdf",
                                    RutaArchivo = rutaCompleta,
                                    FechaCarga = DateTime.Now,
                                    UsuarioCargaId = 1,
                                    Activo = true,
                                    HashArchivo = hashArchivo
                                };

                                _context.SolicitudMantenimientoDocumentos.Add(documentoEntidad);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception pdfEx)
                {
                    _logger.LogError(pdfEx, "Error al generar o guardar físicamente el PDF del dictamen para la solicitud {SolicitudId}.", dto.SolicitudMantenimientoId);
                }
            }
        }






        public async Task<List<SelectListItem>> ObtenerTiposRechazo()
        {
            return await _context.TipoRechazoSolicituds
                .AsNoTracking()
                .Where(x => x.Activo == true)
                .OrderBy(x => x.Nombre)
                .Select(x => new SelectListItem
                {
                    Value = x.TipoRechazoSolicitudId.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();
        }


        public async Task<UnidadVehicularEliminarViewModel?> ObtenerEliminarAsync(int id)
        {
            return await _context.UnidadVehiculars
                .Where(x => x.UnidadVehicularId == id && x.Activo)
                .Select(x => new UnidadVehicularEliminarViewModel
                {
                    UnidadVehicularId = x.UnidadVehicularId,
                    NumeroEconomico = x.NumeroEconomico,
                    PlacaActual = x.PlacaActual,
                    NumeroSerie = x.NumeroSerie,
                    Marca = x.Marca.MarcaNombre,
                    Modelo = x.Modelo.ModeloNombre,
                    Anio = x.AnioId
                })
                .FirstOrDefaultAsync();
        }


        public async Task<bool> EliminarAsync(UnidadVehicularEliminarDto dto)
        {
            var entidad = await _context.UnidadVehiculars
                .FirstOrDefaultAsync(x =>
                    x.UnidadVehicularId == dto.UnidadVehicularId);

            if (entidad == null)
                return false;

            entidad.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<TipoCatalogoServicioIndexDto> ObtenerIndexTipoCatalogoServicioAsync(int pagina)
        {
            const int pageSize = 10;

            var query = _context.TipoCatalogoServicios
                .AsNoTracking()
                .Where(x => x.Activo);

            var totalRegistros =
                await query.CountAsync();

            var registros = await query
                .OrderBy(x => x.TipoCatalogoServicioNombre)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TipoCatalogoServicioRowDto
                {
                    TipoCatalogoServicioId =
                        x.TipoCatalogoServicioId,

                    TipoCatalogoServicioNombre =
                        x.TipoCatalogoServicioNombre,

                    Activo = x.Activo
                })
                .ToListAsync();

            return new TipoCatalogoServicioIndexDto
            {
                Registros = registros,
                PaginaActual = pagina,
                TotalRegistros = totalRegistros,
                TotalPaginas =
                    (int)Math.Ceiling(
                        totalRegistros / (double)pageSize)
            };
        }



        public async Task<ConceptoServicioIndexDto> ObtenerIndexConceptoServicioAsync(int pagina)
        {
            const int pageSize = 10;

            var query = _context.ConceptoServicios
                .AsNoTracking()
                .Include(x => x.TipoCatalogoServicio)
                .Where(x => x.Activo);

            var totalRegistros = await query.CountAsync();

            var registros = await query
                .OrderByDescending(x => x.FechaCreacion)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ConceptoServicioRowDto
                {
                    ConceptoServicioId = x.ConceptoServicioId,
                    TipoCatalogoServicioNombre = x.TipoCatalogoServicio.TipoCatalogoServicioNombre,
                    ConceptoServicioNombre = x.ConceptoServicioNombre,
                    ConceptoServicioDescripcion = x.ConceptoServicioDescripcion,
                    Activo = x.Activo,
                    FechaCreacion = x.FechaCreacion
                })
                .ToListAsync();

            return new ConceptoServicioIndexDto
            {
                Registros = registros,
                PaginaActual = pagina,
                TotalRegistros = totalRegistros,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize)
            };
        }




        public async Task<ConceptoServicioCostoIndexDto>
       ObtenerIndexConceptoServicioCostoAsync(int pagina)
        {
            const int pageSize = 10;

            var query = _context.ConceptoServicioCostos
                .AsNoTracking()
                .Include(x => x.ConceptoServicio)
                .Include(x => x.Marca)
                .Include(x => x.Modelo)
                .Include(x => x.Anio)
                .Include(x => x.Transmision)
                .Include(x => x.Cilindro)
                .Where(x => x.Activo);

            var totalRegistros =
                await query.CountAsync();

            var registros = await query
                .OrderByDescending(x => x.FechaCreacion)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ConceptoServicioCostoRowDto
                {
                    ConceptoServicioCostoId =
                        x.ConceptoServicioCostoId,

                    ConceptoServicioNombre =
                        x.ConceptoServicio != null
                            ? x.ConceptoServicio.ConceptoServicioNombre
                            : string.Empty,

                    CostoManoObra = x.CostoManoObra,

                    CostoRefacciones = x.CostoRefacciones,

                    FechaInicioVigencia = x.FechaInicioVigencia,

                    FechaFinVigencia = x.FechaFinVigencia,

                    MarcaNombre =
                        x.Marca != null
                            ? x.Marca.MarcaNombre
                            : string.Empty,

                    ModeloNombre =
                        x.Modelo != null
                            ? x.Modelo.ModeloNombre
                            : string.Empty,

                    AnioNombre =
                        x.Anio != null
                            ? x.Anio.AnioDescripcion
                            : string.Empty,

                    TransmisionNombre =
                        x.Transmision != null
                            ? x.Transmision.TransmisionNombre
                            : string.Empty,

                    CilindroNombre =
                        x.Cilindro != null
                            ? x.Cilindro.CilindroNombre
                            : string.Empty,

                    Observaciones =
                        x.Observaciones,

                    FechaCreacion =
                        x.FechaCreacion,

                    Activo =
                        x.Activo
                })
                .ToListAsync();

            return new ConceptoServicioCostoIndexDto
            {
                Registros = registros,

                PaginaActual = pagina,

                TotalRegistros = totalRegistros,

                TotalPaginas =
                    (int)Math.Ceiling(
                        totalRegistros / (double)pageSize)
            };
        }




        public async Task CrearTipoCatalogoServicioAsync(TipoCatalogoServicioCreateDto dto)
        {
            // NORMALIZAR
            dto.TipoCatalogoServicioNombre =
                dto.TipoCatalogoServicioNombre
                    .Trim()
                    .ToUpper();

            // VALIDAR DUPLICADO
            var existe =
                await _context.TipoCatalogoServicios
                    .AnyAsync(x =>
                        x.Activo
                        &&
                        x.TipoCatalogoServicioNombre
                            == dto.TipoCatalogoServicioNombre);

            if (existe)
            {
                throw new Exception("WARNING|Ya existe un tipo de catálogo con el mismo nombre.");
            }

            // ENTIDAD

            var entidad = new TipoCatalogoServicio
            {
                TipoCatalogoServicioNombre =
                    dto.TipoCatalogoServicioNombre,

                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.TipoCatalogoServicios
                .Add(entidad);

            await _context.SaveChangesAsync();
        }






        public async Task<TipoCatalogoServicioEditViewModel> ObtenerTipoCatalogoServicioEditarAsync(int id)
        {
            var registro =
                await _context.TipoCatalogoServicios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.TipoCatalogoServicioId == id);

            if (registro == null)
            {
                throw new Exception("WARNING|Tipo de catálogo no encontrado.");
            }

            return new TipoCatalogoServicioEditViewModel
            {
                TipoCatalogoServicioId =
                    registro.TipoCatalogoServicioId,

                TipoCatalogoServicioNombre =
                    registro.TipoCatalogoServicioNombre
            };
        }



        public async Task EditarTipoCatalogoServicioAsync(TipoCatalogoServicioEditDto dto)
        {
            var entidad =
                await _context.TipoCatalogoServicios
                    .FirstOrDefaultAsync(x =>
                        x.TipoCatalogoServicioId
                            == dto.TipoCatalogoServicioId);

            if (entidad == null)
            {
                throw new Exception("WARNING|Tipo de catálogo no encontrado.");
            }

            // NORMALIZAR

            dto.TipoCatalogoServicioNombre =
                dto.TipoCatalogoServicioNombre
                    .Trim()
                    .ToUpper();

            // VALIDAR DUPLICADO

            var duplicado =
                await _context.TipoCatalogoServicios
                    .AnyAsync(x =>
                        x.TipoCatalogoServicioId
                            != dto.TipoCatalogoServicioId
                        &&
                        x.Activo
                        &&
                        x.TipoCatalogoServicioNombre
                            == dto.TipoCatalogoServicioNombre);

            if (duplicado)
            {
                throw new Exception("WARNING|Ya existe un tipo de catálogo con el mismo nombre.");
            }

            // ACTUALIZAR

            entidad.TipoCatalogoServicioNombre =
                dto.TipoCatalogoServicioNombre;

            await _context.SaveChangesAsync();
        }





        public async Task<TipoCatalogoServicioDeleteViewModel> ObtenerTipoCatalogoEliminarAsync(int id)
        {
            var registro =
                await _context.TipoCatalogoServicios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.TipoCatalogoServicioId == id);

            if (registro == null)
            {
                throw new Exception("WARNING|Tipo de catálogo no encontrado.");
            }

            return new TipoCatalogoServicioDeleteViewModel
            {
                TipoCatalogoServicioId =
                    registro.TipoCatalogoServicioId,

                TipoCatalogoServicioNombre =
                    registro.TipoCatalogoServicioNombre,

                Activo =
                    registro.Activo
            };
        }





        public async Task EliminarTipoCatalogoAsync(int id)
        {
            var entidad = await _context.TipoCatalogoServicios
                    .FirstOrDefaultAsync(x =>
                        x.TipoCatalogoServicioId == id);

            if (entidad == null)
            {
                throw new Exception("Tipo de catálogo no encontrado.");
            }

            // VALIDAR RELACIONES

            var tieneConceptos = await _context.ConceptoServicios
                    .AnyAsync(x =>
                        x.TipoCatalogoServicioId == id
                        &&
                        x.Activo == true);

            if (tieneConceptos)
            {
                throw new Exception("No es posible eliminar el tipo de catalogo porque tiene conceptos asociados.");
            }

            // ELIMINADO LÓGICO

            entidad.Activo = false;

            await _context.SaveChangesAsync();
        }




        //combo para crear concepto de servicio
        public async Task<List<SelectListItem>> ObtenerTiposCatalogoServicio()
        {
            return await _context.TipoCatalogoServicios
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.TipoCatalogoServicioNombre)
                .Select(x => new SelectListItem
                {
                    Value =
                        x.TipoCatalogoServicioId.ToString(),

                    Text =
                        x.TipoCatalogoServicioNombre
                })
                .ToListAsync();
        }



        public async Task CrearConceptoServicioAsync(ConceptoServicioCreateDto dto)
        {
            // NORMALIZAR

            dto.ConceptoServicioNombre = dto.ConceptoServicioNombre
                    .Trim()
                    .ToUpper();

            dto.ConceptoServicioDescripcion =
                dto.ConceptoServicioDescripcion?
                    .Trim();

            // VALIDAR DUPLICADO

            var existe =
                await _context.ConceptoServicios
                    .AnyAsync(x =>
                        x.Activo
                        &&
                        x.TipoCatalogoServicioId
                            == dto.TipoCatalogoServicioId
                        &&
                        x.ConceptoServicioNombre
                            == dto.ConceptoServicioNombre);

            if (existe)
            {
                throw new Exception("WARNING|Ya existe un concepto con el mismo nombre en el tipo de catálogo seleccionado.");
            }

            // ENTIDAD

            var entidad =
                new ConceptoServicio
                {
                    TipoCatalogoServicioId =
                        dto.TipoCatalogoServicioId,

                    ConceptoServicioNombre =
                        dto.ConceptoServicioNombre,

                    ConceptoServicioDescripcion =
                        dto.ConceptoServicioDescripcion,

                    Activo = true,

                    FechaCreacion = DateTime.Now
                };

            _context.ConceptoServicios
                .Add(entidad);

            await _context.SaveChangesAsync();
        }




        public async Task<ConceptoServicioEditViewModel> ObtenerConceptoServicioEdit(int id)
        {
            var entidad =
                await _context.ConceptoServicios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioId == id);

            if (entidad == null)
            {
                throw new Exception("WARNING|Concepto de servicio no encontrado.");
            }

            return new ConceptoServicioEditViewModel
            {
                ConceptoServicioId =
                    entidad.ConceptoServicioId,

                TipoCatalogoServicioId =
                    entidad.TipoCatalogoServicioId,

                ConceptoServicioNombre =
                    entidad.ConceptoServicioNombre,

                ConceptoServicioDescripcion =
                    entidad.ConceptoServicioDescripcion,

                TiposCatalogoServicio = await ObtenerTiposCatalogoServicioSelect()
            };
        }






        public async Task<ResultadoOperacionDto> EditarConceptoServicio(ConceptoServicioEditDto dto)
        {
            try
            {
                var entidad =
                    await _context.ConceptoServicios
                        .FirstOrDefaultAsync(x =>
                            x.ConceptoServicioId
                                == dto.ConceptoServicioId);

                if (entidad == null)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Concepto de servicio no encontrado."
                    };
                }

                var nombreExiste =
                    await _context.ConceptoServicios
                        .AnyAsync(x =>
                            x.ConceptoServicioNombre
                                == dto.ConceptoServicioNombre.Trim()
                            &&
                            x.ConceptoServicioId
                                != dto.ConceptoServicioId
                            &&
                            x.Activo);

                if (nombreExiste)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje = "Ya existe un concepto de servicio con el mismo nombre."
                    };
                }

                entidad.TipoCatalogoServicioId =
                    dto.TipoCatalogoServicioId;

                entidad.ConceptoServicioNombre =
                    dto.ConceptoServicioNombre.Trim();

                entidad.ConceptoServicioDescripcion =
                    dto.ConceptoServicioDescripcion?.Trim();

                await _context.SaveChangesAsync();

                return new ResultadoOperacionDto
                {
                    Exitoso = true,
                    Mensaje =
                        "Concepto de servicio actualizado correctamente."
                };
            }
            catch
            {
                return new ResultadoOperacionDto
                {
                    Exitoso = false,
                    Mensaje =
                        "Ocurrió un error al actualizar el concepto de servicio."
                };
            }
        }




        public async Task<List<SelectListItem>> ObtenerTiposCatalogoServicioSelect()
        {
            return await _context.TipoCatalogoServicios
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.TipoCatalogoServicioNombre)
                .Select(x => new SelectListItem
                {
                    Value =
                        x.TipoCatalogoServicioId.ToString(),

                    Text =
                        x.TipoCatalogoServicioNombre
                })
                .ToListAsync();
        }





        public async Task<ConceptoServicioDeleteViewModel> ObtenerConceptoServicioDelete(int id)
        {
            var entidad =
                await _context.ConceptoServicios
                    .AsNoTracking()
                    .Include(x => x.TipoCatalogoServicio)
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioId == id);

            if (entidad == null)
            {
                throw new Exception("WARNING|Concepto de servicio no encontrado.");
            }

            return new ConceptoServicioDeleteViewModel
            {
                ConceptoServicioId =
                    entidad.ConceptoServicioId,

                TipoCatalogoServicioNombre =
                    entidad.TipoCatalogoServicio
                        .TipoCatalogoServicioNombre,

                ConceptoServicioNombre =
                    entidad.ConceptoServicioNombre,

                ConceptoServicioDescripcion =
                    entidad.ConceptoServicioDescripcion
            };
        }





        public async Task<ResultadoOperacionDto> EliminarConceptoServicio(int id)
        {
            try
            {
                var entidad =
                    await _context.ConceptoServicios
                        .FirstOrDefaultAsync(x =>
                            x.ConceptoServicioId == id);

                if (entidad == null)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje = "Concepto de servicio no encontrado."
                    };
                }

                // VALIDAR USO EN MATRIZ

                var existeEnMatriz =
                    await _context.ConceptoServicioCostos
                        .AnyAsync(x =>
                            x.ConceptoServicioId == id
                            && x.Activo == true
                            )
                            ;

                if (existeEnMatriz)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "No es posible eliminar el concepto porque ya se encuentra asignado en la matriz de costos."
                    };
                }

                // VALIDAR USO EN SOLICITUDES

                var existeEnSolicitudes =
                    await _context.SolicitudMantenimientoDetalles
                        .AnyAsync(x =>
                            x.ConceptoServicioId == id);

                if (existeEnSolicitudes)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "No es posible eliminar el concepto porque ya tiene movimientos registrados."
                    };
                }

                // ELIMINACION LOGICA

                entidad.Activo = false;

                await _context.SaveChangesAsync();

                return new ResultadoOperacionDto
                {
                    Exitoso = true,
                    Mensaje =
                        "Concepto de servicio eliminado correctamente."
                };
            }
            catch
            {
                return new ResultadoOperacionDto
                {
                    Exitoso = false,
                    Mensaje =
                        "Ocurrió un error al eliminar el concepto de servicio."
                };
            }
        }





        public async Task<ResultadoOperacionDto> CrearConceptoServicioCosto(ConceptoServicioCostoCreateDto dto)
        {
            try
            {
                // VALIDAR EXISTENCIA

                var existe =
                    await _context.ConceptoServicioCostos
                        .AnyAsync(x =>
                            x.ConceptoServicioId
                                == dto.ConceptoServicioId
                            &&
                            x.MarcaId
                                == dto.MarcaId
                            &&
                            x.ModeloId
                                == dto.ModeloId
                            &&
                            x.AnioId
                                == dto.AnioId
                            &&
                            x.TransmisionId
                                == dto.TransmisionId
                            &&
                            x.CilindroId
                                == dto.CilindroId
                            &&
                            x.Activo);

                if (existe)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,

                        Mensaje =
                            "Ya existe una configuración de costos para el vehículo seleccionado."
                    };
                }

                // VALIDAR FECHAS

                if (dto.FechaFinVigencia.HasValue && dto.FechaFinVigencia < dto.FechaInicioVigencia)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,

                        Mensaje =
                            "La fecha fin de vigencia no puede ser menor a la fecha inicio."
                    };
                }

                // ENTIDAD

                var entidad =
                    new ConceptoServicioCosto
                    {
                        ConceptoServicioId =
                            dto.ConceptoServicioId,

                        MarcaId =
                            dto.MarcaId,

                        ModeloId =
                            dto.ModeloId,

                        AnioId =
                            dto.AnioId,

                        TransmisionId =
                            dto.TransmisionId,

                        CilindroId =
                            dto.CilindroId,

                        CostoManoObra =
                            dto.CostoManoObra,

                        CostoRefacciones =
                            dto.CostoRefacciones,

                        FechaInicioVigencia =
                            dto.FechaInicioVigencia,

                        FechaFinVigencia =
                            dto.FechaFinVigencia,

                        Observaciones =
                            dto.Observaciones?.Trim(),

                        Activo = true,

                        FechaCreacion =
                            DateTime.Now
                    };

                _context.ConceptoServicioCostos
                    .Add(entidad);

                await _context.SaveChangesAsync();

                return new ResultadoOperacionDto
                {
                    Exitoso = true,

                    Mensaje =
                        "Costo de concepto registrado correctamente."
                };
            }
            catch
            {
                return new ResultadoOperacionDto
                {
                    Exitoso = false,

                    Mensaje =
                        "Ocurrió un error al registrar el costo."
                };
            }
        }





        public async Task<List<SelectListItem>> ObtenerConceptosServicioSelect()
        {
            return await _context.ConceptoServicios
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.ConceptoServicioNombre)
                //.Take(5)
                .Select(x => new SelectListItem
                {
                    Value =
                        x.ConceptoServicioId.ToString(),

                    Text =
                        x.ConceptoServicioNombre
                })
                .ToListAsync();
        }



        public async Task<ConceptoServicioCostoEditViewModel> ObtenerConceptoServicioCostoEdit(int id)
        {
            var entidad =
                await _context.ConceptoServicioCostos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioCostoId == id);

            if (entidad == null)
            {
                throw new Exception("WARNING|Registro no encontrado.");
            }

            var concepto =
                await _context.ConceptoServicios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioId == entidad.ConceptoServicioId);

            if (concepto == null)
            {
                throw new Exception("WARNING|Concepto no encontrado.");
            }

            return new ConceptoServicioCostoEditViewModel
            {
                ConceptoServicioCostoId = entidad.ConceptoServicioCostoId,

                ConceptoServicioId = entidad.ConceptoServicioId,

                MarcaId = entidad.MarcaId,
                ModeloId = entidad.ModeloId,
                AnioId = entidad.AnioId,
                FechaInicioVigencia = entidad.FechaInicioVigencia,
                FechaFinVigencia = entidad.FechaFinVigencia,
                TransmisionId = entidad.TransmisionId,
                CilindroId = entidad.CilindroId,
                CostoManoObra = entidad.CostoManoObra,
                CostoRefacciones = entidad.CostoRefacciones,
                Observaciones = entidad.Observaciones,

                ConceptosServicio =
                    await ObtenerConceptosServicio(concepto.TipoCatalogoServicioId),

                Marcas =
                    await ObtenerMarcas(),

                Modelos =
                    await ObtenerModelosPorMarca(entidad.MarcaId),

                Anios =
                    await ObtenerAnios(),

                Transmisiones =
                    await ObtenerTransmision(),

                Cilindros =
                    await ObtenerCilindro()
            };
        }




        public async Task EditarConceptoServicioCosto(ConceptoServicioCostoEditDto dto)
        {
            var entidad =
                await _context.ConceptoServicioCostos
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioCostoId
                            == dto.ConceptoServicioCostoId);

            if (entidad == null)
            {
                throw new Exception("WARNING|Registro no encontrado.");
            }

            // VALIDAR DUPLICADOS

            var existe =
                await _context.ConceptoServicioCostos
                    .AnyAsync(x =>

                        x.ConceptoServicioCostoId
                            != dto.ConceptoServicioCostoId

                        &&

                        x.ConceptoServicioId
                            == dto.ConceptoServicioId

                        &&

                        x.MarcaId
                            == dto.MarcaId

                        &&

                        x.ModeloId
                            == dto.ModeloId

                        &&

                        x.AnioId
                            == dto.AnioId

                        &&

                        x.TransmisionId
                            == dto.TransmisionId

                        &&

                        x.CilindroId
                            == dto.CilindroId

                        &&

                        x.Activo
                    );

            if (existe)
            {
                throw new Exception("WARNING|Ya existe una configuración de costos para el vehículo seleccionado.");
            }

            // VALIDAR FECHAS

            if (
                dto.FechaFinVigencia.HasValue
                &&
                dto.FechaFinVigencia.Value
                    < dto.FechaInicioVigencia
            )
            {
                throw new Exception("WARNING|La fecha fin de vigencia no puede ser menor a la fecha inicio.");
            }

            // ACTUALIZAR

            entidad.ConceptoServicioId =
                dto.ConceptoServicioId;

            entidad.MarcaId =
                dto.MarcaId;

            entidad.ModeloId =
                dto.ModeloId;

            entidad.AnioId =
                dto.AnioId;

            entidad.TransmisionId =
                dto.TransmisionId;

            entidad.CilindroId =
                dto.CilindroId;

            entidad.FechaInicioVigencia =
                dto.FechaInicioVigencia;

            entidad.FechaFinVigencia =
                dto.FechaFinVigencia;

            entidad.CostoManoObra =
                dto.CostoManoObra;

            entidad.CostoRefacciones =
                dto.CostoRefacciones;

            entidad.Observaciones =
                dto.Observaciones?.Trim();

            await _context.SaveChangesAsync();
        }




        public async Task<ConceptoServicioCostoDeleteViewModel> ObtenerConceptoServicioCostoDelete(int id)
        {
            var entidad =
                await _context.ConceptoServicioCostos
                    .AsNoTracking()
                    .Include(x => x.ConceptoServicio)
                    .Include(x => x.Marca)
                    .Include(x => x.Modelo)
                    .Include(x => x.Anio)
                    .Include(x => x.Transmision)
                    .Include(x => x.Cilindro)
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioCostoId == id);

            if (entidad == null)
            {
                throw new Exception("WARNING|Registro no encontrado.");
            }

            return new ConceptoServicioCostoDeleteViewModel
            {
                ConceptoServicioCostoId =
                    entidad.ConceptoServicioCostoId,

                ConceptoServicioNombre =
                    entidad.ConceptoServicio
                        .ConceptoServicioNombre,

                MarcaNombre =
                    entidad.Marca.MarcaNombre,

                ModeloNombre =
                    entidad.Modelo.ModeloNombre,

                AnioNombre =
                    entidad.Anio.AnioDescripcion,

                TransmisionNombre =
                    entidad.Transmision.TransmisionNombre,

                CilindroNombre =
                    entidad.Cilindro.CilindroNombre,

                CostoManoObra =
                    entidad.CostoManoObra,

                CostoRefacciones =
                    entidad.CostoRefacciones,

                FechaInicioVigencia =
                    entidad.FechaInicioVigencia,

                FechaFinVigencia =
                    entidad.FechaFinVigencia,

                Observaciones =
                    entidad.Observaciones
            };
        }




        public async Task EliminarConceptoServicioCosto(int id)
        {
            var entidad =
                await _context.ConceptoServicioCostos
                    .FirstOrDefaultAsync(x =>
                        x.ConceptoServicioCostoId == id);

            if (entidad == null)
            {
                throw new Exception("WARNING|Registro no encontrado.");
            }

            entidad.Activo = false;

            await _context.SaveChangesAsync();
        }








        public async Task<SolicitudMantenimientoEditViewModel> ObtenerSolicitudMantenimientoEdit(int id)
        {
            var entidad = await _context.SolicitudMantenimientos
                .AsNoTracking()
                .Include(x => x.UnidadVehicular)
                .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == id && x.Activo);

            if (entidad == null)
            {
                throw new Exception("WARNING|La solicitud no fue encontrada.");
            }

            // =========================================
            // OBTENER DOCUMENTOS
            // =========================================
            var documentos = await _context.SolicitudMantenimientoDocumentos
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == entidad.SolicitudMantenimientoId && x.Activo)
                .OrderByDescending(x => x.FechaCarga)
                .Select(x => new SolicitudMantenimientoDocumentoViewModel
                {
                    SolicitudMantenimientoDocumentoId = x.SolicitudMantenimientoDocumentoId,
                    TipoDocumento = x.TipoDocumento,
                    NombreArchivo = x.NombreArchivo,
                    FechaCarga = x.FechaCarga
                })
                .ToListAsync();

            // =========================================
            // OBTENER SERVICIOS
            // =========================================
            var servicios = await _context.SolicitudMantenimientoDetalles
                .AsNoTracking()
                .Include(x => x.IdObjetoGastoManoObraNavigation)
                .Include(x => x.IdObjetoGastoRefaccionesNavigation)
                .Where(x => x.SolicitudMantenimientoId == entidad.SolicitudMantenimientoId)
                .Select(x => new SolicitudMantenimientoServicioViewModel
                {
                    ConceptoServicioId = x.ConceptoServicioId,
                    Cantidad = x.Cantidad,
                    CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                    CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                    IdObjetoGastoManoObra = x.IdObjetoGastoManoObra,
                    IdObjetoGastoRefacciones = x.IdObjetoGastoRefacciones,
                    ImporteCotizacion = (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad,
                    ClaveObjetoGastoManoObra = x.IdObjetoGastoManoObraNavigation != null ? x.IdObjetoGastoManoObraNavigation.ClaveObjGasto : "",
                    ClaveObjetoGastoRefacciones = x.IdObjetoGastoRefaccionesNavigation != null ? x.IdObjetoGastoRefaccionesNavigation.ClaveObjGasto : "",
                    Observaciones = x.Observaciones
                })
                .ToListAsync();

            // =========================================
            // CONFIGURACIONES PRESUPUESTALES DEL TALLER
            // =========================================
            var configuracionesTaller = new List<SelectListItem>();
            if (entidad.TallerId.HasValue && entidad.TallerId.Value > 0)
            {
                configuracionesTaller = await _context.TallerConfiguracions
                    .AsNoTracking()
                    .Where(tc => tc.TallerId == entidad.TallerId.Value && tc.Activo && tc.TallerConfiguracionPresupuestal.Activo)
                    .Select(tc => new SelectListItem
                    {
                        Value = tc.TallerConfiguracionPresupuestalId.ToString(),
                        Text = $"Contrato: {tc.TallerConfiguracionPresupuestal.Contrato} | Recurso: {tc.TallerConfiguracionPresupuestal.Recurso.Nombre} (Monto Máx: {tc.TallerConfiguracionPresupuestal.MontoMaximo:C})",
                        Selected = tc.TallerConfiguracionPresupuestalId == entidad.TallerConfiguracionPresupuestalId
                    })
                    .ToListAsync();
            }

            return new SolicitudMantenimientoEditViewModel
            {
                SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                Folio = entidad.Folio,
                NumeroOficio = entidad.NumeroOficio,
                FechaSolicitud = entidad.FechaSolicitud,
                FechaRegistro = entidad.FechaCreacion,
                AreaSolicitanteId = entidad.AreaSolicitanteId,
                UnidadVehicularId = entidad.UnidadVehicularId,
                TallerId = (int)entidad.TallerId!,
                TallerConfiguracionPresupuestalId = entidad.TallerConfiguracionPresupuestalId, // <--- CAMPO INTEGRADO
                TipoMantenimientoId = entidad.TipoMantenimientoId,
                KilometrajeActual = entidad.KilometrajeActual,
                Observaciones = entidad.Observaciones,
                ImporteCotizacion = entidad.ImporteCotizacion,
                EstatusSolicitudMantenimientoId = entidad.EstatusSolicitudMantenimientoId,
                EstadoOdometroId = entidad.EstadoOdometroId,

                // SERVICIOS Y DOCUMENTOS
                Servicios = servicios,
                ConceptosServicio = await ObtenerConceptosServicioFiltrados(entidad.UnidadVehicularId),
                ObjetosGasto = await ObtenerObjetosGasto(),
                Documentos = documentos,
                TieneDocumentos = documentos.Any(),
                TotalDocumentos = documentos.Count,

                // CATÁLOGOS
                Areas = await ObtenerAreas(),
                UnidadesVehiculares = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Value = entidad.UnidadVehicularId.ToString(),
                                Text = entidad.UnidadVehicular != null
                                    ? $"{entidad.UnidadVehicular.NumeroSerie} - {entidad.UnidadVehicular.NumeroEconomico} - {entidad.UnidadVehicular.PlacaActual}"
                                    : "Unidad no encontrada",
                                Selected = true
                            }
                        },
                TiposMantenimiento = await ObtenerTiposMantenimiento(),
                Talleres = await ObtenerTalleres(),
                ConfiguracionesTaller = configuracionesTaller, // <--- LISTA ASIGNADA
                Estatuses = await ObtenerEstatusSolicitudMantenimiento(),
                EstadosOdometro = await ObtenerEstadosOdometro()
            };
        }





        public async Task EditarSolicitudMantenimiento(SolicitudMantenimientoEditDto dto, IEnumerable<IFormFile>? archivos)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var archivosGuardados = new List<string>();

            try
            {
                // OBTENER ENTIDAD
                var entidad = await _context.SolicitudMantenimientos
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == dto.SolicitudMantenimientoId && x.Activo);

                if (entidad == null)
                {
                    throw new Exception("La solicitud no fue encontrada.");
                }

                // VALIDAR FOLIO DUPLICADO
                var existeFolio = await _context.SolicitudMantenimientos
                    .AnyAsync(x => x.SolicitudMantenimientoId != dto.SolicitudMantenimientoId
                                && x.Folio == dto.Folio
                                && x.Activo);

                if (existeFolio)
                {
                    throw new Exception("WARNING|El folio ya se encuentra registrado.");
                }

                // =====================================================
                // PROTECCIÓN EN SERVIDOR: VALIDAR CONFIGURACIÓN Y TALLER
                // =====================================================
                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    throw new Exception("WARNING|Debe seleccionar un contrato/configuración presupuestal válido.");
                }

                var existeConfiguracionValida = await (
                    from tc in _context.TallerConfiguracions
                    join tcp in _context.TallerConfiguracionPresupuestals
                        on tc.TallerConfiguracionPresupuestalId equals tcp.TallerConfiguracionPresupuestalId
                    where tc.TallerId == dto.TallerId
                          && tc.TallerConfiguracionPresupuestalId == dto.TallerConfiguracionPresupuestalId
                          && tc.Activo
                          && tcp.Activo
                    select tcp
                ).AnyAsync();

                if (!existeConfiguracionValida)
                {
                    throw new Exception("WARNING|El contrato o configuración presupuestal seleccionada no pertenece al taller elegido o no se encuentra activa.");
                }

                // =========================================
                // VALIDAR KILOMETRAJE
                // =========================================
                if (dto.KilometrajeActual < 0)
                {
                    throw new Exception("El kilometraje no puede ser negativo.");
                }

                if (dto.KilometrajeActual > 999999)
                {
                    throw new Exception("El kilometraje debe estar entre 0 y 999999.");
                }

                if (dto.EstadoOdometroId == 1 && dto.KilometrajeActual <= 0)
                {
                    throw new Exception("Cuando el odómetro opera correctamente, el kilometraje debe ser mayor a cero.");
                }

                if (dto.EstadoOdometroId != 1 && dto.KilometrajeActual > 0)
                {
                    throw new Exception("Si el odómetro no opera correctamente, el kilometraje debe ser cero.");
                }

                // ACTUALIZAR SOLICITUD
                entidad.Folio = dto.Folio.Trim();
                entidad.NumeroOficio = dto.NumeroOficio?.Trim();
                entidad.FechaSolicitud = dto.FechaSolicitud;
                entidad.AreaSolicitanteId = dto.AreaSolicitanteId;
                entidad.UnidadVehicularId = dto.UnidadVehicularId;
                entidad.TallerId = dto.TallerId;
                entidad.TallerConfiguracionPresupuestalId = dto.TallerConfiguracionPresupuestalId; // <--- ASIGNADO
                entidad.TipoMantenimientoId = dto.TipoMantenimientoId;
                entidad.KilometrajeActual = (int)dto.KilometrajeActual;
                entidad.EstadoOdometroId = dto.EstadoOdometroId;
                entidad.Observaciones = dto.Observaciones?.Trim();

                await _context.SaveChangesAsync();

                var serviciosBloqueados = entidad.EstatusSolicitudMantenimientoId >= 5 && entidad.EstatusSolicitudMantenimientoId != 6;

                if (serviciosBloqueados)
                {
                    await transaction.CommitAsync();
                    return;
                }

                if (!serviciosBloqueados)
                {
                    dto.ImporteCotizacion = dto.Servicios.Sum(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad);
                    entidad.ImporteCotizacion = dto.ImporteCotizacion;
                }

                // VALIDACION HISTORICA EDIT
                var conceptosServicio = dto.Servicios
                    .Where(x => x.ConceptoServicioId > 0)
                    .Select(x => x.ConceptoServicioId)
                    .Distinct()
                    .ToList();

                if (conceptosServicio.Any())
                {
                    var resultadoHistorico = await ValidarServiciosHistoricos(dto.UnidadVehicularId, conceptosServicio, dto.TipoMantenimientoId, dto.SolicitudMantenimientoId);
                    dto.ValidacionHistorica = resultadoHistorico;
                }

                var serviciosActuales = await _context.SolicitudMantenimientoDetalles
                    .Where(x => x.SolicitudMantenimientoId == entidad.SolicitudMantenimientoId)
                    .ToListAsync();

                if (serviciosActuales.Any())
                {
                    _context.SolicitudMantenimientoDetalles.RemoveRange(serviciosActuales);
                }

                foreach (var servicio in dto.Servicios)
                {
                    if (servicio.IdObjetoGastoManoObra.HasValue &&
                        servicio.IdObjetoGastoRefacciones.HasValue &&
                        servicio.IdObjetoGastoManoObra == servicio.IdObjetoGastoRefacciones)
                    {
                        throw new Exception("WARNING|La partida de mano de obra y la de refacciones no pueden ser la misma.");
                    }

                    if (servicio.CostoEstimadoManoObra > 0 && !servicio.IdObjetoGastoManoObra.HasValue)
                    {
                        throw new Exception("WARNING|Debe seleccionar una partida para mano de obra.");
                    }

                    if (servicio.CostoEstimadoRefacciones > 0 && !servicio.IdObjetoGastoRefacciones.HasValue)
                    {
                        throw new Exception("WARNING|Debe seleccionar una partida para refacciones.");
                    }
                }

                if (dto.Servicios != null && dto.Servicios.Any())
                {
                    var nuevosServicios = dto.Servicios
                        .Select(x => new SolicitudMantenimientoDetalle
                        {
                            SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                            ConceptoServicioId = x.ConceptoServicioId,
                            Cantidad = (int)x.Cantidad,
                            CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                            CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                            IdObjetoGastoManoObra = x.IdObjetoGastoManoObra,
                            IdObjetoGastoRefacciones = x.IdObjetoGastoRefacciones,
                            Observaciones = x.Observaciones,
                            FechaCreacion = DateTime.Now,
                            Activo = true
                        })
                        .ToList();

                    await _context.SolicitudMantenimientoDetalles.AddRangeAsync(nuevosServicios);
                }

                await _context.SaveChangesAsync();

                // VALIDAR Y GUARDAR DOCUMENTOS
                if (archivos != null && archivos.Any())
                {
                    var rutaBase = _configuration["Storage:DocumentosRuta"];

                    if (string.IsNullOrWhiteSpace(rutaBase))
                    {
                        throw new Exception("WARNING|No se configuró la ruta de almacenamiento.");
                    }

                    var carpetaSolicitud = Path.Combine(rutaBase, entidad.Folio);

                    if (!Directory.Exists(carpetaSolicitud))
                    {
                        Directory.CreateDirectory(carpetaSolicitud);
                    }

                    const long tamanioMaximo = 2 * 1024 * 1024;
                    const int maxArchivos = 5;

                    if (archivos.Count() > maxArchivos)
                    {
                        throw new Exception($"WARNING|Solo se permiten {maxArchivos} archivos.");
                    }

                    var extensionesPermitidas = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png" };
                    var contentTypesPermitidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "application/pdf", "image/jpeg", "image/png" };
                    var provider = new FileExtensionContentTypeProvider();

                    foreach (var archivo in archivos)
                    {
                        if (archivo == null || archivo.Length <= 0)
                        {
                            continue;
                        }

                        if (archivo.Length > tamanioMaximo)
                        {
                            throw new Exception($"WARNING|El archivo '{archivo.FileName}' excede 2 MB.");
                        }

                        var nombreOriginal = Path.GetFileName(archivo.FileName);

                        if (string.IsNullOrWhiteSpace(nombreOriginal))
                        {
                            throw new Exception("WARNING|Nombre de archivo inválido.");
                        }

                        if (nombreOriginal.Length > 150)
                        {
                            throw new Exception($"WARNING|El nombre '{nombreOriginal}' es demasiado largo.");
                        }

                        var nombreLower = nombreOriginal.ToLowerInvariant();

                        if (nombreLower.Contains(".exe") || nombreLower.Contains(".bat") || nombreLower.Contains(".cmd") ||
                            nombreLower.Contains(".js") || nombreLower.Contains(".vbs") || nombreLower.Contains(".ps1") ||
                            nombreLower.Contains(".msi"))
                        {
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' contiene extensiones no permitidas.");
                        }

                        var extension = Path.GetExtension(nombreOriginal).ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' tiene una extensión no permitida.");
                        }

                        if (!provider.TryGetContentType(nombreOriginal, out _))
                        {
                            throw new Exception($"WARNING|No fue posible validar '{nombreOriginal}'.");
                        }

                        var contentType = archivo.ContentType.Trim().ToLowerInvariant();

                        if (!contentTypesPermitidos.Contains(contentType))
                        {
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' tiene un MIME inválido.");
                        }

                        using var memoryStream = new MemoryStream();
                        await archivo.CopyToAsync(memoryStream);
                        var bytes = memoryStream.ToArray();

                        if (!ArchivoValidoPorFirma(extension, bytes))
                        {
                            throw new Exception($"WARNING|El contenido real de '{nombreOriginal}' no coincide con la extensión.");
                        }

                        string hashArchivo;
                        using (var sha256 = SHA256.Create())
                        {
                            var hashBytes = sha256.ComputeHash(bytes);
                            hashArchivo = Convert.ToHexString(hashBytes);
                        }

                        var nombreFisico = $"{Guid.NewGuid()}{extension}";
                        var rutaCompleta = Path.Combine(carpetaSolicitud, nombreFisico);

                        await File.WriteAllBytesAsync(rutaCompleta, bytes);
                        archivosGuardados.Add(rutaCompleta);

                        var documento = new SolicitudMantenimientoDocumento
                        {
                            SolicitudMantenimientoId = entidad.SolicitudMantenimientoId,
                            TipoDocumento = "DOCUMENTO_ADICIONAL",
                            NombreArchivo = nombreOriginal,
                            RutaArchivo = rutaCompleta,
                            FechaCarga = DateTime.Now,
                            UsuarioCargaId = 1,
                            Activo = true,
                            HashArchivo = hashArchivo
                        };

                        _context.SolicitudMantenimientoDocumentos.Add(documento);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                foreach (var ruta in archivosGuardados)
                {
                    if (File.Exists(ruta))
                    {
                        File.Delete(ruta);
                    }
                }

                throw;
            }
        }








        public async Task<List<Select2Dto>> BuscarUnidadesVehiculares(string term)
        {
            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
                var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

            if (usuarioGuid.ToString() == "0bf13a21-fe5a-45f9-a584-1f6d338e1995")
            {
                var query = _context.UnidadVehiculars
                   .AsNoTracking()
                   .Where(x => x.Activo == true && x.AreaId == 22);

                // SI VIENE BUSQUEDA

                if (!string.IsNullOrWhiteSpace(term))
                {
                    term = term.Trim();

                    query = query.Where(x =>
                    x.NumeroEconomico.Contains(term) ||
                    x.NumeroSerie.Contains(term) ||
                    x.PlacaActual.Contains(term) ||
                    x.Marca.MarcaNombre.Contains(term) ||
                    x.Modelo.ModeloNombre.Contains(term)
                    );
                }

                var registros = await query
                        .OrderBy(x => x.NumeroEconomico)
                        .Take(5)
                        .Select(x => new Select2Dto
                        {
                            id = x.UnidadVehicularId,
                            text = x.NumeroEconomico + " - " + x.NumeroSerie + " - " + x.PlacaActual + " - " + x.Marca.MarcaNombre + " " + x.Modelo.ModeloNombre
                        })
                        .ToListAsync();

                return registros;
            }
            else
            {

                // este debe estar afuera del IF
                var query = _context.UnidadVehiculars
               .AsNoTracking()
               .Where(x => x.Activo == true);

                // SI VIENE BUSQUEDA

                if (!string.IsNullOrWhiteSpace(term))
                {
                    term = term.Trim();

                    query = query.Where(x =>
                    x.NumeroEconomico.Contains(term) ||
                    x.NumeroSerie.Contains(term) ||
                    x.PlacaActual.Contains(term) ||
                    x.Marca.MarcaNombre.Contains(term) ||
                    x.Modelo.ModeloNombre.Contains(term)
                    );
                }

                var registros = await query
                        .OrderBy(x => x.NumeroEconomico)
                        .Take(5)
                        .Select(x => new Select2Dto
                        {
                            id = x.UnidadVehicularId,
                            text = x.NumeroEconomico + " - " + x.NumeroSerie + " - " + x.PlacaActual + " - " + x.Marca.MarcaNombre + " " + x.Modelo.ModeloNombre
                        })
                        .ToListAsync();

                return registros;
            }

            //////////////////////////////////////////////// ESTO ES PARA EL CHIPOTE
        }






        public async Task<SolicitudMantenimientoInfoDto> ObtenerSolicitudMantenimientoPorId(int id)
        {
            var entidad =
                await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .Include(x => x.UnidadVehicular)
                    .FirstOrDefaultAsync(x =>
                        x.SolicitudMantenimientoId == id
                        &&
                        x.Activo);

            if (entidad == null)
            {
                return null;
            }

            return new SolicitudMantenimientoInfoDto
            {
                SolicitudMantenimientoId =
                    entidad.SolicitudMantenimientoId,

                Folio =
                    entidad.Folio,

                AreaSolicitanteId =
                    entidad.AreaSolicitanteId,

                UnidadVehicularId =
                    entidad.UnidadVehicularId,

                EstatusSolicitudMantenimientoId =
                    entidad.EstatusSolicitudMantenimientoId,

                FechaCreacion =
                    entidad.FechaCreacion,

                UnidadVehicularTexto =
                    entidad.UnidadVehicular != null
                    ? $"{entidad.UnidadVehicular.NumeroEconomico} - {entidad.UnidadVehicular.PlacaActual}"
                    : "Unidad no encontrada"
            };
        }







        public async Task<UnidadVehicularDetailsViewModel> ObtenerUnidadVehicularDetails(int id)
        {
            var entidad =
                await _context.UnidadVehiculars
                    .AsNoTracking()
                    .Include(x => x.Marca)
                    .Include(x => x.Modelo)
                    .Include(x => x.Anio)
                    .Include(x => x.Transmision)
                    .Include(x => x.Cilindro)
                    .Include(x => x.Area)
                    .Include(x => x.Color)
                    .Include(x => x.Municipio)
                    .FirstOrDefaultAsync(x =>
                        x.UnidadVehicularId == id
                        &&
                        x.Activo);

            if (entidad == null)
            {
                throw new Exception(
                    "La unidad vehicular no fue encontrada.");
            }

            return new UnidadVehicularDetailsViewModel
            {
                UnidadVehicularId = entidad.UnidadVehicularId,

                NumeroSerie = entidad.NumeroSerie,

                PlacaActual = entidad.PlacaActual,

                NumeroEconomico = entidad.NumeroEconomico,

                MarcaNombre = entidad.Marca?.MarcaNombre,

                ModeloNombre = entidad.Modelo?.ModeloNombre,

                AnioNombre = entidad.Anio?.AnioDescripcion,

                TransmisionNombre = entidad.Transmision?.TransmisionNombre,

                CilindroNombre = entidad.Cilindro?.CilindroNombre,

                AreaNombre = entidad.Area?.AreaNombre,

                ColorNombre = entidad.Color?.ColorNombre,

                MunicipioNombre = entidad.Municipio?.Nombre,

                FechaCreacion = entidad.FechaCreacion,

                Activo = entidad.Activo
            };
        }



        public async Task CrearTransferenciaUnidad(TransferenciaUnidadCreateDto dto, IFormFile? documento)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            string? rutaDocumentoGuardado = null;

            try
            {
                // =========================================
                // VALIDAR UNIDAD
                // =========================================

                var unidad =
                    await _context.UnidadVehiculars
                        .FirstOrDefaultAsync(x =>
                            x.UnidadVehicularId ==
                                dto.UnidadVehicularId
                            &&
                            x.Activo);

                if (unidad == null)
                {
                    throw new Exception("La unidad vehicular no fue encontrada.");
                }

                // =========================================
                // VALIDAR ÁREAS
                // =========================================

                if (dto.AreaOrigenId == dto.AreaDestinoId)
                {
                    throw new Exception("El área destino debe ser diferente al área origen.");
                }

                if (unidad.AreaId != dto.AreaOrigenId)
                {
                    throw new Exception("La unidad no pertenece al área origen seleccionada.");
                }

                // =========================================
                // VALIDAR DOCUMENTO
                // =========================================

                if (documento == null || documento.Length <= 0)
                {
                    throw new Exception("Debe adjuntar el documento de transferencia en PDF.");
                }

                const long tamanioMaximo =
                    2 * 1024 * 1024;

                if (documento.Length > tamanioMaximo)
                {
                    throw new Exception("El documento excede el tamaño máximo permitido de 2 MB.");
                }

                var nombreOriginal =
                    Path.GetFileName(documento.FileName);

                if (string.IsNullOrWhiteSpace(nombreOriginal))
                {
                    throw new Exception("Nombre de archivo inválido.");
                }

                if (nombreOriginal.Length > 150)
                {
                    throw new Exception("El nombre del archivo es demasiado largo.");
                }

                var extension =
                    Path.GetExtension(nombreOriginal)
                        .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    throw new Exception("Solo se permiten documentos PDF.");
                }

                // =========================================
                // VALIDAR DOBLE EXTENSIÓN
                // =========================================

                var nombreLower =
                    nombreOriginal.ToLowerInvariant();

                if (
                    nombreLower.Contains(".exe")
                    ||
                    nombreLower.Contains(".bat")
                    ||
                    nombreLower.Contains(".cmd")
                    ||
                    nombreLower.Contains(".js")
                    ||
                    nombreLower.Contains(".vbs")
                    ||
                    nombreLower.Contains(".ps1")
                    ||
                    nombreLower.Contains(".msi")
                )
                {
                    throw new Exception("WARNING|El archivo contiene extensiones no permitidas.");
                }

                // =========================================
                // VALIDAR MIME TYPE
                // =========================================

                var contentType =
                    documento.ContentType
                        .Trim()
                        .ToLowerInvariant();

                if (contentType != "application/pdf")
                {
                    throw new Exception("WARNING|El tipo MIME del archivo no es válido.");
                }

                // =========================================
                // VALIDAR CONTENIDO REAL
                // =========================================

                using var memoryStream =
                    new MemoryStream();

                await documento.CopyToAsync(memoryStream);

                var bytes =
                    memoryStream.ToArray();

                if (!ArchivoValidoPorFirma(".pdf", bytes))
                {
                    throw new Exception("WARNING|El contenido real del archivo no corresponde a un PDF válido.");
                }

                // =========================================
                // GENERAR HASH
                // =========================================

                string hashArchivo;

                using (var sha256 = SHA256.Create())
                {
                    var hashBytes =
                        sha256.ComputeHash(bytes);

                    hashArchivo =
                        Convert.ToHexString(hashBytes);
                }

                // =========================================
                // GENERAR FOLIO
                // =========================================

                var fechaActual = DateTime.Now;

                //var consecutivo = await _context.TransferenciaUnidads.CountAsync();

                var folio = $"TR-{DateTime.Now:yyyyMMddHHmmssfff}";


                //consecutivo++;

                //var folio = $"TR-{fechaActual:yyyyMMdd}-{consecutivo:D5}";

                // =========================================
                // GUARDAR DOCUMENTO
                // =========================================

                var rutaBase =
                    _configuration["Storage:DocumentosRuta"];

                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    throw new Exception("WARNING|No se configuró la ruta de almacenamiento.");
                }

                var carpetaTransferencia =
                    Path.Combine(
                        rutaBase,
                        "Transferencias",
                        folio);

                if (!Directory.Exists(carpetaTransferencia))
                {
                    Directory.CreateDirectory(carpetaTransferencia);
                }

                var nombreFisico =
                    $"{Guid.NewGuid()}{extension}";

                rutaDocumentoGuardado =
                    Path.Combine(
                        carpetaTransferencia,
                        nombreFisico);

                await File.WriteAllBytesAsync(
                    rutaDocumentoGuardado,
                    bytes);


                // OBTENER DEL USUARIO AUTENTICADO
                string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

                // =========================================
                // CREAR TRANSFERENCIA
                // =========================================

                var entidad =
                    new TransferenciaUnidad
                    {
                        Folio =
                            folio,

                        UnidadVehicularId =
                            dto.UnidadVehicularId,

                        AreaOrigenId =
                            dto.AreaOrigenId,

                        AreaDestinoId =
                            dto.AreaDestinoId,

                        FechaSolicitud =
                            fechaActual,

                        MotivoTransferencia =
                            dto.MotivoTransferencia.Trim(),

                        Observaciones =
                            dto.Observaciones?.Trim(),

                        DocumentoRuta =
                            rutaDocumentoGuardado,

                        DocumentoNombreOriginal =
                            nombreOriginal,

                        EstatusTransferenciaUnidadId = 1,

                        UsuarioSolicitaId = usuarioId,

                        FechaCreacion =
                            fechaActual,

                        Activo = true
                    };

                _context.TransferenciaUnidads
                    .Add(entidad);

                // ACTUALIZAR ÁREA DE LA UNIDAD

                //unidad.AreaId =
                //    dto.AreaDestinoId;

                // =========================================
                // BITÁCORA
                // =========================================

                var seguimiento =
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidad =
                            entidad,

                        EstatusTransferenciaUnidadId = 1,

                        Observaciones =
                            "Transferencia registrada.",

                        UsuarioId = usuarioId,

                        FechaMovimiento =
                            fechaActual,

                        FechaCreacion =
                            fechaActual,

                        Activo = true
                    };

                _context.TransferenciaUnidadSeguimientos
                    .Add(seguimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                // ELIMINAR ARCHIVO SI FALLA

                if (
                    !string.IsNullOrWhiteSpace(rutaDocumentoGuardado)
                    &&
                    File.Exists(rutaDocumentoGuardado)
                )
                {
                    File.Delete(rutaDocumentoGuardado);
                }

                throw;
            }
        }







        public async Task<List<TransferenciaUnidadViewModel>> ObtenerTransferencias()
        {
            return await _context.TransferenciaUnidads
                .AsNoTracking()

                .Include(x => x.UnidadVehicular)

                .Include(x => x.AreaOrigen)

                .Include(x => x.AreaDestino)

                .Include(x => x.EstatusTransferenciaUnidad)

                .Where(x => x.Activo)

                .OrderByDescending(x => x.FechaCreacion)

                .Select(x =>
                    new TransferenciaUnidadViewModel
                    {
                        TransferenciaUnidadId =
                            x.TransferenciaUnidadId,

                        Folio =
                            x.Folio,

                        // =====================================
                        // UNIDAD
                        // =====================================

                        UnidadVehicularId =
                            x.UnidadVehicularId,

                        UnidadVehicularTexto =
                            x.UnidadVehicular.NumeroEconomico
                            + " - "
                            + x.UnidadVehicular.NumeroSerie
                            + " - "
                            + x.UnidadVehicular.PlacaActual
                            ,

                        // =====================================
                        // ÁREAS
                        // =====================================

                        AreaOrigenId =
                            x.AreaOrigenId,

                        AreaOrigenTexto =
                            x.AreaOrigen.AreaNombre,

                        AreaDestinoId =
                            x.AreaDestinoId,

                        AreaDestinoTexto =
                            x.AreaDestino.AreaNombre,

                        // =====================================
                        // ESTATUS
                        // =====================================

                        EstatusTransferenciaUnidadId =
                            x.EstatusTransferenciaUnidadId,

                        EstatusTexto =
                            x.EstatusTransferenciaUnidad.Nombre,

                        // =====================================
                        // FECHAS
                        // =====================================

                        FechaSolicitud =
                            x.FechaSolicitud,

                        FechaCreacion =
                            x.FechaCreacion,

                        // =====================================
                        // OPERACIÓN
                        // =====================================

                        MotivoTransferencia =
                            x.MotivoTransferencia,

                        Observaciones =
                            x.Observaciones
                    })
                .ToListAsync();
        }


        public async Task<TransferenciaUnidadCreateViewModel> ObtenerTransferenciaCreate()
        {
            return new TransferenciaUnidadCreateViewModel
            {
                FechaTransferencia =
                    DateTime.Now,

                Areas =
                    await ObtenerAreas(),

                UnidadesVehiculares =
                    await ObtenerUnidadesVehicularesSelectList()
            };
        }




        public async Task<TransferenciaUnidadDetalleViewModel> ObtenerTransferenciaDetalle(int id)
        {
            var entidad =
                await _context.TransferenciaUnidads
                    .AsNoTracking()

                    .Include(x => x.UnidadVehicular)
                        .ThenInclude(x => x.Marca)

                    .Include(x => x.UnidadVehicular.Modelo)

                    .Include(x => x.UnidadVehicular.Color)

                    .Include(x => x.AreaOrigen)

                    .Include(x => x.AreaDestino)

                    .Include(x => x.EstatusTransferenciaUnidad)

                    .FirstOrDefaultAsync(x =>
                        x.TransferenciaUnidadId == id
                        &&
                        x.Activo);

            if (entidad == null)
            {
                throw new Exception("WARNINGLa transferencia no fue encontrada.");
            }

            // =========================================
            // SEGUIMIENTOS
            // =========================================

            var seguimientos =
                await _context.TransferenciaUnidadSeguimientos
                    .AsNoTracking()

                    .Include(x => x.EstatusTransferenciaUnidad)

                    .Where(x =>
                        x.TransferenciaUnidadId ==
                            entidad.TransferenciaUnidadId
                        &&
                        x.Activo)

                    .OrderByDescending(x => x.FechaMovimiento)

                    .Select(x =>
                        new TransferenciaUnidadSeguimientoViewModel
                        {
                            TransferenciaUnidadSeguimientoId =
                                x.TransferenciaUnidadSeguimientoId,

                            EstatusTexto =
                                x.EstatusTransferenciaUnidad.Nombre,

                            TipoMovimiento =
                                x.TipoMovimiento,

                            FechaMovimiento =
                                x.FechaMovimiento,

                            Observaciones =
                                x.Observaciones,

                            FechaRegistro =
                                x.FechaCreacion,

                            DocumentoNombreOriginal = x.DocumentoNombreOriginal
                        })
                    .ToListAsync();

            // =========================================
            // DETALLE
            // =========================================

            return new TransferenciaUnidadDetalleViewModel
            {
                TransferenciaUnidadId =
                    entidad.TransferenciaUnidadId,

                Folio =
                    entidad.Folio,

                // =========================================
                // UNIDAD
                // =========================================

                UnidadVehicularId =
                    entidad.UnidadVehicularId,

                NumeroSerie =
                    entidad.UnidadVehicular.NumeroSerie,

                PlacaActual =
                    entidad.UnidadVehicular.PlacaActual,

                NumeroEconomico =
                    entidad.UnidadVehicular.NumeroEconomico,

                MarcaModelo =
                    entidad.UnidadVehicular.Marca.MarcaNombre
                    + " "
                    + entidad.UnidadVehicular.Modelo.ModeloNombre,

                Color =
                    entidad.UnidadVehicular.Color.ColorNombre,

                // =========================================
                // ÁREAS
                // =========================================

                AreaOrigenId =
                    entidad.AreaOrigenId,

                AreaOrigenTexto =
                    entidad.AreaOrigen.AreaNombre,

                AreaDestinoId =
                    entidad.AreaDestinoId,

                AreaDestinoTexto =
                    entidad.AreaDestino.AreaNombre,

                // =========================================
                // ESTATUS
                // =========================================

                EstatusTransferenciaUnidadId =
                    entidad.EstatusTransferenciaUnidadId,

                EstatusTexto =
                    entidad.EstatusTransferenciaUnidad.Nombre,

                // =========================================
                // FECHAS
                // =========================================

                FechaSolicitud =
                    entidad.FechaSolicitud,

                FechaRespuesta =
                    entidad.FechaRespuesta,

                FechaEntrega =
                    entidad.FechaEntrega,

                FechaRecepcion =
                    entidad.FechaRecepcion,

                FechaCancelacion =
                    entidad.FechaCancelacion,

                FechaCreacion =
                    entidad.FechaCreacion,

                // =========================================
                // OPERACIÓN
                // =========================================

                MotivoTransferencia =
                    entidad.MotivoTransferencia,

                ComentariosAutorizacion =
                    entidad.ComentariosAutorizacion,

                ObservacionesEntrega =
                    entidad.ObservacionesEntrega,

                ObservacionesRecepcion =
                    entidad.ObservacionesRecepcion,

                DocumentoNombreOriginal =
                    entidad.DocumentoNombreOriginal,

                // =========================================
                // USUARIOS
                // =========================================

                UsuarioSolicitaId =
                    entidad.UsuarioSolicitaId,

                UsuarioAutorizaId =
                    entidad.UsuarioAutorizaId,

                UsuarioEntregaId =
                    entidad.UsuarioEntregaId,

                UsuarioRecibeId =
                    entidad.UsuarioRecibeId,

                // =========================================
                // SEGUIMIENTO
                // =========================================

                Seguimientos =
                    seguimientos
            };
        }




        public async Task AutorizarTransferencia(TransferenciaUnidadAutorizarDto dto, IFormFile? documento)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var entidad =
                    await _context.TransferenciaUnidads
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadId ==
                                dto.TransferenciaUnidadId
                            &&
                            x.Activo);

                if (entidad == null)
                {
                    throw new Exception("WARNING|La transferencia no existe.");
                }

                if (entidad.EstatusTransferenciaUnidadId != 1)
                {
                    throw new Exception("WARNING|Solo pueden autorizarse transferencias pendientes.");
                }


                // OBTENER DEL USUARIO AUTENTICADO
                string usuarioId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";



                entidad.EstatusTransferenciaUnidadId = 2;

                entidad.UsuarioAutorizaId = usuarioId;

                entidad.ComentariosAutorizacion = dto.ComentariosAutorizacion;

                entidad.FechaRespuesta =
                    DateTime.Now;

                var seguimiento =
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidadId =
                            entidad.TransferenciaUnidadId,

                        EstatusTransferenciaUnidadId = 2,

                        TipoMovimiento =
                            "AUTORIZACION",

                        Observaciones =
                            dto.ComentariosAutorizacion,

                        UsuarioId = dto.UsuarioAutorizaId,

                        FechaMovimiento =
                            DateTime.Now,

                        FechaCreacion =
                            DateTime.Now,

                        Activo = true
                    };

                _context
                    .TransferenciaUnidadSeguimientos
                    .Add(seguimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }





        public async Task RechazarTransferencia(TransferenciaUnidadRechazarDto dto)
        {
            var entidad =
                await _context.TransferenciaUnidads
                    .FirstOrDefaultAsync(x =>
                        x.TransferenciaUnidadId ==
                            dto.TransferenciaUnidadId
                        &&
                        x.Activo);

            if (entidad == null)
            {
                throw new Exception("WARNING|Transferencia no encontrada.");
            }

            if (entidad.EstatusTransferenciaUnidadId != 1)
            {
                throw new Exception("WARNING|Solo pueden rechazarse transferencias pendientes.");
            }

            entidad.EstatusTransferenciaUnidadId = 3;

            entidad.FechaRespuesta =
                DateTime.Now;

            entidad.ComentariosAutorizacion =
                dto.Observaciones;

            _context.TransferenciaUnidadSeguimientos.Add(
                new TransferenciaUnidadSeguimiento
                {
                    TransferenciaUnidadId =
                        entidad.TransferenciaUnidadId,

                    EstatusTransferenciaUnidadId = 3,

                    TipoMovimiento =
                        "RECHAZO",

                    Observaciones =
                        dto.Observaciones,

                    UsuarioId =
                        dto.UsuarioId,

                    FechaMovimiento =
                        DateTime.Now,

                    FechaCreacion =
                        DateTime.Now,

                    Activo = true
                });

            await _context.SaveChangesAsync();
        }






        public async Task RecibirTransferencia(TransferenciaUnidadRecibirDto dto, IFormFile? documento)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var transferencia =
                    await _context.TransferenciaUnidads
                        .Include(x => x.UnidadVehicular)
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadId ==
                                dto.TransferenciaUnidadId
                            &&
                            x.Activo);

                if (transferencia == null)
                {
                    throw new Exception("WARNING|Transferencia no encontrada.");
                }

                if (transferencia.EstatusTransferenciaUnidadId != 5)
                {
                    throw new Exception("WARNING|Solo pueden recibirse transferencias entregadas.");
                }

                // =========================================
                // CAMBIO REAL DE ÁREA
                // =========================================

                transferencia.UnidadVehicular.AreaId =
                    transferencia.AreaDestinoId;

                transferencia.EstatusTransferenciaUnidadId = 6;

                transferencia.UsuarioRecibeId = dto.UsuarioRecibeId;

                transferencia.FechaRecepcion =
                    DateTime.Now;

                transferencia.ObservacionesRecepcion =
                    dto.ObservacionesRecepcion;

                _context.TransferenciaUnidadSeguimientos.Add(
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidadId =
                            transferencia.TransferenciaUnidadId,

                        EstatusTransferenciaUnidadId = 6,

                        TipoMovimiento =
                            "RECEPCION",

                        Observaciones =
                            dto.ObservacionesRecepcion,

                        UsuarioId =
                            dto.UsuarioRecibeId,

                        FechaMovimiento =
                            DateTime.Now,

                        FechaCreacion =
                            DateTime.Now,

                        Activo = true
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }





        public async Task CancelarTransferencia(TransferenciaUnidadCancelarDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================================
                // OBTENER TRANSFERENCIA
                // =========================================

                var entidad =
                    await _context.TransferenciaUnidads
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadId ==
                                dto.TransferenciaUnidadId
                            &&
                            x.Activo);

                if (entidad == null)
                {
                    throw new Exception("WARNING|La transferencia no fue encontrada.");
                }

                // =========================================
                // VALIDAR ESTATUS
                // =========================================

                // SOLO:
                // PENDIENTE = 1
                // AUTORIZADA = 2

                if (
                    entidad.EstatusTransferenciaUnidadId != 1
                    &&
                    entidad.EstatusTransferenciaUnidadId != 2
                )
                {
                    throw new Exception("WARNING|Solo pueden cancelarse transferencias pendientes o autorizadas.");
                }

                // =========================================
                // ACTUALIZAR
                // =========================================

                entidad.EstatusTransferenciaUnidadId = 4;

                entidad.FechaCancelacion =
                    DateTime.Now;

                entidad.Observaciones =
                    dto.MotivoCancelacion;

                // =========================================
                // BITÁCORA
                // =========================================

                var seguimiento =
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidadId =
                            entidad.TransferenciaUnidadId,

                        EstatusTransferenciaUnidadId = 4,

                        TipoMovimiento =
                            "CANCELACION",

                        Observaciones =
                            dto.MotivoCancelacion,

                        UsuarioId =
                            dto.UsuarioId,

                        FechaMovimiento =
                            DateTime.Now,

                        FechaCreacion =
                            DateTime.Now,

                        Activo = true
                    };

                _context
                    .TransferenciaUnidadSeguimientos
                    .Add(seguimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        public async Task EntregarTransferencia(TransferenciaUnidadEntregarDto dto, IFormFile documento)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var archivosGuardados =
                new List<string>();

            try
            {
                // =========================================
                // VALIDAR TRANSFERENCIA
                // =========================================

                var entidad =
                    await _context.TransferenciaUnidads
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadId ==
                                dto.TransferenciaUnidadId
                            &&
                            x.Activo);

                if (entidad == null)
                {
                    throw new Exception(
                        "WARNING|La transferencia no fue encontrada.");
                }

                // =========================================
                // VALIDAR ESTATUS
                // =========================================

                if (entidad.EstatusTransferenciaUnidadId != 2)
                {
                    throw new Exception(
                        "WARNING|Solo pueden entregarse transferencias autorizadas.");
                }

                // =========================================
                // VALIDAR DOCUMENTO
                // =========================================

                if (documento == null || documento.Length <= 0)
                {
                    throw new Exception(
                        "INFO|Debe adjuntar el acta de entrega en PDF.");
                }

                const long tamanioMaximo =
                    2 * 1024 * 1024;

                if (documento.Length > tamanioMaximo)
                {
                    throw new Exception(
                        "WARNING|El documento excede el tamaño máximo permitido.");
                }

                var nombreOriginal =
                    Path.GetFileName(documento.FileName);

                if (string.IsNullOrWhiteSpace(nombreOriginal))
                {
                    throw new Exception(
                        "WARNING|Nombre de archivo inválido.");
                }

                var extension =
                    Path.GetExtension(nombreOriginal)
                        .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    throw new Exception(
                        "WARNING|Solo se permiten archivos PDF.");
                }

                if (
                    documento.ContentType
                        .Trim()
                        .ToLowerInvariant()
                    !=
                    "application/pdf")
                {
                    throw new Exception(
                        "WARNING|El archivo no tiene un MIME válido.");
                }

                // =========================================
                // LEER ARCHIVO
                // =========================================

                using var memoryStream =
                    new MemoryStream();

                await documento.CopyToAsync(memoryStream);

                var bytes =
                    memoryStream.ToArray();

                // =========================================
                // VALIDAR FIRMA
                // =========================================

                if (!ArchivoValidoPorFirma(
                        extension,
                        bytes))
                {
                    throw new Exception(
                        "WARNING|El archivo PDF es inválido.");
                }

                // =========================================
                // HASH
                // =========================================

                string hashArchivo;

                using (var sha256 = SHA256.Create())
                {
                    var hashBytes =
                        sha256.ComputeHash(bytes);

                    hashArchivo =
                        Convert.ToHexString(hashBytes);
                }

                // =========================================
                // RUTA
                // =========================================

                var rutaBase =
                    _configuration["Storage:DocumentosRuta"];

                if (string.IsNullOrWhiteSpace(rutaBase))
                {
                    throw new Exception(
                        "WARNING|No existe configuración de almacenamiento.");
                }

                var carpetaTransferencia =
                    Path.Combine(
                        rutaBase,
                        "Transferencias",
                        entidad.Folio);

                if (!Directory.Exists(carpetaTransferencia))
                {
                    Directory.CreateDirectory(
                        carpetaTransferencia);
                }

                // =========================================
                // GUARDAR ARCHIVO
                // =========================================

                var nombreFisico =
                    $"{Guid.NewGuid():N}{extension}";

                var rutaCompleta =
                    Path.Combine(
                        carpetaTransferencia,
                        nombreFisico);

                await File.WriteAllBytesAsync(
                    rutaCompleta,
                    bytes);

                archivosGuardados.Add(rutaCompleta);

                // =========================================
                // ACTUALIZAR TRANSFERENCIA
                // =========================================

                // ENTREGADA

                entidad.EstatusTransferenciaUnidadId = 5;

                entidad.FechaEntrega =
                    DateTime.Now;

                entidad.UsuarioEntregaId = dto.UsuarioEntregaId;

                entidad.ObservacionesEntrega =
                    dto.ObservacionesEntrega;

                entidad.DocumentoRuta =
                    rutaCompleta;

                entidad.DocumentoNombreOriginal =
                    nombreOriginal;

                entidad.FechaTransferencia =
                    DateTime.Now;

                // =========================================
                // BITÁCORA
                // =========================================

                var seguimiento =
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidadId =
                            entidad.TransferenciaUnidadId,

                        EstatusTransferenciaUnidadId = 5,

                        TipoMovimiento =
                            "ENTREGA",

                        Observaciones =
                            dto.ObservacionesEntrega,

                        UsuarioId = dto.UsuarioEntregaId,

                        FechaMovimiento =
                            DateTime.Now,

                        FechaCreacion =
                            DateTime.Now,

                        Activo = true,

                        DocumentoRuta =
                            rutaCompleta,

                        DocumentoNombreOriginal =
                            nombreOriginal,

                        HashDocumento =
                            hashArchivo
                    };

                _context
                    .TransferenciaUnidadSeguimientos
                    .Add(seguimiento);

                // =========================================
                // GUARDAR
                // =========================================

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                foreach (var ruta in archivosGuardados)
                {
                    if (File.Exists(ruta))
                    {
                        File.Delete(ruta);
                    }
                }

                throw;
            }
        }





        public async Task<string> ObtenerTextoUnidadVehicular(int unidadVehicularId)
        {
            var unidad =
                await _context.UnidadVehiculars
                    .AsNoTracking()
                    .Where(x =>
                        x.UnidadVehicularId == unidadVehicularId
                        &&
                        x.Activo)
                    .Select(x => new
                    {
                        x.NumeroSerie,
                        x.PlacaActual,
                        Marca =
                            x.Marca.MarcaNombre,
                        Modelo =
                            x.Modelo.ModeloNombre
                    })
                    .FirstOrDefaultAsync();

            if (unidad == null)
            {
                return string.Empty;
            }

            return
                $"{unidad.NumeroSerie} - " +
                $"{unidad.PlacaActual} - " +
                $"{unidad.Marca} {unidad.Modelo}";
        }



        //09102026
        // al parecer en la tabla TallerMovimientoPresupuesto hay comprometido pero en la vista DSP no muestra el DSP comprometido
        //public async Task<DSPViewModel> ObtenerDSP(int solicitudId)
        //{
        //    try
        //    {
        //        // =========================================================
        //        // 1. VALIDAR ID
        //        // =========================================================

        //        if (solicitudId <= 0)
        //        {
        //            throw new Exception(
        //                "WARNING|La solicitud de mantenimiento no es válida.");
        //        }

        //        var fechaActual = DateTime.Now;
        //        var fechaActualDateOnly =
        //            DateOnly.FromDateTime(fechaActual);


        //        // =========================================================
        //        // 2. OBTENER SOLICITUD
        //        // =========================================================

        //        var solicitud =
        //            await _context.SolicitudMantenimientos
        //                .AsNoTracking()
        //                .Include(x => x.AreaSolicitante)
        //                .Include(x => x.Taller)
        //                .Include(x => x.SolicitudMantenimientoDetalles)
        //                    .ThenInclude(x =>
        //                        x.IdObjetoGastoManoObraNavigation)
        //                .Include(x => x.SolicitudMantenimientoDetalles)
        //                    .ThenInclude(x =>
        //                        x.IdObjetoGastoRefaccionesNavigation)
        //                .FirstOrDefaultAsync(x =>
        //                    x.SolicitudMantenimientoId == solicitudId);

        //        if (solicitud == null)
        //        {
        //            throw new Exception(
        //                "WARNING|La solicitud de mantenimiento no fue encontrada.");
        //        }


        //        // =========================================================
        //        // 3. VALIDAR ESTADO Y RELACIONES
        //        // =========================================================

        //        if (solicitud.EstatusSolicitudMantenimientoId == 9)
        //        {
        //            throw new Exception(
        //                "WARNING|La solicitud se encuentra cancelada.");
        //        }

        //        if (solicitud.AreaSolicitante == null)
        //        {
        //            throw new Exception(
        //                "ERROR|La solicitud no tiene un área solicitante válida.");
        //        }

        //        if (!solicitud.TallerId.HasValue ||
        //            solicitud.Taller == null)
        //        {
        //            throw new Exception(
        //                "WARNING|La solicitud no tiene un taller asignado.");
        //        }

        //        var tallerId = solicitud.TallerId.Value;

        //        var estaRechazada =
        //            solicitud.EstatusSolicitudMantenimientoId == 6;

        //        var solicitudDSPAutorizada =
        //            solicitud.EstatusSolicitudMantenimientoId >= 4
        //            && !estaRechazada;

        //        var solicitudPendienteDSP =
        //            solicitud.EstatusSolicitudMantenimientoId < 4;



        //        // =========================================================
        //        // 4. OBTENER CONFIGURACIÓN PRESUPUESTAL ACTIVA DEL TALLER
        //        // =========================================================
        //        //
        //        // TallerConfiguracion:
        //        //   Relaciona el taller con una configuración presupuestal.
        //        //
        //        // TallerConfiguracionPresupuestal:
        //        //   Identifica el contrato y el recurso presupuestal.
        //        //
        //        // No se debe asumir un RecursoId específico.
        //        // =========================================================

        //        var configuracionesTaller = await (
        //            from tc in _context.TallerConfiguracions
        //            join tcp in _context.TallerConfiguracionPresupuestals
        //                on tc.TallerConfiguracionPresupuestalId
        //                equals tcp.TallerConfiguracionPresupuestalId
        //            where tc.TallerId == tallerId
        //                  && tc.Activo
        //                  && tcp.Activo
        //            select tcp
        //        )
        //        .AsNoTracking()
        //        .ToListAsync();

        //        if (configuracionesTaller.Count == 0)
        //        {
        //            throw new Exception(
        //                "WARNING|El taller no tiene una configuración presupuestal activa asociada. Verifique sus contratos y recursos configurados.");
        //        }

        //        // La vista actual utiliza un solo control presupuestal.
        //        // Si hay varias configuraciones activas, no se debe elegir
        //        // una arbitrariamente ni mezclar recursos o contratos.
        //        if (configuracionesTaller.Count > 1)
        //        {
        //            _logger.LogWarning(
        //                "El taller {TallerId} tiene {CantidadConfiguraciones} configuraciones presupuestales activas para la solicitud {SolicitudId}.",
        //                tallerId,
        //                configuracionesTaller.Count,
        //                solicitudId);

        //            throw new Exception(
        //                "WARNING|El taller tiene varias configuraciones presupuestales activas. No es posible determinar automáticamente el contrato y recurso aplicables a esta solicitud. Debe identificarse la configuración correspondiente antes de consultar la DSP.");
        //        }

        //        var configuracionTaller = configuracionesTaller[0];

        //        var tallerConfiguracionPresupuestalId =
        //            configuracionTaller.TallerConfiguracionPresupuestalId;


        //        // =========================================================
        //        // 5. OBTENER DATOS CONTRACTUALES Y UMBRALES
        //        // =========================================================

        //        var montoMinimoTaller =
        //            configuracionTaller.MontoMinimo;

        //        var montoMaximoTaller =
        //            configuracionTaller.MontoMaximo;

        //        var porcentajeAlertaTaller =
        //            configuracionTaller.PorcentajeAlerta;

        //        var montoAlertaTaller =
        //            configuracionTaller.MontoAlerta;

        //        var porcentajeCriticoTaller =
        //            configuracionTaller.PorcentajeCritico;

        //        var montoCriticoTaller =
        //            configuracionTaller.MontoCritico;




        //        // =========================================================
        //        // 6. VALIDAR CONFIGURACIÓN PRESUPUESTAL
        //        // =========================================================

        //        if (montoMaximoTaller <= 0)
        //        {
        //            throw new Exception(
        //                "ERROR|La configuración presupuestal del taller tiene un monto máximo inválido.");
        //        }

        //        if (montoMinimoTaller < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|La configuración presupuestal del taller tiene un monto mínimo inválido.");
        //        }

        //        if (montoMinimoTaller > montoMaximoTaller)
        //        {
        //            throw new Exception(
        //                "ERROR|La configuración presupuestal es inconsistente: el monto mínimo no puede ser mayor que el monto máximo.");
        //        }

        //        if (porcentajeAlertaTaller.HasValue &&
        //            (porcentajeAlertaTaller.Value < 0 ||
        //             porcentajeAlertaTaller.Value > 100))
        //        {
        //            throw new Exception(
        //                "ERROR|El porcentaje de alerta del taller debe estar entre 0 y 100.");
        //        }

        //        if (porcentajeCriticoTaller.HasValue &&
        //            (porcentajeCriticoTaller.Value < 0 ||
        //             porcentajeCriticoTaller.Value > 100))
        //        {
        //            throw new Exception(
        //                "ERROR|El porcentaje crítico del taller debe estar entre 0 y 100.");
        //        }

        //        if (montoAlertaTaller.HasValue &&
        //            montoAlertaTaller.Value < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El monto de alerta del taller no puede ser negativo.");
        //        }

        //        if (montoCriticoTaller.HasValue &&
        //            montoCriticoTaller.Value < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El monto crítico del taller no puede ser negativo.");
        //        }

        //        if (montoAlertaTaller.HasValue &&
        //            montoCriticoTaller.HasValue &&
        //            montoCriticoTaller.Value > montoAlertaTaller.Value)
        //        {
        //            throw new Exception(
        //                "ERROR|La configuración es inconsistente: el monto crítico no puede superar el monto de alerta.");
        //        }




        //        // =========================================================
        //        // 7. OBTENER CONTROL PRESUPUESTAL ANUAL DEL TALLER
        //        // =========================================================

        //        var ejercicioActual = checked((short)fechaActual.Year);

        //        var controlesEjercicio = await _context.TallerControlPresupuestals
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.TallerConfiguracionPresupuestalId ==
        //                    tallerConfiguracionPresupuestalId
        //                && x.Ejercicio == ejercicioActual)
        //            .ToListAsync();

        //        if (controlesEjercicio.Count == 0)
        //        {
        //            throw new Exception(
        //                "WARNING|No existe un control presupuestal para la configuración del taller y el ejercicio actual. Registre el control desde el módulo de Control Presupuestal.");
        //        }

        //        var controlesActivos = controlesEjercicio
        //            .Where(x => x.Activo)
        //            .ToList();

        //        if (controlesActivos.Count == 0)
        //        {
        //            throw new Exception(
        //                "WARNING|Existe un control presupuestal para el ejercicio actual, pero está inactivo. Active o regularice el control antes de continuar.");
        //        }

        //        if (controlesActivos.Count > 1)
        //        {
        //            _logger.LogError(
        //                "Existen {Cantidad} controles presupuestales activos para la configuración {ConfiguracionId} y el ejercicio {Ejercicio}.",
        //                controlesActivos.Count,
        //                tallerConfiguracionPresupuestalId,
        //                ejercicioActual);

        //            throw new Exception(
        //                "ERROR|Existe una inconsistencia: la configuración del taller tiene más de un control presupuestal activo para el mismo ejercicio.");
        //        }

        //        var controlTaller = controlesActivos[0];

        //        if (controlTaller.FechaInicio > controlTaller.FechaFin)
        //        {
        //            throw new Exception(
        //                "ERROR|El periodo de vigencia del control presupuestal del taller es inválido.");
        //        }

        //        if (solicitudPendienteDSP &&
        //            (fechaActualDateOnly < controlTaller.FechaInicio ||
        //             fechaActualDateOnly > controlTaller.FechaFin))
        //        {
        //            throw new Exception(
        //                "WARNING|El control presupuestal del taller no está vigente en la fecha actual.");
        //        }



        //        // =========================================================
        //        // 8. VALIDAR INTEGRIDAD DEL CONTROL
        //        // =========================================================

        //        if (controlTaller.PresupuestoAsignado <= 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El presupuesto asignado al control del taller debe ser mayor que cero.");
        //        }

        //        if (controlTaller.PresupuestoAsignado > montoMaximoTaller)
        //        {
        //            throw new Exception(
        //                "ERROR|El presupuesto asignado al control supera el monto máximo configurado para el taller.");
        //        }

        //        if (controlTaller.PresupuestoComprometido < 0 ||
        //            controlTaller.PresupuestoEjercido < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El control presupuestal contiene importes comprometidos o ejercidos negativos.");
        //        }

        //        var presupuestoTaller =
        //            controlTaller.PresupuestoAsignado;

        //        var comprometidoTaller =
        //            controlTaller.PresupuestoComprometido;

        //        var ejercidoTaller =
        //            controlTaller.PresupuestoEjercido;

        //        var disponibleTaller =
        //            presupuestoTaller
        //            - comprometidoTaller
        //            - ejercidoTaller;

        //        if (disponibleTaller < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El control presupuestal del taller es inconsistente: el comprometido más el ejercido supera el presupuesto asignado.");
        //        }




        //        // =========================================================
        //        // 9. OBTENER DSP DEL ÁREA
        //        // =========================================================

        //        // Historial: incluye DSP inactivas para no perder movimientos
        //        // asociados a solicitudes previamente procesadas.
        //        var dspsArea = await _context.DispPresupuestals
        //            .AsNoTracking()
        //            .Include(x => x.IdObjetoGastoNavigation)
        //            .Include(x => x.IdDspNavigation)
        //            .Where(x => x.IdArea == solicitud.AreaSolicitanteId)
        //            .ToListAsync();

        //        // Disponibilidad actual: solo DSP y catálogos activos.
        //        var dsps = dspsArea
        //            .Where(x =>
        //                x.Activo
        //                && x.IdDspNavigation != null
        //                && x.IdDspNavigation.Activo
        //                && x.IdObjetoGastoNavigation != null
        //                && x.IdObjetoGastoNavigation.Activo)
        //            .ToList();

        //        if (dsps.Any(x => x.Importe < 0))
        //        {
        //            throw new Exception(
        //                "ERROR|Existe una DSP activa con importe negativo. Debe corregirse antes de continuar.");
        //        }

        //        // Las disponibilidades se calculan exclusivamente con DSP activas.
        //        var idsDSPActivas = dsps
        //            .Select(x => x.IdDispPresupuestal)
        //            .Distinct()
        //            .ToList();

        //        // Los movimientos se recuperan de todas las DSP del área.
        //        var idsDSPArea = dspsArea
        //            .Select(x => x.IdDispPresupuestal)
        //            .Distinct()
        //            .ToList();

        //        var movimientosArea = idsDSPArea.Count == 0
        //            ? new List<MovimientoPresupuestal>()
        //            : await _context.MovimientoPresupuestals
        //                .AsNoTracking()
        //                .Where(x =>
        //                    x.Activo == true
        //                    && x.IdDispPresupuestal.HasValue
        //                    && idsDSPArea.Contains(x.IdDispPresupuestal.Value))
        //                .ToListAsync();

        //        var tiposMovimientoValidos = new HashSet<string>(
        //            StringComparer.OrdinalIgnoreCase)
        //                {
        //                    "COMPROMETIDO",
        //                    "DESCOMPROMETIDO",
        //                    "DEVENGADO"
        //                };

        //        if (movimientosArea.Any(x => string.IsNullOrWhiteSpace(x.TipoMovimiento)
        //            || !tiposMovimientoValidos.Contains(x.TipoMovimiento)))
        //        {
        //            throw new Exception(
        //                "ERROR|Existen movimientos presupuestales con un tipo inválido. Deben revisarse antes de continuar.");
        //        }

        //        if (movimientosArea.Any(x => (x.Importe ?? 0) < 0))
        //        {
        //            throw new Exception(
        //                "ERROR|Existen movimientos presupuestales con importes negativos. Deben corregirse antes de continuar.");
        //        }

        //        // Movimientos válidos para calcular disponibilidad actual.
        //        var movimientos = movimientosArea
        //            .Where(x =>
        //                x.IdDispPresupuestal.HasValue
        //                && idsDSPActivas.Contains(x.IdDispPresupuestal.Value))
        //            .ToList();

        //        // Historial completo de movimientos de esta solicitud en el área,
        //        // incluyendo movimientos de DSP que ya no estén activas.
        //        var movimientosHistoricosSolicitud = movimientosArea
        //            .Where(x =>
        //                x.SolicitudMantenimientoId ==
        //                solicitud.SolicitudMantenimientoId)
        //            .ToList();






        //        // =========================================================
        //        // 9.1 VALIDAR INTEGRIDAD DE LAS DISPONIBILIDADES
        //        // =========================================================

        //        if (dsps.Any(x => x.Importe < 0))
        //        {
        //            throw new Exception(
        //                "ERROR|Existe una disponibilidad presupuestal con importe negativo. Debe corregirse el registro antes de consultar la DSP.");
        //        }

        //        // No se rechazan varias DSP para el mismo objeto de gasto.
        //        // Sus importes y movimientos se consolidan posteriormente.


        //        // =========================================================
        //        // 10. OBTENER MOVIMIENTOS ACTIVOS DEL ÁREA
        //        // =========================================================
        //        //
        //        // Se consultan una sola vez para reutilizarlos en los
        //        // cálculos globales, por objeto de gasto y por solicitud.
        //        // =========================================================

        //        var idsDSP = dsps
        //            .Select(x => x.IdDispPresupuestal)
        //            .Distinct()
        //            .ToList();




        //        if (movimientos.Any(x =>
        //                string.IsNullOrWhiteSpace(x.TipoMovimiento)
        //                || !tiposMovimientoValidos.Contains(x.TipoMovimiento)))
        //        {
        //            throw new Exception(
        //                "ERROR|Existen movimientos presupuestales con un tipo inválido. Deben revisarse antes de calcular la disponibilidad.");
        //        }

        //        if (movimientos.Any(x => (x.Importe ?? 0) < 0))
        //        {
        //            throw new Exception(
        //                "ERROR|Existen movimientos presupuestales con importes negativos. Deben corregirse antes de calcular la disponibilidad.");
        //        }


        //        // =========================================================
        //        // 12. CALCULAR DISPONIBILIDAD GLOBAL DEL ÁREA
        //        // =========================================================
        //        //
        //        // Este resumen corresponde al conjunto de disponibilidades
        //        // activas del área, no exclusivamente a un recurso.
        //        //
        //        // Si se necesita disponibilidad por recurso, debe existir
        //        // una relación explícita entre la disponibilidad DSP y
        //        // el recurso presupuestal aplicable.
        //        // =========================================================

        //        var presupuestoAsignado = dsps.Sum(x => x.Importe);

        //        var comprometidoGeneral = movimientos
        //            .Where(x =>
        //                string.Equals(
        //                    x.TipoMovimiento,
        //                    "COMPROMETIDO",
        //                    StringComparison.OrdinalIgnoreCase))
        //            .Sum(x => x.Importe ?? 0);

        //        var descomprometidoGeneral = movimientos
        //            .Where(x =>
        //                string.Equals(
        //                    x.TipoMovimiento,
        //                    "DESCOMPROMETIDO",
        //                    StringComparison.OrdinalIgnoreCase))
        //            .Sum(x => x.Importe ?? 0);

        //        if (descomprometidoGeneral > comprometidoGeneral)
        //        {
        //            throw new Exception(
        //                "ERROR|Los movimientos de liberación del área superan los compromisos registrados. Debe revisarse la integridad de los movimientos presupuestales.");
        //        }

        //        var comprometido =
        //            comprometidoGeneral - descomprometidoGeneral;

        //        var devengado = movimientos
        //            .Where(x =>
        //                string.Equals(
        //                    x.TipoMovimiento,
        //                    "DEVENGADO",
        //                    StringComparison.OrdinalIgnoreCase))
        //            .Sum(x => x.Importe ?? 0);

        //        var disponible =
        //            presupuestoAsignado
        //            - comprometido
        //            - devengado;

        //        if (disponible < 0)
        //        {
        //            throw new Exception(
        //                "ERROR|El presupuesto global del área presenta una inconsistencia: los compromisos y devengados superan el importe asignado.");
        //        }


        //        // =========================================================
        //        // 13. CONSTRUIR PARTIDAS A PARTIR DE LA SOLICITUD
        //        // =========================================================
        //        //
        //        // Agrupar mano de obra y refacciones por objeto de gasto.
        //        // Rechazar importes positivos sin objeto de gasto asociado.
        //        // No sustituir cantidades inválidas por una cantidad ficticia.
        //        // =========================================================

        //        var partidas = new List<DSPPartidaSolicitudViewModel>();

        //        foreach (var detalle in solicitud.SolicitudMantenimientoDetalles)
        //        {
        //            if (detalle.Cantidad <= 0)
        //            {
        //                throw new Exception(
        //                    "ERROR|La solicitud contiene un detalle con cantidad inválida.");
        //            }

        //            if (detalle.CostoEstimadoManoObra < 0 ||
        //                detalle.CostoEstimadoRefacciones < 0)
        //            {
        //                throw new Exception(
        //                    "ERROR|La solicitud contiene un costo estimado negativo.");
        //            }

        //            // -----------------------------------------------------
        //            // MANO DE OBRA
        //            // -----------------------------------------------------

        //            if (detalle.CostoEstimadoManoObra > 0)
        //            {
        //                if (!detalle.IdObjetoGastoManoObra.HasValue)
        //                {
        //                    throw new Exception(
        //                        "WARNING|Existe un importe de mano de obra sin objeto de gasto asociado. Complete la clasificación presupuestal.");
        //                }

        //                var idObjeto = detalle.IdObjetoGastoManoObra.Value;

        //                var partida = partidas.FirstOrDefault(x =>
        //                    x.IdObjetoGasto == idObjeto);

        //                if (partida == null)
        //                {
        //                    partida = new DSPPartidaSolicitudViewModel
        //                    {
        //                        IdObjetoGasto = idObjeto,

        //                        ClaveObjetoGasto =
        //                            detalle.IdObjetoGastoManoObraNavigation
        //                                ?.ClaveObjGasto
        //                                ?? string.Empty,

        //                        ObjetoGastoDescripcion =
        //                            detalle.IdObjetoGastoManoObraNavigation
        //                                ?.Descripcion
        //                                ?? string.Empty
        //                    };

        //                    partidas.Add(partida);
        //                }

        //                partida.ImporteSolicitado +=
        //                    detalle.Cantidad * detalle.CostoEstimadoManoObra;
        //            }


        //            // -----------------------------------------------------
        //            // REFACCIONES
        //            // -----------------------------------------------------

        //            if (detalle.CostoEstimadoRefacciones > 0)
        //            {
        //                if (!detalle.IdObjetoGastoRefacciones.HasValue)
        //                {
        //                    throw new Exception(
        //                        "WARNING|Existe un importe de refacciones sin objeto de gasto asociado. Complete la clasificación presupuestal.");
        //                }

        //                var idObjeto = detalle.IdObjetoGastoRefacciones.Value;

        //                var partida = partidas.FirstOrDefault(x =>
        //                    x.IdObjetoGasto == idObjeto);

        //                if (partida == null)
        //                {
        //                    partida = new DSPPartidaSolicitudViewModel
        //                    {
        //                        IdObjetoGasto = idObjeto,

        //                        ClaveObjetoGasto =
        //                            detalle.IdObjetoGastoRefaccionesNavigation
        //                                ?.ClaveObjGasto
        //                                ?? string.Empty,

        //                        ObjetoGastoDescripcion =
        //                            detalle.IdObjetoGastoRefaccionesNavigation
        //                                ?.Descripcion
        //                                ?? string.Empty
        //                    };

        //                    partidas.Add(partida);
        //                }

        //                partida.ImporteSolicitado +=
        //                    detalle.Cantidad * detalle.CostoEstimadoRefacciones;
        //            }
        //        }


        //        // =========================================================
        //        // 14. VALIDAR PARTIDAS E IMPORTE
        //        // =========================================================

        //        if (partidas.Count == 0)
        //        {
        //            throw new Exception(
        //                "WARNING|La solicitud no contiene partidas presupuestales válidas para procesar la DSP.");
        //        }

        //        if (partidas.Any(x =>
        //                x.ImporteSolicitado <= 0 ||
        //                x.IdObjetoGasto <= 0))
        //        {
        //            throw new Exception(
        //                "ERROR|La solicitud contiene una partida con importe u objeto de gasto inválido.");
        //        }

        //        var importeSolicitud =
        //            partidas.Sum(x => x.ImporteSolicitado);

        //        if (importeSolicitud <= 0)
        //        {
        //            throw new Exception(
        //                "WARNING|El importe presupuestal de la solicitud debe ser mayor que cero.");
        //        }

        //        // =========================================================
        //        // 15. EVALUAR DISPONIBILIDAD POSTERIOR DEL TALLER
        //        // =========================================================

        //        var disponibleTallerPosterior =
        //            disponibleTaller - importeSolicitud;

        //        var tallerTieneDisponibilidad =
        //            disponibleTaller >= importeSolicitud;

        //        // Si no hay suficiencia, el saldo real posterior sería
        //        // negativo. Para evaluar el semáforo visual se muestra
        //        // cero, pero la insuficiencia se conserva por separado.
        //        var saldoParaEvaluacion =
        //            Math.Max(disponibleTallerPosterior, 0);

        //        var tallerEnCritico =
        //            montoCriticoTaller.HasValue
        //            && saldoParaEvaluacion <= montoCriticoTaller.Value;

        //        var tallerEnAlerta =
        //            !tallerEnCritico
        //            && montoAlertaTaller.HasValue
        //            && saldoParaEvaluacion <= montoAlertaTaller.Value;


        //        // =========================================================
        //        // 16. EVALUAR DISPONIBILIDAD POR OBJETO DE GASTO
        //        // =========================================================
        //        //
        //        // Para una solicitud pendiente se calcula la suficiencia
        //        // actual y proyectada.
        //        //
        //        // Para una solicitud histórica se muestran los movimientos
        //        // asociados a la solicitud sin volver a comprometerla.
        //        // =========================================================

        //        foreach (var partida in partidas)
        //        {
        //            var dsp =
        //                dsps.FirstOrDefault(x =>
        //                    x.IdObjetoGasto == partida.IdObjetoGasto);


        //            // -----------------------------------------------------
        //            // VALIDAR EXISTENCIA DE DSP ACTIVAS
        //            // -----------------------------------------------------

        //            var dspsPartida = dsps
        //                .Where(x => x.IdObjetoGasto == partida.IdObjetoGasto)
        //                .ToList();

        //            if (dspsPartida.Count == 0)
        //            {
        //                partida.AutorizadoPorArea = 0;
        //                partida.DisponibleDSP = 0;
        //                partida.DisponiblePosterior = 0;
        //                partida.TieneSuficiencia = false;
        //                partida.DSPDisponibles = new List<SelectListItem>();

        //                continue;
        //            }

        //            var idsDSPPartida = dspsPartida
        //                .Select(x => x.IdDispPresupuestal)
        //                .ToHashSet();

        //            var movimientosPartida = movimientos
        //                .Where(x =>
        //                    x.IdDispPresupuestal.HasValue &&
        //                    idsDSPPartida.Contains(x.IdDispPresupuestal.Value))
        //                .ToList();

        //            var comprometidoPartida = movimientosPartida
        //                .Where(x => string.Equals(
        //                    x.TipoMovimiento, "COMPROMETIDO",
        //                    StringComparison.OrdinalIgnoreCase))
        //                .Sum(x => x.Importe ?? 0);

        //            var descomprometidoPartida = movimientosPartida
        //                .Where(x => string.Equals(
        //                    x.TipoMovimiento, "DESCOMPROMETIDO",
        //                    StringComparison.OrdinalIgnoreCase))
        //                .Sum(x => x.Importe ?? 0);

        //            if (descomprometidoPartida > comprometidoPartida)
        //            {
        //                throw new Exception(
        //                    $"ERROR|Las liberaciones del objeto de gasto {partida.ClaveObjetoGasto} superan los compromisos registrados.");
        //            }

        //            var comprometidoNetoPartida =
        //                comprometidoPartida - descomprometidoPartida;

        //            var devengadoPartida = movimientosPartida
        //                .Where(x => string.Equals(
        //                    x.TipoMovimiento, "DEVENGADO",
        //                    StringComparison.OrdinalIgnoreCase))
        //                .Sum(x => x.Importe ?? 0);

        //            var asignadoObjeto = dspsPartida.Sum(x => x.Importe);

        //            var disponibleObjeto =
        //                asignadoObjeto - comprometidoNetoPartida - devengadoPartida;

        //            if (disponibleObjeto < 0)
        //            {
        //                throw new Exception(
        //                    $"ERROR|La DSP del objeto de gasto {partida.ClaveObjetoGasto} presenta una inconsistencia: los compromisos netos y devengados superan el importe asignado.");
        //            }




        //            // -----------------------------------------------------
        //            // IMPORTE COMPROMETIDO POR ESTA SOLICITUD
        //            // -----------------------------------------------------

        //            var comprometidoSolicitud =
        //                movimientosHistoricosSolicitud
        //                    .Where(x =>
        //                        x.IdDispPresupuestal.HasValue
        //                        && idsDSPPartida.Contains(
        //                            x.IdDispPresupuestal.Value)
        //                        && x.TipoMovimiento == "COMPROMETIDO")
        //                    .Sum(x => x.Importe ?? 0);

        //            var descomprometidoSolicitud =
        //                movimientosHistoricosSolicitud
        //                    .Where(x =>
        //                        x.IdDispPresupuestal.HasValue
        //                        && idsDSPPartida.Contains(
        //                            x.IdDispPresupuestal.Value)
        //                        && x.TipoMovimiento == "DESCOMPROMETIDO")
        //                    .Sum(x => x.Importe ?? 0);

        //            var comprometidoNetoSolicitud =
        //                Math.Max(
        //                    comprometidoSolicitud - descomprometidoSolicitud,
        //                    0);


        //            // -----------------------------------------------------
        //            // IDENTIFICAR EL ÚLTIMO MOVIMIENTO DE ESTA SOLICITUD
        //            // -----------------------------------------------------
        //            var ultimoMovimientoSolicitud = movimientosHistoricosSolicitud
        //                .Where(x =>
        //                    x.IdDispPresupuestal.HasValue
        //                    && idsDSPPartida.Contains(x.IdDispPresupuestal.Value))
        //                .OrderByDescending(x => x.FechaMovimiento)
        //                .ThenByDescending(x => x.IdMovimiento)
        //                .FirstOrDefault();



        //            var estaLiberadaActualmente =
        //                ultimoMovimientoSolicitud?.TipoMovimiento
        //                == "DESCOMPROMETIDO";


        //            // -----------------------------------------------------
        //            // DATOS BASE DE LA PARTIDA
        //            // -----------------------------------------------------

        //            partida.AutorizadoPorArea =
        //                asignadoObjeto;

        //            partida.DisponibleDSP =
        //                disponibleObjeto;

        //            partida.DSPDisponibles = dspsPartida
        //                 .Select(x => new SelectListItem
        //                 {
        //                     Value = x.IdDispPresupuestal.ToString(),
        //                     Text = $"{x.IdDsp} - " +
        //                            $"{x.IdObjetoGastoNavigation?.Descripcion ?? string.Empty}"
        //                 })
        //                 .ToList();


        //            // -----------------------------------------------------
        //            // SOLICITUD RECHAZADA
        //            // -----------------------------------------------------

        //            if (estaRechazada)
        //            {
        //                partida.ImporteSolicitado =
        //                    comprometidoSolicitud;

        //                partida.ImporteLiberado =
        //                    descomprometidoSolicitud;

        //                partida.FueDescomprometida =
        //                    descomprometidoSolicitud > 0;

        //                partida.EstaRechazada = true;

        //                partida.HistorialPresupuestal =
        //                    descomprometidoSolicitud > 0
        //                        ? "Compromiso liberado"
        //                        : "Solicitud rechazada";

        //                // No se valida suficiencia para autorizar una
        //                // solicitud rechazada.
        //                partida.TieneSuficiencia = true;

        //                partida.DisponiblePosterior =
        //                    disponibleObjeto;

        //                partida.LimiteMinimo20Porciento =
        //                    asignadoObjeto * 0.20m;

        //                partida.PorcentajeDisponiblePosterior =
        //                    asignadoObjeto > 0
        //                        ? disponibleObjeto / asignadoObjeto * 100m
        //                        : 0;

        //                partida.ViolaPolitica20Porciento =
        //                    asignadoObjeto > 0
        //                    && disponibleObjeto <=
        //                        partida.LimiteMinimo20Porciento;

        //                continue;
        //            }


        //            // -----------------------------------------------------
        //            // SOLICITUD YA VALIDADA
        //            // -----------------------------------------------------

        //            if (solicitudDSPAutorizada)
        //            {
        //                partida.ImporteSolicitado =
        //                    comprometidoNetoSolicitud;

        //                partida.ImporteLiberado =
        //                    estaLiberadaActualmente
        //                        ? descomprometidoSolicitud
        //                        : 0;

        //                partida.FueDescomprometida =
        //                    estaLiberadaActualmente;

        //                partida.HistorialPresupuestal =
        //                    descomprometidoSolicitud > 0
        //                        ? "Con historial de liberación"
        //                        : "Sin incidencias";

        //                // El compromiso ya existe en los movimientos.
        //                // No volver a restarlo del disponible.
        //                partida.TieneSuficiencia = true;

        //                partida.DisponiblePosterior =
        //                    disponibleObjeto;

        //                partida.LimiteMinimo20Porciento =
        //                    asignadoObjeto * 0.20m;

        //                partida.PorcentajeDisponiblePosterior =
        //                    asignadoObjeto > 0
        //                        ? disponibleObjeto / asignadoObjeto * 100m
        //                        : 0;

        //                partida.ViolaPolitica20Porciento =
        //                    asignadoObjeto > 0
        //                    && disponibleObjeto <=
        //                        partida.LimiteMinimo20Porciento;

        //                continue;
        //            }


        //            // -----------------------------------------------------
        //            // SOLICITUD PENDIENTE DE DSP
        //            // -----------------------------------------------------

        //            var disponiblePosterior =
        //                disponibleObjeto - partida.ImporteSolicitado;

        //            partida.DisponiblePosterior =
        //                disponiblePosterior;

        //            partida.TieneSuficiencia =
        //                partida.ImporteSolicitado <= disponibleObjeto;

        //            partida.LimiteMinimo20Porciento =
        //                asignadoObjeto * 0.20m;

        //            partida.PorcentajeDisponiblePosterior =
        //                asignadoObjeto > 0
        //                    ? disponiblePosterior / asignadoObjeto * 100m
        //                    : 0;

        //            partida.ViolaPolitica20Porciento =
        //                asignadoObjeto > 0
        //                && disponiblePosterior <=
        //                    partida.LimiteMinimo20Porciento;
        //        }


        //        // =========================================================
        //        // 17. POLÍTICA DEL 20 %
        //        // =========================================================

        //        var violaPolitica20 =
        //            partidas.Any(x =>
        //                x.ViolaPolitica20Porciento);


        //        // =========================================================
        //        // 18. RESUMEN DE DISPONIBILIDADES DSP
        //        // =========================================================

        //        var partidasDSP =
        //            dsps
        //                .GroupBy(x => new
        //                {
        //                    x.IdObjetoGasto,
        //                    x.IdObjetoGastoNavigation.ClaveObjGasto,
        //                    x.IdObjetoGastoNavigation.Descripcion
        //                })
        //                .Select(g =>
        //                {
        //                    var asignado =
        //                        g.Sum(x => x.Importe);

        //                    var idsGrupo =
        //                        g.Select(x => x.IdDispPresupuestal)
        //                            .ToList();

        //                    var movimientosGrupo =
        //                        movimientos
        //                            .Where(x =>
        //                                x.IdDispPresupuestal.HasValue
        //                                && idsGrupo.Contains(
        //                                    x.IdDispPresupuestal.Value))
        //                            .ToList();

        //                    var comprometidoGrupo =
        //                        movimientosGrupo
        //                            .Where(x =>
        //                                x.TipoMovimiento == "COMPROMETIDO")
        //                            .Sum(x => x.Importe ?? 0);

        //                    var descomprometidoGrupo =
        //                        movimientosGrupo
        //                            .Where(x =>
        //                                x.TipoMovimiento == "DESCOMPROMETIDO")
        //                            .Sum(x => x.Importe ?? 0);

        //                    var comprometidoNeto =
        //                        Math.Max(
        //                            comprometidoGrupo - descomprometidoGrupo,
        //                            0);

        //                    var devengadoGrupo =
        //                        movimientosGrupo
        //                            .Where(x =>
        //                                x.TipoMovimiento == "DEVENGADO")
        //                            .Sum(x => x.Importe ?? 0);

        //                    var solicitadoSolicitud =
        //                        movimientosHistoricosSolicitud
        //                            .Where(x =>
        //                                x.IdDispPresupuestal.HasValue
        //                                && idsGrupo.Contains(
        //                                    x.IdDispPresupuestal.Value)
        //                                && x.TipoMovimiento == "COMPROMETIDO")
        //                            .Sum(x => x.Importe ?? 0);

        //                    var liberadoSolicitud =
        //                        movimientosHistoricosSolicitud
        //                            .Where(x =>
        //                                x.IdDispPresupuestal.HasValue
        //                                && idsGrupo.Contains(
        //                                    x.IdDispPresupuestal.Value)
        //                                && x.TipoMovimiento == "DESCOMPROMETIDO")
        //                            .Sum(x => x.Importe ?? 0);

        //                    var ultimoMovimiento = movimientosHistoricosSolicitud
        //                            .Where(x => x.IdDispPresupuestal.HasValue
        //                                && idsGrupo.Contains(
        //                                    x.IdDispPresupuestal.Value))
        //                            .OrderByDescending(x => x.FechaMovimiento)
        //                            .FirstOrDefault();

        //                    var idsDSPGrupo = g
        //                    .Select(x => x.IdDispPresupuestal)
        //                    .ToHashSet();

        //                    var ultimoMovimientoSolicitud = movimientosHistoricosSolicitud
        //                        .Where(x =>
        //                            x.IdDispPresupuestal.HasValue
        //                            && idsDSPGrupo.Contains(x.IdDispPresupuestal.Value))
        //                        .OrderByDescending(x => x.FechaMovimiento)
        //                        .ThenByDescending(x => x.IdMovimiento)
        //                        .FirstOrDefault();

        //                    var sigueLiberada = string.Equals(
        //                        ultimoMovimientoSolicitud?.TipoMovimiento,
        //                        "DESCOMPROMETIDO",
        //                        StringComparison.OrdinalIgnoreCase);


        //                    return new DSPDisponibleViewModel
        //                    {
        //                        IdObjetoGasto =
        //                            g.Key.IdObjetoGasto,

        //                        ClaveObjetoGasto =
        //                            g.Key.ClaveObjGasto,

        //                        ObjetoGastoDescripcion =
        //                            g.Key.Descripcion,

        //                        PresupuestoAsignado =
        //                            asignado,

        //                        PresupuestoComprometido =
        //                            comprometidoNeto,

        //                        PresupuestoDevengado =
        //                            devengadoGrupo,

        //                        PresupuestoDisponible =
        //                            asignado
        //                            - comprometidoNeto
        //                            - devengadoGrupo,

        //                        ImporteSolicitado =
        //                            Math.Max(
        //                                solicitadoSolicitud - liberadoSolicitud,
        //                                0),

        //                        ImporteLiberado =
        //                            sigueLiberada
        //                                ? liberadoSolicitud
        //                                : 0,

        //                        FueDescomprometida =
        //                            sigueLiberada
        //                    };
        //                })
        //                .ToList();



        //        if (!solicitud.ImporteCotizacion.HasValue || solicitud.ImporteCotizacion.Value <= 0)
        //        {
        //            throw new Exception("ERROR|La solicitud no tiene un importe de cotización válido.");
        //        }

        //        if (solicitud.ImporteCotizacion.Value != importeSolicitud)
        //        {
        //            _logger.LogWarning("El importe de cotización de la solicitud {SolicitudId} ({ImporteCotizacion}) no coincide con la suma de sus partidas ({ImportePartidas}).",
        //                solicitudId,
        //                solicitud.ImporteCotizacion.Value,
        //                importeSolicitud);
        //        }



        //        // =========================================================
        //        // 19. CONSTRUIR VIEWMODEL
        //        // =========================================================

        //        return new DSPViewModel
        //        {
        //            SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,

        //            Folio =
        //                solicitud.Folio,

        //            AreaId =
        //                solicitud.AreaSolicitanteId,

        //            Area =
        //                solicitud.AreaSolicitante.AreaNombre,

        //            ImporteSolicitud = (decimal)solicitud.ImporteCotizacion,

        //            PresupuestoAsignado =
        //                presupuestoAsignado,

        //            PresupuestoComprometido =
        //                comprometido,

        //            PresupuestoDevengado =
        //                devengado,

        //            PresupuestoDisponible =
        //                disponible,

        //            TieneSuficienciaPresupuestal =
        //                partidas.All(x =>
        //                    x.TieneSuficiencia),

        //            PartidasSolicitud =
        //                partidas,

        //            PartidasDSP =
        //                partidasDSP,

        //            EstatusSolicitudMantenimientoId =
        //                solicitud.EstatusSolicitudMantenimientoId,

        //            ViolaPolitica20Porciento =
        //                violaPolitica20,

        //            // -----------------------------------------------------
        //            // TALLER
        //            // -----------------------------------------------------

        //            TallerId =
        //                solicitud.TallerId,

        //            Taller =
        //                solicitud.Taller.RazonSocial.Trim(),

        //            TallerMinimo =
        //                montoMinimoTaller,

        //            TallerMaximo =
        //                montoMaximoTaller,

        //            PresupuestoTaller =
        //                presupuestoTaller,

        //            ComprometidoTaller =
        //                comprometidoTaller,

        //            EjercidoTaller =
        //                ejercidoTaller,

        //            DisponibleTaller =
        //                disponibleTaller,

        //            PorcentajeAlertaTaller =
        //                porcentajeAlertaTaller,

        //            MontoAlertaTaller =
        //                montoAlertaTaller,

        //            PorcentajeCriticoTaller =
        //                porcentajeCriticoTaller,

        //            MontoCriticoTaller =
        //                montoCriticoTaller,

        //            DisponibleTallerPosterior =
        //                disponibleTallerPosterior,

        //            TieneDisponibilidadTaller =
        //                tallerTieneDisponibilidad,

        //            TallerEnAlerta =
        //                tallerEnAlerta,

        //            TallerEnCritico =
        //                tallerEnCritico,

        //            MontoMinimoTaller =
        //                montoMinimoTaller,

        //            MontoMaximoTaller =
        //                montoMaximoTaller,

        //            TallerDisponible =
        //                disponibleTaller,

        //            TallerAsignado =
        //                presupuestoTaller,

        //            TallerComprometido =
        //                comprometidoTaller,

        //            TallerEjercido =
        //                ejercidoTaller
        //        };
        //    }

        //    catch (OperationCanceledException)
        //    {
        //        // No convertir una cancelación en un error de negocio.
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Conservar los mensajes controlados de negocio.
        //        if (ex.Message.StartsWith(
        //                "WARNING|",
        //                StringComparison.OrdinalIgnoreCase) ||
        //            ex.Message.StartsWith(
        //                "INFO|",
        //                StringComparison.OrdinalIgnoreCase) ||
        //            ex.Message.StartsWith(
        //                "ERROR|",
        //                StringComparison.OrdinalIgnoreCase))
        //        {
        //            throw;
        //        }

        //        // Registrar el detalle técnico únicamente en el servidor.
        //        _logger.LogError(
        //            ex,
        //            "Error técnico al obtener la información presupuestal DSP de la solicitud {SolicitudId}.",
        //            solicitudId);

        //        throw new Exception(
        //            "ERROR|Ocurrió un error al obtener la información presupuestal de la solicitud.");
        //    }


        //}



        public async Task<DSPViewModel> ObtenerDSP(int solicitudId)
        {
            try
            {
                // =========================================================
                // 1. VALIDAR ID
                // =========================================================
                if (solicitudId <= 0)
                {
                    throw new Exception("WARNING|La solicitud de mantenimiento no es válida.");
                }

                var fechaActual = DateTime.Now;
                var fechaActualDateOnly = DateOnly.FromDateTime(fechaActual);

                // =========================================================
                // 2. OBTENER SOLICITUD
                // =========================================================
                var solicitud = await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .Include(x => x.AreaSolicitante)
                    .Include(x => x.Taller)
                    .Include(x => x.SolicitudMantenimientoDetalles)
                        .ThenInclude(x => x.IdObjetoGastoManoObraNavigation)
                    .Include(x => x.SolicitudMantenimientoDetalles)
                        .ThenInclude(x => x.IdObjetoGastoRefaccionesNavigation)
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId);

                if (solicitud == null)
                {
                    throw new Exception("WARNING|La solicitud de mantenimiento no fue encontrada.");
                }

                // =========================================================
                // 3. VALIDAR ESTADO Y RELACIONES
                // =========================================================
                if (solicitud.EstatusSolicitudMantenimientoId == 9)
                {
                    throw new Exception("WARNING|La solicitud se encuentra cancelada.");
                }

                if (solicitud.AreaSolicitante == null)
                {
                    throw new Exception("ERROR|La solicitud no tiene un área solicitante válida.");
                }

                if (!solicitud.TallerId.HasValue || solicitud.Taller == null)
                {
                    throw new Exception("WARNING|La solicitud no tiene un taller asignado.");
                }

                var tallerId = solicitud.TallerId.Value;
                var estaRechazada = solicitud.EstatusSolicitudMantenimientoId == 6;
                var solicitudDSPAutorizada = solicitud.EstatusSolicitudMantenimientoId >= 4 && !estaRechazada;
                var solicitudPendienteDSP = solicitud.EstatusSolicitudMantenimientoId < 4;

                // =========================================================
                // 4. OBTENER CONFIGURACIÓN PRESUPUESTAL VINCULADA
                // =========================================================
                if (!solicitud.TallerConfiguracionPresupuestalId.HasValue || solicitud.TallerConfiguracionPresupuestalId.Value <= 0)
                {
                    throw new Exception("WARNING|La solicitud no tiene una configuración presupuestal / contrato especificado.");
                }

                var tallerConfiguracionPresupuestalId = solicitud.TallerConfiguracionPresupuestalId.Value;

                var configuracionTaller = await _context.TallerConfiguracionPresupuestals
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TallerConfiguracionPresupuestalId == tallerConfiguracionPresupuestalId && x.Activo);

                if (configuracionTaller == null)
                {
                    throw new Exception("WARNING|La configuración presupuestal seleccionada no existe o se encuentra inactiva.");
                }

                // =========================================================
                // 5. OBTENER DATOS CONTRACTUALES Y UMBRALES
                // =========================================================
                var montoMinimoTaller = configuracionTaller.MontoMinimo;
                var montoMaximoTaller = configuracionTaller.MontoMaximo;
                var porcentajeAlertaTaller = configuracionTaller.PorcentajeAlerta;
                var montoAlertaTaller = configuracionTaller.MontoAlerta;
                var porcentajeCriticoTaller = configuracionTaller.PorcentajeCritico;
                var montoCriticoTaller = configuracionTaller.MontoCritico;

                // =========================================================
                // 6. VALIDAR CONFIGURACIÓN PRESUPUESTAL
                // =========================================================
                if (montoMaximoTaller <= 0)
                {
                    throw new Exception("ERROR|La configuración presupuestal del taller tiene un monto máximo inválido.");
                }

                if (montoMinimoTaller < 0)
                {
                    throw new Exception("ERROR|La configuración presupuestal del taller tiene un monto mínimo inválido.");
                }

                if (montoMinimoTaller > montoMaximoTaller)
                {
                    throw new Exception("ERROR|La configuración presupuestal es inconsistente: el monto mínimo no puede ser mayor que el monto máximo.");
                }

                if (porcentajeAlertaTaller.HasValue && (porcentajeAlertaTaller.Value < 0 || porcentajeAlertaTaller.Value > 100))
                {
                    throw new Exception("ERROR|El porcentaje de alerta del taller debe estar entre 0 y 100.");
                }

                if (porcentajeCriticoTaller.HasValue && (porcentajeCriticoTaller.Value < 0 || porcentajeCriticoTaller.Value > 100))
                {
                    throw new Exception("ERROR|El porcentaje crítico del taller debe estar entre 0 y 100.");
                }

                if (montoAlertaTaller.HasValue && montoAlertaTaller.Value < 0)
                {
                    throw new Exception("ERROR|El monto de alerta del taller no puede ser negativo.");
                }

                if (montoCriticoTaller.HasValue && montoCriticoTaller.Value < 0)
                {
                    throw new Exception("ERROR|El monto crítico del taller no puede ser negativo.");
                }

                if (montoAlertaTaller.HasValue && montoCriticoTaller.HasValue && montoCriticoTaller.Value > montoAlertaTaller.Value)
                {
                    throw new Exception("ERROR|La configuración es inconsistente: el monto crítico no puede superar el monto de alerta.");
                }

                // =========================================================
                // 7. OBTENER CONTROL PRESUPUESTAL ANUAL DEL TALLER
                // =========================================================
                var ejercicioActual = checked((short)fechaActual.Year);

                var controlesEjercicio = await _context.TallerControlPresupuestals
                    .AsNoTracking()
                    .Where(x => x.TallerConfiguracionPresupuestalId == tallerConfiguracionPresupuestalId && x.Ejercicio == ejercicioActual)
                    .ToListAsync();

                if (controlesEjercicio.Count == 0)
                {
                    throw new Exception("WARNING|No existe un control presupuestal para la configuración del taller y el ejercicio actual. Registre el control desde el módulo de Control Presupuestal.");
                }

                var controlesActivos = controlesEjercicio.Where(x => x.Activo).ToList();

                if (controlesActivos.Count == 0)
                {
                    throw new Exception("WARNING|Existe un control presupuestal para el ejercicio actual, pero está inactivo. Active o regularice el control antes de continuar.");
                }

                if (controlesActivos.Count > 1)
                {
                    _logger.LogError(
                        "Existen {Cantidad} controles presupuestales activos para la configuración {ConfiguracionId} y el ejercicio {Ejercicio}.",
                        controlesActivos.Count,
                        tallerConfiguracionPresupuestalId,
                        ejercicioActual);

                    throw new Exception("ERROR|Existe una inconsistencia: la configuración del taller tiene más de un control presupuestal activo para el mismo ejercicio.");
                }

                var controlTaller = controlesActivos[0];

                if (controlTaller.FechaInicio > controlTaller.FechaFin)
                {
                    throw new Exception("ERROR|El periodo de vigencia del control presupuestal del taller es inválido.");
                }

                if (solicitudPendienteDSP && (fechaActualDateOnly < controlTaller.FechaInicio || fechaActualDateOnly > controlTaller.FechaFin))
                {
                    throw new Exception("WARNING|El control presupuestal del taller no está vigente en la fecha actual.");
                }

                if (controlTaller.PresupuestoAsignado <= 0)
                {
                    throw new Exception("ERROR|El presupuesto asignado al control del taller debe ser mayor que cero.");
                }

                if (controlTaller.PresupuestoAsignado > montoMaximoTaller)
                {
                    throw new Exception("ERROR|El presupuesto asignado al control supera el monto máximo configurado para el taller.");
                }

                if (controlTaller.PresupuestoComprometido < 0 || controlTaller.PresupuestoEjercido < 0)
                {
                    throw new Exception("ERROR|El control presupuestal contiene importes comprometidos o ejercidos negativos.");
                }

                var presupuestoTaller = controlTaller.PresupuestoAsignado;
                var comprometidoTaller = controlTaller.PresupuestoComprometido;
                var ejercidoTaller = controlTaller.PresupuestoEjercido;

                var disponibleTaller = presupuestoTaller - comprometidoTaller - ejercidoTaller;

                if (disponibleTaller < 0)
                {
                    throw new Exception("ERROR|El control presupuestal del taller es inconsistente: el comprometido más el ejercido supera el presupuesto asignado.");
                }

                // =========================================================
                // 9. OBTENER DSP DEL ÁREA
                // =========================================================
                var dspsArea = await _context.DispPresupuestals
                    .AsNoTracking()
                    .Include(x => x.IdObjetoGastoNavigation)
                    .Include(x => x.IdDspNavigation)
                    .Where(x => x.IdArea == solicitud.AreaSolicitanteId)
                    .ToListAsync();

                var dsps = dspsArea
                    .Where(x => x.Activo
                        && x.IdDspNavigation != null
                        && x.IdDspNavigation.Activo
                        && x.IdObjetoGastoNavigation != null
                        && x.IdObjetoGastoNavigation.Activo)
                    .ToList();

                if (dsps.Any(x => x.Importe < 0))
                {
                    throw new Exception("ERROR|Existe una DSP activa con importe negativo. Debe corregirse antes de continuar.");
                }

                var idsDSPActivas = dsps.Select(x => x.IdDispPresupuestal).Distinct().ToList();
                var idsDSPArea = dspsArea.Select(x => x.IdDispPresupuestal).Distinct().ToList();

                var movimientosArea = idsDSPArea.Count == 0
                    ? new List<MovimientoPresupuestal>()
                    : await _context.MovimientoPresupuestals
                        .AsNoTracking()
                        .Where(x => x.Activo == true
                            && x.IdDispPresupuestal.HasValue
                            && idsDSPArea.Contains(x.IdDispPresupuestal.Value))
                        .ToListAsync();

                var tiposMovimientoValidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "COMPROMETIDO",
                            "DESCOMPROMETIDO",
                            "DEVENGADO"
                        };

                if (movimientosArea.Any(x => string.IsNullOrWhiteSpace(x.TipoMovimiento) || !tiposMovimientoValidos.Contains(x.TipoMovimiento)))
                {
                    throw new Exception("ERROR|Existen movimientos presupuestales con un tipo inválido. Deben revisarse antes de continuar.");
                }

                if (movimientosArea.Any(x => (x.Importe ?? 0) < 0))
                {
                    throw new Exception("ERROR|Existen movimientos presupuestales con importes negativos. Deben corregirse antes de continuar.");
                }

                var movimientos = movimientosArea
                    .Where(x => x.IdDispPresupuestal.HasValue && idsDSPActivas.Contains(x.IdDispPresupuestal.Value))
                    .ToList();

                var movimientosHistoricosSolicitud = movimientosArea
                    .Where(x => x.SolicitudMantenimientoId == solicitud.SolicitudMantenimientoId)
                    .ToList();

                // =========================================================
                // 10. CÁLCULO GENERAL DE DISPONIBILIDAD DEL ÁREA
                // =========================================================
                var presupuestoAsignado = dsps.Sum(x => x.Importe);
                var comprometidoGeneral = movimientos
                    .Where(x => string.Equals(x.TipoMovimiento, "COMPROMETIDO", StringComparison.OrdinalIgnoreCase))
                    .Sum(x => x.Importe ?? 0);

                var descomprometidoGeneral = movimientos
                    .Where(x => string.Equals(x.TipoMovimiento, "DESCOMPROMETIDO", StringComparison.OrdinalIgnoreCase))
                    .Sum(x => x.Importe ?? 0);

                if (descomprometidoGeneral > comprometidoGeneral)
                {
                    throw new Exception("ERROR|Los movimientos de liberación del área superan los compromisos registrados. Debe revisarse la integridad de los movimientos presupuestales.");
                }

                var comprometido = comprometidoGeneral - descomprometidoGeneral;
                var devengado = movimientos
                    .Where(x => string.Equals(x.TipoMovimiento, "DEVENGADO", StringComparison.OrdinalIgnoreCase))
                    .Sum(x => x.Importe ?? 0);

                var disponible = presupuestoAsignado - comprometido - devengado;

                if (disponible < 0)
                {
                    throw new Exception("ERROR|El presupuesto global del área presenta una inconsistencia: los compromisos y devengados superan el importe asignado.");
                }

                // =========================================================
                // 13. CONSTRUIR PARTIDAS A PARTIR DE LA SOLICITUD
                // =========================================================
                var partidas = new List<DSPPartidaSolicitudViewModel>();

                foreach (var detalle in solicitud.SolicitudMantenimientoDetalles)
                {
                    if (detalle.Cantidad <= 0)
                    {
                        throw new Exception("ERROR|La solicitud contiene un detalle con cantidad inválida.");
                    }

                    if (detalle.CostoEstimadoManoObra < 0 || detalle.CostoEstimadoRefacciones < 0)
                    {
                        throw new Exception("ERROR|La solicitud contiene un costo estimado negativo.");
                    }

                    // Mano de obra
                    // Mano de obra
                    if (detalle.CostoEstimadoManoObra > 0)
                    {
                        if (!detalle.IdObjetoGastoManoObra.HasValue)
                        {
                            throw new Exception("WARNING|Existe un importe de mano de obra sin objeto de gasto asociado. Complete la clasificación presupuestal.");
                        }

                        var idObjeto = detalle.IdObjetoGastoManoObra.Value;
                        var partida = partidas.FirstOrDefault(x => x.IdObjetoGasto == idObjeto);

                        if (partida == null)
                        {
                            partida = new DSPPartidaSolicitudViewModel
                            {
                                IdObjetoGasto = idObjeto,
                                ClaveObjetoGasto = detalle.IdObjetoGastoManoObraNavigation?.ClaveObjGasto ?? string.Empty,
                                ObjetoGastoDescripcion = detalle.IdObjetoGastoManoObraNavigation?.Descripcion ?? string.Empty
                            };
                            partidas.Add(partida);
                        }

                        // APLICAR IVA (16%) AL COMPROMISO DE LA PARTIDA PARA QUE COINCIDA CON EL TOTAL GLOBAL
                        var subtotalManoObra = detalle.Cantidad * detalle.CostoEstimadoManoObra;
                        partida.ImporteSolicitado += subtotalManoObra * 1.16m;
                    }

                    // Refacciones
                    if (detalle.CostoEstimadoRefacciones > 0)
                    {
                        if (!detalle.IdObjetoGastoRefacciones.HasValue)
                        {
                            throw new Exception("WARNING|Existe un importe de refacciones sin objeto de gasto asociado. Complete la clasificación presupuestal.");
                        }

                        var idObjeto = detalle.IdObjetoGastoRefacciones.Value;
                        var partida = partidas.FirstOrDefault(x => x.IdObjetoGasto == idObjeto);

                        if (partida == null)
                        {
                            partida = new DSPPartidaSolicitudViewModel
                            {
                                IdObjetoGasto = idObjeto,
                                ClaveObjetoGasto = detalle.IdObjetoGastoRefaccionesNavigation?.ClaveObjGasto ?? string.Empty,
                                ObjetoGastoDescripcion = detalle.IdObjetoGastoRefaccionesNavigation?.Descripcion ?? string.Empty
                            };
                            partidas.Add(partida);
                        }

                        // APLICAR IVA (16%) AL COMPROMISO DE LA PARTIDA PARA QUE COINCIDA CON EL TOTAL GLOBAL
                        var subtotalRefacciones = detalle.Cantidad * detalle.CostoEstimadoRefacciones;
                        partida.ImporteSolicitado += subtotalRefacciones * 1.16m;
                    }
                }

                // =========================================================
                // 14. VALIDAR PARTIDAS E IMPORTE TOTAL SOLICITADO
                // =========================================================
                if (partidas.Count == 0)
                {
                    throw new Exception("WARNING|La solicitud no contiene partidas presupuestales válidas para procesar la DSP.");
                }

                if (partidas.Any(x => x.ImporteSolicitado <= 0 || x.IdObjetoGasto <= 0))
                {
                    throw new Exception("ERROR|La solicitud contiene una partida con importe u objeto de gasto inválido.");
                }

                var importeSolicitud = partidas.Sum(x => x.ImporteSolicitado);

                if (importeSolicitud <= 0)
                {
                    throw new Exception("WARNING|El importe presupuestal de la solicitud debe ser mayor que cero.");
                }

                // =========================================================
                // 15. EVALUAR DISPONIBILIDAD POSTERIOR DEL TALLER
                // =========================================================
                var disponibleTallerPosterior = disponibleTaller - importeSolicitud;

                var tallerTieneDisponibilidad = solicitudDSPAutorizada
                    || estaRechazada
                    || disponibleTaller >= importeSolicitud;

                var saldoParaEvaluacion = Math.Max(disponibleTallerPosterior, 0);

                var tallerEnCritico = montoCriticoTaller.HasValue && saldoParaEvaluacion <= montoCriticoTaller.Value;
                var tallerEnAlerta = !tallerEnCritico && montoAlertaTaller.HasValue && saldoParaEvaluacion <= montoAlertaTaller.Value;

                // =========================================================
                // 16. EVALUAR CADA PARTIDA DE LA SOLICITUD
                // =========================================================
                foreach (var partida in partidas)
                {
                    var dspsPartida = dsps.Where(x => x.IdObjetoGasto == partida.IdObjetoGasto).ToList();

                    if (dspsPartida.Count == 0)
                    {
                        partida.AutorizadoPorArea = 0;
                        partida.DisponibleDSP = 0;
                        partida.DisponiblePosterior = 0;
                        partida.TieneSuficiencia = false;
                        partida.DSPDisponibles = new List<SelectListItem>();
                        continue;
                    }

                    var idsDSPPartida = dspsPartida.Select(x => x.IdDispPresupuestal).ToHashSet();

                    var movimientosPartida = movimientos
                        .Where(x => x.IdDispPresupuestal.HasValue && idsDSPPartida.Contains(x.IdDispPresupuestal.Value))
                        .ToList();

                    var comprometidoPartida = movimientosPartida
                        .Where(x => string.Equals(x.TipoMovimiento, "COMPROMETIDO", StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.Importe ?? 0);

                    var descomprometidoPartida = movimientosPartida
                        .Where(x => string.Equals(x.TipoMovimiento, "DESCOMPROMETIDO", StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.Importe ?? 0);

                    if (descomprometidoPartida > comprometidoPartida)
                    {
                        throw new Exception($"ERROR|Las liberaciones del objeto de gasto {partida.ClaveObjetoGasto} superan los compromisos registrados.");
                    }

                    var comprometidoNetoPartida = comprometidoPartida - descomprometidoPartida;

                    var devengadoPartida = movimientosPartida
                        .Where(x => string.Equals(x.TipoMovimiento, "DEVENGADO", StringComparison.OrdinalIgnoreCase))
                        .Sum(x => x.Importe ?? 0);

                    var asignadoObjeto = dspsPartida.Sum(x => x.Importe);

                    var disponibleObjeto = asignadoObjeto - comprometidoNetoPartida - devengadoPartida;

                    if (disponibleObjeto < 0)
                    {
                        throw new Exception($"ERROR|La DSP del objeto de gasto {partida.ClaveObjetoGasto} presenta una inconsistencia: los compromisos netos y devengados superan el importe asignado.");
                    }

                    var comprometidoSolicitud = movimientosHistoricosSolicitud
                        .Where(x => x.IdDispPresupuestal.HasValue
                            && idsDSPPartida.Contains(x.IdDispPresupuestal.Value)
                            && x.TipoMovimiento == "COMPROMETIDO")
                        .Sum(x => x.Importe ?? 0);

                    var descomprometidoSolicitud = movimientosHistoricosSolicitud
                        .Where(x => x.IdDispPresupuestal.HasValue
                            && idsDSPPartida.Contains(x.IdDispPresupuestal.Value)
                            && x.TipoMovimiento == "DESCOMPROMETIDO")
                        .Sum(x => x.Importe ?? 0);

                    var comprometidoNetoSolicitud = Math.Max(comprometidoSolicitud - descomprometidoSolicitud, 0);

                    var ultimoMovimientoSolicitud = movimientosHistoricosSolicitud
                        .Where(x => x.IdDispPresupuestal.HasValue && idsDSPPartida.Contains(x.IdDispPresupuestal.Value))
                        .OrderByDescending(x => x.FechaMovimiento)
                        .ThenByDescending(x => x.IdMovimiento)
                        .FirstOrDefault();

                    var estaLiberadaActualmente = ultimoMovimientoSolicitud?.TipoMovimiento == "DESCOMPROMETIDO";

                    partida.AutorizadoPorArea = asignadoObjeto;
                    partida.DisponibleDSP = disponibleObjeto;
                    partida.DSPDisponibles = dspsPartida
                        .Select(x => new SelectListItem
                        {
                            Value = x.IdDispPresupuestal.ToString(),
                            Text = $"{x.IdDsp} - {x.IdObjetoGastoNavigation?.Descripcion ?? string.Empty}"
                        })
                        .ToList();

                    if (estaRechazada)
                    {
                        partida.ImporteSolicitado = comprometidoSolicitud;
                        partida.ImporteLiberado = descomprometidoSolicitud;
                        partida.FueDescomprometida = descomprometidoSolicitud > 0;
                        partida.EstaRechazada = true;
                        partida.HistorialPresupuestal = descomprometidoSolicitud > 0 ? "Compromiso liberado" : "Solicitud rechazada";
                        partida.TieneSuficiencia = true;
                        partida.DisponiblePosterior = disponibleObjeto;
                        partida.LimiteMinimo20Porciento = asignadoObjeto * 0.20m;
                        partida.PorcentajeDisponiblePosterior = asignadoObjeto > 0 ? (disponibleObjeto / asignadoObjeto) * 100m : 0;
                        partida.ViolaPolitica20Porciento = asignadoObjeto > 0 && disponibleObjeto <= partida.LimiteMinimo20Porciento;
                        continue;
                    }

                    if (solicitudDSPAutorizada)
                    {
                        partida.ImporteSolicitado = comprometidoNetoSolicitud;
                        partida.ImporteLiberado = estaLiberadaActualmente ? descomprometidoSolicitud : 0;
                        partida.FueDescomprometida = estaLiberadaActualmente;
                        partida.HistorialPresupuestal = descomprometidoSolicitud > 0 ? "Con historial de liberación" : "Sin incidencias";
                        partida.TieneSuficiencia = true;
                        partida.DisponiblePosterior = disponibleObjeto;
                        partida.LimiteMinimo20Porciento = asignadoObjeto * 0.20m;
                        partida.PorcentajeDisponiblePosterior = asignadoObjeto > 0 ? (disponibleObjeto / asignadoObjeto) * 100m : 0;
                        partida.ViolaPolitica20Porciento = asignadoObjeto > 0 && disponibleObjeto <= partida.LimiteMinimo20Porciento;
                        continue;
                    }

                    var disponiblePosterior = disponibleObjeto - partida.ImporteSolicitado;
                    partida.DisponiblePosterior = disponiblePosterior;
                    partida.TieneSuficiencia = partida.ImporteSolicitado <= disponibleObjeto;
                    partida.LimiteMinimo20Porciento = asignadoObjeto * 0.20m;
                    partida.PorcentajeDisponiblePosterior = asignadoObjeto > 0 ? (disponiblePosterior / asignadoObjeto) * 100m : 0;
                    partida.ViolaPolitica20Porciento = asignadoObjeto > 0 && disponiblePosterior <= partida.LimiteMinimo20Porciento;
                }

                // =========================================================
                // 17. POLÍTICA DEL 20 %
                // =========================================================
                var violaPolitica20 = partidas.Any(x => x.ViolaPolitica20Porciento);

                // =========================================================
                // 18. RESUMEN DE DISPONIBILIDADES DSP POR OBJETO DE GASTO
                // =========================================================
                var partidasDSP = dsps
                    .GroupBy(x => new
                    {
                        x.IdObjetoGasto,
                        x.IdObjetoGastoNavigation.ClaveObjGasto,
                        x.IdObjetoGastoNavigation.Descripcion
                    })
                    .Select(g =>
                    {
                        var asignado = g.Sum(x => x.Importe);
                        var idsGrupo = g.Select(x => x.IdDispPresupuestal).ToList();

                        var movimientosGrupo = movimientos
                            .Where(x => x.IdDispPresupuestal.HasValue && idsGrupo.Contains(x.IdDispPresupuestal.Value))
                            .ToList();

                        var comprometidoGrupo = movimientosGrupo
                            .Where(x => x.TipoMovimiento == "COMPROMETIDO")
                            .Sum(x => x.Importe ?? 0);

                        var descomprometidoGrupo = movimientosGrupo
                            .Where(x => x.TipoMovimiento == "DESCOMPROMETIDO")
                            .Sum(x => x.Importe ?? 0);

                        var comprometidoNeto = Math.Max(comprometidoGrupo - descomprometidoGrupo, 0);

                        var devengadoGrupo = movimientosGrupo
                            .Where(x => x.TipoMovimiento == "DEVENGADO")
                            .Sum(x => x.Importe ?? 0);

                        var solicitadoSolicitud = movimientosHistoricosSolicitud
                            .Where(x => x.IdDispPresupuestal.HasValue && idsGrupo.Contains(x.IdDispPresupuestal.Value) && x.TipoMovimiento == "COMPROMETIDO")
                            .Sum(x => x.Importe ?? 0);

                        var liberadoSolicitud = movimientosHistoricosSolicitud
                            .Where(x => x.IdDispPresupuestal.HasValue && idsGrupo.Contains(x.IdDispPresupuestal.Value) && x.TipoMovimiento == "DESCOMPROMETIDO")
                            .Sum(x => x.Importe ?? 0);

                        var idsDSPGrupo = g.Select(x => x.IdDispPresupuestal).ToHashSet();

                        var ultimoMovimientoSolicitud = movimientosHistoricosSolicitud
                            .Where(x => x.IdDispPresupuestal.HasValue && idsDSPGrupo.Contains(x.IdDispPresupuestal.Value))
                            .OrderByDescending(x => x.FechaMovimiento)
                            .ThenByDescending(x => x.IdMovimiento)
                            .FirstOrDefault();

                        var sigueLiberada = string.Equals(
                            ultimoMovimientoSolicitud?.TipoMovimiento,
                            "DESCOMPROMETIDO",
                            StringComparison.OrdinalIgnoreCase);

                        return new DSPDisponibleViewModel
                        {
                            IdObjetoGasto = g.Key.IdObjetoGasto,
                            ClaveObjetoGasto = g.Key.ClaveObjGasto,
                            ObjetoGastoDescripcion = g.Key.Descripcion,
                            PresupuestoAsignado = asignado,
                            PresupuestoComprometido = comprometidoNeto,
                            PresupuestoDevengado = devengadoGrupo,
                            PresupuestoDisponible = asignado - comprometidoNeto - devengadoGrupo,
                            ImporteSolicitado = Math.Max(solicitadoSolicitud - liberadoSolicitud, 0),
                            ImporteLiberado = sigueLiberada ? liberadoSolicitud : 0,
                            FueDescomprometida = sigueLiberada
                        };
                    })
                    .ToList();

                if (!solicitud.ImporteCotizacion.HasValue || solicitud.ImporteCotizacion.Value <= 0)
                {
                    throw new Exception("ERROR|La solicitud no tiene un importe de cotización válido.");
                }

                if (solicitud.ImporteCotizacion.Value != importeSolicitud)
                {
                    _logger.LogWarning(
                        "El importe de cotización de la solicitud {SolicitudId} ({ImporteCotizacion}) no coincide con la suma de sus partidas ({ImportePartidas}).",
                        solicitudId,
                        solicitud.ImporteCotizacion.Value,
                        importeSolicitud);
                }


                // VALIDAR CON USUARIO SI SE PERMITEN DIFERENCIAS MAYORES A $1 PESO
                // Calculamos la diferencia absoluta para tolerar variaciones de centavos por redondeo del IVA
                //var diferencia = Math.Abs(solicitud.ImporteCotizacion.Value - importeSolicitud);
                //if (diferencia > 1.00m) // Tolerancia mayor a 1 peso por posibles redondeos de partidas múltiples
                //{
                //    _logger.LogWarning(
                //        "El importe de cotización de la solicitud {SolicitudId} ({ImporteCotizacion}) difiere de la suma de sus partidas con IVA ({ImportePartidas}).",
                //        solicitudId,
                //        solicitud.ImporteCotizacion.Value,
                //        importeSolicitud);
                //}





                // =========================================================
                // 19. CONSTRUIR VIEWMODEL FINAL
                // =========================================================
                return new DSPViewModel
                {
                    SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                    Folio = solicitud.Folio,
                    AreaId = solicitud.AreaSolicitanteId,
                    Area = solicitud.AreaSolicitante.AreaNombre,
                    ImporteSolicitud = (decimal)solicitud.ImporteCotizacion * 1.16m,
                    PresupuestoAsignado = presupuestoAsignado,
                    PresupuestoComprometido = comprometido,
                    PresupuestoDevengado = devengado,
                    PresupuestoDisponible = disponible,

                    // Suficiencia Presupuestal del Área
                    TieneSuficienciaPresupuestal = partidas.All(x => x.TieneSuficiencia),

                    PartidasSolicitud = partidas,
                    PartidasDSP = partidasDSP,
                    EstatusSolicitudMantenimientoId = solicitud.EstatusSolicitudMantenimientoId,
                    ViolaPolitica20Porciento = violaPolitica20,

                    // Taller
                    TallerId = solicitud.TallerId,
                    Taller = solicitud.Taller.RazonSocial.Trim(),
                    TallerMinimo = montoMinimoTaller,
                    TallerMaximo = montoMaximoTaller,
                    PresupuestoTaller = presupuestoTaller,
                    ComprometidoTaller = comprometidoTaller,
                    EjercidoTaller = ejercidoTaller,
                    DisponibleTaller = disponibleTaller,
                    PorcentajeAlertaTaller = porcentajeAlertaTaller,
                    MontoAlertaTaller = montoAlertaTaller,
                    PorcentajeCriticoTaller = porcentajeCriticoTaller,
                    MontoCriticoTaller = montoCriticoTaller,
                    DisponibleTallerPosterior = disponibleTallerPosterior,

                    // Suficiencia Presupuestal del Taller
                    TieneDisponibilidadTaller = tallerTieneDisponibilidad,

                    TallerEnAlerta = tallerEnAlerta,
                    TallerEnCritico = tallerEnCritico,
                    MontoMinimoTaller = montoMinimoTaller,
                    MontoMaximoTaller = montoMaximoTaller,
                    TallerDisponible = disponibleTaller,
                    TallerAsignado = presupuestoTaller,
                    TallerComprometido = comprometidoTaller,
                    TallerEjercido = ejercidoTaller
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("WARNING|", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.StartsWith("INFO|", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.StartsWith("ERROR|", StringComparison.OrdinalIgnoreCase))
                {
                    throw;
                }

                _logger.LogError(
                    ex, "Error técnico al obtener la información presupuestal DSP de la solicitud {SolicitudId}.", solicitudId);

                throw;
            }
        }





        public async Task<IEnumerable<SelectListItem>> ObtenerDSPDisponibles(int areaId)
        {
            var dsps = await _context.DispPresupuestals
                    .Where(x =>
                        x.IdArea == areaId
                        &&
                        x.Activo)
                    .ToListAsync();

            var movimientos =
                await _context.MovimientoPresupuestals
                    //.Where(x => (bool)x.Activo)
                    .ToListAsync();

            var resultado =
                dsps.Select(dsp =>
                {
                    var comprometido =
                        movimientos
                            .Where(x =>
                                x.IdDispPresupuestal ==
                                    dsp.IdDispPresupuestal
                                &&
                                x.TipoMovimiento ==
                                    "COMPROMETIDO")
                            .Sum(x => x.Importe);

                    var devengado =
                        movimientos
                            .Where(x =>
                                x.IdDispPresupuestal ==
                                    dsp.IdDispPresupuestal
                                &&
                                x.TipoMovimiento ==
                                    "DEVENGADO")
                            .Sum(x => x.Importe);

                    var disponible = dsp.Importe
                        -
                        comprometido
                        -
                        devengado;

                    return new SelectListItem
                    {
                        Value =
                            dsp.IdDispPresupuestal
                                .ToString(),

                        Text =
                            $"DSP:{dsp.IdDsp} | Disponible: {disponible:C}"
                    };
                });

            return resultado;
        }

        //09102026
        //    public async Task<ProcesarDSPResultadoDto> ProcesarDSP(
        //DSPDto dto)
        //    {
        //        // =========================================================
        //        // 1. VALIDACIONES DEL DTO
        //        // =========================================================

        //        if (dto == null)
        //        {
        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso = false,
        //                Warnings =
        //                    new List<string>
        //                    {
        //                "La información de la solicitud no es válida."
        //                    }
        //            };
        //        }


        //        if (dto.SolicitudMantenimientoId <= 0)
        //        {
        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso = false,
        //                Warnings =
        //                    new List<string>
        //                    {
        //                "La solicitud de mantenimiento no es válida."
        //                    }
        //            };
        //        }


        //        if (dto.Partidas == null ||
        //            !dto.Partidas.Any())
        //        {
        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso = false,
        //                Warnings =
        //                    new List<string>
        //                    {
        //                "La solicitud no contiene partidas presupuestales."
        //                    }
        //            };
        //        }


        //        // =========================================================
        //        // 2. FECHA Y USUARIO
        //        // =========================================================

        //        var fechaActual =
        //            DateTime.Now;

        //        var usuarioGuid =
        //            Guid.TryParse(
        //                _httpContextAccessor
        //                    .HttpContext?
        //                    .User?
        //                    .FindFirst(
        //                        ClaimTypes.NameIdentifier)?
        //                    .Value,
        //                out var guid)
        //                    ? guid
        //                    : (Guid?)null;


        //        if (!usuarioGuid.HasValue)
        //        {
        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso = false,
        //                Warnings =
        //                    new List<string>
        //                    {
        //                "No fue posible identificar al usuario que intenta realizar la validación presupuestal."
        //                    }
        //            };
        //        }


        //        var warnings =
        //            new List<string>();


        //        // =========================================================
        //        // 3. TRANSACCIÓN SERIALIZABLE
        //        // =========================================================
        //        //
        //        // Protege el cálculo:
        //        //
        //        // Disponible
        //        //      ↓
        //        // Validación
        //        //      ↓
        //        // Compromiso
        //        //
        //        // contra dos procesos DSP simultáneos.
        //        //
        //        // SQL Server utiliza SERIALIZABLE para evitar modificaciones
        //        // o inserciones concurrentes dentro del rango leído.
        //        // =========================================================

        //        await using var transaction =
        //            await _context.Database
        //                .BeginTransactionAsync(
        //                    System.Data.IsolationLevel.Serializable);


        //        try
        //        {
        //            // =====================================================
        //            // 4. OBTENER SOLICITUD
        //            // =====================================================

        //            var solicitud =
        //                await _context
        //                    .SolicitudMantenimientos
        //                    .Include(x =>
        //                        x.AreaSolicitante)
        //                    .FirstOrDefaultAsync(x =>
        //                        x.SolicitudMantenimientoId ==
        //                        dto.SolicitudMantenimientoId);


        //            if (solicitud == null)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|La solicitud de mantenimiento no fue encontrada.");
        //            }


        //            // =====================================================
        //            // 5. VALIDAR ESTADO
        //            // =====================================================

        //            if (solicitud.EstatusSolicitudMantenimientoId >= 4)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|La solicitud ya cuenta con validación presupuestal y no puede ser procesada nuevamente.");
        //            }


        //            // =====================================================
        //            // 6. VALIDAR TALLER
        //            // =====================================================

        //            if (!solicitud.TallerId.HasValue)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|La solicitud no tiene un taller asignado.");
        //            }


        //            var taller =
        //                await _context
        //                    .Tallers
        //                    .FirstOrDefaultAsync(x =>
        //                        x.TallerId ==
        //                        solicitud.TallerId.Value
        //                        &&
        //                        x.Activo);


        //            if (taller == null)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El taller no existe o se encuentra inactivo.");
        //            }


        //            // =====================================================
        //            // 7. CONFIGURACIÓN PRESUPUESTAL ESTATAL
        //            // =====================================================

        //            const int recursoEstatalId = 1;


        //            var configuracionesTaller =
        //                await (
        //                    from tc in
        //                        _context.TallerConfiguracions

        //                    join tcp in
        //                        _context.TallerConfiguracionPresupuestals

        //                        on tc.TallerConfiguracionPresupuestalId
        //                        equals tcp.TallerConfiguracionPresupuestalId

        //                    where
        //                        tc.TallerId ==
        //                            taller.TallerId

        //                        &&
        //                        tc.Activo

        //                        &&
        //                        tcp.Activo

        //                        &&
        //                        tcp.RecursoId ==
        //                            recursoEstatalId

        //                    select tcp
        //                )
        //                .ToListAsync();


        //            if (!configuracionesTaller.Any())
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El taller no tiene una configuración presupuestal ESTATAL activa.");
        //            }


        //            if (configuracionesTaller.Count > 1)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El taller tiene más de una configuración presupuestal ESTATAL activa. Debe existir una sola configuración activa para procesar la solicitud.");
        //            }


        //            var configuracionTaller =
        //                configuracionesTaller.Single();


        //            var tallerConfiguracionPresupuestalId =
        //                configuracionTaller
        //                    .TallerConfiguracionPresupuestalId;


        //            // =====================================================
        //            // 8. VALIDAR MONTO MÁXIMO
        //            // =====================================================

        //            if (configuracionTaller.MontoMaximo <= 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El taller no tiene un monto máximo presupuestal válido configurado.");
        //            }


        //            // =====================================================
        //            // 9. VALIDAR UMBRALES
        //            // =====================================================

        //            if (configuracionTaller.PorcentajeAlerta.HasValue &&
        //                (
        //                    configuracionTaller.PorcentajeAlerta.Value < 0
        //                    ||
        //                    configuracionTaller.PorcentajeAlerta.Value > 100
        //                ))
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El porcentaje de alerta del taller no es válido.");
        //            }


        //            if (configuracionTaller.PorcentajeCritico.HasValue &&
        //                (
        //                    configuracionTaller.PorcentajeCritico.Value < 0
        //                    ||
        //                    configuracionTaller.PorcentajeCritico.Value > 100
        //                ))
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El porcentaje crítico del taller no es válido.");
        //            }


        //            if (configuracionTaller.MontoAlerta.HasValue &&
        //                configuracionTaller.MontoAlerta.Value < 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El monto de alerta del taller no es válido.");
        //            }


        //            if (configuracionTaller.MontoCritico.HasValue &&
        //                configuracionTaller.MontoCritico.Value < 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El monto crítico del taller no es válido.");
        //            }


        //            if (configuracionTaller.MontoAlerta.HasValue &&
        //                configuracionTaller.MontoCritico.HasValue &&
        //                configuracionTaller.MontoCritico.Value >
        //                configuracionTaller.MontoAlerta.Value)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|La configuración presupuestal del taller no es válida: el monto crítico no puede ser mayor que el monto de alerta.");
        //            }


        //            // =====================================================
        //            // 10. EJERCICIO
        //            // =====================================================

        //            var ejercicio =
        //                (short)fechaActual.Year;


        //            var fechaActualDateOnly =
        //                DateOnly.FromDateTime(
        //                    fechaActual);


        //            // =====================================================
        //            // 11. OBTENER CONTROL ANUAL EXISTENTE
        //            // =====================================================
        //            //
        //            // IMPORTANTE:
        //            //
        //            // DSP NO CREA EL CONTROL.
        //            //
        //            // El control debe haber sido creado y administrado
        //            // previamente por el módulo de Control Presupuestal.
        //            // =====================================================

        //            var control =
        //                await _context
        //                    .TallerControlPresupuestals
        //                    .FirstOrDefaultAsync(x =>
        //                        x.TallerConfiguracionPresupuestalId ==
        //                            tallerConfiguracionPresupuestalId
        //                        &&
        //                        x.Ejercicio ==
        //                            ejercicio);


        //            if (control == null)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|No existe un control presupuestal anual para la configuración del taller y el ejercicio actual. Debe registrarse desde el módulo de Control Presupuestal antes de procesar la DSP.");
        //            }


        //            // =====================================================
        //            // 12. CONTROL ACTIVO
        //            // =====================================================

        //            if (!control.Activo)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal anual correspondiente al taller se encuentra inactivo.");
        //            }


        //            // =====================================================
        //            // 13. VALIDAR PERIODO
        //            // =====================================================

        //            if (control.FechaInicio >
        //                control.FechaFin)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal presenta un periodo de vigencia inválido.");
        //            }


        //            if (fechaActualDateOnly <
        //                control.FechaInicio)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal aún no se encuentra vigente.");
        //            }


        //            if (fechaActualDateOnly >
        //                control.FechaFin)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El periodo de vigencia del control presupuestal ha concluido.");
        //            }


        //            // =====================================================
        //            // 14. VALIDAR INTEGRIDAD DEL CONTROL
        //            // =====================================================

        //            if (control.PresupuestoAsignado <= 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal no tiene un presupuesto asignado válido.");
        //            }


        //            if (control.PresupuestoAsignado >
        //                configuracionTaller.MontoMaximo)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El presupuesto asignado al control presupuestal excede el monto máximo configurado para el taller.");
        //            }


        //            if (control.PresupuestoComprometido < 0 ||
        //                control.PresupuestoEjercido < 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal contiene importes negativos que generan una inconsistencia.");
        //            }


        //            var disponibleCalculadoTaller =
        //                control.PresupuestoAsignado
        //                -
        //                control.PresupuestoComprometido
        //                -
        //                control.PresupuestoEjercido;


        //            if (disponibleCalculadoTaller < 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El control presupuestal presenta una inconsistencia: el presupuesto comprometido más el ejercido supera el presupuesto asignado.");
        //            }


        //            // =====================================================
        //            // 15. DISPONIBILIDAD REAL DEL TALLER
        //            // =====================================================
        //            //
        //            // NO utilizamos PresupuestoDisponible como fuente de
        //            // verdad.
        //            //
        //            // La disponibilidad se calcula.
        //            // =====================================================

        //            var disponibleTaller =
        //                disponibleCalculadoTaller;


        //            // =====================================================
        //            // 16. VALIDAR IMPORTES DE LAS PARTIDAS
        //            // =====================================================

        //            if (dto.Partidas.Any(x =>
        //                x.ImporteSolicitado <= 0))
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|Una o más partidas tienen un importe solicitado inválido.");
        //            }


        //            var importeTotalSolicitud =
        //                dto.Partidas.Sum(
        //                    x =>
        //                        x.ImporteSolicitado);


        //            if (importeTotalSolicitud <= 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|El importe total de la solicitud debe ser mayor a cero.");
        //            }


        //            // =====================================================
        //            // 17. VALIDAR DISPONIBILIDAD DEL TALLER
        //            // =====================================================

        //            if (importeTotalSolicitud >
        //                disponibleTaller)
        //            {
        //                throw new InvalidOperationException(
        //                    $"WARNING|El taller no tiene disponibilidad presupuestal suficiente. Disponible actual: {disponibleTaller:C}; solicitado: {importeTotalSolicitud:C}.");
        //            }


        //            // =====================================================
        //            // 18. DISPONIBILIDAD PROYECTADA DEL TALLER
        //            // =====================================================

        //            var disponiblePosteriorTaller =
        //                disponibleTaller -
        //                importeTotalSolicitud;


        //            // =====================================================
        //            // 19. UMBRAL CRÍTICO DEL TALLER
        //            // =====================================================

        //            if (configuracionTaller.MontoCritico.HasValue &&
        //                disponiblePosteriorTaller <=
        //                configuracionTaller.MontoCritico.Value)
        //            {
        //                warnings.Add(
        //                    $"El taller '{taller.RazonSocial.Trim()}' quedará en nivel crítico después de comprometer la solicitud. Saldo proyectado: {disponiblePosteriorTaller:C}. Umbral crítico: {configuracionTaller.MontoCritico.Value:C}.");
        //            }

        //            // =====================================================
        //            // 20. UMBRAL DE ALERTA DEL TALLER
        //            // =====================================================

        //            else if (
        //                configuracionTaller.MontoAlerta.HasValue &&
        //                disponiblePosteriorTaller <=
        //                configuracionTaller.MontoAlerta.Value)
        //            {
        //                warnings.Add(
        //                    $"El taller '{taller.RazonSocial.Trim()}' quedará en nivel de alerta después de comprometer la solicitud. Saldo proyectado: {disponiblePosteriorTaller:C}. Umbral de alerta: {configuracionTaller.MontoAlerta.Value:C}.");
        //            }


        //            // =====================================================
        //            // 21. OBTENER DSP DEL ÁREA
        //            // =====================================================
        //            //
        //            // Primero obtenemos todos los objetos de gasto solicitados.
        //            // Así evitamos hacer una consulta por cada partida.
        //            // =====================================================

        //            var objetosGasto =
        //                dto.Partidas
        //                    .Select(x =>
        //                        x.IdObjetoGasto)
        //                    .Distinct()
        //                    .ToList();


        //            var dsps =
        //                await _context
        //                    .DispPresupuestals
        //                    .Where(x =>
        //                        x.IdArea ==
        //                            solicitud.AreaSolicitanteId

        //                        &&
        //                        x.Activo

        //                        &&
        //                        objetosGasto.Contains(
        //                            x.IdObjetoGasto))
        //                    .ToListAsync();


        //            // =====================================================
        //            // 22. VALIDAR QUE NO EXISTAN DSP DUPLICADAS
        //            // =====================================================

        //            var dspDuplicadas =
        //                dsps
        //                    .GroupBy(x =>
        //                        x.IdObjetoGasto)
        //                    .Where(g =>
        //                        g.Count() > 1)
        //                    .ToList();


        //            if (dspDuplicadas.Any())
        //            {
        //                var objetos =
        //                    string.Join(
        //                        ", ",
        //                        dspDuplicadas
        //                            .Select(x =>
        //                                x.Key));


        //                throw new InvalidOperationException(
        //                    $"WARNING|El área solicitante tiene más de una DSP activa para los siguientes objetos de gasto: {objetos}. Debe existir una sola DSP activa por objeto de gasto.");
        //            }


        //            // =====================================================
        //            // 23. ÍNDICE DE DSP
        //            // =====================================================

        //            var dspPorObjeto =
        //                dsps.ToDictionary(
        //                    x =>
        //                        x.IdObjetoGasto);


        //            // =====================================================
        //            // 24. OBTENER MOVIMIENTOS DE LAS DSP INVOLUCRADAS
        //            // =====================================================
        //            //
        //            // Ya no cargamos todos los movimientos presupuestales
        //            // de toda la base.
        //            // =====================================================

        //            var idsDSP =
        //                dsps
        //                    .Select(x =>
        //                        x.IdDispPresupuestal)
        //                    .ToList();


        //            var movimientosDSP =
        //                idsDSP.Any()
        //                    ? await _context
        //                        .MovimientoPresupuestals
        //                        .Where(x =>
        //                            x.Activo == true
        //                            &&
        //                            idsDSP.Contains((int)x.IdDispPresupuestal))
        //                        .ToListAsync()
        //                    : new List<MovimientoPresupuestal>();


        //            // =====================================================
        //            // 25. PROYECCIÓN DE COMPROMISOS DEL REQUEST
        //            // =====================================================
        //            //
        //            // Importante:
        //            //
        //            // Si dos partidas del POST corresponden al mismo
        //            // objeto de gasto, la segunda debe considerar lo que
        //            // ya consumió la primera.
        //            // =====================================================

        //            var compromisosProyectados =
        //                new Dictionary<int, decimal>();


        //            // =====================================================
        //            // 26. PROCESAR PARTIDAS DSP
        //            // =====================================================

        //            foreach (var partida in dto.Partidas)
        //            {
        //                // =================================================
        //                // 26.1 OBTENER DSP
        //                // =================================================

        //                if (!dspPorObjeto.TryGetValue(
        //                        partida.IdObjetoGasto,
        //                        out var dsp))
        //                {
        //                    throw new InvalidOperationException(
        //                        $"WARNING|No existe una DSP activa para el objeto de gasto {partida.IdObjetoGasto} del área solicitante.");
        //                }


        //                // =================================================
        //                // 26.2 MOVIMIENTOS DE LA DSP
        //                // =================================================

        //                var movimientosPartida =
        //                    movimientosDSP
        //                        .Where(x =>
        //                            x.IdDispPresupuestal ==
        //                            dsp.IdDispPresupuestal)
        //                        .ToList();


        //                // =================================================
        //                // 26.3 COMPROMETIDO
        //                // =================================================

        //                var comprometidoDSP =
        //                    movimientosPartida
        //                        .Where(x =>
        //                            x.TipoMovimiento ==
        //                            "COMPROMETIDO")
        //                        .Sum(x =>
        //                            x.Importe ?? 0);


        //                // =================================================
        //                // 26.4 DESCOMPROMETIDO
        //                // =================================================

        //                var descomprometidoDSP =
        //                    movimientosPartida
        //                        .Where(x =>
        //                            x.TipoMovimiento ==
        //                            "DESCOMPROMETIDO")
        //                        .Sum(x =>
        //                            x.Importe ?? 0);


        //                // =================================================
        //                // 26.5 COMPROMETIDO NETO
        //                // =================================================

        //                var comprometidoNetoDSP =
        //                    Math.Max(
        //                        comprometidoDSP -
        //                        descomprometidoDSP,
        //                        0);


        //                // =================================================
        //                // 26.6 DEVENGADO
        //                // =================================================

        //                var devengadoDSP =
        //                    movimientosPartida
        //                        .Where(x =>
        //                            x.TipoMovimiento ==
        //                            "DEVENGADO")
        //                        .Sum(x =>
        //                            x.Importe ?? 0);


        //                // =================================================
        //                // 26.7 COMPROMISOS PROYECTADOS
        //                // =================================================

        //                var compromisoYaProyectado =
        //                    compromisosProyectados.TryGetValue(
        //                        dsp.IdDispPresupuestal,
        //                        out var acumulado)
        //                            ? acumulado
        //                            : 0;


        //                // =================================================
        //                // 26.8 DISPONIBLE BASE DSP
        //                // =================================================

        //                var disponibleBaseDSP =
        //                    dsp.Importe
        //                    -
        //                    comprometidoNetoDSP
        //                    -
        //                    devengadoDSP;


        //                if (disponibleBaseDSP < 0)
        //                {
        //                    throw new InvalidOperationException(
        //                        $"WARNING|La DSP {partida.ClaveObjetoGasto ?? partida.IdObjetoGasto.ToString()} presenta una inconsistencia presupuestal: sus compromisos y devengados superan el importe disponible.");
        //                }


        //                // =================================================
        //                // 26.9 DISPONIBLE REAL PARA ESTA PARTIDA
        //                // =================================================

        //                var disponibleDSP =
        //                    disponibleBaseDSP -
        //                    compromisoYaProyectado;


        //                if (disponibleDSP < 0)
        //                {
        //                    disponibleDSP = 0;
        //                }


        //                // =================================================
        //                // 26.10 VALIDAR SUFICIENCIA
        //                // =================================================

        //                if (partida.ImporteSolicitado >
        //                    disponibleDSP)
        //                {
        //                    throw new InvalidOperationException(
        //                        $"WARNING|No existe suficiencia presupuestal para la partida {partida.ClaveObjetoGasto ?? partida.IdObjetoGasto.ToString()}. Disponible real considerando la proyección de esta solicitud: {disponibleDSP:C}; solicitado: {partida.ImporteSolicitado:C}.");
        //                }


        //                // =================================================
        //                // 26.11 DISPONIBILIDAD PROYECTADA
        //                // =================================================

        //                var disponiblePosteriorDSP =
        //                    disponibleDSP -
        //                    partida.ImporteSolicitado;


        //                // =================================================
        //                // 26.12 UMBRAL 20 %
        //                // =================================================

        //                var limite20 =
        //                    dsp.Importe *
        //                    0.20m;


        //                if (disponiblePosteriorDSP <=
        //                    limite20)
        //                {
        //                    var porcentaje =
        //                        dsp.Importe == 0
        //                            ? 0
        //                            : Math.Round(
        //                                (
        //                                    disponiblePosteriorDSP /
        //                                    dsp.Importe
        //                                ) * 100m,
        //                                2);


        //                    warnings.Add(
        //                        $"La partida {partida.ClaveObjetoGasto ?? partida.IdObjetoGasto.ToString()} quedará por debajo del umbral recomendado del 20%. Disponible proyectado: {disponiblePosteriorDSP:C} ({porcentaje}%). Se recomienda gestionar ajuste presupuestal.");
        //                }


        //                // =================================================
        //                // 26.13 ACUMULAR PROYECCIÓN
        //                // =================================================

        //                compromisosProyectados[
        //                    dsp.IdDispPresupuestal] =
        //                        compromisoYaProyectado +
        //                        partida.ImporteSolicitado;


        //                // =================================================
        //                // 26.14 REGISTRAR COMPROMISO DSP
        //                // =================================================

        //                _context
        //                    .MovimientoPresupuestals
        //                    .Add(
        //                        new MovimientoPresupuestal
        //                        {
        //                            SolicitudMantenimientoId =
        //                                solicitud
        //                                    .SolicitudMantenimientoId,

        //                            IdDispPresupuestal =
        //                                dsp.IdDispPresupuestal,

        //                            TipoMovimiento =
        //                                "COMPROMETIDO",

        //                            Importe =
        //                                partida.ImporteSolicitado,

        //                            Observaciones =
        //                                dto.Observaciones,

        //                            FechaMovimiento =
        //                                fechaActual,

        //                            FechaCaptura =
        //                                fechaActual,

        //                            UsuarioCaptura =
        //                                usuarioGuid,

        //                            Activo =
        //                                true
        //                        });
        //            }


        //            // =====================================================
        //            // 27. REGISTRAR MOVIMIENTO DEL TALLER
        //            // =====================================================

        //            _context
        //                .TallerMovimientoPresupuestos
        //                .Add(
        //                    new TallerMovimientoPresupuesto
        //                    {
        //                        TallerId =
        //                            taller.TallerId,

        //                        IdControlTaller =
        //                            control.IdControlTaller,

        //                        SolicitudMantenimientoId =
        //                            solicitud
        //                                .SolicitudMantenimientoId,

        //                        TipoMovimiento =
        //                            "COMPROMETIDO",

        //                        Importe =
        //                            importeTotalSolicitud,

        //                        FechaMovimiento =
        //                            fechaActual,

        //                        UsuarioCaptura =
        //                            usuarioGuid,

        //                        Activo =
        //                            true
        //                    });


        //            // =====================================================
        //            // 28. ACTUALIZAR CONTROL DEL TALLER
        //            // =====================================================

        //            control.PresupuestoComprometido +=
        //                importeTotalSolicitud;


        //            var nuevoDisponibleTaller =
        //                control.PresupuestoAsignado
        //                -
        //                control.PresupuestoComprometido
        //                -
        //                control.PresupuestoEjercido;


        //            // =====================================================
        //            // 29. VALIDACIÓN FINAL DEL CONTROL
        //            // =====================================================

        //            if (nuevoDisponibleTaller < 0)
        //            {
        //                throw new InvalidOperationException(
        //                    "WARNING|La operación produciría una inconsistencia en el control presupuestal del taller.");
        //            }


        //            control.PresupuestoDisponible =
        //                nuevoDisponibleTaller;


        //            control.FechaActualizacion =
        //                fechaActual;


        //            // =====================================================
        //            // 30. ACTUALIZAR SOLICITUD
        //            // =====================================================

        //            solicitud.EstatusSolicitudMantenimientoId =
        //                4;


        //            solicitud.FechaAutorizacion =
        //                fechaActual;


        //            solicitud.UsuarioAutorizaId =
        //                usuarioGuid;


        //            // =====================================================
        //            // 31. REGISTRAR SEGUIMIENTO
        //            // =====================================================

        //            _context
        //                .SolicitudMantenimientoSeguimientos
        //                .Add(
        //                    new SolicitudMantenimientoSeguimiento
        //                    {
        //                        SolicitudMantenimientoId =
        //                            solicitud
        //                                .SolicitudMantenimientoId,

        //                        EstatusSolicitudMantenimientoId =
        //                            4,

        //                        Observaciones =
        //                            warnings.Any()
        //                                ? string.Join(
        //                                    Environment.NewLine,
        //                                    warnings)
        //                                : "Solicitud validada DSP",

        //                        FechaMovimiento =
        //                            fechaActual,

        //                        UsuarioId =
        //                            usuarioGuid
        //                    });


        //            // =====================================================
        //            // 32. GUARDAR TODO COMO UNA SOLA OPERACIÓN
        //            // =====================================================

        //            await _context.SaveChangesAsync();


        //            // =====================================================
        //            // 33. CONFIRMAR TRANSACCIÓN
        //            // =====================================================

        //            await transaction.CommitAsync();


        //            // =====================================================
        //            // 34. RESULTADO EXITOSO
        //            // =====================================================

        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso =
        //                    true,

        //                Warnings =
        //                    warnings
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            // =====================================================
        //            // 35. ROLLBACK
        //            // =====================================================

        //            try
        //            {
        //                await transaction.RollbackAsync();
        //            }
        //            catch
        //            {
        //                // -------------------------------------------------
        //                // No sustituir el mensaje original por un error
        //                // secundario ocurrido durante el rollback.
        //                // -------------------------------------------------
        //            }


        //            // =====================================================
        //            // 36. IDENTIFICAR MENSAJE CONTROLADO
        //            // =====================================================

        //            var mensaje =
        //                ex.Message;

        //            var tipo =
        //                "ERROR";


        //            var separador =
        //                mensaje.IndexOf('|');


        //            if (separador > 0)
        //            {
        //                var posibleTipo =
        //                    mensaje
        //                        .Substring(
        //                            0,
        //                            separador)
        //                        .Trim()
        //                        .ToUpperInvariant();


        //                if (posibleTipo == "WARNING" ||
        //                    posibleTipo == "INFO" ||
        //                    posibleTipo == "ERROR")
        //                {
        //                    tipo =
        //                        posibleTipo;

        //                    mensaje =
        //                        mensaje.Substring(
        //                            separador + 1)
        //                        .Trim();
        //                }
        //            }


        //            // =====================================================
        //            // 37. NORMALIZAR MENSAJE
        //            // =====================================================

        //            if (string.IsNullOrWhiteSpace(mensaje))
        //            {
        //                mensaje =
        //                    "No fue posible procesar la validación presupuestal de la solicitud.";
        //            }


        //            // =====================================================
        //            // 38. NO EXPONER ERROR TÉCNICO
        //            // =====================================================

        //            if (tipo == "ERROR")
        //            {
        //                mensaje =
        //                    "No fue posible procesar la validación presupuestal de la solicitud.";
        //            }


        //            // =====================================================
        //            // 39. RESULTADO CONTROLADO
        //            // =====================================================

        //            return new ProcesarDSPResultadoDto
        //            {
        //                Exitoso = false,
        //                Warnings = new List<string> {
        //                mensaje }
        //            };
        //        }
        //    }





        public async Task<ProcesarDSPResultadoDto> ProcesarDSP(DSPDto dto)
        {
            // =====================================================
            // 1. VALIDACIONES INICIALES
            // =====================================================
            if (dto == null)
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "La información de la solicitud no es válida." }
                };
            }

            if (dto.SolicitudMantenimientoId <= 0)
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "La solicitud de mantenimiento no es válida." }
                };
            }

            if (dto.Partidas == null || !dto.Partidas.Any())
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "La solicitud no contiene partidas presupuestales." }
                };
            }

            if (dto.Partidas.Any(x => x == null || x.IdObjetoGasto <= 0 || x.ImporteSolicitado <= 0))
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "Una o más partidas contienen identificadores o importes inválidos." }
                };
            }

            // =====================================================
            // 2. AGRUPAR PARTIDAS REPETIDAS
            // =====================================================
            var partidas = dto.Partidas
                .GroupBy(x => x.IdObjetoGasto)
                .Select(g => new DSPPartidaDto
                {
                    IdObjetoGasto = g.Key,
                    ImporteSolicitado = g.Sum(x => x.ImporteSolicitado),
                    ClaveObjetoGasto = g.Select(x => x.ClaveObjetoGasto)
                        .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                })
                .ToList();

            if (partidas.Any(x => x.ImporteSolicitado <= 0))
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "El importe acumulado de una o más partidas no es válido." }
                };
            }

            var importeTotalSolicitud = partidas.Sum(x => x.ImporteSolicitado);

            if (importeTotalSolicitud <= 0)
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "El importe total de la solicitud debe ser mayor a cero." }
                };
            }

            // =====================================================
            // 3. FECHA Y USUARIO AUTENTICADO
            // =====================================================
            var fechaActual = DateTime.Now;

            var usuarioGuid = Guid.TryParse(
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var guid) ? guid : (Guid?)null;

            if (!usuarioGuid.HasValue)
            {
                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { "No fue posible identificar al usuario que intenta realizar la validación presupuestal." }
                };
            }

            var warnings = new List<string>();

            // =====================================================
            // 4. TRANSACCIÓN ATÓMICA
            // =====================================================
            await using var transaction = await _context.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable);

            try
            {
                // =====================================================
                // 5. OBTENER SOLICITUD
                // =====================================================
                var solicitud = await _context.SolicitudMantenimientos
                    .Include(x => x.AreaSolicitante)
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == dto.SolicitudMantenimientoId);

                if (solicitud == null)
                {
                    throw new InvalidOperationException("WARNING|La solicitud de mantenimiento no fue encontrada.");
                }

                // =====================================================
                // 6. VALIDAR ESTADO ACTUAL
                // =====================================================
                if (solicitud.EstatusSolicitudMantenimientoId >= 4)
                {
                    throw new InvalidOperationException("WARNING|La solicitud ya cuenta con validación presupuestal y no puede procesarse nuevamente.");
                }

                if (solicitud.EstatusSolicitudMantenimientoId == 9)
                {
                    throw new InvalidOperationException("WARNING|La solicitud está cancelada y no puede procesarse.");
                }

                if (solicitud.AreaSolicitanteId <= 0)
                {
                    throw new InvalidOperationException("WARNING|La solicitud no tiene un área solicitante válida.");
                }

                // =====================================================
                // 7. VALIDAR TALLER
                // =====================================================
                if (!solicitud.TallerId.HasValue)
                {
                    throw new InvalidOperationException("WARNING|La solicitud no tiene un taller asignado.");
                }

                var taller = await _context.Tallers
                    .FirstOrDefaultAsync(x => x.TallerId == solicitud.TallerId.Value && x.Activo);

                if (taller == null)
                {
                    throw new InvalidOperationException("WARNING|El taller no existe o se encuentra inactivo.");
                }

                // =====================================================
                // 8. CONFIGURACIÓN PRESUPUESTAL DEL TALLER
                // =====================================================
                if (!solicitud.TallerConfiguracionPresupuestalId.HasValue || solicitud.TallerConfiguracionPresupuestalId.Value <= 0)
                {
                    throw new InvalidOperationException("WARNING|La solicitud no tiene una configuración presupuestal / contrato asignado.");
                }

                var configuracionId = solicitud.TallerConfiguracionPresupuestalId.Value;

                var configuracionTaller = await _context.TallerConfiguracionPresupuestals
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TallerConfiguracionPresupuestalId == configuracionId && x.Activo);

                if (configuracionTaller == null)
                {
                    throw new InvalidOperationException("WARNING|La configuración presupuestal asociada a la solicitud no existe o se encuentra inactiva.");
                }

                // =====================================================
                // 9. VALIDAR LÍMITES DE LA CONFIGURACIÓN DEL TALLER
                // =====================================================
                if (configuracionTaller.MontoMinimo < 0 ||
                    configuracionTaller.MontoMaximo <= 0 ||
                    configuracionTaller.MontoMinimo > configuracionTaller.MontoMaximo)
                {
                    throw new InvalidOperationException("WARNING|La configuración presupuestal del taller contiene montos mínimos o máximos inválidos.");
                }

                if (configuracionTaller.PorcentajeAlerta.HasValue &&
                    (configuracionTaller.PorcentajeAlerta.Value < 0 || configuracionTaller.PorcentajeAlerta.Value > 100))
                {
                    throw new InvalidOperationException("WARNING|El porcentaje de alerta del taller debe estar entre 0 y 100.");
                }

                if (configuracionTaller.PorcentajeCritico.HasValue &&
                    (configuracionTaller.PorcentajeCritico.Value < 0 || configuracionTaller.PorcentajeCritico.Value > 100))
                {
                    throw new InvalidOperationException("WARNING|El porcentaje crítico del taller debe estar entre 0 y 100.");
                }

                if (configuracionTaller.MontoAlerta.HasValue && configuracionTaller.MontoAlerta.Value < 0)
                {
                    throw new InvalidOperationException("WARNING|El monto de alerta del taller no puede ser negativo.");
                }

                if (configuracionTaller.MontoCritico.HasValue && configuracionTaller.MontoCritico.Value < 0)
                {
                    throw new InvalidOperationException("WARNING|El monto crítico del taller no puede ser negativo.");
                }

                if (configuracionTaller.MontoAlerta.HasValue &&
                    configuracionTaller.MontoCritico.HasValue &&
                    configuracionTaller.MontoCritico.Value > configuracionTaller.MontoAlerta.Value)
                {
                    throw new InvalidOperationException("WARNING|El monto crítico no puede ser mayor que el monto de alerta.");
                }

                // =====================================================
                // 10. OBTENER CONTROL ANUAL DEL TALLER
                // =====================================================
                var ejercicio = (short)fechaActual.Year;
                var fechaActualDateOnly = DateOnly.FromDateTime(fechaActual);

                var control = await _context.TallerControlPresupuestals
                    .FirstOrDefaultAsync(x => x.TallerConfiguracionPresupuestalId == configuracionId && x.Ejercicio == ejercicio);

                if (control == null)
                {
                    throw new InvalidOperationException("WARNING|No existe un control presupuestal anual para la configuración del taller y el ejercicio actual. Debe registrarse desde el módulo de Control Presupuestal antes de procesar la DSP.");
                }

                if (!control.Activo)
                {
                    throw new InvalidOperationException("WARNING|El control presupuestal anual del taller se encuentra inactivo.");
                }

                if (control.FechaInicio > control.FechaFin)
                {
                    throw new InvalidOperationException("WARNING|El control presupuestal presenta un periodo de vigencia inválido.");
                }

                if (fechaActualDateOnly < control.FechaInicio)
                {
                    throw new InvalidOperationException("WARNING|El control presupuestal aún no se encuentra vigente.");
                }

                if (fechaActualDateOnly > control.FechaFin)
                {
                    throw new InvalidOperationException("WARNING|El periodo de vigencia del control presupuestal ha concluido.");
                }

                // =====================================================
                // 11. VALIDAR INTEGRIDAD Y SUFICIENCIA DEL TALLER
                // =====================================================
                if (control.PresupuestoAsignado <= 0)
                {
                    throw new InvalidOperationException("WARNING|El control presupuestal no tiene un presupuesto asignado válido.");
                }

                if (control.PresupuestoAsignado > configuracionTaller.MontoMaximo)
                {
                    throw new InvalidOperationException("WARNING|El presupuesto asignado supera el monto máximo configurado para el taller.");
                }

                if (control.PresupuestoComprometido < 0 || control.PresupuestoEjercido < 0)
                {
                    throw new InvalidOperationException("WARNING|El control presupuestal contiene importes negativos.");
                }

                var disponibleTaller = control.PresupuestoAsignado - control.PresupuestoComprometido - control.PresupuestoEjercido;

                if (disponibleTaller < 0)
                {
                    throw new InvalidOperationException("WARNING|El comprometido más el ejercido supera el presupuesto asignado al taller.");
                }

                if (importeTotalSolicitud > disponibleTaller)
                {
                    throw new InvalidOperationException($"WARNING|El taller no tiene disponibilidad presupuestal suficiente. Disponible actual: {disponibleTaller:C}; solicitado: {importeTotalSolicitud:C}.");
                }

                var disponiblePosteriorTaller = disponibleTaller - importeTotalSolicitud;

                // =====================================================
                // 13. EVALUAR UMBRALES DE ADVERTENCIA Y BLOQUEO CRÍTICO DEL TALLER
                // =====================================================
                if (configuracionTaller.MontoCritico.HasValue && disponiblePosteriorTaller <= configuracionTaller.MontoCritico.Value)
                {
                    throw new InvalidOperationException(
                        $"WARNING|El taller '{taller.RazonSocial.Trim()}' quedará en nivel crítico presupuestal. Saldo proyectado: {disponiblePosteriorTaller:C}; umbral crítico: {configuracionTaller.MontoCritico.Value:C}. No es posible procesar la DSP.");
                }

                
                if (configuracionTaller.MontoAlerta.HasValue && disponiblePosteriorTaller <= configuracionTaller.MontoAlerta.Value)
                {
                    warnings.Add($"El taller '{taller.RazonSocial.Trim()}' quedará en nivel de alerta. Saldo proyectado: {disponiblePosteriorTaller:C}; umbral de alerta: {configuracionTaller.MontoAlerta.Value:C}.");
                }

                // =====================================================
                // 14. OBTENER DSP ACTIVAS DEL ÁREA
                // =====================================================
                var objetosGasto = partidas.Select(x => x.IdObjetoGasto).Distinct().ToList();

                var dsps = await _context.DispPresupuestals
                    .Where(x => x.IdArea == solicitud.AreaSolicitanteId && x.Activo && objetosGasto.Contains(x.IdObjetoGasto))
                    .ToListAsync();

                var dspDuplicadas = dsps
                    .GroupBy(x => x.IdObjetoGasto)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (dspDuplicadas.Any())
                {
                    throw new InvalidOperationException("WARNING|El área solicitante tiene más de una DSP activa para los objetos de gasto: " + string.Join(", ", dspDuplicadas) + ". Debe existir una sola DSP activa por objeto de gasto.");
                }

                var dspPorObjeto = dsps.ToDictionary(x => x.IdObjetoGasto);

                // =====================================================
                // 15. VALIDAR EXISTENCIA DE TODAS LAS DSP
                // =====================================================
                var objetosSinDSP = objetosGasto.Where(id => !dspPorObjeto.ContainsKey(id)).ToList();

                if (objetosSinDSP.Any())
                {
                    throw new InvalidOperationException("WARNING|No existe una DSP activa para los siguientes objetos de gasto del área solicitante: " + string.Join(", ", objetosSinDSP) + ".");
                }

                // =====================================================
                // 16. OBTENER MOVIMIENTOS DE LAS DSP INVOLUCRADAS
                // =====================================================
                var idsDSP = dsps.Select(x => x.IdDispPresupuestal).ToList();

                var movimientosDSP = idsDSP.Any()
                    ? await _context.MovimientoPresupuestals
                        .Where(x => x.Activo == true && idsDSP.Contains((int)x.IdDispPresupuestal))
                        .ToListAsync()
                    : new List<MovimientoPresupuestal>();

                // =====================================================
                // 17. VALIDAR Y REGISTRAR COMPROMISOS POR PARTIDA (ÁREA)
                // =====================================================
                foreach (var partida in partidas)
                {
                    var dsp = dspPorObjeto[partida.IdObjetoGasto];

                    var movimientosPartida = movimientosDSP
                        .Where(x => x.IdDispPresupuestal == dsp.IdDispPresupuestal)
                        .ToList();

                    var comprometidoDSP = movimientosPartida
                        .Where(x => x.TipoMovimiento == "COMPROMETIDO")
                        .Sum(x => x.Importe ?? 0m);

                    var descomprometidoDSP = movimientosPartida
                        .Where(x => x.TipoMovimiento == "DESCOMPROMETIDO")
                        .Sum(x => x.Importe ?? 0m);

                    var devengadoDSP = movimientosPartida
                        .Where(x => x.TipoMovimiento == "DEVENGADO")
                        .Sum(x => x.Importe ?? 0m);

                    if (comprometidoDSP < 0 || descomprometidoDSP < 0 || devengadoDSP < 0)
                    {
                        throw new InvalidOperationException($"WARNING|La DSP del objeto de gasto {partida.IdObjetoGasto} contiene movimientos con importes negativos.");
                    }

                    var comprometidoNetoDSP = comprometidoDSP - descomprometidoDSP;

                    if (comprometidoNetoDSP < 0)
                    {
                        throw new InvalidOperationException($"WARNING|La DSP del objeto de gasto {partida.IdObjetoGasto} presenta una inconsistencia: los descompromisos superan los compromisos.");
                    }

                    var disponibleBaseDSP = dsp.Importe - comprometidoNetoDSP - devengadoDSP;

                    if (disponibleBaseDSP < 0)
                    {
                        throw new InvalidOperationException($"WARNING|La DSP del objeto de gasto {partida.IdObjetoGasto} presenta una inconsistencia: sus compromisos y devengados superan el presupuesto asignado.");
                    }

                    if (partida.ImporteSolicitado > disponibleBaseDSP)
                    {
                        var clave = !string.IsNullOrWhiteSpace(partida.ClaveObjetoGasto) ? partida.ClaveObjetoGasto : partida.IdObjetoGasto.ToString();
                        throw new InvalidOperationException($"WARNING|No existe suficiencia presupuestal para la partida {clave}. Disponible real: {disponibleBaseDSP:C}; solicitado: {partida.ImporteSolicitado:C}.");
                    }

                    var disponiblePosteriorDSP = disponibleBaseDSP - partida.ImporteSolicitado;

                    // UMBRAL DEL 20 %
                    var limite20 = dsp.Importe * 0.20m;

                    if (disponiblePosteriorDSP <= limite20)
                    {
                        var porcentaje = dsp.Importe > 0 ? Math.Round(disponiblePosteriorDSP / dsp.Importe * 100m, 2) : 0m;
                        var clave = !string.IsNullOrWhiteSpace(partida.ClaveObjetoGasto) ? partida.ClaveObjetoGasto : partida.IdObjetoGasto.ToString();

                        warnings.Add($"La partida {clave} quedará en o por debajo del umbral recomendado del 20 %. Disponible proyectado: {disponiblePosteriorDSP:C} ({porcentaje} %).");
                    }

                    // REGISTRAR COMPROMISO EN ÁREA
                    _context.MovimientoPresupuestals.Add(new MovimientoPresupuestal
                    {
                        SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                        IdDispPresupuestal = dsp.IdDispPresupuestal,
                        TipoMovimiento = "COMPROMETIDO",
                        Importe = partida.ImporteSolicitado,
                        Observaciones = dto.Observaciones,
                        FechaMovimiento = fechaActual,
                        FechaCaptura = fechaActual,
                        UsuarioCaptura = usuarioGuid,
                        Activo = true
                    });
                }

                // =====================================================
                // 18. REGISTRAR COMPROMISO EN TALLER
                // =====================================================
                _context.TallerMovimientoPresupuestos.Add(new TallerMovimientoPresupuesto
                {
                    TallerId = taller.TallerId,
                    IdControlTaller = control.IdControlTaller,
                    SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                    TipoMovimiento = "COMPROMETIDO",
                    Importe = importeTotalSolicitud,
                    FechaMovimiento = fechaActual,
                    UsuarioCaptura = usuarioGuid,
                    Activo = true
                });

                // =====================================================
                // 19. ACTUALIZAR CONTROL ANUAL DEL TALLER
                // =====================================================
                control.PresupuestoComprometido += importeTotalSolicitud;

                var nuevoDisponibleTaller = control.PresupuestoAsignado - control.PresupuestoComprometido - control.PresupuestoEjercido;

                if (nuevoDisponibleTaller < 0)
                {
                    throw new InvalidOperationException("WARNING|La operación produciría una inconsistencia en el control presupuestal del taller.");
                }

                control.PresupuestoDisponible = nuevoDisponibleTaller;
                control.FechaActualizacion = fechaActual;

                // =====================================================
                // 20. ACTUALIZAR ESTADO DE LA SOLICITUD
                // =====================================================
                solicitud.EstatusSolicitudMantenimientoId = 4;
                solicitud.FechaAutorizacion = fechaActual;
                solicitud.UsuarioAutorizaId = usuarioGuid;

                // =====================================================
                // 21. REGISTRAR SEGUIMIENTO HISTÓRICO
                // =====================================================
                _context.SolicitudMantenimientoSeguimientos.Add(new SolicitudMantenimientoSeguimiento
                {
                    SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                    EstatusSolicitudMantenimientoId = 4,
                    Observaciones = warnings.Any() ? string.Join(Environment.NewLine, warnings) : "Solicitud validada DSP.",
                    FechaMovimiento = fechaActual,
                    UsuarioId = usuarioGuid
                });

                // =====================================================
                // 22. PERSISTIR Y CONFIRMAR TRANSACCIÓN
                // =====================================================
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ProcesarDSPResultadoDto
                {
                    Exitoso = true,
                    Warnings = warnings
                };
            }
            catch (Exception ex)
            {
                // =====================================================
                // 23. REVERTIR TRANSACCIÓN
                // =====================================================
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(
                        rollbackEx,
                        "Error al revertir la transacción de DSP para la solicitud {SolicitudId}.",
                        dto.SolicitudMantenimientoId);
                }

                // =====================================================
                // 24. CLASIFICAR Y SANITIZAR ERROR
                // =====================================================
                var mensaje = ex.Message ?? string.Empty;
                var tipo = "ERROR";

                var separador = mensaje.IndexOf('|');

                if (separador > 0)
                {
                    var posibleTipo = mensaje.Substring(0, separador).Trim().ToUpperInvariant();

                    if (posibleTipo == "WARNING" || posibleTipo == "INFO" || posibleTipo == "ERROR")
                    {
                        tipo = posibleTipo;
                        mensaje = mensaje.Substring(separador + 1).Trim();
                    }
                }

                if (string.IsNullOrWhiteSpace(mensaje))
                {
                    mensaje = "No fue posible procesar la validación presupuestal de la solicitud.";
                }

                if (tipo == "ERROR")
                {
                    _logger.LogError(
                        ex,
                        "Error no controlado al procesar DSP para la solicitud {SolicitudId}.",
                        dto.SolicitudMantenimientoId);

                    mensaje = "No fue posible procesar la validación presupuestal de la solicitud.";
                }

                return new ProcesarDSPResultadoDto
                {
                    Exitoso = false,
                    Warnings = new List<string> { mensaje }
                };
            }
        }



        public async Task<TransferenciaUnidadEntregarViewModel> ObtenerTransferenciaParaEntrega(int transferenciaId)
        {
            var transferencia =
                await _context.TransferenciaUnidads

                    .Include(x => x.UnidadVehicular)

                    .Include(x => x.AreaOrigen)

                    .Include(x => x.AreaDestino)

                    .FirstOrDefaultAsync(x =>
                        x.TransferenciaUnidadId ==
                            transferenciaId
                        &&
                        x.Activo);

            if (transferencia == null)
            {
                throw new Exception(
                    "WARNING|La transferencia no fue encontrada.");
            }

            // =========================================
            // VALIDAR ESTATUS
            // =========================================

            // SOLO AUTORIZADAS

            if (transferencia.EstatusTransferenciaUnidadId != 2)
            {
                throw new Exception(
                    "WARNING|Solo pueden entregarse transferencias autorizadas.");
            }

            return new TransferenciaUnidadEntregarViewModel
            {
                TransferenciaUnidadId =
                    transferencia.TransferenciaUnidadId,

                Folio =
                    transferencia.Folio,

                Vehiculo =
                    transferencia.UnidadVehicular.NumeroEconomico
                    +
                    " - "
                    +
                    transferencia.UnidadVehicular.PlacaActual,

                AreaOrigen =
                    transferencia.AreaOrigen.AreaNombre,

                AreaDestino =
                    transferencia.AreaDestino.AreaNombre,

                FechaSolicitud =
                    transferencia.FechaSolicitud,

                MotivoTransferencia =
                    transferencia.MotivoTransferencia
            };
        }






        public async Task<TransferenciaUnidadRecepcionarViewModel> ObtenerRecepcionTransferencia(int transferenciaUnidadId)
        {
            var entidad =
                await _context.TransferenciaUnidads
                    .Include(x => x.UnidadVehicular)
                    .Include(x => x.AreaOrigen)
                    .Include(x => x.AreaDestino)
                    .FirstOrDefaultAsync(x =>
                        x.TransferenciaUnidadId ==
                            transferenciaUnidadId
                        &&
                        x.Activo);

            if (entidad == null)
            {
                throw new Exception(
                    "WARNING|La transferencia no existe.");
            }

            // SOLO ENTREGADAS

            if (entidad.EstatusTransferenciaUnidadId != 5)
            {
                throw new Exception(
                    "WARNING|La transferencia aún no ha sido entregada.");
            }

            return new TransferenciaUnidadRecepcionarViewModel
            {
                TransferenciaUnidadId =
                    entidad.TransferenciaUnidadId,

                Folio =
                    entidad.Folio,

                Vehiculo =
                    entidad.UnidadVehicular.NumeroEconomico,

                AreaOrigen =
                    entidad.AreaOrigen.AreaNombre,

                AreaDestino =
                    entidad.AreaDestino.AreaNombre,

                FechaEntrega =
                    entidad.FechaEntrega,

                ObservacionesEntrega =
                    entidad.ObservacionesEntrega
            };
        }






        public async Task RecepcionarTransferencia(TransferenciaUnidadRecepcionarDto dto, IFormFile documento)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var archivosGuardados =
                new List<string>();

            try
            {
                // =========================================
                // VALIDAR TRANSFERENCIA
                // =========================================

                var entidad =
                    await _context.TransferenciaUnidads
                        .FirstOrDefaultAsync(x =>
                            x.TransferenciaUnidadId ==
                                dto.TransferenciaUnidadId
                            &&
                            x.Activo);

                if (entidad == null)
                {
                    throw new Exception(
                        "WARNING|La transferencia no existe.");
                }

                // =========================================
                // VALIDAR ESTATUS
                // =========================================

                // SOLO ENTREGADAS

                if (entidad.EstatusTransferenciaUnidadId != 5)
                {
                    throw new Exception(
                        "WARNING|La transferencia aún no ha sido entregada.");
                }

                // =========================================
                // VALIDAR DOCUMENTO
                // =========================================

                if (documento == null || documento.Length <= 0)
                {
                    throw new Exception(
                        "INFO|Debe adjuntar el acta de recepción.");
                }

                const long tamanioMaximo =
                    2 * 1024 * 1024;

                if (documento.Length > tamanioMaximo)
                {
                    throw new Exception(
                        "WARNING|El archivo excede 2 MB.");
                }

                var extension =
                    Path.GetExtension(documento.FileName)
                        .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    throw new Exception(
                        "WARNING|Solo se permiten archivos PDF.");
                }

                using var memoryStream =
                    new MemoryStream();

                await documento.CopyToAsync(memoryStream);

                var bytes =
                    memoryStream.ToArray();

                if (!ArchivoValidoPorFirma(extension, bytes))
                {
                    throw new Exception(
                        "WARNING|El archivo no es un PDF válido.");
                }

                // =========================================
                // HASH
                // =========================================

                string hashArchivo;

                using (var sha256 = SHA256.Create())
                {
                    var hashBytes =
                        sha256.ComputeHash(bytes);

                    hashArchivo =
                        Convert.ToHexString(hashBytes);
                }

                // =========================================
                // RUTA
                // =========================================

                var rutaBase =
                    _configuration["Storage:DocumentosRuta"];

                var carpetaTransferencia =
                    Path.Combine(
                        rutaBase,
                        "Transferencias",
                        entidad.Folio);

                if (!Directory.Exists(carpetaTransferencia))
                {
                    Directory.CreateDirectory(
                        carpetaTransferencia);
                }

                var nombreFisico =
                    $"{Guid.NewGuid():N}.pdf";

                var rutaCompleta =
                    Path.Combine(
                        carpetaTransferencia,
                        nombreFisico);

                await File.WriteAllBytesAsync(
                    rutaCompleta,
                    bytes);

                archivosGuardados.Add(rutaCompleta);

                // ACTUALIZAR TRANSFERENCIA

                entidad.EstatusTransferenciaUnidadId = 6;

                entidad.FechaRecepcion = DateTime.Now;

                entidad.UsuarioRecibeId = dto.UsuarioRecibeId;

                entidad.ObservacionesRecepcion = dto.ObservacionesRecepcion;

                // =========================================
                // ACTUALIZAR ADSCRIPCIÓN DE LA UNIDAD
                // =========================================

                var unidad =
                    await _context.UnidadVehiculars
                        .FirstOrDefaultAsync(x =>
                            x.UnidadVehicularId ==
                                entidad.UnidadVehicularId);

                if (unidad == null)
                {
                    throw new Exception(
                        "WARNING|La unidad vehicular no fue encontrada.");
                }

                unidad.AreaId =
                    entidad.AreaDestinoId;

                // BITÁCORA

                var seguimiento =
                    new TransferenciaUnidadSeguimiento
                    {
                        TransferenciaUnidadId =
                            entidad.TransferenciaUnidadId,

                        EstatusTransferenciaUnidadId = 6,

                        TipoMovimiento =
                            "RECEPCION",

                        Observaciones =
                            dto.ObservacionesRecepcion,

                        UsuarioId =
                            dto.UsuarioRecibeId,

                        FechaMovimiento =
                            DateTime.Now,

                        FechaCreacion =
                            DateTime.Now,

                        Activo = true,

                        DocumentoRuta =
                            rutaCompleta,

                        DocumentoNombreOriginal =
                            documento.FileName,

                        HashDocumento =
                            hashArchivo
                    };

                _context
                    .TransferenciaUnidadSeguimientos
                    .Add(seguimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                foreach (var ruta in archivosGuardados)
                {
                    if (File.Exists(ruta))
                    {
                        File.Delete(ruta);
                    }
                }

                throw;
            }
        }




        public async Task<IEnumerable<SelectListItem>> ObtenerObjetosGasto()
        {
            return await _context.ObjetoGastos
                .AsNoTracking()
                .Where(x => x.ClaveObjGasto == "261001" || x.ClaveObjGasto == "261002" || x.ClaveObjGasto == "355001" || x.ClaveObjGasto == "291001"
                || x.ClaveObjGasto == "296001"
                )
                .OrderBy(x => x.ClaveObjGasto)
                .Select(x => new SelectListItem
                {
                    Value = x.IdObjetoGasto.ToString(),
                    Text = x.ClaveObjGasto + " - " + x.Descripcion
                })
                .ToListAsync();
        }




        public async Task<int?> ObtenerAreaPorUnidad(int unidadVehicularId)
        {
            return await _context.UnidadVehiculars
                .Where(x => x.UnidadVehicularId == unidadVehicularId)
                .Select(x => (int?)x.AreaId)
                .FirstOrDefaultAsync();
        }



        public async Task IngresarSolicitudATaller(int solicitudMantenimientoId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var usuarioGuid =
                    Guid.TryParse(
                        _httpContextAccessor.HttpContext?
                            .User?
                            .FindFirst(ClaimTypes.NameIdentifier)?
                            .Value,
                        out var guid)
                    ? guid
                    : (Guid?)null;

                // ============================================
                // OBTENER SOLICITUD
                // ============================================

                var solicitud =
                    await _context.SolicitudMantenimientos
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId ==
                                solicitudMantenimientoId
                            &&
                            x.Activo);

                if (solicitud == null)
                {
                    throw new Exception("La solicitud no fue encontrada.");
                }

                // ============================================
                // VALIDAR ESTATUS
                // ============================================

                const int ESTATUS_AUTORIZADA = 5;

                if (solicitud.EstatusSolicitudMantenimientoId != ESTATUS_AUTORIZADA)
                {
                    throw new Exception("La solicitud debe estar autorizada para ingresar al taller.");
                }

                // ============================================
                // VALIDAR QUE NO EXISTA MANTENIMIENTO
                // ============================================

                var existeMantenimiento =
                    await _context.Mantenimientos
                        .AnyAsync(x =>
                            x.SolicitudMantenimientoId ==
                                solicitudMantenimientoId
                            &&
                            x.Activo);

                if (existeMantenimiento)
                {
                    throw new Exception("La solicitud ya fue ingresada al taller.");
                }

                // ============================================
                // VALIDAR QUE EXISTAN SERVICIOS
                // ============================================

                var existenServicios =
                    await _context.SolicitudMantenimientoDetalles
                        .AnyAsync(x =>
                            x.SolicitudMantenimientoId ==
                                solicitudMantenimientoId
                            &&
                            x.Activo);

                if (!existenServicios)
                {
                    throw new Exception("La solicitud no contiene servicios.");
                }

                //if (string.IsNullOrWhiteSpace(solicitud.MotivoSolicitud))
                //{
                //    throw new Exception("La solicitud no contiene un motivo de mantenimiento.");
                //}

                // ============================================
                // CREAR CABECERA DEL MANTENIMIENTO
                // (ORDEN DE TRABAJO EN PROCESO)
                // ============================================

                const int ESTATUS_MANT_EN_PROCESO = 1;

                var mantenimiento =
                    new Mantenimiento
                    {
                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId,

                        UnidadVehicularId =
                            solicitud.UnidadVehicularId,

                        TipoMantenimientoId =
                            solicitud.TipoMantenimientoId,

                        TallerId =
                            solicitud.TallerId!.Value,

                        FechaIngreso =
                            DateTime.Now,

                        KilometrajeEntrada =
                            solicitud.KilometrajeActual,

                        MotivoMantenimiento = "Se ingresó al taller para mantenimiento",
                        //solicitud.MotivoSolicitud.Trim()"",

                        Diagnostico =
                            solicitud.DiagnosticoInicial,

                        //Observaciones =
                        //    solicitud.Observaciones,

                        FechaCreacion =
                            DateTime.Now,

                        Folio =
                            solicitud.Folio,

                        UsuarioCreacionId =
                            usuarioGuid,

                        EstatusMantenimientoId = ESTATUS_MANT_EN_PROCESO,

                        // Estos campos se llenarán al finalizar
                        FechaInicioTrabajo = null,
                        FechaFinTrabajo = null,
                        FechaSalida = null,
                        FechaCierre = null,
                        FechaEjecucion = null,
                        KilometrajeSalida = null,
                        KilometrajeEjecucion = null,
                        TrabajoRealizado = null,
                        DiasFueraServicio = null,
                        CostoRealTotal = 0,

                        Activo = true
                    };

                _context.Mantenimientos.Add(mantenimiento);

                await _context.SaveChangesAsync();

                // ============================================
                // CAMBIAR ESTATUS DE LA SOLICITUD
                // ============================================

                const int ESTATUS_EN_TALLER = 7;

                solicitud.EstatusSolicitudMantenimientoId = ESTATUS_EN_TALLER;

                // ============================================
                // SEGUIMIENTO DE LA SOLICITUD
                // ============================================

                _context.SolicitudMantenimientoSeguimientos.Add(
                    new SolicitudMantenimientoSeguimiento
                    {
                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId,

                        EstatusSolicitudMantenimientoId =
                            ESTATUS_EN_TALLER,

                        Observaciones =
                            "La unidad fue ingresada al taller.",

                        UsuarioId =
                            usuarioGuid,

                        FechaMovimiento =
                            DateTime.Now
                    });

                // ============================================
                // SEGUIMIENTO DEL MANTENIMIENTO
                // ============================================

                _context.MantenimientoSeguimientos.Add(
                    new MantenimientoSeguimiento
                    {
                        MantenimientoId =
                            mantenimiento.MantenimientoId,

                        EstatusMantenimientoId =
                            ESTATUS_MANT_EN_PROCESO,

                        Observaciones = "Unidad ingresada al taller.",

                        UsuarioId = usuarioGuid,

                        FechaMovimiento =
                            DateTime.Now
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }






        public async Task<MantenimientoCreateViewModel?> ObtenerSolicitudParaIngresoTaller(int solicitudId)
        {
            var solicitud =
                await _context.SolicitudMantenimientos
                    .AsNoTracking()
                    .Include(x => x.UnidadVehicular)
                    .Include(x => x.TipoMantenimiento)
                    .Include(x => x.Taller)
                    .FirstOrDefaultAsync(x =>
                        x.SolicitudMantenimientoId == solicitudId
                        &&
                        x.Activo);

            if (solicitud == null)
            {
                return null;
            }

            // ============================================
            // VALIDAR ESTATUS AUTORIZADA
            // ============================================
            if (solicitud.EstatusSolicitudMantenimientoId != 5)
            {
                throw new Exception(
                    "La solicitud no se encuentra autorizada para ingresar al taller.");
            }

            // ============================================
            // VALIDAR QUE NO EXISTA MANTENIMIENTO
            // ============================================
            var existeMantenimiento =
                await _context.Mantenimientos
                    .AnyAsync(x =>
                        x.SolicitudMantenimientoId == solicitudId
                        &&
                        x.Activo);

            if (existeMantenimiento)
            {
                throw new Exception(
                    "La solicitud ya fue ingresada al taller.");
            }

            var detalles = await _context.SolicitudMantenimientoDetalles
                .AsNoTracking()
                .Include(x => x.ConceptoServicio)
                .Where(x => x.SolicitudMantenimientoId == solicitudId && x.Activo)
                .ToListAsync();

            // ============================================
            // MAPEAR VIEWMODEL CON IVA INCLUIDO (16%)
            // ============================================
            return new MantenimientoCreateViewModel
            {
                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                Folio = solicitud.Folio,
                NumeroOficio = solicitud.NumeroOficio,
                UnidadVehicularId = solicitud.UnidadVehicularId,
                UnidadTexto = solicitud.UnidadVehicular.PlacaActual + " - " + solicitud.UnidadVehicular.NumeroSerie + " - " + solicitud.UnidadVehicular.NumeroEconomico,
                TipoMantenimientoId = solicitud.TipoMantenimientoId,
                TallerId = (int)solicitud.TallerId,
                KilometrajeEntrada = solicitud.KilometrajeActual,
                MotivoMantenimiento = solicitud.MotivoSolicitud,
                Diagnostico = solicitud.DiagnosticoInicial,
                Observaciones = solicitud.Observaciones,
                FechaIngreso = DateTime.Now,
                Taller = solicitud.Taller.RazonSocial,
                UnidadVehicularTexto = solicitud.UnidadVehicular != null
                    ? $"{solicitud.UnidadVehicular.NumeroEconomico} {solicitud.UnidadVehicular.PlacaActual}"
                    : string.Empty,

                Servicios = detalles.Select(x =>
                {
                    // Costos base unitarios originales
                    var moBase = x.CostoEstimadoManoObra;
                    var refBase = x.CostoEstimadoRefacciones;

                    // Costos con IVA (16%) aplicados para coincidir con DSP y Revisión Técnica
                    var moConIva = moBase * 1.16m;
                    var refConIva = refBase * 1.16m;

                    var subtotalRenglon = x.Cantidad * (moConIva + refConIva);

                    return new MantenimientoServicioViewModel
                    {
                        ConceptoServicioId = x.ConceptoServicioId,
                        ConceptoServicio = x.ConceptoServicio.ConceptoServicioNombre,
                        Cantidad = (int)x.Cantidad,

                        // Exponemos las bases y los valores con IVA para la vista
                        CostoEstimadoManoObra = moBase,
                        CostoEstimadoRefacciones = refBase,

                        CostoManoObraConIva = moConIva,
                        CostoRefaccionesConIva = refConIva,

                        TotalEstimado = subtotalRenglon,
                        Observaciones = x.Observaciones
                    };
                }).ToList()
            };
        }








        public async Task<MantenimientoEditViewModel> ObtenerMantenimientoEditar(int solicitudId)
        {
            var mantenimiento =
                await _context.Mantenimientos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.SolicitudMantenimientoId ==
                            solicitudId
                        &&
                        x.Activo);

            if (mantenimiento == null)
            {
                throw new Exception(
                    "No existe un mantenimiento asociado.");
            }



            var tieneDetalles = await _context.MantenimientoDetalles.AnyAsync(x => x.MantenimientoId == mantenimiento.MantenimientoId);

            if (tieneDetalles)
            {
                return await ObtenerMantenimientoEditarPorMantenimientoId(mantenimiento.MantenimientoId);
            }

            return await ObtenerMantenimientoEditarDesdeSolicitud(mantenimiento.MantenimientoId, solicitudId);
            //return await ObtenerMantenimientoEditarPorMantenimientoId(mantenimiento.MantenimientoId);
        }






        public async Task<MantenimientoEditViewModel> ObtenerMantenimientoEditarDesdeSolicitud(int mantenimientoId, int solicitudId)
        {
            var mantenimiento = await _context.Mantenimientos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MantenimientoId == mantenimientoId && x.Activo);

            if (mantenimiento == null)
            {
                throw new Exception("No existe el mantenimiento.");
            }

            var encabezado = await (
                from s in _context.SolicitudMantenimientos
                join u in _context.UnidadVehiculars on s.UnidadVehicularId equals u.UnidadVehicularId
                join t in _context.Tallers on s.TallerId equals t.TallerId
                where s.SolicitudMantenimientoId == solicitudId
                select new
                {
                    s.Folio,
                    s.NumeroOficio,
                    UnidadVehicularTexto = u.NumeroEconomico + " - " + u.PlacaActual + " - " + u.NumeroSerie.Trim() + " - " + u.Marca.MarcaNombre.Trim() + " - " + u.Modelo.ModeloNombre.Trim(),
                    Marca = u.Marca.MarcaNombre,
                    Modelo = u.Modelo.ModeloNombre,
                    NumeroSerie = u.NumeroSerie,
                    PlacaActual = u.PlacaActual,
                    NumeroEconomico = u.NumeroEconomico,
                    TallerTexto = t.RazonSocial
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (encabezado == null)
            {
                throw new Exception("No se encontró la solicitud.");
            }

            var serviciosRaw = await (
                from d in _context.SolicitudMantenimientoDetalles
                join c in _context.ConceptoServicios on d.ConceptoServicioId equals c.ConceptoServicioId
                join ogmo in _context.ObjetoGastos on d.IdObjetoGastoManoObra equals ogmo.IdObjetoGasto into ogmoJoin
                from ogmo in ogmoJoin.DefaultIfEmpty()
                join ogref in _context.ObjetoGastos on d.IdObjetoGastoRefacciones equals ogref.IdObjetoGasto into ogrefJoin
                from ogref in ogrefJoin.DefaultIfEmpty()
                where d.SolicitudMantenimientoId == solicitudId && d.Activo
                select new
                {
                    d.ConceptoServicioId,
                    ConceptoServicio = c.ConceptoServicioNombre,
                    d.Cantidad,
                    CostoEstimadoManoObra = d.CostoEstimadoManoObra,
                    CostoEstimadoRefacciones = d.CostoEstimadoRefacciones,
                    d.IdObjetoGastoManoObra,
                    d.IdObjetoGastoRefacciones,
                    ObjetoGastoManoObraTexto = ogmo != null ? $"{ogmo.ClaveObjGasto} {ogmo.Descripcion}" : "",
                    ObjetoGastoRefaccionesTexto = ogref != null ? $"{ogref.ClaveObjGasto} {ogref.Descripcion}" : "",
                    d.Observaciones
                })
                .AsNoTracking()
                .ToListAsync();

            // Mapeo aplicando el factor del 16% de IVA de manera segura en memoria
            var servicios = serviciosRaw.Select(d =>
            {
                var moConIva = d.CostoEstimadoManoObra * 1.16m;
                var refConIva = d.CostoEstimadoRefacciones * 1.16m;
                var subMo = d.Cantidad * moConIva;
                var subRef = d.Cantidad * refConIva;

                return new MantenimientoDetalleViewModel
                {
                    MantenimientoDetalleId = null,
                    ConceptoServicioId = d.ConceptoServicioId,
                    ConceptoServicio = d.ConceptoServicio,
                    Cantidad = d.Cantidad,
                    CostoUnitarioManoObra = moConIva,
                    CostoUnitarioRefacciones = refConIva,
                    CostoEstimadoManoObra = d.CostoEstimadoManoObra,
                    CostoEstimadoRefacciones = d.CostoEstimadoRefacciones,
                    SubtotalManoObra = subMo,
                    SubtotalRefacciones = subRef,
                    TotalLinea = subMo + subRef,
                    IdObjetoGastoManoObra = d.IdObjetoGastoManoObra,
                    IdObjetoGastoRefacciones = d.IdObjetoGastoRefacciones,
                    ObjetoGastoManoObraTexto = d.ObjetoGastoManoObraTexto,
                    ObjetoGastoRefaccionesTexto = d.ObjetoGastoRefaccionesTexto,
                    Observaciones = d.Observaciones
                };
            }).ToList();

            // =========================================================
            // NORMALIZACIÓN DE FECHAS PARA NUEVOS REGISTROS O DESDE SOLICITUD
            // =========================================================
            DateTime LimpiarMilisegundos(DateTime dt) => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);

            DateTime fechaInicioRaw = mantenimiento.FechaInicioTrabajo ?? (mantenimiento.FechaIngreso != DateTime.MinValue ? mantenimiento.FechaIngreso : DateTime.Now);
            DateTime fechaInicioVal = LimpiarMilisegundos(fechaInicioRaw);

            DateTime fechaFinRaw = (mantenimiento.FechaFinTrabajo.HasValue && mantenimiento.FechaFinTrabajo >= fechaInicioVal)
                ? mantenimiento.FechaFinTrabajo.Value
                : fechaInicioVal;
            DateTime fechaFinVal = LimpiarMilisegundos(fechaFinRaw);

            return new MantenimientoEditViewModel
            {
                MantenimientoId = mantenimiento.MantenimientoId,
                SolicitudMantenimientoId = solicitudId,
                UnidadVehicularId = mantenimiento.UnidadVehicularId,
                TallerId = mantenimiento.TallerId,
                TipoMantenimientoId = mantenimiento.TipoMantenimientoId,

                Folio = encabezado.Folio,
                NumeroOficio = encabezado.NumeroOficio,
                UnidadVehicularTexto = encabezado.UnidadVehicularTexto,
                Marca = $"{encabezado.Marca}",
                Modelo = $"{encabezado.Modelo}",
                NumeroSerie = $"{encabezado.NumeroSerie}",
                PlacaActual = $"{encabezado.PlacaActual}",
                NumeroEconomico = $"{encabezado.NumeroEconomico}",
                TallerTexto = encabezado.TallerTexto,

                FechaIngreso = mantenimiento.FechaIngreso,
                FechaSalida = mantenimiento.FechaSalida,

                FechaInicioTrabajo = fechaInicioVal,
                FechaFinTrabajo = fechaFinVal,

                KilometrajeEntrada = mantenimiento.KilometrajeEntrada,
                KilometrajeSalida = mantenimiento.KilometrajeSalida,
                MotivoMantenimiento = mantenimiento.MotivoMantenimiento,
                Diagnostico = mantenimiento.Diagnostico,
                TrabajoRealizado = mantenimiento.TrabajoRealizado,
                Observaciones = mantenimiento.Observaciones,
                EstatusMantenimientoId = (int)mantenimiento.EstatusMantenimientoId,
                CostoRealTotal = servicios.Sum(x => x.TotalLinea),
                Servicios = servicios
            };
        }





        public async Task EditarMantenimiento(MantenimientoEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var usuarioGuid =
                    Guid.TryParse(
                        _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value,
                        out var guid)
                        ? guid
                        : (Guid?)null;


                var mantenimiento =
                    await _context.Mantenimientos
                    .FirstOrDefaultAsync(x =>
                        x.MantenimientoId == dto.MantenimientoId
                        &&
                        x.Activo);


                if (mantenimiento == null)
                {
                    throw new Exception(
                        "WARNING|El mantenimiento no existe.");
                }


                if (mantenimiento.EstatusMantenimientoId != 1)
                {
                    throw new Exception(
                        "WARNING|El mantenimiento ya fue finalizado.");
                }


                mantenimiento.FechaInicioTrabajo =
                    dto.FechaInicioTrabajo;


                mantenimiento.FechaFinTrabajo = dto.FechaFinTrabajo;


                mantenimiento.Diagnostico =
                    dto.Diagnostico;


                mantenimiento.TrabajoRealizado =
                    dto.TrabajoRealizado;


                mantenimiento.Observaciones =
                    dto.Observaciones;


                mantenimiento.KilometrajeSalida =
                    dto.KilometrajeSalida;

                mantenimiento.FechaSalida = dto.FechaFinTrabajo;

                mantenimiento.UsuarioCierreId = usuarioGuid;

                mantenimiento.DiasFueraServicio = (mantenimiento.FechaSalida - mantenimiento.FechaIngreso)?.Days;


                await _context.SaveChangesAsync();


                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        private async Task<MantenimientoEditViewModel> ObtenerMantenimientoEditarPorMantenimientoId(int mantenimientoId)
        {
            var mantenimiento =
                await _context.Mantenimientos
                    .Include(x => x.UnidadVehicular)
                    .Include(x => x.Taller)
                    .Include(x => x.TipoMantenimiento)
                    .Include(x => x.SolicitudMantenimiento)
                    .FirstOrDefaultAsync(x => x.MantenimientoId == mantenimientoId);

            if (mantenimiento == null)
            {
                throw new Exception("Mantenimiento no encontrado.");
            }

            var detalles =
                await _context.MantenimientoDetalles
                    .Include(x => x.ConceptoServicio)
                    .Where(x => x.MantenimientoId == mantenimientoId)
                    .ToListAsync();

            if (!detalles.Any())
            {
                throw new Exception($"No existen detalles para el mantenimiento {mantenimientoId}");
            }

            var idsObjetoGasto = detalles
                .SelectMany(x => new[] { x.IdObjetoGastoManoObra, x.IdObjetoGastoRefacciones })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var objetosGasto = await _context.ObjetoGastos
                .Where(x => idsObjetoGasto.Contains(x.IdObjetoGasto))
                .ToDictionaryAsync(x => x.IdObjetoGasto, x => $"{x.ClaveObjGasto} {x.Descripcion}");

            // =========================================================
            // NORMALIZACIÓN DE FECHAS PARA EVITAR INCONSISTENCIAS AL CARGAR
            // =========================================================
            DateTime LimpiarMilisegundos(DateTime dt) => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, dt.Kind);

            DateTime fechaInicioRaw = mantenimiento.FechaInicioTrabajo ?? (mantenimiento.FechaIngreso != DateTime.MinValue ? mantenimiento.FechaIngreso : DateTime.Now);
            DateTime fechaInicioVal = LimpiarMilisegundos(fechaInicioRaw);

            DateTime fechaFinRaw = (mantenimiento.FechaFinTrabajo.HasValue && mantenimiento.FechaFinTrabajo >= fechaInicioVal)
                ? mantenimiento.FechaFinTrabajo.Value
                : fechaInicioVal;
            DateTime fechaFinVal = LimpiarMilisegundos(fechaFinRaw);

            return new MantenimientoEditViewModel
            {
                MantenimientoId = mantenimiento.MantenimientoId,
                Folio = mantenimiento.SolicitudMantenimiento?.Folio ?? string.Empty,
                NumeroOficio = mantenimiento.SolicitudMantenimiento?.NumeroOficio ?? string.Empty,
                SolicitudMantenimientoId = mantenimiento.SolicitudMantenimientoId ?? 0,
                UnidadVehicularId = mantenimiento.UnidadVehicularId,
                TallerId = mantenimiento.TallerId,
                TipoMantenimientoId = mantenimiento.TipoMantenimientoId,

                FechaIngreso = mantenimiento.FechaIngreso,
                FechaSalida = mantenimiento.FechaSalida ?? DateTime.MinValue,

                FechaInicioTrabajo = fechaInicioVal,
                FechaFinTrabajo = fechaFinVal,

                KilometrajeEntrada = mantenimiento.KilometrajeEntrada,
                KilometrajeSalida = mantenimiento.KilometrajeSalida,
                MotivoMantenimiento = mantenimiento.MotivoMantenimiento,
                Diagnostico = mantenimiento.Diagnostico,
                TrabajoRealizado = mantenimiento.TrabajoRealizado,
                Observaciones = mantenimiento.Observaciones,
                EstatusMantenimientoId = mantenimiento.EstatusMantenimientoId ?? 0,
                CostoRealTotal = mantenimiento.CostoRealTotal,

                Marca = $"{mantenimiento.UnidadVehicular.Marca}",
                Modelo = $"{mantenimiento.UnidadVehicular.Modelo}",
                NumeroSerie = $"{mantenimiento.UnidadVehicular.NumeroSerie}",
                PlacaActual = $"{mantenimiento.UnidadVehicular.PlacaActual}",
                NumeroEconomico = $"{mantenimiento.UnidadVehicular.NumeroEconomico}",
                TallerTexto = mantenimiento.Taller.RazonSocial,
                TipoMantenimientoTexto = mantenimiento.TipoMantenimiento.Nombre,

                Servicios = detalles.Select(d =>
                {
                    // Aplicación segura del IVA (16%) sobre los costos unitarios y subtotales
                    var moConIva = d.CostoUnitarioManoObra * 1.16m;
                    var refConIva = d.CostoUnitarioRefacciones * 1.16m;
                    var subMo = d.Cantidad * moConIva;
                    var subRef = d.Cantidad * refConIva;

                    return new MantenimientoDetalleViewModel
                    {
                        MantenimientoDetalleId = d.MantenimientoDetalleId,
                        MantenimientoId = d.MantenimientoId,
                        ConceptoServicioId = d.ConceptoServicioId,
                        ConceptoServicio = d.ConceptoServicio.ConceptoServicioNombre,
                        Cantidad = Convert.ToInt32(d.Cantidad),

                        CostoUnitarioManoObra = moConIva,
                        CostoUnitarioRefacciones = refConIva,
                        SubtotalManoObra = subMo,
                        SubtotalRefacciones = subRef,
                        TotalLinea = subMo + subRef,

                        Observaciones = d.Observaciones,
                        IdObjetoGastoManoObra = d.IdObjetoGastoManoObra,
                        IdObjetoGastoRefacciones = d.IdObjetoGastoRefacciones,
                        ObjetoGastoManoObraTexto = d.IdObjetoGastoManoObra.HasValue && objetosGasto.ContainsKey(d.IdObjetoGastoManoObra.Value) ? objetosGasto[d.IdObjetoGastoManoObra.Value] : string.Empty,
                        ObjetoGastoRefaccionesTexto = d.IdObjetoGastoRefacciones.HasValue && objetosGasto.ContainsKey(d.IdObjetoGastoRefacciones.Value) ? objetosGasto[d.IdObjetoGastoRefacciones.Value] : string.Empty
                    };
                }).ToList()
            };
        }


        public async Task FinalizarMantenimiento(int mantenimientoId, List<MantenimientoDetalleDto> servicios)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {

                var usuarioGuid =
                    Guid.TryParse(
                        _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value,
                        out var guid)
                        ? guid
                        : (Guid?)null;



                var mantenimiento =
                    await _context.Mantenimientos
                    .Include(x => x.MantenimientoDetalles)
                    .FirstOrDefaultAsync(x =>
                        x.MantenimientoId == mantenimientoId
                        &&
                        x.Activo);



                if (mantenimiento == null)
                {
                    throw new Exception("El mantenimiento no existe.");
                }



                if (mantenimiento.EstatusMantenimientoId != 1)
                {
                    throw new Exception("El mantenimiento ya fue finalizado.");
                }



                if (!mantenimiento.FechaInicioTrabajo.HasValue)
                {
                    throw new Exception("Debe indicar inicio de trabajo.");
                }



                if (!mantenimiento.FechaFinTrabajo.HasValue)
                {
                    throw new Exception("Debe indicar fin de trabajo.");
                }



                if (mantenimiento.FechaFinTrabajo <
                    mantenimiento.FechaInicioTrabajo)
                {
                    throw new Exception("La fecha final no puede ser menor.");
                }



                //if (string.IsNullOrWhiteSpace(
                //    mantenimiento.TrabajoRealizado))
                //{
                //    throw new Exception("Debe capturar el trabajo realizado.");
                //}



                if (servicios == null || !servicios.Any())
                {
                    throw new Exception("No existen servicios.");
                }



                //-----------------------------------------
                // CREAR DETALLES REALIZADOS
                //-----------------------------------------

                decimal costoTotal = 0;


                foreach (var servicio in servicios)
                {

                    var subtotalMano =
                        servicio.Cantidad *
                        servicio.CostoUnitarioManoObra;


                    var subtotalRef =
                        servicio.Cantidad *
                        servicio.CostoUnitarioRefacciones;


                    var total =
                        subtotalMano +
                        subtotalRef;


                    costoTotal += total;



                    var detalle =
                        new MantenimientoDetalle
                        {

                            MantenimientoId =
                                mantenimiento.MantenimientoId,


                            ConceptoServicioId =
                                servicio.ConceptoServicioId,


                            Cantidad =
                                servicio.Cantidad,


                            CostoUnitarioManoObra =
                                servicio.CostoUnitarioManoObra,


                            CostoUnitarioRefacciones =
                                servicio.CostoUnitarioRefacciones,


                            SubtotalManoObra =
                                subtotalMano,


                            SubtotalRefacciones =
                                subtotalRef,


                            TotalLinea =
                                total,


                            IdObjetoGastoManoObra =
                                servicio.IdObjetoGastoManoObra,


                            IdObjetoGastoRefacciones =
                                servicio.IdObjetoGastoRefacciones,


                            Observaciones =
                                servicio.Observaciones,


                            FechaCreacion =
                                DateTime.Now
                        };


                    _context.MantenimientoDetalles.Add(detalle);

                }



                mantenimiento.CostoRealTotal =
                    costoTotal;



                //-----------------------------------------
                // CERRAR MANTENIMIENTO
                //-----------------------------------------

                const int FINALIZADO = 5;


                mantenimiento.EstatusMantenimientoId =
                    FINALIZADO;


                mantenimiento.FechaSalida = mantenimiento.FechaSalida;

                mantenimiento.FechaFinTrabajo = mantenimiento.FechaFinTrabajo;


                mantenimiento.FechaCierre = DateTime.Now;


                mantenimiento.FechaEjecucion = DateTime.Now;


                mantenimiento.KilometrajeEjecucion = mantenimiento.KilometrajeSalida;




                //-----------------------------------------
                // SEGUIMIENTO DEL MANTENIMIENTO
                //-----------------------------------------

                _context.MantenimientoSeguimientos.Add(
                    new MantenimientoSeguimiento
                    {
                        MantenimientoId =
                            mantenimiento.MantenimientoId,

                        EstatusMantenimientoId =
                            FINALIZADO,

                        Observaciones =
                            "Mantenimiento finalizado y unidad liberada.",

                        UsuarioId =
                            usuarioGuid,

                        FechaMovimiento =
                            DateTime.Now
                    });







                //-----------------------------------------
                // ACTUALIZAR SOLICITUD
                //-----------------------------------------

                if (mantenimiento.SolicitudMantenimientoId.HasValue)
                {
                    var solicitud =
                        await _context.SolicitudMantenimientos
                            .FirstAsync(x =>
                                x.SolicitudMantenimientoId ==
                                mantenimiento.SolicitudMantenimientoId);

                    const int ESTATUS_SOLICITUD_FINALIZADA = 8;

                    solicitud.EstatusSolicitudMantenimientoId =
                        ESTATUS_SOLICITUD_FINALIZADA;



                    _context.SolicitudMantenimientoSeguimientos.Add(
                        new SolicitudMantenimientoSeguimiento
                        {
                            SolicitudMantenimientoId =
                                solicitud.SolicitudMantenimientoId,

                            EstatusSolicitudMantenimientoId =
                                ESTATUS_SOLICITUD_FINALIZADA,

                            Observaciones =
                                "Mantenimiento finalizado y servicios realizados.",

                            UsuarioId =
                                usuarioGuid,

                            FechaMovimiento =
                                DateTime.Now
                        });
                }



                await _context.SaveChangesAsync();


                await transaction.CommitAsync();

            }
            catch
            {


                await transaction.RollbackAsync();
                throw;
            }
        }










        public async Task RechazarValidacionTecnica(int solicitudId, string observaciones)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {

                var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;

                var solicitud =
                    await _context.SolicitudMantenimientos
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId ==
                            solicitudId);

                if (solicitud == null)
                {
                    throw new Exception(
                        "Solicitud no encontrada.");
                }

                // ====================================
                // OBTENER COMPROMISOS
                // ====================================

                var movimientosComprometidos = await _context.MovimientoPresupuestals
                        .Where(x => x.Activo == true
                            &&
                            x.TipoMovimiento ==
                                "COMPROMETIDO"
                            &&
                            x.SolicitudMantenimientoId ==
                                solicitudId)
                        .ToListAsync();

                // ====================================
                // GENERAR DESCOMPROMISO
                // ====================================

                foreach (var movimiento in movimientosComprometidos)
                {
                    _context.MovimientoPresupuestals.Add(
                        new MovimientoPresupuestal
                        {
                            SolicitudMantenimientoId =
                                solicitudId,

                            IdDispPresupuestal =
                                movimiento.IdDispPresupuestal,

                            FechaMovimiento =
                                DateTime.Now,

                            TipoMovimiento =
                                "DESCOMPROMETIDO",

                            Importe =
                                movimiento.Importe,

                            Observaciones =
                                "Liberación automática por rechazo técnico",

                            FechaCaptura =
                                DateTime.Now,

                            UsuarioCaptura = usuarioGuid,

                            Activo = true
                        });
                }

                // ====================================
                // CAMBIAR ESTATUS
                // ====================================

                solicitud.EstatusSolicitudMantenimientoId = 6;

                // ====================================
                // SEGUIMIENTO
                // ====================================

                _context.SolicitudMantenimientoSeguimientos.Add(
                    new SolicitudMantenimientoSeguimiento
                    {
                        SolicitudMantenimientoId =
                            solicitudId,

                        EstatusSolicitudMantenimientoId =
                            6,

                        Observaciones = observaciones,

                        FechaMovimiento = DateTime.Now,
                        UsuarioId = usuarioGuid

                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<List<ConfiguracionValidacionTecnicaDto>> ObtenerConfiguracionValidacion(List<int> conceptos)
        {
            var reglasDb =
                await (
                    from cfg in _context.ConfiguracionValidacionTecnicas
                    join c in _context.ConceptoServicios
                        on cfg.ConceptoServicioId equals c.ConceptoServicioId
                    where cfg.Activo
                          && c.Activo
                          && conceptos.Contains((int)cfg.ConceptoServicioId)
                    select new
                    {
                        cfg.ConfiguracionValidacionTecnicaId,
                        cfg.ConceptoServicioId,
                        c.TipoCatalogoServicioId,
                        cfg.ValidarSolicitudesAbiertas,
                        cfg.ValidarRecurrencia,
                        cfg.DiasRecurrencia
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            var configIds = reglasDb
                .Select(x => x.ConfiguracionValidacionTecnicaId)
                .Distinct()
                .ToList();

            var estatusDb = await _context.ConfiguracionValidacionEstatuses
                .AsNoTracking()
                .Where(x =>
                    x.Activo &&
                    configIds.Contains(x.ConfiguracionValidacionTecnicaId))
                .ToListAsync();

            var estatusLookup =
                estatusDb
                    .GroupBy(x => x.ConfiguracionValidacionTecnicaId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.EstatusSolicitudMantenimientoId).ToList()
                    );

            return reglasDb
                .Select(x =>
                {
                    estatusLookup.TryGetValue(x.ConfiguracionValidacionTecnicaId, out var estatus);

                    return new ConfiguracionValidacionTecnicaDto
                    {
                        ConfiguracionValidacionTecnicaId = x.ConfiguracionValidacionTecnicaId,

                        ConceptoServicioId = (int)x.ConceptoServicioId,
                        TipoCatalogoServicioId = x.TipoCatalogoServicioId,

                        ValidarSolicitudesAbiertas = x.ValidarSolicitudesAbiertas,
                        ValidarRecurrencia = x.ValidarRecurrencia,

                        DiasRecurrencia =
                            x.DiasRecurrencia.HasValue && x.DiasRecurrencia > 0
                                ? x.DiasRecurrencia.Value
                                : 30,

                        EstatusPermitidos = estatus ?? new List<int>()
                    };
                })
                .ToList();
        }






        //public async Task GuardarConfiguracion(ConfiguracionValidacionTecnicaViewModel vm)
        public async Task<ResultadoOperacionDto> GuardarConfiguracion(ConfiguracionValidacionTecnicaViewModel vm)
        {
            var resultado =
                new ResultadoOperacionDto();

            //---------------------------------------
            // VALIDACIONES
            //---------------------------------------

            if (
                vm.ConceptoServicioId == null
                ||
                vm.TipoCatalogoServicioId == null
               )
            {
                resultado.Exitoso = false;

                resultado.Mensaje =
                    "Debe seleccionar tipo y concepto.";

                return resultado;
            }

            vm.EstatusPermitidos ??=
                new List<int>();

            //---------------------------------------
            // VALIDAR DUPLICADOS
            //---------------------------------------

            var duplicado =
                await _context.ConfiguracionValidacionTecnicas
                .AnyAsync(x =>

                    x.Activo

                    &&

                    x.ConfiguracionValidacionTecnicaId
                        != vm.ConfiguracionValidacionTecnicaId

                    &&

                    x.ConceptoServicioId
                        == vm.ConceptoServicioId

                    &&

                    x.TipoCatalogoServicioId
                        == vm.TipoCatalogoServicioId
                );

            if (duplicado)
            {
                resultado.Exitoso = false;

                resultado.Mensaje =
                    "Ya existe configuración para este concepto.";

                return resultado;
            }

            //---------------------------------------
            // INSERT
            //---------------------------------------

            if (!vm.ConfiguracionValidacionTecnicaId.HasValue)
            {
                var entidad =
                    new ConfiguracionValidacionTecnica
                    {
                        TipoCatalogoServicioId =
                            vm.TipoCatalogoServicioId.Value,

                        ConceptoServicioId =
                            vm.ConceptoServicioId.Value,

                        ValidarSolicitudesAbiertas =
                            vm.ValidarSolicitudesAbiertas,

                        ValidarRecurrencia =
                            vm.ValidarRecurrencia,

                        DiasRecurrencia =
                            vm.DiasRecurrencia,

                        Activo =
                            vm.Activo,

                        //-----------------------------------
                        // YA NO GUARDAR CSV
                        //-----------------------------------

                        EstatusSolicitudesAbiertas =
                            null,

                        FechaCreacion =
                            DateTime.Now
                    };

                _context
                    .ConfiguracionValidacionTecnicas
                    .Add(entidad);

                await _context.SaveChangesAsync();

                //-----------------------------------
                // TABLA RELACIONAL
                //-----------------------------------

                await GuardarEstatus(
                    entidad.ConfiguracionValidacionTecnicaId,
                    vm.EstatusPermitidos);
            }

            //---------------------------------------
            // UPDATE
            //---------------------------------------

            else
            {
                var entidad =
                    await _context
                    .ConfiguracionValidacionTecnicas
                    .FirstOrDefaultAsync(x =>
                        x.ConfiguracionValidacionTecnicaId
                        ==
                        vm.ConfiguracionValidacionTecnicaId);

                if (entidad == null)
                {
                    resultado.Exitoso = false;

                    resultado.Mensaje =
                        "Configuración no encontrada.";

                    return resultado;
                }

                entidad.TipoCatalogoServicioId =
                    vm.TipoCatalogoServicioId.Value;

                entidad.ConceptoServicioId =
                    vm.ConceptoServicioId.Value;

                entidad.ValidarSolicitudesAbiertas =
                    vm.ValidarSolicitudesAbiertas;

                entidad.ValidarRecurrencia =
                    vm.ValidarRecurrencia;

                entidad.DiasRecurrencia =
                    vm.DiasRecurrencia;

                entidad.Activo =
                    vm.Activo;

                //-----------------------------------
                // YA NO GUARDAR CSV
                //-----------------------------------

                entidad.EstatusSolicitudesAbiertas =
                    null;

                entidad.FechaModificacion =
                    DateTime.Now;

                await _context.SaveChangesAsync();

                //-----------------------------------
                // REEMPLAZAR RELACIONES
                //-----------------------------------

                await ReemplazarEstatus(
                    entidad.ConfiguracionValidacionTecnicaId,
                    vm.EstatusPermitidos);
            }

            resultado.Exitoso = true;

            resultado.Mensaje =
                "Configuración guardada correctamente.";

            return resultado;
        }





        private async Task ReemplazarEstatus(int configuracionId, List<int> estatus)
        {
            var actuales =
                await _context
                .ConfiguracionValidacionEstatuses
                .Where(x =>
                    x.ConfiguracionValidacionTecnicaId ==
                    configuracionId)
                .ToListAsync();

            if (actuales.Any())
            {
                _context
                    .ConfiguracionValidacionEstatuses
                    .RemoveRange(actuales);
            }

            if (estatus != null && estatus.Any())
            {
                var nuevos =
                    estatus.Select(x =>
                        new ConfiguracionValidacionEstatus
                        {
                            ConfiguracionValidacionTecnicaId =
                                configuracionId,

                            EstatusSolicitudMantenimientoId =
                                x,

                            Activo =
                                true,

                            FechaCreacion =
                                DateTime.Now
                        });

                await _context
                    .ConfiguracionValidacionEstatuses
                    .AddRangeAsync(nuevos);
            }

            await _context.SaveChangesAsync();
        }





        private async Task GuardarEstatus(int configuracionId, List<int> estatus)
        {
            if (estatus == null || !estatus.Any())
                return;

            var entidades =
                estatus.Select(x =>
                    new ConfiguracionValidacionEstatus
                    {
                        ConfiguracionValidacionTecnicaId =
                            configuracionId,

                        EstatusSolicitudMantenimientoId =
                            x,

                        Activo =
                            true,

                        FechaCreacion =
                            DateTime.Now
                    });

            await _context
                .ConfiguracionValidacionEstatuses
                .AddRangeAsync(entidades);

            await _context.SaveChangesAsync();
        }



        public async Task<List<SelectListItem>> ObtenerConceptosPorTipo(int tipoCatalogoServicioId)
        {
            return await _context.ConceptoServicios
                .AsNoTracking()
                .Where(x =>
                    x.Activo &&
                    x.TipoCatalogoServicioId == tipoCatalogoServicioId)
                .OrderBy(x => x.ConceptoServicioNombre)
                .Select(x => new SelectListItem
                {
                    Value = x.ConceptoServicioId.ToString(),
                    Text = x.ConceptoServicioNombre
                })
                .ToListAsync();
        }





        public async Task<List<ConfiguracionValidacionTecnicaListDto>> ObtenerConfiguracionesValidacion()
        {
            var data =
                await (
                    from cfg in _context.ConfiguracionValidacionTecnicas

                    join tipo in _context.TipoCatalogoServicios
                        on cfg.TipoCatalogoServicioId equals tipo.TipoCatalogoServicioId

                    join con in _context.ConceptoServicios
                        on cfg.ConceptoServicioId equals con.ConceptoServicioId

                    join ce in _context.ConfiguracionValidacionEstatuses
                        on cfg.ConfiguracionValidacionTecnicaId equals ce.ConfiguracionValidacionTecnicaId into ceJoin

                    from ce in ceJoin.DefaultIfEmpty()

                    join est in _context.EstatusSolicitudMantenimientos
                        on ce.EstatusSolicitudMantenimientoId equals est.EstatusSolicitudMantenimientoId into estJoin

                    from est in estJoin.DefaultIfEmpty()

                    where cfg.Activo

                    select new ConfiguracionValidacionTecnicaListDto
                    {
                        ConfiguracionValidacionTecnicaId = cfg.ConfiguracionValidacionTecnicaId,

                        TipoCatalogoServicioId = (int)cfg.TipoCatalogoServicioId,
                        ConceptoServicioId = (int)cfg.ConceptoServicioId,

                        TipoCatalogoServicioNombre = tipo.TipoCatalogoServicioNombre,
                        ConceptoServicioNombre = con.ConceptoServicioNombre,

                        ValidarSolicitudesAbiertas = cfg.ValidarSolicitudesAbiertas,
                        ValidarRecurrencia = cfg.ValidarRecurrencia,
                        DiasRecurrencia = cfg.DiasRecurrencia,

                        EstatusSolicitudMantenimientoNombre =
                            est != null ? est.Nombre : "-",

                        Activo = cfg.Activo
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            return data;
        }



        public async Task<ResultadoOperacionDto> CrearConfiguracion(ConfiguracionValidacionTecnicaCreateDto dto)
        {
            var resultado = new ResultadoOperacionDto();

            // VALIDACIÓN BÁSICA
            if (dto.ConceptoServicioId <= 0 || dto.TipoCatalogoServicioId <= 0)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Debe seleccionar tipo y concepto.";
                return resultado;
            }

            // DUPLICADO
            var existe = await _context.ConfiguracionValidacionTecnicas.AnyAsync(x =>
                x.Activo &&
                x.TipoCatalogoServicioId == dto.TipoCatalogoServicioId &&
                x.ConceptoServicioId == dto.ConceptoServicioId);

            if (existe)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Ya existe configuración para este concepto.";
                return resultado;
            }

            var estatusCsv = dto.EstatusPermitidos != null && dto.EstatusPermitidos.Any()
                ? string.Join(",", dto.EstatusPermitidos)
                : null;

            var entidad = new ConfiguracionValidacionTecnica
            {
                TipoCatalogoServicioId = dto.TipoCatalogoServicioId,
                ConceptoServicioId = dto.ConceptoServicioId,
                ValidarSolicitudesAbiertas = dto.ValidarSolicitudesAbiertas,
                ValidarRecurrencia = dto.ValidarRecurrencia,
                DiasRecurrencia = dto.DiasRecurrencia,
                EstatusSolicitudesAbiertas = estatusCsv,
                Activo = dto.Activo,
                FechaCreacion = DateTime.Now
            };

            _context.ConfiguracionValidacionTecnicas.Add(entidad);
            await _context.SaveChangesAsync();

            // guardar estatus relacional
            if (dto.EstatusPermitidos.Any())
            {
                var estatus = dto.EstatusPermitidos.Select(x => new ConfiguracionValidacionEstatus
                {
                    ConfiguracionValidacionTecnicaId = entidad.ConfiguracionValidacionTecnicaId,
                    EstatusSolicitudMantenimientoId = Convert.ToInt32(x),
                    Activo = true
                });

                _context.ConfiguracionValidacionEstatuses.AddRange(estatus);
                await _context.SaveChangesAsync();
            }

            resultado.Exitoso = true;
            resultado.Mensaje = "Configuración creada correctamente.";

            return resultado;
        }


        //public async Task<ResultadoValidacionHistoricaDto> ValidarServiciosHistoricos(int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId, int? solicitudExcluirId = null)
        //{
        //    var resultado = new ResultadoValidacionHistoricaDto();

        //    var fechaActual = DateTime.Now;

        //    // =====================================================
        //    // CONFIGURACIÓN
        //    // =====================================================

        //    var configuraciones = await ObtenerConfiguracionValidacion(conceptos);

        //    if (!configuraciones.Any())
        //        return resultado;

        //    var configIds =
        //        configuraciones
        //            .Select(x => x.ConfiguracionValidacionTecnicaId)
        //            .Distinct()
        //            .ToList();

        //    var estatusPermitidos =
        //        await _context.ConfiguracionValidacionEstatuses
        //            .AsNoTracking()
        //            .Where(x =>
        //                x.Activo &&
        //                configIds.Contains(x.ConfiguracionValidacionTecnicaId))
        //            .ToListAsync();

        //    // SOLICITUDES ABIERTAS (BLOQUEANTE)

        //    //var solicitudesActivas =
        //    //    await (
        //    //        from s in _context.SolicitudMantenimientos
        //    //        join d in _context.SolicitudMantenimientoDetalles
        //    //            on s.SolicitudMantenimientoId equals d.SolicitudMantenimientoId
        //    //        join c in _context.ConceptoServicios
        //    //            on d.ConceptoServicioId equals c.ConceptoServicioId
        //    //        where s.Activo
        //    //        && d.Activo
        //    //        && s.UnidadVehicularId == unidadVehicularId
        //    //        && conceptos.Contains(d.ConceptoServicioId)
        //    //        && (!solicitudExcluirId.HasValue || s.SolicitudMantenimientoId != solicitudExcluirId.Value)



        //    //        select new
        //    //        {
        //    //            s.SolicitudMantenimientoId,
        //    //            s.Folio,
        //    //            s.EstatusSolicitudMantenimientoId,
        //    //            d.ConceptoServicioId,
        //    //            c.TipoCatalogoServicioId,
        //    //            c.ConceptoServicioNombre
        //    //        }
        //    //    )
        //    //    .AsNoTracking()
        //    //    .ToListAsync();

        //    //var duplicados =
        //    //    solicitudesActivas
        //    //        .Where(x =>
        //    //        {
        //    //            var regla =
        //    //                configuraciones.FirstOrDefault(r =>
        //    //                    r.ConceptoServicioId == x.ConceptoServicioId &&
        //    //                    r.TipoCatalogoServicioId == x.TipoCatalogoServicioId &&
        //    //                    r.ValidarSolicitudesAbiertas);

        //    //            if (regla == null)
        //    //                return false;

        //    //            var estatusAplicables =
        //    //                estatusPermitidos
        //    //                    .Where(e =>
        //    //                        e.ConfiguracionValidacionTecnicaId ==
        //    //                        regla.ConfiguracionValidacionTecnicaId)
        //    //                    .Select(e => e.EstatusSolicitudMantenimientoId)
        //    //                    .ToList();

        //    //            return estatusAplicables.Contains(x.EstatusSolicitudMantenimientoId);
        //    //        })
        //    //        .ToList();

        //    //if (duplicados.Any())
        //    //{
        //    //    resultado.TieneDuplicadosActivos = true;

        //    //    foreach (var x in duplicados)
        //    //    {
        //    //        resultado.Mensajes.Add(
        //    //            $"El servicio '{x.ConceptoServicioNombre}' ya existe en la solicitud {x.Folio}.");
        //    //    }
        //    //}

        //    //// HISTORIAL (RECURRENCIA - SOLO ALERTA)

        //    //var historial =
        //    //    await (
        //    //        from m in _context.Mantenimientos
        //    //        join md in _context.MantenimientoDetalles
        //    //            on m.MantenimientoId equals md.MantenimientoId
        //    //        join c in _context.ConceptoServicios
        //    //            on md.ConceptoServicioId equals c.ConceptoServicioId
        //    //        where m.Activo
        //    //            && m.FechaEjecucion != null
        //    //            && m.UnidadVehicularId == unidadVehicularId
        //    //            && conceptos.Contains(md.ConceptoServicioId)
        //    //        select new
        //    //        {
        //    //            md.ConceptoServicioId,
        //    //            c.TipoCatalogoServicioId,
        //    //            c.ConceptoServicioNombre,
        //    //            Fecha = m.FechaEjecucion.Value
        //    //        }
        //    //    )
        //    //    .AsNoTracking()
        //    //    .ToListAsync();

        //    //var historialReciente =
        //    //          historial
        //    //              .Where(x =>
        //    //              {
        //    //                  var regla =
        //    //                      configuraciones.FirstOrDefault(r =>
        //    //                          r.ConceptoServicioId == x.ConceptoServicioId &&
        //    //                          r.TipoCatalogoServicioId == x.TipoCatalogoServicioId &&
        //    //                          r.ValidarRecurrencia);

        //    //                  if (regla == null)
        //    //                      return false;

        //    //                  if (regla.DiasRecurrencia == null || regla.DiasRecurrencia <= 0)
        //    //                      return false;

        //    //                  var fechaMinima = fechaActual.AddDays(-regla.DiasRecurrencia);

        //    //                  return x.Fecha >= fechaMinima;
        //    //              })
        //    //              .ToList();




        //    //if (historialReciente.Any())
        //    //{
        //    //    resultado.TieneRecurrencia = true;

        //    //    foreach (var x in historialReciente)
        //    //    {
        //    //        var regla =
        //    //            configuraciones.First(r =>
        //    //                r.ConceptoServicioId == x.ConceptoServicioId &&
        //    //                r.TipoCatalogoServicioId == x.TipoCatalogoServicioId);

        //    //        resultado.Mensajes.Add(
        //    //            $"El servicio '{x.ConceptoServicioNombre}' ya fue realizado el {x.Fecha:dd/MM/yyyy}. Deben transcurrir al menos {regla.DiasRecurrencia} días.");
        //    //    }
        //    //}

        //    //// =====================================================
        //    //// RESULTADO FINAL
        //    //// =====================================================

        //    //resultado.EsValido =
        //    //    !resultado.TieneDuplicadosActivos &&
        //    //    !resultado.TieneRecurrencia;

        //    return resultado;
        //}


        public async Task<ResultadoValidacionHistoricaDto> ValidarServiciosHistoricos(int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId, int? solicitudExcluirId = null)
        {
            var fechaActual = DateTime.Now;

            //----------------------------------------------------
            // CONFIGURACIÓN
            //----------------------------------------------------

            var configuraciones = await ObtenerConfiguracionValidacion(conceptos);

            if (!configuraciones.Any())
            {
                return new ResultadoValidacionHistoricaDto
                {
                    EsValido = true
                };
            }

            //----------------------------------------------------
            // SOLICITUDES
            // NO se filtran estatus aquí.
            // El motor aplicará la configuración.
            //----------------------------------------------------

            var solicitudesActivas =
                await (
                    from s in _context.SolicitudMantenimientos

                    join d in _context.SolicitudMantenimientoDetalles
                        on s.SolicitudMantenimientoId equals d.SolicitudMantenimientoId

                    join c in _context.ConceptoServicios
                        on d.ConceptoServicioId equals c.ConceptoServicioId

                    where s.Activo
                        && d.Activo
                        && s.UnidadVehicularId == unidadVehicularId
                        && conceptos.Contains(d.ConceptoServicioId)
                        && (
                            !solicitudExcluirId.HasValue
                            || s.SolicitudMantenimientoId != solicitudExcluirId.Value
                        )

                    select new ValidacionSolicitudActivaDto
                    {
                        SolicitudMantenimientoId =
                            s.SolicitudMantenimientoId,

                        Folio =
                            s.Folio,

                        EstatusSolicitudMantenimientoId =
                            s.EstatusSolicitudMantenimientoId,

                        ConceptoServicioId =
                            d.ConceptoServicioId,

                        TipoCatalogoServicioId =
                            c.TipoCatalogoServicioId,

                        ConceptoServicioNombre =
                            c.ConceptoServicioNombre
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            //----------------------------------------------------
            // HISTORIAL EJECUTADO
            //----------------------------------------------------

            var historial =
                await (
                    from m in _context.Mantenimientos

                    join md in _context.MantenimientoDetalles
                        on m.MantenimientoId equals md.MantenimientoId

                    join c in _context.ConceptoServicios
                        on md.ConceptoServicioId equals c.ConceptoServicioId

                    where m.Activo
                        && m.EstatusMantenimientoId == 5
                        && m.FechaEjecucion != null
                        && m.UnidadVehicularId == unidadVehicularId
                        && conceptos.Contains(md.ConceptoServicioId)

                    select new ValidacionHistorialServicioDto
                    {
                        ConceptoServicioId =
                            md.ConceptoServicioId,

                        TipoCatalogoServicioId =
                            c.TipoCatalogoServicioId,

                        ConceptoServicioNombre =
                            c.ConceptoServicioNombre,

                        Fecha =
                            m.FechaEjecucion.Value
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            //----------------------------------------------------
            // MOTOR
            //----------------------------------------------------

            return EvaluarValidacionTecnica(
                configuraciones,
                solicitudesActivas,
                historial,
                fechaActual);
        }



        private async Task ValidarFacturaAsync(FacturaXmlDto factura, int mantenimientoId, DateOnly fechaRecepcion)
        {
            const string RFC_INSTITUCION = "SSP010130T45";

            //-----------------------------------
            // UUID
            //-----------------------------------

            if (string.IsNullOrWhiteSpace(
                factura.UUID))
            {
                throw new Exception("WARNING|El XML no contiene UUID.");
            }

            //-----------------------------------
            // DUPLICADA
            //-----------------------------------
            // SE QUITA MIENTRAS PARA LA PRUEBA 09/11/2026
            
            //if (
            //    await _context
            //    .FacturaMantenimientos
            //    .AnyAsync(x =>
            //        x.Uuid ==
            //        factura.UUID
            //        &&
            //        x.Activo))
            //{
            //    throw new Exception(
            //        "WARNING|La factura ya fue registrada anteriormente.");
            //}

            //-----------------------------------

            var mantenimiento =
                await _context
                .Mantenimientos
                .FirstOrDefaultAsync(x =>
                    x.MantenimientoId ==
                    mantenimientoId
                    &&
                    x.Activo);

            if (mantenimiento == null)
                throw new Exception(
                    "WARNING|No existe el mantenimiento.");

            //-----------------------------------
            // RFC EMISOR
            //-----------------------------------

            var taller =
                await _context
                .Tallers
                .FirstAsync(x =>
                    x.TallerId ==
                    mantenimiento.TallerId);

            if (!string.Equals(taller.Rfc?.Trim(), factura.RFCEmisor?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"WARNING|RFC emisor inválido ({factura.RFCEmisor}).");
            }

            //-----------------------------------
            // A RFC RECEPTOR
            //-----------------------------------

            if (!string.Equals(factura.RFCReceptor, RFC_INSTITUCION, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("WARNING|La factura no está emitida para la institución.");
            }

            //-----------------------------------
            // B FECHA TIMBRADO
            //-----------------------------------

            if (factura.FechaTimbrado != null && DateOnly.FromDateTime(factura.FechaTimbrado.Value) > fechaRecepcion)
            {
                throw new Exception("WARNING|Fecha recepción inválida.");
            }

            //-----------------------------------
            // C FECHA FUTURA
            //-----------------------------------

            if (factura.FechaFactura > DateTime.Now.AddDays(2))
            {
                throw new Exception("WARNING|La factura tiene fecha futura.");
            }

            //-----------------------------------
            // D IVA
            //-----------------------------------

            var ivaEsperado = Math.Round(factura.Subtotal * 0.16m, 2);

            if (Math.Abs(ivaEsperado - factura.IVA) > 1m)
            {
                throw new Exception("WARNING|IVA inconsistente.");
            }

            //-----------------------------------
            // E TOTAL
            //-----------------------------------

            var totalEsperado = factura.Subtotal + factura.IVA - factura.Retencion;

            if (Math.Abs(totalEsperado - factura.Total) > 1m)
            {
                throw new Exception("WARNING|Total CFDI inconsistente.");
            }




            // -------------------------------------------------------------------------
            // MONTOS: Validación de congruencia entre Mantenimiento y XML (IVA 16% Incluido)
            // -------------------------------------------------------------------------
            var totalMantenimientoConIva =
                await _context
                .MantenimientoDetalles
                .Where(x => x.MantenimientoId == mantenimientoId)
                .SumAsync(x => x.TotalLinea ?? ((x.SubtotalManoObra ?? 0m) + (x.SubtotalRefacciones ?? 0m)));
            
            // Como el mantenimiento trae IVA incluido, obtenemos su base equivalente para compararla con el subtotal de la factura
            var subtotalMantenimientoBase = Math.Round(totalMantenimientoConIva / 1.16m, 2);
            var diferencia = Math.Abs(subtotalMantenimientoBase - factura.Subtotal); // Tolerancia de .99 centavos
            if (diferencia >= 1m)
            {
                throw new Exception(
                    $"WARNING|Subtotal XML (${factura.Subtotal:N2}) distinto del subtotal base calculado del mantenimiento (${subtotalMantenimientoBase:N2}).");
            }



            //-----------------------------------

            if (factura.TipoComprobante
                != "I")
            {
                throw new Exception(
                    "WARNING|Solo CFDI ingreso.");
            }

            if (factura.Moneda
                != "MXN")
            {
                throw new Exception(
                    "WARNING|La moneda debe ser MXN.");
            }

            if (factura.Total <= 0)
            {
                throw new Exception(
                    "WARNING|Total inválido.");
            }

            //-----------------------------------
            //CHIPOTE SE QUITA PARA PRUEBAS
            //if (factura.FechaFactura <
            //    mantenimiento.FechaSalida)
            //{
            //    throw new Exception("WARNING|La fecha de la factura no puede ser menor a la fecha de mantenimiento realizado");
            //}




        }






        //public async Task RegistrarFacturaAsync(FacturaMantenimientoCreateDto dto, IFormFile xml, IFormFile pdf)
        //{

        public async Task<ValidacionSatDto> RegistrarFacturaAsync(FacturaMantenimientoCreateDto dto, IFormFile xml, IFormFile pdf)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            var archivosGuardados = new List<string>();

            try
            {
                // -------------------------------------------------
                // 1. IDENTIFICACIÓN DE USUARIO AUTENTICADO (GUID)
                // -------------------------------------------------
                var userClaims = _httpContextAccessor.HttpContext?.User;

                Guid? usuarioGuid = Guid.TryParse(
                    userClaims?.FindFirstValue(ClaimTypes.NameIdentifier) ?? userClaims?.FindFirstValue("sub"),
                    out var parsedGuid) ? parsedGuid : null;

                // -------------------------------------------------
                // 2. PARSEAR Y VALIDAR XML
                // -------------------------------------------------
                var factura = await ParsearFacturaAsync(xml);

                DateOnly fechaRecepcion = DateOnly.FromDateTime(dto.FechaRecepcion);
                await ValidarFacturaAsync(factura, dto.MantenimientoId, fechaRecepcion);

                // -------------------------------------------------
                // 3. VALIDACIÓN ANTE EL SAT
                // -------------------------------------------------
                var sat = await ValidarAsync(factura.UUID, factura.RFCEmisor, factura.RFCReceptor, factura.Total);

                if (sat.ConsultaExitosa)
                {
                    if (!string.Equals(sat.Estado, "Vigente", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"WARNING|El SAT reporta que el CFDI se encuentra en estado '{sat.Estado}'.");
                    }

                    if (string.Equals(sat.EstatusCancelacion, "Cancelado", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("WARNING|El CFDI aparece cancelado ante el SAT.");
                    }
                }
                else
                {
                    sat.Estado = "PENDIENTE";
                    sat.CodigoEstatus = "Servicio SAT no disponible temporalmente.";
                }

                // -------------------------------------------------
                // 4. OBTENER Y VALIDAR MANTENIMIENTO
                // -------------------------------------------------
                var mantenimiento = await _context.Mantenimientos
                    .FirstOrDefaultAsync(x => x.MantenimientoId == dto.MantenimientoId && x.Activo);

                if (mantenimiento == null)
                {
                    throw new Exception("ERROR|El mantenimiento especificado no existe o no se encuentra activo.");
                }

                // -------------------------------------------------
                // 5. REGISTRAR ENTIDAD FACTURA
                // -------------------------------------------------
                var entidad = new FacturaMantenimiento
                {
                    MantenimientoId = mantenimiento.MantenimientoId,
                    TallerId = mantenimiento.TallerId,
                    EstatusFacturaMantenimientoId = 1, // Registrada / Activa
                    Uuid = factura.UUID,
                    Serie = factura.Serie,
                    Folio = factura.Folio,
                    FechaFactura = DateOnly.FromDateTime(factura.FechaFactura),
                    FechaRecepcion = dto.FechaRecepcion,
                    FechaTimbrado = factura.FechaTimbrado,
                    Rfcemisor = factura.RFCEmisor,
                    Rfcreceptor = factura.RFCReceptor,
                    RazonSocial = factura.RazonSocial,
                    MetodoPago = factura.MetodoPago,
                    FormaPago = factura.FormaPago,
                    UsoCfdi = factura.UsoCFDI,
                    Moneda = factura.Moneda,
                    TipoComprobante = factura.TipoComprobante,
                    Subtotal = factura.Subtotal,
                    Iva = factura.IVA,
                    Retencion = factura.Retencion,
                    Total = factura.Total,
                    CodigoPostalEmision = factura.LugarExpedicion,
                    NoCertificado = factura.NoCertificado,
                    NoCertificadoSat = factura.NoCertificadoSAT,
                    Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim(),
                    FechaRegistro = DateTime.Now,
                    EstadoSat = sat.Estado,
                    FechaValidacionSat = sat.FechaConsulta,
                    CodigoEstatusSat = sat.CodigoEstatus,
                    UsuarioRegistroId = usuarioGuid, // Mapeo directo de uniqueidentifier
                    Activo = true
                };

                _context.FacturaMantenimientos.Add(entidad);
                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // 6. DETALLES DE FACTURA (CONGRUENCIA CON IVA 16% INCLUIDO)
                // -------------------------------------------------
                var detalles = await _context.MantenimientoDetalles
                    .Where(x => x.MantenimientoId == mantenimiento.MantenimientoId)
                    .ToListAsync();

                foreach (var item in detalles)
                {
                    // El Total de la línea ya incluye el IVA integrado desde el taller
                    decimal totalLineaConIva = item.TotalLinea ?? ((item.SubtotalManoObra ?? 0m) + (item.SubtotalRefacciones ?? 0m));

                    // Desglose matemático exacto por partida (Base y 16% de IVA)
                    decimal subtotalBaseLinea = Math.Round(totalLineaConIva / 1.16m, 2);
                    decimal ivaLinea = Math.Round(totalLineaConIva - subtotalBaseLinea, 2); // Operador de resta corregido

                    _context.FacturaMantenimientoDetalles.Add(new FacturaMantenimientoDetalle
                    {
                        FacturaMantenimientoId = entidad.FacturaMantenimientoId,
                        MantenimientoDetalleId = item.MantenimientoDetalleId,
                        ConceptoServicioId = item.ConceptoServicioId,
                        Cantidad = item.Cantidad,
                        CostoManoObra = Math.Round(item.CostoUnitarioManoObra / 1.16m, 2),
                        CostoRefacciones = Math.Round(item.CostoUnitarioRefacciones / 1.16m, 2),
                        Subtotal = subtotalBaseLinea,
                        Iva = ivaLinea,
                        TotalLinea = totalLineaConIva,
                        IdObjetoGastoManoObra = item.IdObjetoGastoManoObra,
                        IdObjetoGastoRefacciones = item.IdObjetoGastoRefacciones
                    });
                }

                await _context.SaveChangesAsync();

                // -------------------------------------------------
                // 7. MOVIMIENTOS PRESUPUESTALES (DEVENGADO)
                // -------------------------------------------------
                var movimientos = await _context.MovimientoPresupuestals
                    .Where(x => x.SolicitudMantenimientoId == mantenimiento.SolicitudMantenimientoId && x.Activo == true)
                    .ToListAsync();

                foreach (var m in movimientos)
                {
                    _context.MovimientoPresupuestals.Add(new MovimientoPresupuestal
                    {
                        IdDispPresupuestal = m.IdDispPresupuestal,
                        FechaMovimiento = DateTime.Now,
                        TipoMovimiento = "DEVENGADO",
                        Importe = m.Importe,
                        Observaciones = $"Devengado de Factura Folio/UUID: {factura.UUID}",
                        FechaCaptura = DateTime.Now,
                        UsuarioCaptura = usuarioGuid,
                        SolicitudMantenimientoId = mantenimiento.SolicitudMantenimientoId,
                        FacturaMantenimientoId = entidad.FacturaMantenimientoId,
                        Activo = true
                    });
                }

                // -------------------------------------------------
                // 8. ALMACENAR ARCHIVOS FÍSICOS (XML y PDF)
                // -------------------------------------------------
                await GuardarDocumentoFactura(entidad.FacturaMantenimientoId, xml, "XML", archivosGuardados);
                await GuardarDocumentoFactura(entidad.FacturaMantenimientoId, pdf, "PDF", archivosGuardados);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return sat;
            }
            catch
            {
                await tx.RollbackAsync();

                // Limpieza física de archivos creados si la transacción falla
                foreach (var ruta in archivosGuardados)
                {
                    if (File.Exists(ruta))
                    {
                        try { File.Delete(ruta); } catch { /* Ignorar errores de limpieza secundarias */ }
                    }
                }

                throw;
            }
        }






        public async Task<FacturaMantenimientoViewModel> PrepararVistaAsync(int mantenimientoId)
        {
            var mantenimiento =
                await _context.Mantenimientos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.MantenimientoId == mantenimientoId &&
                        x.Activo);

            if (mantenimiento == null)
                throw new Exception("El mantenimiento no existe.");

            if (mantenimiento.EstatusMantenimientoId != 5)
                throw new Exception("WARNING|Solo se pueden facturar mantenimientos finalizados.");

            var yaFacturado =
                await _context.FacturaMantenimientos
                    .AnyAsync(x =>
                        x.MantenimientoId == mantenimientoId &&
                        x.Activo);

            if (yaFacturado)
                throw new Exception("WARNING|Este mantenimiento ya tiene factura registrada.");

            var detalles =
                await _context.MantenimientoDetalles
                    .Include(x => x.ConceptoServicio)
                    .Where(x => x.MantenimientoId == mantenimientoId )
                    .ToListAsync();

            if (!detalles.Any())
                throw new Exception("WARNING|El mantenimiento no contiene detalles o partidas registradas.");

            var taller = await _context.Tallers.FirstOrDefaultAsync(x => x.TallerId == mantenimiento.TallerId);

            // Como los subtotales de la línea ya traen el IVA integrado (flujo anterior),
            // el importe total con IVA incluido es la suma de TotalLinea o (SubtotalManoObra + SubtotalRefacciones)
            var totalConIva = detalles.Sum(x => (x.TotalLinea ?? ((x.SubtotalManoObra ?? 0m) + (x.SubtotalRefacciones ?? 0m))));

            // Desglose matemático seguro (Base / 1.16 y el resto es IVA)
            var subtotalBase = Math.Round(totalConIva / 1.16m, 2);
            var ivaCalculado = Math.Round(totalConIva - subtotalBase, 2);
            var totalFinal = Math.Round(totalConIva, 2);

            return new FacturaMantenimientoViewModel
            {
                MantenimientoId = mantenimientoId,
                TallerId = mantenimiento.TallerId,
                RazonSocial = taller?.RazonSocial,
                FechaRecepcion = DateTime.Now,
                FechaFactura = DateTime.Today,

                Subtotal = subtotalBase,
                IVA = ivaCalculado,
                Total = totalFinal,

                Detalles = detalles.Select(x =>
                {
                    var totalLineaItem = x.TotalLinea ?? ((x.SubtotalManoObra ?? 0m) + (x.SubtotalRefacciones ?? 0m));
                    var baseItem = totalLineaItem / 1.16m;

                    return new FacturaDetalleViewModel
                    {
                        ConceptoServicioId = x.ConceptoServicioId,
                        ConceptoServicioNombre = x.ConceptoServicio?.ConceptoServicioNombre,
                        Cantidad = (int)x.Cantidad,
                        CostoManoObra = (x.CostoUnitarioManoObra / 1.16m), // Mostramos base unitaria si se requiere
                        CostoRefacciones = (x.CostoUnitarioRefacciones / 1.16m),
                        Subtotal = Math.Round(baseItem, 2)
                    };
                }).ToList()
            };
        }








        public async Task CruzarPresupuestoAsync(int mantenimientoId)
        {
            var usuarioGuid = Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : (Guid?)null;
            var detalles = await _context.MantenimientoDetalles.Where(x => x.MantenimientoId == mantenimientoId).ToListAsync();

            foreach (var d in detalles)
            {
                var importe =
                    (d.SubtotalManoObra + d.SubtotalRefacciones);
                if (importe <= 0)
                    continue;
                var movimiento = new MovimientoPresupuestal
                {
                    SolicitudMantenimientoId = mantenimientoId,
                    FechaMovimiento = DateTime.Now,
                    TipoMovimiento = "DEVENGADO",
                    Importe = importe,
                    Observaciones = $"Factura mantenimiento partida {d.ConceptoServicioId}",
                    FechaCaptura = DateTime.Now,
                    UsuarioCaptura = usuarioGuid,
                    Activo = true
                };

                _context.MovimientoPresupuestals.Add(movimiento);
            }

            await _context.SaveChangesAsync();
        }







        public async Task<FacturaMantenimientoDto> ObtenerFacturaAsync(int facturaId)
        {
            var factura =
                await _context
                    .FacturaMantenimientos
                    .Include(
                        x =>
                        x.FacturaMantenimientoDetalles)

                    .FirstOrDefaultAsync(
                        x =>
                        x.FacturaMantenimientoId ==
                        facturaId);

            if (factura == null)
                return null;

            return new FacturaMantenimientoDto
            {
                FacturaMantenimientoId =
                    factura
                    .FacturaMantenimientoId,

                MantenimientoId =
                    factura
                    .MantenimientoId,

                UUID =
                    factura.Uuid,

                Serie =
                    factura.Serie,

                Folio =
                    factura.Folio,

                FechaFactura =
                    factura
                    .FechaFactura
                    .ToDateTime(
                        TimeOnly.MinValue),

                FechaRecepcion =
                    factura
                    .FechaRecepcion,

                //FechaTimbrado =factura.FechaTimbrado,

                RFCEmisor =
                    factura.Rfcemisor,

                RFCReceptor =
                    factura.Rfcreceptor,

                RazonSocial =
                    factura.RazonSocial,

                MetodoPago =
                    factura.MetodoPago,

                FormaPago =
                    factura.FormaPago,

                UsoCFDI =
                    factura.UsoCfdi,

                Moneda =
                    factura.Moneda,

                TipoComprobante =
                    factura.TipoComprobante,

                Subtotal =
                    (decimal)factura.Subtotal,

                IVA =
                    (decimal)factura.Iva,

                Retencion =
                    (decimal)factura.Retencion,

                Total =
                    (decimal)factura.Total,

                Detalles =
                    factura
                    .FacturaMantenimientoDetalles
                    .Select(
                        x =>
                        new FacturaDetalleDto
                        {
                            ConceptoServicioId =
                                x.ConceptoServicioId,

                            Cantidad =
                                x.Cantidad,

                            Subtotal =
                                x.Subtotal,

                            IVA =
                                x.Iva,

                            Total =
                                x.TotalLinea
                        })
                    .ToList()
            };
        }






        public async Task<List<FacturaMantenimientoIndexDto>> ObtenerFacturasAsync()
        {
            var query =
                await (from f in _context.FacturaMantenimientos
                       join t in _context.Tallers
                           on f.TallerId equals t.TallerId
                       join m in _context.Mantenimientos
                           on f.MantenimientoId equals m.MantenimientoId
                       join u in _context.UnidadVehiculars
                           on m.UnidadVehicularId equals u.UnidadVehicularId
                       join e in _context.EstatusFacturaMantenimientos
                           on f.EstatusFacturaMantenimientoId equals e.EstatusFacturaMantenimientoId
                       where f.Activo
                       select new FacturaMantenimientoIndexDto
                       {
                           FacturaMantenimientoId = f.FacturaMantenimientoId,
                           UUID = f.Uuid,
                           Serie = f.Serie,
                           Folio = f.Folio,

                           TallerNombre = t.RazonSocial,
                           UnidadVehicular = u.NumeroEconomico,

                           FechaFactura = f.FechaFactura,
                           FechaRecepcion = f.FechaRecepcion,

                           Total = (decimal)f.Total,

                           Estatus = e.Nombre,
                           ColorEstatus = e.Color
                       })
                .AsNoTracking()
                .ToListAsync();

            return query;
        }





        public async Task<FacturaIndexViewModel> ObtenerDashboardFacturacionAsync(
    string? filtroUnidad = null,
    string? filtroTaller = null,
    string? filtroArea = null,
    DateTime? fechaDesde = null,
    DateTime? fechaHasta = null,
    bool esAdminMaquinaria = false,
    bool esAreaOperativa = false,
    string areaUsuario = "")
        {
            try
            {
                var hoy = DateTime.Now.Date;

                // 1. Consulta Base para Pendientes por Facturar
                var queryPendientes =
                    from m in _context.Mantenimientos
                    join u in _context.UnidadVehiculars on m.UnidadVehicularId equals u.UnidadVehicularId
                    join t in _context.Tallers on m.TallerId equals t.TallerId
                    join a in _context.Areas on u.AreaId equals a.AreaId
                    where m.Activo
                       && m.EstatusMantenimientoId == 5 // Estatus 5: Concluido / Finalizado
                       && !_context.FacturaMantenimientos.Any(f => f.MantenimientoId == m.MantenimientoId && f.Activo)
                    select new { m, u, t, a };

                // --------------------------------------------------
                // APLICACIÓN DE RESTRICCIONES DE SEGURIDAD POR ROL / ÁREA
                // --------------------------------------------------
                if (!esAdminMaquinaria)
                {
                    if (esAreaOperativa)
                    {
                        // Si es usuario operativo, confinar estrictamente a su área asignada por nombre
                        queryPendientes = queryPendientes.Where(x => x.a.AreaNombre == areaUsuario);
                    }
                    else
                    {
                        // Si no tiene privilegios de administrador ni rol válido
                        queryPendientes = queryPendientes.Where(x => false);
                    }
                }

                // --------------------------------------------------
                // APLICACIÓN DE FILTROS DINÁMICOS
                // --------------------------------------------------
                if (!string.IsNullOrWhiteSpace(filtroUnidad))
                {
                    string busquedaUnidad = filtroUnidad.Trim().ToUpper();
                    queryPendientes = queryPendientes.Where(x =>
                        x.u.NumeroEconomico.ToUpper().Contains(busquedaUnidad) ||
                        (x.u.PlacaActual != null && x.u.PlacaActual.ToUpper().Contains(busquedaUnidad)) ||
                        (x.m.Folio != null && x.m.Folio.ToUpper().Contains(busquedaUnidad)));
                }

                if (!string.IsNullOrWhiteSpace(filtroTaller))
                {
                    string busquedaTaller = filtroTaller.Trim().ToUpper();
                    queryPendientes = queryPendientes.Where(x =>
                        x.t.RazonSocial.ToUpper().Contains(busquedaTaller));
                }

                // Filtro por Área seguro (evita excepciones con int.TryParse)
                if (!string.IsNullOrWhiteSpace(filtroArea) && int.TryParse(filtroArea, out int idAreaFiltro))
                {
                    queryPendientes = queryPendientes.Where(x => x.a.AreaId == idAreaFiltro);
                }

                if (fechaDesde.HasValue)
                {
                    var fDesde = fechaDesde.Value.Date;
                    queryPendientes = queryPendientes.Where(x =>
                        x.m.FechaSalida.HasValue && x.m.FechaSalida.Value.Date >= fDesde);
                }

                if (fechaHasta.HasValue)
                {
                    var fHasta = fechaHasta.Value.Date;
                    queryPendientes = queryPendientes.Where(x =>
                        x.m.FechaSalida.HasValue && x.m.FechaSalida.Value.Date <= fHasta);
                }

                // 3. Proyección final del listado de Pendientes
                var pendientes = await queryPendientes
                    .Select(x => new FacturaPendienteViewModel
                    {
                        MantenimientoId = x.m.MantenimientoId,
                        Folio = x.m.Folio ?? "S/F",
                        Unidad = x.u.NumeroEconomico + " - " + x.u.Marca.MarcaNombre + " " + x.u.Modelo.ModeloNombre,
                        Placa = x.u.PlacaActual ?? "SIN PLACA",
                        Taller = x.t.RazonSocial,
                        FechaFinalizacion = x.m.FechaSalida,
                        DiasTranscurridos = x.m.FechaSalida.HasValue ? (hoy - x.m.FechaSalida.Value.Date).Days : 0,
                        CantidadConceptos = _context.MantenimientoDetalles.Count(d => d.MantenimientoId == x.m.MantenimientoId),
                        Area = x.a.AreaNombre,
                        TotalEstimado = _context.MantenimientoDetalles
                            .Where(d => d.MantenimientoId == x.m.MantenimientoId)
                            .Sum(d => (decimal?)d.TotalLinea) ?? 0m
                    })
                    .OrderByDescending(p => p.FechaFinalizacion)
                    .ToListAsync();

                // 4. Listado de Facturas Registradas (Aplicando las mismas reglas de seguridad y filtros)
                var queryFacturas =
                    from f in _context.FacturaMantenimientos
                    join t in _context.Tallers on f.TallerId equals t.TallerId
                    join m in _context.Mantenimientos on f.MantenimientoId equals m.MantenimientoId
                    join u in _context.UnidadVehiculars on m.UnidadVehicularId equals u.UnidadVehicularId
                    join a in _context.Areas on u.AreaId equals a.AreaId
                    where f.Activo
                    select new { f, t, m, u, a };

                if (!esAdminMaquinaria)
                {
                    if (esAreaOperativa)
                    {
                        queryFacturas = queryFacturas.Where(x => x.a.AreaNombre == areaUsuario);
                    }
                    else
                    {
                        queryFacturas = queryFacturas.Where(x => false);
                    }
                }

                // Aplicar filtro de área en facturas registradas de forma segura
                if (!string.IsNullOrWhiteSpace(filtroArea) && int.TryParse(filtroArea, out int idAreaFacturaFiltro))
                {
                    queryFacturas = queryFacturas.Where(x => x.a.AreaId == idAreaFacturaFiltro);
                }

                if (!string.IsNullOrWhiteSpace(filtroTaller))
                {
                    string busquedaTaller = filtroTaller.Trim().ToUpper();
                    queryFacturas = queryFacturas.Where(x => x.t.RazonSocial.ToUpper().Contains(busquedaTaller));
                }

                var facturas = await queryFacturas
                    .OrderByDescending(x => x.f.FechaRegistro)
                    .Select(x => new FacturaListadoViewModel
                    {
                        FacturaId = x.f.FacturaMantenimientoId,
                        UUID = x.f.Uuid,
                        Taller = x.t.RazonSocial,
                        Fecha = x.f.FechaFactura,
                        Total = (decimal)x.f.Total
                    })
                    .ToListAsync();

                return new FacturaIndexViewModel
                {
                    Pendientes = pendientes,
                    Facturas = facturas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el dashboard de facturación con filtros y restricciones de área.");
                return new FacturaIndexViewModel();
            }
        }






        public async Task ValidarFacturaAsync(FacturaParseadaDto factura, int mantenimientoId)
        {
            var mantenimiento =
                await _context.Mantenimientos
                    .FirstAsync(
                        x =>
                        x.MantenimientoId ==
                        mantenimientoId);

            var taller =
                await _context.Tallers
                    .FirstAsync(
                        x =>
                        x.TallerId ==
                        mantenimiento.TallerId);

            if (
                await _context
                    .FacturaMantenimientos
                    .AnyAsync(
                        x =>
                        x.Uuid ==
                        factura.UUID
                    ))
            {
                throw new Exception(
                    "WARNING|La factura ya fue registrada.");
            }

            if (
                factura.RFCEmisor
                !=
                taller.Rfc
                )
            {
                throw new Exception(
                    "WARNING|El RFC emisor no corresponde al taller.");
            }

            if (
                factura.TipoComprobante
                !=
                "I")
            {
                throw new Exception(
                    "WARNING|Solo se permiten CFDI tipo ingreso.");
            }

            if (
                factura.Moneda
                !=
                "MXN")
            {
                throw new Exception(
                    "WARNING|Solo se permiten facturas en MXN.");
            }

            if (
                mantenimiento.FechaFinTrabajo
                != null
                &&
                factura.FechaFactura
                <
                mantenimiento.FechaFinTrabajo
            )
            {
                throw new Exception(
                    "WARNING|La factura es anterior al cierre del mantenimiento.");
            }

            var diferencia =
                Math.Abs(
                    (decimal)(mantenimiento.CostoRealTotal
                    -
                    factura.Total));

            if (diferencia > 100)
            {
                throw new Exception(
                    $"WARNING|La factura excede tolerancia. Diferencia: {diferencia:N2}");
            }
        }




        private async Task GuardarDocumentoFactura(int facturaId, IFormFile archivo, string tipoDocumento, List<string> archivosGuardados)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;

            Guid? usuarioGuid = Guid.TryParse(
                userClaims?.FindFirstValue(ClaimTypes.NameIdentifier) ?? userClaims?.FindFirstValue("sub"),
                out var parsedGuid) ? parsedGuid : null;

            if (archivo == null || archivo.Length <= 0)
            {
                throw new Exception(
                    $"WARNING|No se recibió el archivo {tipoDocumento}.");
            }

            const long tamanioMaximo =
                2 * 1024 * 1024; // 2 MB

            if (archivo.Length > tamanioMaximo)
            {
                throw new Exception(
                    $"WARNING|El archivo '{archivo.FileName}' excede el tamaño permitido.");
            }

            var extensionesPermitidas =
                new Dictionary<string, string[]>
                {
                    ["PDF"] = new[] { ".pdf" },
                    ["XML"] = new[] { ".xml" }
                };

            var contentTypesPermitidos =
                new Dictionary<string, string[]>
                {
                    ["PDF"] = new[]
                    {
                "application/pdf"
                    },

                    ["XML"] = new[]
                    {
                "application/xml",
                "text/xml"
                    }
                };

            var extension =
                Path.GetExtension(
                    archivo.FileName)
                .ToLowerInvariant();

            if (!extensionesPermitidas[tipoDocumento]
                .Contains(extension))
            {
                throw new Exception(
                    $"WARNING|El archivo {tipoDocumento} tiene una extensión inválida.");
            }

            var contentType =
                archivo.ContentType?
                    .Trim()
                    .ToLowerInvariant();

            if (!contentTypesPermitidos[tipoDocumento]
                .Contains(contentType))
            {
                throw new Exception(
                    $"WARNING|El archivo {tipoDocumento} tiene MIME inválido.");
            }

            byte[] bytes;

            using (
                var memory =
                new MemoryStream())
            {
                await archivo.CopyToAsync(memory);

                bytes =
                    memory.ToArray();
            }

            // ===================================
            // VALIDACIÓN BINARIA
            // ===================================

            if (tipoDocumento == "PDF")
            {
                var firmaPdf =
                    bytes
                    .Take(4)
                    .SequenceEqual(
                        new byte[]
                        {
                    0x25,
                    0x50,
                    0x44,
                    0x46
                        });

                if (!firmaPdf)
                {
                    throw new Exception(
                        "WARNING|El contenido del PDF es inválido.");
                }
            }

            if (tipoDocumento == "XML")
            {
                try
                {
                    using var ms =
                        new MemoryStream(bytes);

                    XDocument.Load(ms);
                }
                catch
                {
                    throw new Exception(
                        "WARNING|El XML no tiene una estructura válida.");
                }
            }

            // ===================================
            // HASH
            // ===================================

            string hash;

            using (
                var sha =
                SHA256.Create())
            {
                hash =
                    Convert.ToHexString(
                        sha.ComputeHash(bytes));
            }

            // ===================================
            // RUTA
            // ===================================

            var rutaBase = _configuration["Storage:FacturasRuta"];

            if (string.IsNullOrWhiteSpace(rutaBase))
            {
                throw new Exception(
                    "WARNING|No existe configuración de almacenamiento.");
            }

            var carpeta =
                Path.Combine(
                    rutaBase,
                    facturaId.ToString());

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            var nombreFisico =
                $"{Guid.NewGuid()}{extension}";

            var rutaCompleta =
                Path.Combine(
                    carpeta,
                    nombreFisico);

            await File.WriteAllBytesAsync(
                rutaCompleta,
                bytes);

            archivosGuardados.Add(
                rutaCompleta);

            // ===================================
            // REGISTRO DB
            // ===================================

            var documento =
                new FacturaMantenimientoDocumento
                {
                    FacturaMantenimientoId =
                        facturaId,

                    TipoDocumento =
                        tipoDocumento,

                    NombreArchivo =
                        Path.GetFileName(
                            archivo.FileName),

                    RutaArchivo =
                        rutaCompleta,

                    HashArchivo =
                        hash,

                    FechaCarga =
                        DateTime.Now,

                    UsuarioCargaId = usuarioGuid, 

                    Activo =
                        true
                };

            await _context
                .FacturaMantenimientoDocumentos
                .AddAsync(documento);
        }





        private async Task<FacturaXmlDto> ParsearFacturaAsync(IFormFile xml)
        {
            //-------------------------------------------------
            // VALIDACIONES ARCHIVO
            //-------------------------------------------------

            if (xml == null || xml.Length == 0)
                throw new Exception(
                    "WARNING|Debe adjuntar XML.");

            if (xml.Length > (5 * 1024 * 1024))
                throw new Exception(
                    "WARNING|El XML excede el tamaño permitido (5 MB).");

            var extension =
                Path.GetExtension(xml.FileName);

            if (!string.Equals(
                extension,
                ".xml",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "WARNING|El archivo debe ser XML.");
            }

            if (
                !xml.ContentType.Contains(
                    "xml",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "WARNING|Tipo de archivo XML inválido.");
            }

            //-------------------------------------------------

            await using var stream =
                xml.OpenReadStream();

            var doc =
                XDocument.Load(stream);

            XNamespace cfdi =
                "http://www.sat.gob.mx/cfd/4";

            XNamespace tfd =
                "http://www.sat.gob.mx/TimbreFiscalDigital";

            var comprobante =
                doc.Element(cfdi + "Comprobante");

            if (comprobante == null)
                throw new Exception(
                    "WARNING|XML CFDI inválido.");

            var emisor =
                comprobante.Element(
                    cfdi + "Emisor");

            var receptor =
                comprobante.Element(
                    cfdi + "Receptor");

            var impuestos =
                comprobante.Element(
                    cfdi + "Impuestos");

            var timbre =
                comprobante
                .Descendants(
                    tfd +
                    "TimbreFiscalDigital")
                .FirstOrDefault();

            //-------------------------------------------------
            // F TIMBRADO
            //-------------------------------------------------

            if (timbre == null)
            {
                throw new Exception("WARNING|El CFDI no contiene timbre SAT.");
            }

            //-------------------------------------------------

            var conceptos =
                comprobante
                .Descendants(cfdi + "Concepto")
                .Select(x =>
                {
                    decimal iva = 0;

                    decimal.TryParse(
                        x.Descendants(cfdi + "Traslado")
                        .FirstOrDefault()
                        ?.Attribute("Importe")
                        ?.Value,
                        out iva);

                    return new FacturaConceptoXmlDto
                    {
                        Descripcion =
                            x.Attribute("Descripcion")?.Value,

                        Cantidad =
                            Convert.ToDecimal(
                                x.Attribute("Cantidad")?.Value),

                        ValorUnitario =
                            Convert.ToDecimal(
                                x.Attribute("ValorUnitario")?.Value),

                        Importe =
                            Convert.ToDecimal(
                                x.Attribute("Importe")?.Value),

                        IVA = iva
                    };

                }).ToList();

            decimal ivaTotal = 0;

            decimal.TryParse(
                impuestos
                ?.Attribute(
                    "TotalImpuestosTrasladados")
                ?.Value,
                out ivaTotal);

            return new FacturaXmlDto
            {
                UUID =
                    timbre.Attribute("UUID")?.Value,

                Serie =
                    comprobante.Attribute("Serie")?.Value,

                Folio =
                    comprobante.Attribute("Folio")?.Value,

                FechaFactura =
                    DateTime.Parse(
                        comprobante
                        .Attribute("Fecha")
                        .Value),

                FechaTimbrado =
                    DateTime.Parse(
                        timbre
                        .Attribute("FechaTimbrado")
                        .Value),

                RFCEmisor =
                    emisor
                    ?.Attribute("Rfc")
                    ?.Value,

                RFCReceptor =
                    receptor
                    ?.Attribute("Rfc")
                    ?.Value,

                RazonSocial =
                    emisor
                    ?.Attribute("Nombre")
                    ?.Value,

                MetodoPago =
                    comprobante
                    .Attribute("MetodoPago")
                    ?.Value,

                FormaPago =
                    comprobante
                    .Attribute("FormaPago")
                    ?.Value,

                UsoCFDI =
                    receptor
                    ?.Attribute("UsoCFDI")
                    ?.Value,

                Moneda =
                    comprobante
                    .Attribute("Moneda")
                    ?.Value,

                TipoComprobante =
                    comprobante
                    .Attribute("TipoDeComprobante")
                    ?.Value,

                LugarExpedicion =
                    comprobante
                    .Attribute("LugarExpedicion")
                    ?.Value,

                NoCertificado =
                    comprobante
                    .Attribute("NoCertificado")
                    ?.Value,

                NoCertificadoSAT =
                    timbre
                    .Attribute("NoCertificadoSAT")
                    ?.Value,

                Subtotal =
                    Convert.ToDecimal(
                        comprobante
                        .Attribute("SubTotal")
                        .Value),

                IVA =
                    ivaTotal,

                Retencion =
                    0,

                Total =
                    Convert.ToDecimal(
                        comprobante
                        .Attribute("Total")
                        .Value),

                Conceptos =
                    conceptos
            };
        }





public async Task<ValidacionSatDto> ValidarAsync(string uuid,string rfcEmisor,string rfcReceptor,    decimal total,CancellationToken cancellationToken = default)
    {
        // =====================================================
        // 1. FORMATO EXPRESIÓN IMPRESA (ESTÁNDAR VIGENTE SAT)
        // =====================================================
        var expresion = $"?re={rfcEmisor}&rr={rfcReceptor}&tt={total.ToString("0.######", CultureInfo.InvariantCulture)}&id={uuid}";
        var url = "https://consultaqr.facturaelectronica.sat.gob.mx/ConsultaCFDIService.svc";

        // =====================================================
        // 2. CONSTRUCCIÓN SOAP REQUEST 1.1 / 1.2
        // =====================================================
        var soapEnvelope = $"""
        <?xml version="1.0" encoding="utf-8"?>
        <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:temp="http://tempuri.org/">
           <soapenv:Header/>
           <soapenv:Body>
              <temp:Consulta>
                 <temp:expresionImpresa>{SecurityElement.Escape(expresion)}</temp:expresionImpresa>
              </temp:Consulta>
           </soapenv:Body>
        </soapenv:Envelope>
        """;

        try
        {
            // =====================================================
            // 3. CONFIGURAR REQUISICIÓN CON TIMEOUT Y CANCELLATION
            // =====================================================
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            request.Headers.Add("SOAPAction", "\"http://tempuri.org/IConsultaCFDIService/Consulta\"");

            // Se agrega un CancellationTokenSource para establecer un timeout máximo de 10 segundos
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            // =====================================================
            // 4. CONSUMO DEL SERVICIO WEB SAT
            // =====================================================
            using var response = await _http.SendAsync(request, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                return new ValidacionSatDto
                {
                    ConsultaExitosa = false,
                    EsValida = false,
                    Estado = "PENDIENTE",
                    CodigoEstatus = $"SAT devolvió un código HTTP de error: {(int)response.StatusCode} ({response.ReasonPhrase})",
                    FechaConsulta = DateTime.Now,
                    Mensaje = $"Respuesta Http no exitosa desde el servicio del SAT. Estatus: {response.StatusCode}"
                };
            }

            var contenido = await response.Content.ReadAsStringAsync(cts.Token);

            if (string.IsNullOrWhiteSpace(contenido))
            {
                return new ValidacionSatDto
                {
                    ConsultaExitosa = false,
                    EsValida = false,
                    Estado = "PENDIENTE",
                    CodigoEstatus = "Servicio SAT devolvió un cuerpo de respuesta vacío.",
                    FechaConsulta = DateTime.Now
                };
            }

            // =====================================================
            // 5. PARSEO SEGURO DEL XML DE RESPUESTA
            // =====================================================
            var doc = XDocument.Parse(contenido);

            string? ObtenerValorNodo(string nombreNodo)
            {
                return doc.Descendants()
                    .FirstOrDefault(x => string.Equals(x.Name.LocalName, nombreNodo, StringComparison.OrdinalIgnoreCase))
                    ?.Value
                    ?.Trim();
            }

            var estado = ObtenerValorNodo("Estado");
            var codigoEstatus = ObtenerValorNodo("CodigoEstatus");
            var cancelable = ObtenerValorNodo("EsCancelable");
            var estatusCancelacion = ObtenerValorNodo("EstatusCancelacion");

            // Validar respuesta estructural
            if (string.IsNullOrWhiteSpace(estado))
            {
                return new ValidacionSatDto
                {
                    ConsultaExitosa = false,
                    EsValida = false,
                    Estado = "PENDIENTE",
                    CodigoEstatus = codigoEstatus ?? "SAT no devolvió el nodo 'Estado' del CFDI.",
                    FechaConsulta = DateTime.Now
                };
            }

            bool esVigente = string.Equals(estado, "Vigente", StringComparison.OrdinalIgnoreCase);

            // =====================================================
            // 6. RESPUESTA DE ÉXITO EN CONSULTA
            // =====================================================
            return new ValidacionSatDto
            {
                ConsultaExitosa = true,
                EsValida = esVigente,
                Estado = estado.ToUpper(),
                CodigoEstatus = codigoEstatus,
                Cancelable = cancelable,
                EstatusCancelacion = estatusCancelacion,
                FechaConsulta = DateTime.Now,
                Mensaje = null
            };
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Se disparó el timeout de 10 segundos
            return new ValidacionSatDto
            {
                ConsultaExitosa = false,
                EsValida = false,
                Estado = "PENDIENTE",
                CodigoEstatus = "Tiempo de espera agotado al conectar con el SAT (Timeout 10s).",
                FechaConsulta = DateTime.Now,
                Mensaje = "El servidor del SAT tardó demasiado en responder: " + ex.Message
            };
        }
        catch (Exception ex)
        {
            // Captura cualquier falla de red, DNS, TLS o deserialización sin romper el flujo del sistema
            return new ValidacionSatDto
            {
                ConsultaExitosa = false,
                EsValida = false,
                Estado = "PENDIENTE",
                CodigoEstatus = "Servicio SAT no disponible temporalmente.",
                FechaConsulta = DateTime.Now,
                Mensaje = ex.Message
            };
        }
    }






    private ResultadoValidacionHistoricaDto EvaluarValidacionTecnica(List<ConfiguracionValidacionTecnicaDto> configuraciones, List<ValidacionSolicitudActivaDto> solicitudesActivas,
        List<ValidacionHistorialServicioDto> historial, DateTime fechaActual)
        {
            var resultado = new ResultadoValidacionHistoricaDto
            {
                EsValido = true
            };

            //--------------------------------------------------------
            // RECORRER CADA REGLA CONFIGURADA
            //--------------------------------------------------------

            foreach (var regla in configuraciones)
            {
                //----------------------------------------------------
                // VALIDAR SOLICITUDES ABIERTAS
                //----------------------------------------------------

                if (regla.ValidarSolicitudesAbiertas)
                {
                    var solicitudes = solicitudesActivas
                        .Where(x =>
                            x.ConceptoServicioId == regla.ConceptoServicioId
                            &&
                            x.TipoCatalogoServicioId == regla.TipoCatalogoServicioId);

                    if (regla.EstatusPermitidos.Any())
                    {
                        solicitudes =
                            solicitudes.Where(x =>
                                regla.EstatusPermitidos.Contains(
                                    x.EstatusSolicitudMantenimientoId));
                    }

                    foreach (var solicitud in solicitudes)
                    {
                        resultado.TieneDuplicadosActivos = true;

                        resultado.Mensajes.Add(
                            $"El servicio '{solicitud.ConceptoServicioNombre}' ya existe en la solicitud {solicitud.Folio}.");
                    }
                }

                //----------------------------------------------------
                // VALIDAR RECURRENCIA
                //----------------------------------------------------

                if (regla.ValidarRecurrencia)
                {
                    var fechaLimite =
                        fechaActual.AddDays(-regla.DiasRecurrencia);

                    var serviciosHistoricos =
                        historial
                            .Where(x =>
                                x.ConceptoServicioId == regla.ConceptoServicioId
                                &&
                                x.TipoCatalogoServicioId == regla.TipoCatalogoServicioId
                                &&
                                x.Fecha >= fechaLimite)
                            .OrderByDescending(x => x.Fecha)
                            .ToList();

                    if (serviciosHistoricos.Any())
                    {
                        resultado.TieneRecurrencia = true;

                        foreach (var servicio in serviciosHistoricos)
                        {
                            resultado.Mensajes.Add(
                                $"El servicio '{servicio.ConceptoServicioNombre}' fue realizado el {servicio.Fecha:dd/MM/yyyy}. Deben transcurrir al menos {regla.DiasRecurrencia} días.");
                        }
                    }
                }
            }

            //--------------------------------------------------------
            // ELIMINAR MENSAJES DUPLICADOS
            //--------------------------------------------------------

            resultado.Mensajes =
                resultado.Mensajes
                    .Distinct()
                    .ToList();

            //--------------------------------------------------------
            // NIVEL DE RIESGO
            //--------------------------------------------------------

            if (resultado.TieneDuplicadosActivos)
            {
                resultado.NivelRiesgo = "ALTO";
            }
            else if (resultado.TieneRecurrencia)
            {
                resultado.NivelRiesgo = "MEDIO";
            }
            else
            {
                resultado.NivelRiesgo = "BAJO";
            }

            //--------------------------------------------------------
            // RESULTADO
            //--------------------------------------------------------

            resultado.EsValido =
                !resultado.TieneDuplicadosActivos;

            return resultado;
        }








        // =========================================================
        // OBTENER TALLERES
        // =========================================================

        public async Task<PaginacionDto<TallerDto>> ObtenerTalleresAsync(
     int pagina = 1,
     int registrosPorPagina = 10,
     string? busqueda = null,
     bool? activo = null)
        {
            // =====================================================
            // 1. VALIDAR PAGINACIÓN
            // =====================================================

            if (pagina < 1)
                pagina = 1;

            if (registrosPorPagina <= 0)
                registrosPorPagina = 10;


            // =====================================================
            // 2. CONSULTA BASE
            // =====================================================

            IQueryable<Taller> consulta =
                _context.Tallers
                    .AsNoTracking();


            // =====================================================
            // 3. FILTRO POR ESTADO
            // =====================================================

            if (activo.HasValue)
            {
                consulta =
                    consulta.Where(
                        x => x.Activo == activo.Value);
            }


            // =====================================================
            // 4. BÚSQUEDA
            // =====================================================
            //
            // Los datos presupuestales ya no pertenecen directamente
            // a Taller.
            //
            // Se consultan mediante:
            //
            // Taller
            //   -> TallerConfiguracion
            //   -> TallerConfiguracionPresupuestal
            //
            // =====================================================

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                consulta =
                    consulta.Where(x =>
                        x.RazonSocial.Contains(busqueda) ||
                        x.Rfc.Contains(busqueda) ||
                        x.Ubicacion.Contains(busqueda) ||

                        x.Banco.Nombre.Contains(busqueda) ||

                        (x.Fianza != null &&
                         x.Fianza.Contains(busqueda)) ||

                        (x.Clabe != null &&
                         x.Clabe.Contains(busqueda)) ||

                        x.TallerConfiguracions.Any(tc =>
                            tc.Activo &&
                            tc.TallerConfiguracionPresupuestal.Activo &&
                            (
                                tc.TallerConfiguracionPresupuestal
                                    .Contrato
                                    .Contains(busqueda)

                                ||

                                tc.TallerConfiguracionPresupuestal
                                    .Recurso
                                    .Nombre
                                    .Contains(busqueda)
                            ))
                    );
            }


            // =====================================================
            // 5. TOTAL DE REGISTROS
            // =====================================================

            int totalRegistros =
                await consulta.CountAsync();


            // =====================================================
            // 6. TOTAL DE PÁGINAS
            // =====================================================

            int totalPaginas =
                totalRegistros == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalRegistros /
                        (double)registrosPorPagina);


            // =====================================================
            // 7. AJUSTAR PÁGINA
            // =====================================================

            if (totalPaginas > 0 &&
                pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }


            // =====================================================
            // 8. OBTENER REGISTROS
            // =====================================================

            List<TallerDto> registros =
                await consulta
                    .OrderBy(x => x.RazonSocial)
                    .ThenBy(x => x.TallerId)

                    .Skip(
                        (pagina - 1) *
                        registrosPorPagina)

                    .Take(registrosPorPagina)

                    .Select(x => new TallerDto
                    {
                        // =================================================
                        // DATOS DEL TALLER
                        // =================================================

                        TallerId =
                            x.TallerId,

                        RazonSocial =
                            x.RazonSocial,

                        Rfc =
                            x.Rfc,

                        BancoId =
                            x.BancoId,

                        BancoNombre =
                            x.Banco.Nombre,

                        Fianza =
                            x.Fianza,

                        Clabe =
                            x.Clabe,

                        Ubicacion =
                            x.Ubicacion,

                        Activo =
                            x.Activo,


                        // =================================================
                        // CONFIGURACIÓN PRESUPUESTAL
                        // =================================================

                        RecursoId =
                        x.TallerConfiguracions
                            .Where(tc =>
                                tc.Activo &&
                                tc.TallerConfiguracionPresupuestal.Activo)
                            .Select(tc =>
                                tc.TallerConfiguracionPresupuestal.RecursoId)
                            .FirstOrDefault(),

                        RecursoNombre =
                        x.TallerConfiguracions
                            .Where(tc =>
                                tc.Activo &&
                                tc.TallerConfiguracionPresupuestal.Activo)
                            .Select(tc =>
                                tc.TallerConfiguracionPresupuestal
                                    .Recurso
                                    .Nombre)
                            .FirstOrDefault(),

                        MontoMinimo =
                        x.TallerConfiguracions
                            .Where(tc =>
                                tc.Activo &&
                                tc.TallerConfiguracionPresupuestal.Activo)
                            .Select(tc =>
                                tc.TallerConfiguracionPresupuestal.MontoMinimo)
                            .FirstOrDefault(),

                        MontoMaximo =
                        x.TallerConfiguracions
                            .Where(tc =>
                                tc.Activo &&
                                tc.TallerConfiguracionPresupuestal.Activo)
                            .Select(tc =>
                                tc.TallerConfiguracionPresupuestal.MontoMaximo)
                            .FirstOrDefault(),

                        Contrato =
                            x.TallerConfiguracions
                                .Where(tc =>
                                    tc.Activo &&
                                    tc.TallerConfiguracionPresupuestal.Activo)
                                .Select(tc =>
                                    tc.TallerConfiguracionPresupuestal
                                        .Contrato)
                                .FirstOrDefault(),


                    })

                    .ToListAsync();


            // =====================================================
            // 9. RETORNAR RESULTADO
            // =====================================================

            return new PaginacionDto<TallerDto>
            {
                Registros =
                    registros,

                PaginaActual =
                    pagina,

                RegistrosPorPagina =
                    registrosPorPagina,

                TotalRegistros =
                    totalRegistros,

                TotalPaginas =
                    totalPaginas
            };
        }



        public async Task<List<RecursoDto>> ObtenerRecursosActivosAsync()
        {
            return await _context.Recursos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new RecursoDto
                {
                    RecursoId = x.RecursoId,
                    Nombre = x.Nombre
                })
                .ToListAsync();
        }




        public async Task<List<BancoDto>> ObtenerBancosActivosAsync()
        {
            return await _context.Bancos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new BancoDto
                {
                    BancoId =
                        x.BancoId,

                    Nombre =
                        x.Nombre
                })
                .ToListAsync();
        }





        public async Task<List<FianzaDto>> ObtenerFianzasActivasAsync()
        {
            return await _context.Fianzas
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Fianza1)
                .Select(x => new FianzaDto
                {
                    FianzaId =
                        x.FianzaId,

                    Nombre =
                        x.Fianza1
                })
                .ToListAsync();
        }



        public async Task<List<ObjetivoMantenimientoDto>> ObtenerObjetivosMantenimientoActivosAsync()
        {
            return await _context.ObjetivoMantenimientos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x => new ObjetivoMantenimientoDto
                {
                    ObjetivoMantenimientoId =
                        x.ObjetivoMantenimientoId,

                    Nombre =
                        x.Nombre
                })
                .ToListAsync();
        }





        public async Task<ResultadoOperacion> CrearTallerAsync(TallerCreateDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. VALIDAR DTO
                // =====================================================

                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información del taller no es válida."
                    };
                }


                // =====================================================
                // 2. NORMALIZAR DATOS
                // =====================================================

                dto.RazonSocial =
                    dto.RazonSocial?.Trim() ?? string.Empty;

                dto.Rfc =
                    dto.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

                dto.Contrato =
                    dto.Contrato?.Trim() ?? string.Empty;

                dto.Fianza =
                    dto.Fianza?.Trim() ?? string.Empty;

                dto.Clabe =
                    dto.Clabe?.Trim() ?? string.Empty;

                dto.Ubicacion =
                    dto.Ubicacion?.Trim() ?? string.Empty;


                // =====================================================
                // 3. VALIDAR CAMPOS OBLIGATORIOS
                // =====================================================

                if (string.IsNullOrWhiteSpace(dto.RazonSocial))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La razón social es obligatoria."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Rfc))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El RFC es obligatorio."
                    };
                }


                if (dto.RecursoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un recurso válido."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Contrato))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El contrato es obligatorio."
                    };
                }


                if (dto.BancoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un banco válido."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Fianza))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fianza es obligatoria."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Clabe))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La CLABE interbancaria es obligatoria."
                    };
                }


                if (dto.ObjetivoMantenimientoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un objetivo de mantenimiento válido."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Ubicacion))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La ubicación es obligatoria."
                    };
                }


                // =====================================================
                // 4. VALIDAR LONGITUDES
                // =====================================================

                if (dto.RazonSocial.Length > 200)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La razón social no puede exceder 200 caracteres."
                    };
                }


                if (dto.Rfc.Length != 12 &&
                    dto.Rfc.Length != 13)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El RFC debe contener 12 o 13 caracteres."
                    };
                }


                if (dto.Contrato.Length > 200)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El contrato no puede exceder 200 caracteres."
                    };
                }


                if (dto.Fianza.Length > 50)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fianza no puede exceder 50 caracteres."
                    };
                }


                if (dto.Clabe.Length != 18 ||
                    !dto.Clabe.All(char.IsDigit))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La CLABE debe contener exactamente 18 dígitos."
                    };
                }


                if (dto.Ubicacion.Length > 300)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La ubicación no puede exceder 300 caracteres."
                    };
                }


                // =====================================================
                // 5. VALIDAR MONTOS
                // =====================================================

                if (dto.MontoMinimo < 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto mínimo no puede ser negativo."
                    };
                }


                if (dto.MontoMaximo <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo debe ser mayor que cero."
                    };
                }


                if (dto.MontoMaximo < dto.MontoMinimo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo debe ser mayor o igual al monto mínimo."
                    };
                }


                // =====================================================
                // 6. VALIDAR PORCENTAJES
                // =====================================================

                if (dto.PorcentajeAlerta < 0 ||
                    dto.PorcentajeAlerta > 100)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje de alerta debe estar entre 0 y 100."
                    };
                }


                if (dto.PorcentajeCritico < 0 ||
                    dto.PorcentajeCritico > 100)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe estar entre 0 y 100."
                    };
                }


                if (dto.PorcentajeCritico >=
                    dto.PorcentajeAlerta)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe ser menor que el porcentaje de alerta."
                    };
                }


                // =====================================================
                // 7. CALCULAR MONTOS
                // =====================================================

                dto.MontoAlerta =
                    Math.Round(
                        dto.MontoMaximo *
                        dto.PorcentajeAlerta /
                        100m,
                        2);


                dto.MontoCritico =
                    Math.Round(
                        dto.MontoMaximo *
                        dto.PorcentajeCritico /
                        100m,
                        2);


                // =====================================================
                // 8. VALIDAR RECURSO
                // =====================================================

                var recurso =
                    await _context.Recursos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.RecursoId == dto.RecursoId &&
                                x.Activo);

                if (recurso == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El recurso seleccionado no existe o se encuentra inactivo."
                    };
                }


                // =====================================================
                // 9. VALIDAR BANCO
                // =====================================================

                var banco =
                    await _context.Bancos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.BancoId == dto.BancoId &&
                                x.Activo);

                if (banco == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El banco seleccionado no existe o se encuentra inactivo."
                    };
                }


                // =====================================================
                // 10. VALIDAR OBJETIVO DE MANTENIMIENTO
                // =====================================================

                var objetivoMantenimiento =
                    await _context.ObjetivoMantenimientos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.ObjetivoMantenimientoId ==
                                    dto.ObjetivoMantenimientoId &&
                                x.Activo);

                if (objetivoMantenimiento == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El objetivo de mantenimiento seleccionado no existe o se encuentra inactivo."
                    };
                }


                // =====================================================
                // 11. VALIDAR TALLER DUPLICADO
                // =====================================================
                //
                // Una ubicación se identifica por:
                //
                // RFC + Ubicación + Objetivo de mantenimiento
                //
                // El contrato NO forma parte de esta identidad.
                //
                // La configuración presupuestal es independiente
                // de la ubicación física.
                // =====================================================

                bool existeTaller =
                    await _context.Tallers
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.Rfc == dto.Rfc &&
                                x.Ubicacion == dto.Ubicacion &&
                                x.ObjetivoMantenimientoId ==
                                    dto.ObjetivoMantenimientoId);

                if (existeTaller)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Ya existe un taller registrado con el mismo RFC, ubicación y objetivo de mantenimiento."
                    };
                }


                // =====================================================
                // 12. OBTENER / CREAR CONFIGURACIÓN PRESUPUESTAL
                // =====================================================
                //
                // La identidad presupuestal es:
                //
                // CONTRATO + RECURSO
                //
                // Una misma configuración puede estar relacionada
                // con múltiples ubicaciones mediante TallerConfiguracion.
                // =====================================================

                var configuracionPresupuestal =
                    await _context.TallerConfiguracionPresupuestals
                        .FirstOrDefaultAsync(
                            x =>
                                x.Contrato == dto.Contrato &&
                                x.RecursoId == dto.RecursoId);

                if (configuracionPresupuestal == null)
                {
                    // =================================================
                    // CREAR NUEVA CONFIGURACIÓN PRESUPUESTAL
                    // =================================================

                    configuracionPresupuestal =
                        new TallerConfiguracionPresupuestal
                        {
                            Contrato =
                                dto.Contrato,

                            RecursoId =
                                dto.RecursoId,

                            MontoMinimo =
                                dto.MontoMinimo,

                            MontoMaximo =
                                dto.MontoMaximo,

                            PorcentajeAlerta =
                                dto.PorcentajeAlerta,

                            MontoAlerta =
                                dto.MontoAlerta,

                            PorcentajeCritico =
                                dto.PorcentajeCritico,

                            MontoCritico =
                                dto.MontoCritico,

                            Activo =
                                dto.Activo,

                            FechaCreacion =
                                DateTime.Now,

                            FechaActualizacion =
                                null
                        };

                    _context.TallerConfiguracionPresupuestals.Add(
                        configuracionPresupuestal);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    // =================================================
                    // CONFIGURACIÓN YA EXISTENTE
                    // =================================================
                    //
                    // NO modificamos sus montos ni porcentajes.
                    //
                    // La configuración pertenece al contrato + recurso
                    // y debe ser común para todas sus ubicaciones.
                    // =================================================

                    if (!configuracionPresupuestal.Activo)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                "La configuración presupuestal del contrato y recurso seleccionado se encuentra inactiva."
                        };
                    }
                }


                // =====================================================
                // 13. CREAR TALLER
                // =====================================================

                var taller =
                    new Taller
                    {
                        RazonSocial =
                            dto.RazonSocial,

                        Rfc =
                            dto.Rfc,

                        BancoId =
                            dto.BancoId,

                        Ubicacion =
                            dto.Ubicacion,

                        Activo =
                            dto.Activo,

                        Clabe =
                            dto.Clabe,

                        ObjetivoMantenimientoId =
                            dto.ObjetivoMantenimientoId,

                        Fianza =
                            dto.Fianza
                    };


                // =====================================================
                // 14. AGREGAR TALLER
                // =====================================================

                _context.Tallers.Add(taller);

                await _context.SaveChangesAsync();


                // =====================================================
                // 15. CREAR RELACIÓN TALLER - CONFIGURACIÓN
                // =====================================================

                var tallerConfiguracion =
                    new TallerConfiguracion
                    {
                        TallerId =
                            taller.TallerId,

                        TallerConfiguracionPresupuestalId =
                            configuracionPresupuestal
                                .TallerConfiguracionPresupuestalId,

                        Activo =
                            dto.Activo,

                        FechaCreacion =
                            DateTime.Now
                    };


                _context.TallerConfiguracions.Add(
                    tallerConfiguracion);


                // =====================================================
                // 16. PERSISTIR RELACIÓN
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 17. CONFIRMAR TRANSACCIÓN
                // =====================================================

                await transaction.CommitAsync();


                // =====================================================
                // 18. RETORNAR ÉXITO
                // =====================================================

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        "El taller se registró correctamente."
                };
            }
            catch (DbUpdateException)
            {
                // =====================================================
                // ERROR DE BASE DE DATOS
                // =====================================================

                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible registrar el taller debido a una restricción de datos."
                };
            }
            catch (Exception)
            {
                // =====================================================
                // ERROR GENERAL
                // =====================================================

                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al registrar el taller."
                };
            }
        }







        public async Task<ResultadoOperacion> ActualizarTallerAsync(TallerEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. VALIDAR DTO
                // =====================================================

                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información del taller no es válida."
                    };
                }


                if (dto.TallerId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El identificador del taller no es válido."
                    };
                }


                // =====================================================
                // 2. NORMALIZAR
                // =====================================================

                dto.RazonSocial =
                    dto.RazonSocial?.Trim() ?? string.Empty;

                dto.Rfc =
                    dto.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

                dto.Fianza =
                    dto.Fianza?.Trim() ?? string.Empty;

                dto.Clabe =
                    dto.Clabe?.Trim() ?? string.Empty;

                dto.Ubicacion =
                    dto.Ubicacion?.Trim() ?? string.Empty;


                // =====================================================
                // 3. VALIDAR CAMPOS OBLIGATORIOS
                // =====================================================

                if (string.IsNullOrWhiteSpace(dto.RazonSocial))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La razón social es obligatoria."
                    };
                }


                if (dto.Rfc.Length != 12 &&
                    dto.Rfc.Length != 13)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El RFC debe contener 12 o 13 caracteres."
                    };
                }


                if (dto.BancoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un banco válido."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Fianza))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fianza es obligatoria."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Clabe))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La CLABE interbancaria es obligatoria."
                    };
                }


                if (dto.Clabe.Length != 18 ||
                    !dto.Clabe.All(char.IsDigit))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La CLABE debe contener exactamente 18 dígitos."
                    };
                }


                if (dto.ObjetivoMantenimientoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un objetivo de mantenimiento válido."
                    };
                }


                if (string.IsNullOrWhiteSpace(dto.Ubicacion))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La ubicación es obligatoria."
                    };
                }


                // =====================================================
                // 4. VALIDAR LONGITUDES SEGÚN SQL ACTUAL
                // =====================================================

                if (dto.RazonSocial.Length > 200)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La razón social no puede exceder 200 caracteres."
                    };
                }


                if (dto.Rfc.Length > 13)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El RFC no puede exceder 13 caracteres."
                    };
                }


                if (dto.Fianza.Length > 50)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fianza no puede exceder 50 caracteres."
                    };
                }


                if (dto.Clabe.Length > 18)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La CLABE no puede exceder 18 dígitos."
                    };
                }


                if (dto.Ubicacion.Length > 300)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La ubicación no puede exceder 300 caracteres."
                    };
                }


                // =====================================================
                // 5. OBTENER TALLER
                // =====================================================

                var taller =
                    await _context.Tallers
                        .FirstOrDefaultAsync(
                            x =>
                                x.TallerId ==
                                dto.TallerId);


                if (taller == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El taller que intenta actualizar no existe."
                    };
                }


                // =====================================================
                // 6. VALIDAR BANCO
                // =====================================================

                bool bancoValido =
                    await _context.Bancos
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.BancoId ==
                                    dto.BancoId &&
                                x.Activo);


                if (!bancoValido)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El banco seleccionado no existe o está inactivo."
                    };
                }


                // =====================================================
                // 7. VALIDAR OBJETIVO DE MANTENIMIENTO
                // =====================================================

                bool objetivoValido =
                    await _context.ObjetivoMantenimientos
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.ObjetivoMantenimientoId ==
                                    dto.ObjetivoMantenimientoId &&
                                x.Activo);


                if (!objetivoValido)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El objetivo de mantenimiento seleccionado no existe o está inactivo."
                    };
                }


                // =====================================================
                // 8. VALIDAR TALLER DUPLICADO
                // =====================================================
                //
                // La identidad funcional de la ubicación es:
                //
                // RFC + Ubicación + Objetivo
                //
                // El contrato NO participa porque pertenece a
                // TallerConfiguracionPresupuestal.
                // =====================================================

                bool existeTaller =
                    await _context.Tallers
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.TallerId !=
                                    dto.TallerId &&

                                x.Rfc ==
                                    dto.Rfc &&

                                x.Ubicacion ==
                                    dto.Ubicacion &&

                                x.ObjetivoMantenimientoId ==
                                    dto.ObjetivoMantenimientoId);


                if (existeTaller)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Ya existe otro taller registrado con el mismo RFC, ubicación y objetivo de mantenimiento."
                    };
                }


                // =====================================================
                // 9. ACTUALIZAR ÚNICAMENTE TALLER
                // =====================================================

                taller.RazonSocial =
                    dto.RazonSocial;

                taller.Rfc =
                    dto.Rfc;

                taller.BancoId =
                    dto.BancoId;

                taller.Fianza =
                    dto.Fianza;

                taller.Clabe =
                    dto.Clabe;

                taller.ObjetivoMantenimientoId =
                    dto.ObjetivoMantenimientoId;

                taller.Ubicacion =
                    dto.Ubicacion;

                taller.Activo =
                    dto.Activo;


                // =====================================================
                // 10. GUARDAR CAMBIOS
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 11. CONFIRMAR TRANSACCIÓN
                // =====================================================

                await transaction.CommitAsync();


                // =====================================================
                // 12. RESULTADO
                // =====================================================

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        "El taller se actualizó correctamente."
                };
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible actualizar el taller debido a una restricción de datos."
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al actualizar el taller."
                };
            }
        }





        public async Task<TallerEditarDto?> ObtenerTallerParaEditarAsync(
     int tallerId)
        {
            if (tallerId <= 0)
                return null;


            // =====================================================
            // 1. OBTENER DATOS DEL TALLER
            // =====================================================

            var taller =
                await _context.Tallers
                    .AsNoTracking()
                    .Where(x =>
                        x.TallerId ==
                        tallerId)
                    .Select(x =>
                        new TallerEditarDto
                        {
                            TallerId =
                                x.TallerId,

                            RazonSocial =
                                x.RazonSocial,

                            Rfc =
                                x.Rfc,

                            BancoId =
                                x.BancoId,

                            Ubicacion =
                                x.Ubicacion,

                            Activo =
                                x.Activo,

                            Clabe =
                                x.Clabe ?? string.Empty,

                            ObjetivoMantenimientoId =
                                x.ObjetivoMantenimientoId ?? 0,

                            Fianza =
                                x.Fianza ?? string.Empty
                        })
                    .FirstOrDefaultAsync();


            if (taller == null)
                return null;


            // =====================================================
            // 2. OBTENER CONFIGURACIÓN PRESUPUESTAL
            // =====================================================
            //
            // SOLO LECTURA.
            //
            // No se modifica desde Editar Taller.
            // =====================================================

            var configuracion =
                await _context.TallerConfiguracions
                    .AsNoTracking()
                    .Where(x =>
                        x.TallerId ==
                            tallerId &&

                        x.Activo &&

                        x.TallerConfiguracionPresupuestal.Activo)
                    .Select(x =>
                        new
                        {
                            RecursoNombre =
                                x.TallerConfiguracionPresupuestal
                                    .Recurso
                                    .Nombre,

                            Contrato =
                                x.TallerConfiguracionPresupuestal
                                    .Contrato,

                            MontoMinimo =
                                x.TallerConfiguracionPresupuestal
                                    .MontoMinimo,

                            MontoMaximo =
                                x.TallerConfiguracionPresupuestal
                                    .MontoMaximo,

                            PorcentajeAlerta =
                                x.TallerConfiguracionPresupuestal
                                    .PorcentajeAlerta,

                            MontoAlerta =
                                x.TallerConfiguracionPresupuestal
                                    .MontoAlerta,

                            PorcentajeCritico =
                                x.TallerConfiguracionPresupuestal
                                    .PorcentajeCritico,

                            MontoCritico =
                                x.TallerConfiguracionPresupuestal
                                    .MontoCritico
                        })
                    .FirstOrDefaultAsync();


            // =====================================================
            // 3. SI EXISTE CONFIGURACIÓN, CARGARLA
            // =====================================================

            if (configuracion != null)
            {
                taller.RecursoNombre =
                    configuracion.RecursoNombre;

                taller.Contrato =
                    configuracion.Contrato;

                taller.MontoMinimo =
                    configuracion.MontoMinimo;

                taller.MontoMaximo =
                    configuracion.MontoMaximo;

                taller.PorcentajeAlerta =
                    configuracion.PorcentajeAlerta ?? 0;

                taller.MontoAlerta =
                    configuracion.MontoAlerta ?? 0;

                taller.PorcentajeCritico =
                    configuracion.PorcentajeCritico ?? 0;

                taller.MontoCritico =
                    configuracion.MontoCritico ?? 0;
            }


            return taller;
        }


        public async Task<List<TallerConfiguracionPresupuestalDto>> ObtenerConfiguracionesPresupuestalesAsync(string? busqueda = null, bool? activo = null)
        {
            IQueryable<TallerConfiguracionPresupuestal> consulta = _context.TallerConfiguracionPresupuestals
                .AsNoTracking();

            // =========================================================
            // FILTRO ESTADO
            // =========================================================
            if (activo.HasValue)
            {
                consulta = consulta.Where(x => x.Activo == activo.Value);
            }

            // =========================================================
            // BÚSQUEDA POR TEXTO (CONTRATO O RECURSO)
            // =========================================================
            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                consulta = consulta.Where(x =>
                    (x.Contrato != null && x.Contrato.Contains(busqueda)) ||
                    (x.Recurso != null && x.Recurso.Nombre != null && x.Recurso.Nombre.Contains(busqueda)));
            }

            // =========================================================
            // PROYECCIÓN Y ORDENAMIENTO
            // =========================================================
            return await consulta
                .OrderBy(x => x.Contrato)
                .ThenBy(x => x.Recurso.Nombre)
                .Select(x => new TallerConfiguracionPresupuestalDto
                {
                    TallerConfiguracionPresupuestalId = x.TallerConfiguracionPresupuestalId,
                    Contrato = x.Contrato,
                    RecursoId = x.RecursoId,
                    RecursoNombre = x.Recurso.Nombre,
                    MontoMinimo = x.MontoMinimo,
                    MontoMaximo = x.MontoMaximo,
                    PorcentajeAlerta = x.PorcentajeAlerta,
                    MontoAlerta = x.MontoAlerta,
                    PorcentajeCritico = x.PorcentajeCritico,
                    MontoCritico = x.MontoCritico,
                    Activo = x.Activo,
                    CantidadTalleres = x.TallerConfiguracions.Count(),
                    CantidadTalleresActivos = x.TallerConfiguracions.Count(tc => tc.Activo && tc.Taller.Activo)
                })
                .ToListAsync();
        }






        public async Task<ResultadoOperacion> CrearConfiguracionPresupuestalAsync(TallerConfiguracionPresupuestalCreateDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información de la configuración no es válida."
                    };
                }


                // =========================================================
                // NORMALIZACIÓN
                // =========================================================

                dto.Contrato =
                    dto.Contrato?.Trim() ?? string.Empty;


                if (string.IsNullOrWhiteSpace(dto.Contrato))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El contrato es obligatorio."
                    };
                }


                if (dto.Contrato.Length > 200)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El contrato no puede exceder 200 caracteres."
                    };
                }


                // =========================================================
                // RECURSO
                // =========================================================

                if (dto.RecursoId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar un recurso válido."
                    };
                }


                var recursoExiste =
                    await _context.Recursos
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.RecursoId == dto.RecursoId &&
                            x.Activo);

                if (!recursoExiste)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El recurso seleccionado no existe o está inactivo."
                    };
                }


                // =========================================================
                // MONTOS
                // =========================================================

                if (dto.MontoMinimo < 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto mínimo no puede ser negativo."
                    };
                }


                if (dto.MontoMaximo <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo debe ser mayor que cero."
                    };
                }


                if (dto.MontoMaximo < dto.MontoMinimo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo no puede ser menor que el monto mínimo."
                    };
                }


                // =========================================================
                // PORCENTAJES
                // =========================================================

                if (dto.PorcentajeAlerta.HasValue &&
                    (dto.PorcentajeAlerta.Value < 0 ||
                     dto.PorcentajeAlerta.Value > 100))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje de alerta debe estar entre 0 y 100."
                    };
                }


                if (dto.PorcentajeCritico.HasValue &&
                    (dto.PorcentajeCritico.Value < 0 ||
                     dto.PorcentajeCritico.Value > 100))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe estar entre 0 y 100."
                    };
                }


                if (dto.PorcentajeAlerta.HasValue &&
                    dto.PorcentajeCritico.HasValue &&
                    dto.PorcentajeCritico.Value >=
                    dto.PorcentajeAlerta.Value)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe ser menor que el porcentaje de alerta."
                    };
                }


                // =========================================================
                // VERIFICAR DUPLICADO
                // =========================================================

                var existe =
                    await _context.TallerConfiguracionPresupuestals
                        .AnyAsync(x =>
                            x.Contrato == dto.Contrato &&
                            x.RecursoId == dto.RecursoId);

                if (existe)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Ya existe una configuración presupuestal para el contrato y recurso seleccionados."
                    };
                }


                // =========================================================
                // CALCULAR ALERTAS
                // =========================================================

                decimal? montoAlerta = null;

                if (dto.PorcentajeAlerta.HasValue)
                {
                    montoAlerta =
                        Math.Round(
                            dto.MontoMaximo *
                            dto.PorcentajeAlerta.Value /
                            100,
                            2);
                }


                decimal? montoCritico = null;

                if (dto.PorcentajeCritico.HasValue)
                {
                    montoCritico =
                        Math.Round(
                            dto.MontoMaximo *
                            dto.PorcentajeCritico.Value /
                            100,
                            2);
                }


                // =========================================================
                // CREAR CONFIGURACIÓN
                // =========================================================

                var configuracion =
                    new TallerConfiguracionPresupuestal
                    {
                        Contrato =
                            dto.Contrato,

                        RecursoId =
                            dto.RecursoId,

                        MontoMinimo =
                            dto.MontoMinimo,

                        MontoMaximo =
                            dto.MontoMaximo,

                        PorcentajeAlerta =
                            dto.PorcentajeAlerta,

                        MontoAlerta =
                            montoAlerta,

                        PorcentajeCritico =
                            dto.PorcentajeCritico,

                        MontoCritico =
                            montoCritico,

                        Activo =
                            true,

                        FechaCreacion =
                            DateTime.Now
                    };


                _context.TallerConfiguracionPresupuestals
                    .Add(configuracion);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        "La configuración presupuestal se creó correctamente."
                };
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible crear la configuración debido a una restricción de datos."
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al crear la configuración presupuestal."
                };
            }
        }








        public async Task<ResultadoOperacion> ActualizarConfiguracionPresupuestalAsync(
     TallerConfiguracionPresupuestalEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. VALIDAR DTO
                // =====================================================

                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información de la configuración no es válida."
                    };
                }

                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El identificador de la configuración no es válido."
                    };
                }


                // =====================================================
                // 2. VALIDAR MONTOS
                // =====================================================

                if (dto.MontoMinimo < 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto mínimo no puede ser negativo."
                    };
                }

                if (dto.MontoMaximo <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo debe ser mayor que cero."
                    };
                }

                if (dto.MontoMaximo <
                    dto.MontoMinimo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El monto máximo no puede ser menor que el monto mínimo."
                    };
                }


                // =====================================================
                // 3. VALIDAR PORCENTAJES
                // =====================================================

                if (dto.PorcentajeAlerta.HasValue &&
                    (
                        dto.PorcentajeAlerta.Value < 0
                        ||
                        dto.PorcentajeAlerta.Value > 100
                    ))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje de alerta debe estar entre 0 y 100."
                    };
                }

                if (dto.PorcentajeCritico.HasValue &&
                    (
                        dto.PorcentajeCritico.Value < 0
                        ||
                        dto.PorcentajeCritico.Value > 100
                    ))
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe estar entre 0 y 100."
                    };
                }

                if (dto.PorcentajeAlerta.HasValue &&
                    dto.PorcentajeCritico.HasValue &&
                    dto.PorcentajeCritico.Value >=
                        dto.PorcentajeAlerta.Value)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El porcentaje crítico debe ser menor que el porcentaje de alerta."
                    };
                }


                // =====================================================
                // 4. OBTENER CONFIGURACIÓN
                // =====================================================

                var configuracion =
                    await _context
                        .TallerConfiguracionPresupuestals
                        .FirstOrDefaultAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                                dto.TallerConfiguracionPresupuestalId);

                if (configuracion == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La configuración presupuestal no existe."
                    };
                }


                // =====================================================
                // 5. OBTENER CONTROLES
                // =====================================================
                //
                // El control depende de:
                //
                // Configuración + Ejercicio
                //
                // Por lo tanto cualquier control existente debe
                // respetar el nuevo MontoMaximo.
                // =====================================================

                var controles =
                    await _context
                        .TallerControlPresupuestals
                        .AsNoTracking()
                        .Where(x =>
                            x.TallerConfiguracionPresupuestalId ==
                                configuracion
                                    .TallerConfiguracionPresupuestalId)
                        .ToListAsync();


                // =====================================================
                // 6. PROTEGER PRESUPUESTO ASIGNADO
                // =====================================================

                foreach (var control in controles)
                {
                    if (dto.MontoMaximo <
                        control.PresupuestoAsignado)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                $"El monto máximo no puede ser menor al presupuesto asignado del ejercicio {control.Ejercicio} ({control.PresupuestoAsignado:C})."
                        };
                    }


                    // =================================================
                    // 6.1 PROTEGER COMPROMETIDO
                    // =================================================

                    if (dto.MontoMaximo <
                        control.PresupuestoComprometido)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                $"El monto máximo no puede ser menor al presupuesto comprometido del ejercicio {control.Ejercicio}."
                        };
                    }


                    // =================================================
                    // 6.2 PROTEGER EJERCIDO
                    // =================================================

                    if (dto.MontoMaximo <
                        control.PresupuestoEjercido)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                $"El monto máximo no puede ser menor al presupuesto ejercido del ejercicio {control.Ejercicio}."
                        };
                    }
                }


                // =====================================================
                // 7. PROTEGER DESACTIVACIÓN
                // =====================================================

                if (!dto.Activo &&
                    configuracion.Activo)
                {
                    var talleresActivos =
                        await _context
                            .TallerConfiguracions
                            .AsNoTracking()
                            .AnyAsync(x =>
                                x.TallerConfiguracionPresupuestalId ==
                                    configuracion
                                        .TallerConfiguracionPresupuestalId
                                &&
                                x.Activo
                                &&
                                x.Taller.Activo);

                    if (talleresActivos)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                "No es posible desactivar la configuración porque existen asociaciones activas con talleres."
                        };
                    }
                }


                // =====================================================
                // 8. RECALCULAR ALERTAS
                // =====================================================

                decimal? montoAlerta = null;

                if (dto.PorcentajeAlerta.HasValue)
                {
                    montoAlerta =
                        Math.Round(
                            dto.MontoMaximo *
                            dto.PorcentajeAlerta.Value /
                            100,
                            2);
                }


                decimal? montoCritico = null;

                if (dto.PorcentajeCritico.HasValue)
                {
                    montoCritico =
                        Math.Round(
                            dto.MontoMaximo *
                            dto.PorcentajeCritico.Value /
                            100,
                            2);
                }


                // =====================================================
                // 9. ACTUALIZAR
                // =====================================================

                configuracion.MontoMinimo =
                    dto.MontoMinimo;

                configuracion.MontoMaximo =
                    dto.MontoMaximo;

                configuracion.PorcentajeAlerta =
                    dto.PorcentajeAlerta;

                configuracion.MontoAlerta =
                    montoAlerta;

                configuracion.PorcentajeCritico =
                    dto.PorcentajeCritico;

                configuracion.MontoCritico =
                    montoCritico;

                configuracion.Activo =
                    dto.Activo;

                configuracion.FechaActualizacion =
                    DateTime.Now;


                // =====================================================
                // 10. GUARDAR
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 11. COMMIT
                // =====================================================

                await transaction.CommitAsync();


                // =====================================================
                // 12. RESPUESTA
                // =====================================================

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        "La configuración presupuestal se actualizó correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error de base de datos al actualizar configuración presupuestal {ConfiguracionId}.",
                    dto?.TallerConfiguracionPresupuestalId);

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible actualizar la configuración debido a una restricción de datos."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al actualizar configuración presupuestal {ConfiguracionId}.",
                    dto?.TallerConfiguracionPresupuestalId);

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al actualizar la configuración presupuestal."
                };
            }
        }





        public async Task<TallerConfiguracionPresupuestalViewModel?> ObtenerConfiguracionPresupuestalParaEditarAsync(int id)
        {
            // =========================================================
            // VALIDAR ID
            // =========================================================

            if (id <= 0)
                return null;


            // =========================================================
            // OBTENER CONFIGURACIÓN
            // =========================================================

            var configuracion =
                await _context.TallerConfiguracionPresupuestals
                    .AsNoTracking()
                    .Where(x =>
                        x.TallerConfiguracionPresupuestalId == id)
                    .Select(x =>
                        new TallerConfiguracionPresupuestalViewModel
                        {
                            // =================================================
                            // IDENTIFICADOR
                            // =================================================

                            TallerConfiguracionPresupuestalId =
                                x.TallerConfiguracionPresupuestalId,


                            // =================================================
                            // IDENTIFICACIÓN
                            // =================================================

                            Contrato =
                                x.Contrato,

                            RecursoId =
                                x.RecursoId,

                            RecursoNombre =
                                x.Recurso.Nombre,


                            // =================================================
                            // MONTOS
                            // =================================================

                            MontoMinimo =
                                x.MontoMinimo,

                            MontoMaximo =
                                x.MontoMaximo,


                            // =================================================
                            // ALERTA PREVENTIVA
                            // =================================================

                            PorcentajeAlerta =
                                x.PorcentajeAlerta,

                            MontoAlerta =
                                x.MontoAlerta,


                            // =================================================
                            // ALERTA CRÍTICA
                            // =================================================

                            PorcentajeCritico =
                                x.PorcentajeCritico,

                            MontoCritico =
                                x.MontoCritico,


                            // =================================================
                            // ESTADO
                            // =================================================

                            Activo =
                                x.Activo,


                            // =================================================
                            // INFORMACIÓN DE USO
                            // =================================================

                            CantidadTalleres =
                                x.TallerConfiguracions.Count(),

                            CantidadTalleresActivos =
                                x.TallerConfiguracions.Count(tc =>
                                    tc.Activo &&
                                    tc.Taller.Activo),

                            CantidadControles =
                                x.TallerControlPresupuestals.Count()
                        })
                    .FirstOrDefaultAsync();


            // =========================================================
            // RESULTADO
            // =========================================================

            return configuracion;
        }








        public async Task<List<TallerConfiguracionPresupuestalSelectDto>> ObtenerConfiguracionesPresupuestalesActivasAsync()
        {
            return await _context
                .TallerConfiguracionPresupuestals
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Contrato)
                .ThenBy(x => x.Recurso.Nombre)
                .Select(x =>
                    new TallerConfiguracionPresupuestalSelectDto
                    {
                        TallerConfiguracionPresupuestalId =
                            x.TallerConfiguracionPresupuestalId,

                        Contrato =
                            x.Contrato,

                        RecursoNombre =
                            x.Recurso.Nombre,

                        MontoMaximo =
                            x.MontoMaximo
                    })
                .ToListAsync();
        }







        public async Task<List<TallerControlPresupuestalDto>> ObtenerControlesPresupuestalesAsync(string? busqueda = null, short? ejercicio = null, bool? activo = null)
        {
            IQueryable<TallerControlPresupuestal> consulta =
                _context.TallerControlPresupuestals
                    .AsNoTracking();

            // =========================================================
            // FILTRO POR EJERCICIO
            // =========================================================

            if (ejercicio.HasValue)
            {
                consulta =
                    consulta.Where(x =>
                        x.Ejercicio == ejercicio.Value);
            }


            // =========================================================
            // FILTRO POR ESTADO
            // =========================================================

            if (activo.HasValue)
            {
                consulta =
                    consulta.Where(x =>
                        x.Activo == activo.Value);
            }


            // =========================================================
            // BÚSQUEDA
            // =========================================================

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda =
                    busqueda.Trim();

                consulta =
                    consulta.Where(x =>
                        x.TallerConfiguracionPresupuestal.Contrato
                            .Contains(busqueda)

                        ||

                        x.TallerConfiguracionPresupuestal.Recurso.Nombre
                            .Contains(busqueda)

                        ||

                        x.Ejercicio
                            .ToString()
                            .Contains(busqueda)
                    );
            }


            // =========================================================
            // PROYECCIÓN
            // =========================================================

            return await consulta
                .OrderByDescending(x => x.Ejercicio)
                .ThenBy(x =>
                    x.TallerConfiguracionPresupuestal.Contrato)
                .ThenBy(x =>
                    x.TallerConfiguracionPresupuestal.Recurso.Nombre)
                .Select(x =>
                    new TallerControlPresupuestalDto
                    {
                        IdControlTaller =
                            x.IdControlTaller,

                        TallerConfiguracionPresupuestalId =
                            x.TallerConfiguracionPresupuestalId,

                        Contrato =
                            x.TallerConfiguracionPresupuestal
                                .Contrato,

                        RecursoId =
                            x.TallerConfiguracionPresupuestal
                                .RecursoId,

                        RecursoNombre =
                            x.TallerConfiguracionPresupuestal
                                .Recurso
                                .Nombre,

                        Ejercicio =
                            x.Ejercicio,

                        PresupuestoAsignado =
                            x.PresupuestoAsignado,

                        PresupuestoEjercido =
                            x.PresupuestoEjercido,

                        PresupuestoComprometido =
                            x.PresupuestoComprometido,

                        PresupuestoDisponible =
                            x.PresupuestoDisponible
                            ?? 0,

                        FechaInicio = x.FechaInicio,  //cambiada

                        FechaFin = x.FechaFin,      //cambiada

                        Activo =
                            x.Activo
                    })
                .ToListAsync();
        }






        public async Task<TallerControlPresupuestalEditViewModel?> ObtenerControlPresupuestalParaEditarAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _context
                .TallerControlPresupuestals
                .AsNoTracking()
                .Where(x =>
                    x.IdControlTaller == id)
                .Select(x =>
                    new TallerControlPresupuestalEditViewModel
                    {
                        IdControlTaller =
                            x.IdControlTaller,

                        Contrato =
                            x.TallerConfiguracionPresupuestal
                                .Contrato,

                        RecursoNombre =
                            x.TallerConfiguracionPresupuestal
                                .Recurso
                                .Nombre,

                        Ejercicio =
                            x.Ejercicio,

                        MontoMaximo =
                            x.TallerConfiguracionPresupuestal
                                .MontoMaximo,

                        PresupuestoAsignado =
                            x.PresupuestoAsignado,

                        PresupuestoComprometido =
                            x.PresupuestoComprometido,

                        PresupuestoEjercido =
                            x.PresupuestoEjercido,

                        PresupuestoDisponible =
                            x.PresupuestoDisponible
                            ?? 0,

                        FechaInicio =
                            x.FechaInicio,

                        FechaFin =
                            x.FechaFin,

                        Activo =
                            x.Activo
                    })
                .FirstOrDefaultAsync();
        }






        public async Task<ResultadoOperacion> CrearControlPresupuestalAsync(TallerControlPresupuestalCreateDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // VALIDAR DTO
                // =====================================================

                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información del control presupuestal no es válida."
                    };
                }


                // =====================================================
                // VALIDAR CONFIGURACIÓN
                // =====================================================

                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Debe seleccionar una configuración presupuestal válida."
                    };
                }


                // =====================================================
                // VALIDAR EJERCICIO
                // =====================================================

                if (dto.Ejercicio < 2000 ||
                    dto.Ejercicio > 2100)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El ejercicio presupuestal no es válido."
                    };
                }


                // =====================================================
                // VALIDAR MONTO
                // =====================================================

                if (dto.PresupuestoAsignado <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El presupuesto asignado debe ser mayor que cero."
                    };
                }


                // =====================================================
                // VALIDAR FECHAS
                // =====================================================

                if (dto.FechaInicio > dto.FechaFin)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fecha de inicio no puede ser posterior a la fecha de fin."
                    };
                }


                if (dto.FechaInicio.Year != dto.Ejercicio ||
                    dto.FechaFin.Year != dto.Ejercicio)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Las fechas de vigencia deben pertenecer al ejercicio presupuestal seleccionado."
                    };
                }


                // =====================================================
                // OBTENER CONFIGURACIÓN
                // =====================================================

                var configuracion =
                    await _context
                        .TallerConfiguracionPresupuestals
                        .FirstOrDefaultAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                                dto.TallerConfiguracionPresupuestalId);


                if (configuracion == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La configuración presupuestal no existe."
                    };
                }


                // =====================================================
                // LA CONFIGURACIÓN DEBE ESTAR ACTIVA
                // =====================================================

                if (!configuracion.Activo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "No es posible crear un control para una configuración presupuestal inactiva."
                    };
                }


                // =====================================================
                // VALIDAR CONTRA MONTO MÁXIMO CONTRACTUAL
                // =====================================================

                if (dto.PresupuestoAsignado >
                    configuracion.MontoMaximo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            $"El presupuesto asignado no puede superar el monto máximo contractual de {configuracion.MontoMaximo:C}."
                    };
                }


                // =====================================================
                // VALIDAR DUPLICADO
                // =====================================================

                bool existe =
                    await _context
                        .TallerControlPresupuestals
                        .AnyAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                                dto.TallerConfiguracionPresupuestalId
                            &&
                            x.Ejercicio ==
                                dto.Ejercicio);


                if (existe)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Ya existe un control presupuestal para esta configuración y ejercicio."
                    };
                }


                // =====================================================
                // CREAR CONTROL
                // =====================================================

                var control =
                    new TallerControlPresupuestal
                    {
                        TallerConfiguracionPresupuestalId =
                            dto.TallerConfiguracionPresupuestalId,

                        Ejercicio =
                            dto.Ejercicio,

                        PresupuestoAsignado =
                            dto.PresupuestoAsignado,

                        PresupuestoEjercido =
                            0,

                        PresupuestoComprometido =
                            0,

                        PresupuestoDisponible =
                            dto.PresupuestoAsignado,

                        FechaInicio = dto.FechaInicio,   //cambiada 

                        FechaFin = dto.FechaFin,              //cambiada 

                        Activo =
                            true,

                        FechaActualizacion =
                            DateTime.Now
                    };


                _context
                    .TallerControlPresupuestals
                    .Add(control);


                // =====================================================
                // GUARDAR
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // COMMIT
                // =====================================================

                await transaction.CommitAsync();


                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        "El control presupuestal se creó correctamente."
                };
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible crear el control presupuestal debido a una restricción de datos."
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al crear el control presupuestal."
                };
            }
        }






        public async Task<ResultadoOperacion> ActualizarControlPresupuestalAsync(TallerControlPresupuestalEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. VALIDAR DTO
                // =====================================================

                if (dto == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La información del control presupuestal no es válida."
                    };
                }

                if (dto.IdControlTaller <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El identificador del control presupuestal no es válido."
                    };
                }


                // =====================================================
                // 2. VALIDAR PRESUPUESTO
                // =====================================================

                if (dto.PresupuestoAsignado <= 0)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El presupuesto asignado debe ser mayor que cero."
                    };
                }


                // =====================================================
                // 3. VALIDAR FECHAS
                // =====================================================

                if (dto.FechaInicio > dto.FechaFin)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La fecha de inicio no puede ser posterior a la fecha de fin."
                    };
                }


                // =====================================================
                // 4. OBTENER CONTROL
                // =====================================================

                var control =
                    await _context
                        .TallerControlPresupuestals
                        .FirstOrDefaultAsync(x =>
                            x.IdControlTaller ==
                                dto.IdControlTaller);

                if (control == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "El control presupuestal no existe."
                    };
                }


                // =====================================================
                // 5. OBTENER CONFIGURACIÓN PRESUPUESTAL
                // =====================================================

                var configuracion =
                    await _context
                        .TallerConfiguracionPresupuestals
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                                control.TallerConfiguracionPresupuestalId);

                if (configuracion == null)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "La configuración presupuestal asociada al control no existe."
                    };
                }


                // =====================================================
                // 6. VALIDAR FECHAS CONTRA EJERCICIO
                // =====================================================

                if (dto.FechaInicio.Year !=
                        control.Ejercicio
                    ||
                    dto.FechaFin.Year !=
                        control.Ejercicio)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            "Las fechas de vigencia deben pertenecer al ejercicio presupuestal del control."
                    };
                }


                // =====================================================
                // 7. VALIDAR MONTO MÁXIMO
                // =====================================================

                if (dto.PresupuestoAsignado >
                    configuracion.MontoMaximo)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            $"El presupuesto asignado no puede superar el monto máximo contractual de {configuracion.MontoMaximo:C}."
                    };
                }


                // =====================================================
                // 8. DETERMINAR SI EXISTE HISTORIAL PRESUPUESTAL
                // =====================================================
                //
                // No se filtra por Activo.
                //
                // Un movimiento histórico, aunque posteriormente haya
                // sido desactivado, significa que el control ya tiene
                // historial y no debe permitir modificaciones
                // estructurales.
                // =====================================================

                var tieneMovimientos =
                    await _context
                        .TallerMovimientoPresupuestos
                        .AnyAsync(x =>
                            x.IdControlTaller ==
                                control.IdControlTaller);


                // =====================================================
                // 9. PRESUPUESTO UTILIZADO
                // =====================================================

                var presupuestoUtilizado =
                    control.PresupuestoComprometido
                    +
                    control.PresupuestoEjercido;


                // =====================================================
                // 10. PROTEGER REDUCCIÓN DEL PRESUPUESTO
                // =====================================================

                if (dto.PresupuestoAsignado <
                    presupuestoUtilizado)
                {
                    return new ResultadoOperacion
                    {
                        Exito = false,
                        Mensaje =
                            $"El presupuesto asignado no puede ser menor al presupuesto ya comprometido y ejercido ({presupuestoUtilizado:C})."
                    };
                }


                // =====================================================
                // 11. CONTROL CON HISTORIAL
                // =====================================================

                if (tieneMovimientos)
                {
                    // =================================================
                    // 11.1 NO PERMITIR REDUCIR PRESUPUESTO
                    // =================================================

                    if (dto.PresupuestoAsignado <
                        control.PresupuestoAsignado)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                $"El control presupuestal ya tiene movimientos históricos. El presupuesto asignado no puede disminuir de {control.PresupuestoAsignado:C}."
                        };
                    }


                    // =================================================
                    // 11.2 NO PERMITIR CAMBIO DE FECHAS
                    // =================================================

                    if (dto.FechaInicio !=
                            control.FechaInicio
                        ||
                        dto.FechaFin !=
                            control.FechaFin)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                "No es posible modificar las fechas de vigencia porque el control presupuestal ya tiene movimientos históricos."
                        };
                    }


                    // =================================================
                    // 11.3 NO PERMITIR CAMBIO DE ESTADO
                    // =================================================

                    if (dto.Activo !=
                        control.Activo)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje =
                                "No es posible modificar el estado del control presupuestal porque ya tiene movimientos históricos."
                        };
                    }
                }


                // =====================================================
                // 12. ACTUALIZAR CAMPOS ADMINISTRABLES
                // =====================================================

                control.PresupuestoAsignado =
                    dto.PresupuestoAsignado;

                control.FechaInicio =
                    dto.FechaInicio;

                control.FechaFin =
                    dto.FechaFin;

                control.Activo =
                    dto.Activo;


                // =====================================================
                // 13. RECALCULAR DISPONIBLE
                // =====================================================

                control.PresupuestoDisponible =
                    Math.Max(
                        control.PresupuestoAsignado
                        -
                        control.PresupuestoComprometido
                        -
                        control.PresupuestoEjercido,
                        0);


                // =====================================================
                // 14. FECHA DE ACTUALIZACIÓN
                // =====================================================

                control.FechaActualizacion =
                    DateTime.Now;


                // =====================================================
                // 15. GUARDAR
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 16. CONFIRMAR TRANSACCIÓN
                // =====================================================

                await transaction.CommitAsync();


                // =====================================================
                // 17. RESPUESTA
                // =====================================================

                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje =
                        tieneMovimientos
                            ? "El presupuesto asignado del control presupuestal se actualizó correctamente."
                            : "El control presupuestal se actualizó correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error de base de datos al actualizar control presupuestal {IdControlTaller}.",
                    dto?.IdControlTaller);

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "No fue posible actualizar el control presupuestal debido a una restricción de datos."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al actualizar control presupuestal {IdControlTaller}.",
                    dto?.IdControlTaller);

                return new ResultadoOperacion
                {
                    Exito = false,
                    Mensaje =
                        "Ocurrió un error al actualizar el control presupuestal."
                };
            }
        }





        public async Task<List<SelectListItem>> ObtenerRecursosConfiguracionAsync()
        {
            return await _context
                .Recursos
                .AsNoTracking()
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.RecursoId.ToString(),

                        Text =
                            x.Nombre
                    })
                .ToListAsync();
        }







        public async Task<List<TallerConfiguracionDto>> ObtenerConfiguracionesTallerAsync()
        {
            return await _context.TallerConfiguracions
                .AsNoTracking()
                .Include(x => x.Taller)
                .Include(x => x.TallerConfiguracionPresupuestal)
                    .ThenInclude(x => x.Recurso)
                .OrderBy(x => x.Taller.RazonSocial)
                .Select(x => new TallerConfiguracionDto
                {
                    TallerId = x.TallerId,

                    TallerConfiguracionPresupuestalId =
                        x.TallerConfiguracionPresupuestalId,

                    TallerNombre = x.Taller.RazonSocial,

                    RFC = x.Taller.Rfc,

                    Ubicacion = x.Taller.Ubicacion,

                    Contrato =
                        x.TallerConfiguracionPresupuestal.Contrato,

                    RecursoNombre =
                        x.TallerConfiguracionPresupuestal.Recurso.Nombre,

                    MontoMaximo =
                        x.TallerConfiguracionPresupuestal.MontoMaximo,

                    Activo = x.Activo,

                    FechaCreacion = x.FechaCreacion
                })
                .ToListAsync();
        }






        public async Task<TallerConfiguracionCreateViewModel> CargarCatalogosTallerConfiguracionAsync(TallerConfiguracionCreateViewModel model)
        {
            // =====================================================
            // TALLERES ACTIVOS
            // =====================================================

            model.Talleres =
                await _context.Tallers
                    .AsNoTracking()
                    .Where(t => t.Activo)
                    .OrderBy(t => t.RazonSocial)
                    .Select(t => new SelectListItem
                    {
                        Value =
                            t.TallerId.ToString(),

                        Text =
                            t.RazonSocial +
                            " - " +
                            t.Ubicacion.Trim()
                    })
                    .ToListAsync();


            // =====================================================
            // CONFIGURACIONES PRESUPUESTALES ACTIVAS
            // =====================================================

            model.ConfiguracionesPresupuestales =
                await _context.TallerConfiguracionPresupuestals
                    .AsNoTracking()
                    .Where(x => x.Activo)
                    .OrderBy(x => x.Contrato)
                    .ThenBy(x => x.Recurso.Nombre)
                    .Select(x => new SelectListItem
                    {
                        Value =
                            x.TallerConfiguracionPresupuestalId.ToString(),

                        Text =
                            x.Contrato +
                            " - " +
                            x.Recurso.Nombre +
                            " - Máx. $" +
                            x.MontoMaximo.ToString("N2")
                    })
                    .ToListAsync();

            return model;
        }





        public async Task<TallerConfiguracionEditViewModel?> ObtenerTallerConfiguracionParaEditarAsync(int tallerId, int configuracionId)
        {
            if (tallerId <= 0 ||
                configuracionId <= 0)
            {
                return null;
            }

            var configuracion =
                await _context.TallerConfiguracions
                    .AsNoTracking()
                    .Where(x =>
                        x.TallerId == tallerId &&
                        x.TallerConfiguracionPresupuestalId ==
                            configuracionId)
                    .Select(x =>
                        new TallerConfiguracionEditViewModel
                        {
                            TallerId =
                                x.TallerId,

                            TallerNombre =
                                x.Taller.RazonSocial,

                            RFC =
                                x.Taller.Rfc,

                            Ubicacion =
                                x.Taller.Ubicacion,


                            // =================================================
                            // CONFIGURACIÓN ORIGINAL
                            // =================================================

                            ConfiguracionPresupuestalOriginalId =
                                x.TallerConfiguracionPresupuestalId,


                            // =================================================
                            // CONFIGURACIÓN ACTUAL
                            // =================================================

                            TallerConfiguracionPresupuestalId =
                                x.TallerConfiguracionPresupuestalId,


                            ContratoActual =
                                x.TallerConfiguracionPresupuestal.Contrato,

                            RecursoActual =
                                x.TallerConfiguracionPresupuestal
                                    .Recurso.Nombre,

                            RecursoNombre =
                                x.TallerConfiguracionPresupuestal
                                    .Recurso.Nombre,

                            Activo =
                                x.Activo,

                            FechaCreacion =
                                x.FechaCreacion
                        })
                    .FirstOrDefaultAsync();

            return configuracion;
        }



        public async Task CargarConfiguracionesPresupuestalesParaEditarAsync(TallerConfiguracionEditViewModel model)
        {
            var configuraciones =
                await _context.TallerConfiguracionPresupuestals
                    .AsNoTracking()
                    .Where(x =>
                        x.Activo ||
                        x.TallerConfiguracionPresupuestalId ==
                            model.TallerConfiguracionPresupuestalId)
                    .OrderBy(x => x.Contrato)
                    .ThenBy(x => x.Recurso.Nombre)
                    .Select(x =>
                        new
                        {
                            x.TallerConfiguracionPresupuestalId,
                            x.Contrato,
                            RecursoNombre = x.Recurso.Nombre
                        })
                    .ToListAsync();

            model.ConfiguracionesPresupuestales =
                configuraciones.Select(x =>
                    new SelectListItem
                    {
                        Value =
                            x.TallerConfiguracionPresupuestalId.ToString(),

                        Text =
                            $"{x.Contrato} - {x.RecursoNombre}",

                        Selected =
                            x.TallerConfiguracionPresupuestalId ==
                            model.TallerConfiguracionPresupuestalId
                    });
        }





        public async Task<BaseResponse> CrearTallerConfiguracionAsync(TallerConfiguracionCreateDto dto, ClaimsPrincipal usuario)
        {
            try
            {
                // =====================================================
                // 1. VALIDACIONES BÁSICAS
                // =====================================================

                if (dto.TallerId <= 0)
                {
                    return BaseResponse.Error(
                        "Debe seleccionar un taller.");
                }

                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    return BaseResponse.Error(
                        "Debe seleccionar una configuración presupuestal.");
                }


                // =====================================================
                // 2. VALIDAR TALLER
                // =====================================================

                var taller =
                    await _context.Tallers
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.TallerId == dto.TallerId);

                if (taller == null)
                {
                    return BaseResponse.Error(
                        "El taller seleccionado no existe.");
                }

                if (!taller.Activo)
                {
                    return BaseResponse.Error(
                        "No es posible asociar una configuración a un taller inactivo.");
                }


                // =====================================================
                // 3. VALIDAR CONFIGURACIÓN PRESUPUESTAL
                // =====================================================

                var configuracion =
                    await _context.TallerConfiguracionPresupuestals
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                            dto.TallerConfiguracionPresupuestalId);

                if (configuracion == null)
                {
                    return BaseResponse.Error(
                        "La configuración presupuestal seleccionada no existe.");
                }

                if (!configuracion.Activo)
                {
                    return BaseResponse.Error(
                        "No es posible asociar una configuración presupuestal inactiva.");
                }


                // =====================================================
                // 4. VALIDAR QUE LA ASOCIACIÓN NO EXISTA
                // =====================================================

                var asociacionExistente =
                    await _context.TallerConfiguracions
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.TallerId == dto.TallerId &&
                            x.TallerConfiguracionPresupuestalId ==
                                dto.TallerConfiguracionPresupuestalId);

                if (asociacionExistente)
                {
                    return BaseResponse.Error(
                        "El taller ya tiene registrada esta configuración presupuestal. " +
                        "Si necesita modificar su estado, utilice la opción de edición.");
                }


                // =====================================================
                // 5. CREAR NUEVA ASOCIACIÓN
                // =====================================================

                var nuevaAsociacion =
                    new TallerConfiguracion
                    {
                        TallerId =
                            dto.TallerId,

                        TallerConfiguracionPresupuestalId =
                            dto.TallerConfiguracionPresupuestalId,

                        Activo =
                            dto.Activo,

                        FechaCreacion =
                            DateTime.Now
                    };

                _context.TallerConfiguracions.Add(
                    nuevaAsociacion);


                // =====================================================
                // 6. GUARDAR
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 7. AUDITORÍA
                // =====================================================

                // Mantener aquí el mismo mecanismo de auditoría
                // utilizado actualmente en MaquinariaService.


                // =====================================================
                // 8. RESPUESTA
                // =====================================================

                return BaseResponse.Success(
                    "La configuración presupuestal fue asociada al taller correctamente.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de base de datos al asociar configuración {ConfiguracionId} al taller {TallerId}.",
                    dto.TallerConfiguracionPresupuestalId,
                    dto.TallerId);

                return BaseResponse.Error(
                    "No fue posible guardar la asociación. " +
                    "Verifique que el taller no tenga previamente esta configuración presupuestal.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al asociar configuración presupuestal {ConfiguracionId} al taller {TallerId}.",
                    dto.TallerConfiguracionPresupuestalId,
                    dto.TallerId);

                return BaseResponse.Error(
                    "Ocurrió un error al asociar la configuración presupuestal al taller.");
            }
        }



        public async Task<BaseResponse> EditarTallerConfiguracionAsync(TallerConfiguracionEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================================================
                // 1. VALIDACIONES BÁSICAS
                // =====================================================

                if (dto.TallerId <= 0)
                {
                    return BaseResponse.Error(
                        "El taller no es válido.");
                }

                if (dto.ConfiguracionPresupuestalOriginalId <= 0)
                {
                    return BaseResponse.Error(
                        "La configuración presupuestal original no es válida.");
                }

                if (dto.TallerConfiguracionPresupuestalId <= 0)
                {
                    return BaseResponse.Error(
                        "Debe seleccionar una configuración presupuestal.");
                }


                // =====================================================
                // 2. OBTENER TALLER
                // =====================================================

                var taller =
                    await _context.Tallers
                        .FirstOrDefaultAsync(x =>
                            x.TallerId == dto.TallerId);

                if (taller == null)
                {
                    return BaseResponse.Error(
                        "El taller no existe.");
                }

                if (!taller.Activo)
                {
                    return BaseResponse.Error(
                        "No es posible modificar la configuración de un taller inactivo.");
                }


                // =====================================================
                // 3. OBTENER ASOCIACIÓN ORIGINAL
                // =====================================================

                var asociacionOriginal =
                    await _context.TallerConfiguracions
                        .FirstOrDefaultAsync(x =>
                            x.TallerId == dto.TallerId &&
                            x.TallerConfiguracionPresupuestalId ==
                                dto.ConfiguracionPresupuestalOriginalId);

                if (asociacionOriginal == null)
                {
                    return BaseResponse.Error(
                        "La configuración original del taller no existe.");
                }


                // =====================================================
                // 4. DETERMINAR CAMBIOS
                // =====================================================

                var cambioConfiguracion =
                    dto.ConfiguracionPresupuestalOriginalId !=
                    dto.TallerConfiguracionPresupuestalId;

                var cambioActivo =
                    asociacionOriginal.Activo !=
                    dto.Activo;


                // =====================================================
                // 5. SI NO HAY CAMBIOS
                // =====================================================

                if (!cambioConfiguracion &&
                    !cambioActivo)
                {
                    return BaseResponse.Success(
                        "La configuración del taller ya se encuentra actualizada.");
                }


                // =====================================================
                // 6. PROTEGER CONFIGURACIÓN UTILIZADA
                // =====================================================

                var tieneMovimientos =
                    await TieneMovimientosPresupuestalesAsync(
                        dto.TallerId,
                        dto.ConfiguracionPresupuestalOriginalId);

                if (tieneMovimientos)
                {
                    return BaseResponse.Error(
                        "No es posible modificar esta configuración porque ya existen movimientos presupuestales asociados. La configuración utilizada en movimientos históricos debe conservarse.");
                }


                // =====================================================
                // 7. SI SOLO CAMBIA EL ESTADO
                // =====================================================

                if (!cambioConfiguracion)
                {
                    asociacionOriginal.Activo =
                        dto.Activo;

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return BaseResponse.Success(
                        dto.Activo
                            ? "La configuración del taller fue activada correctamente."
                            : "La configuración del taller fue desactivada correctamente.");
                }


                // =====================================================
                // 8. OBTENER CONFIGURACIÓN DESTINO
                // =====================================================

                var configuracionDestino =
                    await _context.TallerConfiguracionPresupuestals
                        .FirstOrDefaultAsync(x =>
                            x.TallerConfiguracionPresupuestalId ==
                            dto.TallerConfiguracionPresupuestalId);

                if (configuracionDestino == null)
                {
                    return BaseResponse.Error(
                        "La configuración presupuestal seleccionada no existe.");
                }

                if (!configuracionDestino.Activo)
                {
                    return BaseResponse.Error(
                        "No es posible asociar una configuración presupuestal inactiva.");
                }


                // =====================================================
                // 9. VALIDAR QUE NO EXISTA YA LA CONFIGURACIÓN DESTINO
                // =====================================================

                var yaExisteDestino =
                    await _context.TallerConfiguracions
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.TallerId == dto.TallerId &&
                            x.TallerConfiguracionPresupuestalId ==
                                dto.TallerConfiguracionPresupuestalId);

                if (yaExisteDestino)
                {
                    return BaseResponse.Error(
                        "La configuración presupuestal seleccionada ya está asociada al taller.");
                }


                // =====================================================
                // 10. ELIMINAR ASOCIACIÓN ORIGINAL
                // =====================================================

                _context.TallerConfiguracions.Remove(
                    asociacionOriginal);

                await _context.SaveChangesAsync();


                // =====================================================
                // 11. CREAR NUEVA ASOCIACIÓN
                // =====================================================

                var nuevaAsociacion =
                    new TallerConfiguracion
                    {
                        TallerId =
                            dto.TallerId,

                        TallerConfiguracionPresupuestalId =
                            dto.TallerConfiguracionPresupuestalId,

                        Activo =
                            dto.Activo,

                        FechaCreacion =
                            asociacionOriginal.FechaCreacion
                    };

                _context.TallerConfiguracions.Add(
                    nuevaAsociacion);


                // =====================================================
                // 12. GUARDAR NUEVA ASOCIACIÓN
                // =====================================================

                await _context.SaveChangesAsync();


                // =====================================================
                // 13. CONFIRMAR TRANSACCIÓN
                // =====================================================

                await transaction.CommitAsync();


                // =====================================================
                // 14. RESPUESTA
                // =====================================================

                return BaseResponse.Success(
                    "La configuración presupuestal del taller fue actualizada correctamente.");
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error de base de datos al editar configuración del taller {TallerId}.",
                    dto.TallerId);

                return BaseResponse.Error(
                    "No fue posible guardar los cambios de la configuración del taller.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    ex,
                    "Error al editar configuración del taller {TallerId}.",
                    dto.TallerId);

                return BaseResponse.Error(
                    "Ocurrió un error al actualizar la configuración del taller.");
            }
        }




        private async Task<bool> TieneMovimientosPresupuestalesAsync(int tallerId, int configuracionPresupuestalId)
        {
            return await
                (from movimiento in _context.TallerMovimientoPresupuestos
                 join control in _context.TallerControlPresupuestals
                     on movimiento.IdControlTaller
                     equals control.IdControlTaller
                 where
                     movimiento.TallerId == tallerId
                     &&
                     control.TallerConfiguracionPresupuestalId ==
                         configuracionPresupuestalId
                 select movimiento)
                .AnyAsync();
        }







        public async Task<List<FirmanteDto>> ObtenerTodosAsync(int? areaId = null)
        {
            var query = _context.Firmantes
                .Include(f => f.Area)
                .AsNoTracking();

            // Si se especifica un área en el filtro, filtrar estrictamente por ese AreaId
            if (areaId.HasValue && areaId.Value > 0)
            {
                query = query.Where(f => f.AreaId == areaId.Value);
            }

            return await query.Select(f => new FirmanteDto
            {
                FirmanteId = f.FirmanteId,
                ClavePuesto = f.ClavePuesto,
                NombreCompleto = f.NombreCompleto,
                Cargo = f.Cargo,
                AreaId = f.AreaId,
                AreaNombre = f.Area != null ? f.Area.AreaNombre : "GLOBAL (TODAS LAS ÁREAS)",
                Activo = f.Activo,
                FechaCreacion = f.FechaCreacion
            })
            .OrderBy(f => f.ClavePuesto)
            .ThenBy(f => f.AreaId)
            .ToListAsync();
        }




        public async Task<FirmanteDto?> ObtenerPorIdAsync(int firmanteId)
        {
            var firmante = await _context.Firmantes
                .Include(f => f.Area)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FirmanteId == firmanteId);

            if (firmante == null) return null;

            return new FirmanteDto
            {
                FirmanteId = firmante.FirmanteId,
                ClavePuesto = firmante.ClavePuesto,
                NombreCompleto = firmante.NombreCompleto,
                Cargo = firmante.Cargo,
                AreaId = firmante.AreaId,
                AreaNombre = firmante.Area?.AreaNombre,
                Activo = firmante.Activo,
                FechaCreacion = firmante.FechaCreacion
            };
        }

        public async Task<bool> GuardarAsync(FirmanteDto dto)
        {
            if (dto.FirmanteId == 0)
            {
                var nuevoFirmante = new Firmante
                {
                    ClavePuesto = dto.ClavePuesto.ToUpper().Trim(),
                    NombreCompleto = dto.NombreCompleto.ToUpper().Trim(),
                    Cargo = dto.Cargo.ToUpper().Trim(),
                    AreaId = dto.AreaId,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Firmantes.Add(nuevoFirmante);
            }
            else
            {
                var firmanteExistente = await _context.Firmantes.FindAsync(dto.FirmanteId);
                if (firmanteExistente == null) return false;

                firmanteExistente.ClavePuesto = dto.ClavePuesto.ToUpper().Trim();
                firmanteExistente.NombreCompleto = dto.NombreCompleto.ToUpper().Trim();
                firmanteExistente.Cargo = dto.Cargo.ToUpper().Trim();
                firmanteExistente.AreaId = dto.AreaId;
                firmanteExistente.Activo = dto.Activo;
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CambiarEstatusAsync(int firmanteId)
        {
            var firmante = await _context.Firmantes.FindAsync(firmanteId);
            if (firmante == null) return false;

            firmante.Activo = !firmante.Activo;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAsync(int firmanteId)
        {
            var firmante = await _context.Firmantes.FindAsync(firmanteId);
            if (firmante == null) return false;

            _context.Firmantes.Remove(firmante);
            return await _context.SaveChangesAsync() > 0;
        }









        public async Task<bool> GuardarFirmanteAsync(FirmanteDto dto)
        {
            try
            {
                if (dto == null)
                    return false;

                // Limpieza y formateo de cadenas a mayúsculas según norma institucional
                string clavePuestoLimpia = dto.ClavePuesto?.Trim().ToUpper() ?? string.Empty;
                string nombreLimpio = dto.NombreCompleto?.Trim().ToUpper() ?? string.Empty;
                string cargoLimpio = dto.Cargo?.Trim().ToUpper() ?? string.Empty;

                // Validar que los datos obligatorios no vengan vacíos
                if (string.IsNullOrWhiteSpace(clavePuestoLimpia) ||
                    string.IsNullOrWhiteSpace(nombreLimpio) ||
                    string.IsNullOrWhiteSpace(cargoLimpio))
                {
                    return false;
                }

                // 1. REGISTRO NUEVO (FirmanteId == 0)
                if (dto.FirmanteId == 0)
                {
                    var nuevoFirmante = new Firmante
                    {
                        ClavePuesto = clavePuestoLimpia,
                        NombreCompleto = nombreLimpio,
                        Cargo = cargoLimpio,
                        AreaId = dto.AreaId > 0 ? dto.AreaId : null, // Si no se selecciona área, es Global (NULL)
                        Activo = dto.Activo,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Firmantes.Add(nuevoFirmante);
                }
                // 2. ACTUALIZACIÓN DE REGISTRO EXISTENTE
                else
                {
                    var firmanteExistente = await _context.Firmantes
                        .FirstOrDefaultAsync(f => f.FirmanteId == dto.FirmanteId);

                    if (firmanteExistente == null)
                        return false;

                    firmanteExistente.ClavePuesto = clavePuestoLimpia;
                    firmanteExistente.NombreCompleto = nombreLimpio;
                    firmanteExistente.Cargo = cargoLimpio;
                    firmanteExistente.AreaId = dto.AreaId > 0 ? dto.AreaId : null;
                    firmanteExistente.Activo = dto.Activo;

                    _context.Firmantes.Update(firmanteExistente);
                }

                // Guardar cambios en la BD
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la información del firmante ID {FirmanteId}", dto?.FirmanteId);
                return false;
            }
        }









        public async Task<byte[]> GenerarOficioSolicitudPdfAsync(int solicitudMantenimientoId)
        {
            var solicitud = await _context.SolicitudMantenimientos
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == solicitudMantenimientoId)
                .Select(x => new
                {
                    x.Folio,
                    x.NumeroOficio,
                    x.FechaSolicitud,
                    AreaSolicitante = x.AreaSolicitante.AreaNombre,
                    AreaId = x.AreaSolicitanteId,
                    Unidad = new
                    {
                        Marca = x.UnidadVehicular.Marca.MarcaNombre,
                        Modelo = x.UnidadVehicular.Modelo.ModeloNombre,
                        Anio = x.UnidadVehicular.Anio.AnioDescripcion,
                        Serie = x.UnidadVehicular.NumeroSerie,
                        Placa = x.UnidadVehicular.PlacaActual,
                        Economico = x.UnidadVehicular.NumeroEconomico,
                        Cilindros = x.UnidadVehicular.Cilindro.CilindroNombre,
                        Km = x.KilometrajeActual
                    },
                    Taller = x.Taller != null ? x.Taller.RazonSocial : "SIN TALLER",
                    Detalles = x.SolicitudMantenimientoDetalles
                        .Where(d => d.Activo)
                        .Select(d => new
                        {
                            d.ConceptoServicio.ConceptoServicioNombre,
                            d.Cantidad,
                            d.Observaciones
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (solicitud == null)
                throw new Exception("No se encontró la solicitud de mantenimiento especificada.");

            // Obtener signatarios activos
            var OficialMayor = await _context.Firmantes.FirstOrDefaultAsync(s => s.Cargo.Contains("OFICIAL MAYOR") && s.Activo && s.FirmanteId == 9);
            var Solicitante = await _context.Firmantes.FirstOrDefaultAsync(s => s.ClavePuesto.Contains("SOLICITANTE") && s.Activo && solicitud.AreaId == s.Area.AreaId);
            var Director = await _context.Firmantes.FirstOrDefaultAsync(s => s.ClavePuesto.Contains("DIRECTOR_RECURSOS") && s.Activo && s.FirmanteId == 2);
            var subdirector = await _context.Firmantes.FirstOrDefaultAsync(s => s.ClavePuesto.Contains("SUBDIRECTOR_MAQUINARIA") && s.Activo);

            string fechaFormateada = $"Xalapa, Veracruz, a {solicitud.FechaSolicitud.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX"))}";

            // Generar documento PDF con QuestPDF
            var documento = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    // Reducción estratégica del margen (1.6 cm) para maximizar área útil vertical
                    page.Margin(1.6f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10f).FontFamily("Gotham").FontColor(Colors.Black));

                    // --- CINTILLO FIJO AL PIE DE PÁGINA ---
                    page.Footer().Element(footerContainer =>
                    {
                        var cintilloPath = Path.Combine(_env.WebRootPath, "img", "veracruz", "Cintillo_Corazón.png");

                        if (File.Exists(cintilloPath))
                        {
                            footerContainer.ExtendHorizontal().Image(cintilloPath, ImageScaling.FitWidth);
                        }
                        else
                        {
                            footerContainer.AlignCenter().Text("CINTILLO VERACRUZ — SSP").Bold()
                                .FontSize(8).FontFamily("Gotham").FontColor(Colors.Grey.Darken2);
                        }
                    });

                    // Contenido principal de la hoja con distribución calibrada
                    page.Content().Column(col =>
                    {
                        // Espaciado dinámico balanceado
                        col.Spacing(6);

                        // --- LOGO / ENCABEZADO SUPERIOR ---
                        var logoPath = Path.Combine(_env.WebRootPath, "img", "veracruz", "veracruz2025_2.png");

                        if (File.Exists(logoPath))
                        {
                            col.Item().Height(44).AlignLeft().Image(logoPath, ImageScaling.FitArea);
                        }
                        else
                        {
                            col.Item().Text("GOBIERNO DEL ESTADO DE VERACRUZ — SSP").Bold()
                                .FontSize(8).FontFamily("Gotham").FontColor(Colors.Grey.Darken2);
                        }

                        // Membrete superior derecho
                        col.Item().AlignRight().Column(c =>
                        {
                            c.Spacing(1.5f);
                            c.Item().Text(solicitud.AreaSolicitante.ToUpper()).FontSize(9).FontFamily("Gotham");
                            c.Item().Text($"Oficio No. {solicitud.NumeroOficio}").FontSize(9).FontFamily("Gotham");
                            c.Item().Text($"Asunto: Solicitud de mantenimiento vehicular").FontSize(9).FontFamily("Gotham");
                            c.Item().PaddingTop(2).Text(fechaFormateada).FontSize(9).FontFamily("Gotham");
                        });

                        col.Item().PaddingTop(4);

                        // Destinatario
                        col.Item().Column(c =>
                        {
                            c.Spacing(1);
                            c.Item().Text(OficialMayor?.NombreCompleto ?? "C. OFICIAL MAYOR").Bold().FontFamily("Gotham").FontSize(10.5f);
                            c.Item().Text(OficialMayor?.Cargo ?? "OFICIAL MAYOR").Bold().FontFamily("Gotham").FontSize(10.5f);
                            c.Item().Text("P R E S E N T E").Bold().FontFamily("Gotham").FontSize(10.5f);
                        });

                        col.Item().PaddingTop(4);

                        // Párrafo inicial
                        col.Item().Text(text =>
                        {
                            text.Justify();
                            text.Span("Por medio del presente, me permito requerir de su valioso apoyo para que sea autorizado el servicio de mantenimiento vehicular, relativo a la unidad automotora que se describe a continuación:")
                                .FontSize(10f).FontFamily("Gotham");
                        });

                        col.Item().PaddingTop(2);

                        // Tabla de Especificaciones del Vehículo (Padding óptimo 3.5f)
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(0.7f);
                                columns.RelativeColumn(1.8f);
                                columns.RelativeColumn(1.1f);
                                columns.RelativeColumn(0.9f);
                                columns.RelativeColumn(0.6f);
                                columns.RelativeColumn(0.8f);
                            });

                            table.Header(header =>
                            {
                                void HeaderCell(string text) => header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(3f).AlignCenter().Text(text).Bold().FontSize(8.5f).FontFamily("Gotham");

                                HeaderCell("Marca");
                                HeaderCell("Tipo");
                                HeaderCell("Mod.");
                                HeaderCell("Núm. Serie");
                                HeaderCell("Placa");
                                HeaderCell("Núm. Econ.");
                                HeaderCell("Cil.");
                                HeaderCell("Km.");
                            });

                            void DataCell(string text) => table.Cell().Border(1).BorderColor(Colors.Black).Padding(3f).AlignCenter().Text(text).FontSize(8f).FontFamily("Gotham");

                            DataCell(solicitud.Unidad.Marca);
                            DataCell(solicitud.Unidad.Modelo);
                            DataCell(solicitud.Unidad.Anio.ToString());
                            DataCell(solicitud.Unidad.Serie);
                            DataCell(solicitud.Unidad.Placa);
                            DataCell(solicitud.Unidad.Economico);
                            DataCell(solicitud.Unidad.Cilindros?.ToString() ?? "N/A");
                            DataCell(solicitud.Unidad.Km.ToString("N0"));
                        });

                        col.Item().PaddingTop(4);

                        // --- SECCIÓN DE SERVICIOS SOLICITADOS ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35); // Cantidad
                                columns.RelativeColumn();   // Concepto
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(3f).AlignCenter().Text("Cant.").Bold().FontSize(8.5f);
                                header.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(3f).AlignLeft().Text("Servicio Solicitado").Bold().FontSize(8.5f);
                            });

                            foreach (var det in solicitud.Detalles)
                            {
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3f).AlignCenter().Text(det.Cantidad.ToString()).FontSize(8.5f).FontFamily("Gotham");
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(3f).AlignLeft().Column(c =>
                                {
                                    c.Item().Text(det.ConceptoServicioNombre).FontSize(8.5f).FontFamily("Gotham");
                                    if (!string.IsNullOrWhiteSpace(det.Observaciones))
                                    {
                                        c.Item().Text(det.Observaciones).FontSize(8f).FontColor(Colors.Grey.Darken2);
                                    }
                                });
                            }
                        });

                        col.Item().PaddingTop(4);

                        // --- PÁRRAFOS COMPLEMENTARIOS ---
                        col.Item().Text(text =>
                        {
                            text.Justify();
                            text.Span("Derivado de lo anterior, adjunto cotización emitida por el prestador de servicios ")
                                .FontSize(10f).FontFamily("Gotham");
                            text.Span(solicitud.Taller).Bold().FontSize(10f).FontFamily("Gotham");
                            text.Span(", el cual fue considerado para la reparación del vehículo antes citado, ya que ofrece las mejores condiciones en cuanto a precio, tiempo de respuesta y calidad en el servicio.")
                                .FontSize(10f).FontFamily("Gotham");
                        });

                        col.Item().Text(text =>
                        {
                            text.Justify();
                            text.Span("Cabe mencionar que dicha petición es con la finalidad de mantener en óptimas condiciones el parque vehicular de esta ")
                                .FontSize(10f).FontFamily("Gotham");
                            text.Span(solicitud.AreaSolicitante).Bold().FontSize(10f).FontFamily("Gotham");
                            text.Span(" y garantizar su uso operativo.").FontSize(10f).FontFamily("Gotham");
                        });

                        col.Item().Text(text =>
                        {
                            text.Justify();
                            text.Span("Sin más por el momento, me suscribo a sus órdenes.")
                                .FontSize(10f).FontFamily("Gotham");
                        });

                        // --- BLOQUE DE FIRMA (PaddingTop amplio para dar peso visual y llenar espacio) ---
                        col.Item().PaddingTop(12).AlignLeft().Column(c =>
                        {
                            c.Spacing(1);
                            c.Item().Text("A T E N T A M E N T E").Bold().FontSize(10f).FontFamily("Gotham");
                            c.Item().PaddingTop(18).Text(Solicitante?.NombreCompleto ?? "Sin Datos... verifique el catálogo.").Bold().FontFamily("Gotham").FontSize(10f);
                            c.Item().Text(solicitud.AreaSolicitante).FontSize(10f).FontFamily("Gotham").Bold();
                        });

                        // --- C.C.P. ---
                        col.Item().PaddingTop(6).Column(c =>
                        {
                            c.Spacing(1);
                            c.Item().Text($"C.C.P. {Director?.NombreCompleto ?? "Sin datos."}.- Director de Recursos Materiales y Servicios Generales. - Para su conocimiento.").FontSize(8f).FontColor(Colors.Grey.Darken2).FontFamily("Gotham");
                            c.Item().Text($"C.C.P. {subdirector?.NombreCompleto ?? "Sin datos."}.- Subdirector de Maquinaria. - Para su conocimiento.").FontSize(8f).FontColor(Colors.Grey.Darken2).FontFamily("Gotham");
                        });
                    });
                });
            });

            byte[] pdfBytes = documento.GeneratePdf();
            return await Task.FromResult(pdfBytes);
        }




        public async Task<byte[]> GenerarDictamenTecnicoPdfAsync(int solicitudId)
        {
            // 1. Consultar la solicitud de mantenimiento con todas sus relaciones de forma segura
            var solicitud = await _context.SolicitudMantenimientos
                .Include(s => s.AreaSolicitante)
                .Include(s => s.EstadoOdometro)
                .Include(s => s.EstatusSolicitudMantenimiento)
                .Include(s => s.TipoMantenimiento)
                .Include(s => s.Taller)
                .Include(s => s.UnidadVehicular)
                    .ThenInclude(u => u.Marca)
                .Include(s => s.UnidadVehicular)
                    .ThenInclude(u => u.Modelo)
                .Include(s => s.UnidadVehicular)
                    .ThenInclude(u => u.Anio)
                .Include(s => s.UnidadVehicular)
                    .ThenInclude(u => u.Cilindro)
                .Include(s => s.SolicitudMantenimientoDetalles)
                    .ThenInclude(d => d.ConceptoServicio)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SolicitudMantenimientoId == solicitudId && s.Activo == true);

            if (solicitud == null)
            {
                throw new Exception($"No se encontró la solicitud activa con ID {solicitudId} para generar el dictamen técnico.");
            }

            // 2. Resolver datos del vehículo de manera 100% segura ante nulos
            var unidad = solicitud.UnidadVehicular;
            var marca = unidad?.Marca?.MarcaNombre ?? "N/A";
            var modeloNombre = unidad?.Modelo?.ModeloNombre ?? "N/A";
            var placa = string.IsNullOrWhiteSpace(solicitud.PlacaVehiculo) ? (unidad?.PlacaActual ?? "N/A") : solicitud.PlacaVehiculo;
            var numSerie = unidad?.NumeroSerie ?? "N/A";
            var numEcon = unidad?.NumeroEconomico ?? "S/N";
            var anioModelo = unidad?.Anio?.AnioDescripcion ?? "N/A";
            var cilindros = unidad?.Cilindro?.CilindroNombre ?? "N/A";
            var kilometraje = solicitud.KilometrajeActual.ToString("N0");
            var fechaFormateada = (solicitud.FechaAutorizacion ?? solicitud.FechaSolicitud).ToString("dd/MM/yyyy");

            // 3. Lógica de tipos de mantenimiento (Casillas)
            bool esPreventivo = solicitud.TipoMantenimientoId == 1 || (solicitud.TipoMantenimiento?.Nombre?.Contains("Preventivo", StringComparison.OrdinalIgnoreCase) ?? false);
            bool esCorrectivo = solicitud.TipoMantenimientoId == 2 || (solicitud.TipoMantenimiento?.Nombre?.Contains("Correctivo", StringComparison.OrdinalIgnoreCase) ?? false);
            bool esEnLlantado = solicitud.TipoMantenimientoId == 7 || (solicitud.TipoMantenimiento?.Nombre?.Contains("Enllantado", StringComparison.OrdinalIgnoreCase) ?? false);

            bool tieneRefacciones = solicitud.SolicitudMantenimientoDetalles != null && solicitud.SolicitudMantenimientoDetalles.Any(d => d.CostoEstimadoRefacciones > 0);
            bool tieneManoObra = solicitud.SolicitudMantenimientoDetalles != null && solicitud.SolicitudMantenimientoDetalles.Any(d => d.CostoEstimadoManoObra > 0);

            // 4. Consultar firmantes activos (Área + Globales)
            int? areaId = solicitud.AreaSolicitanteId;
            var firmantesConsulta = await _context.Firmantes
                .Where(f => f.Activo && (f.AreaId == areaId || f.AreaId == null))
                .AsNoTracking()
                .ToListAsync();

            var dictFirmantes = firmantesConsulta
                .GroupBy(f => f.ClavePuesto)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(f => f.AreaId.HasValue && f.AreaId == areaId).First()
                );

            string ObtenerNombre(string clave) =>
                dictFirmantes.TryGetValue(clave, out var f) ? f.NombreCompleto ?? "" : "";

            string ObtenerCargo(string clave) =>
                dictFirmantes.TryGetValue(clave, out var f) ? f.Cargo ?? "" : "";

            // 5. Construcción del documento con QuestPDF
            var documento = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(0.8f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(7.5f).FontFamily("Gotham").FontColor(Colors.Black));

                    page.Content().Column(col =>
                    {
                        col.Spacing(2.5f);

                        // --- 1. LOGO / ENCABEZADO ---
                        var logoPath = Path.Combine(_env.WebRootPath, "img", "veracruz", "veracruz2025_2.png");
                        if (File.Exists(logoPath))
                        {
                            col.Item().Height(50).AlignLeft().Image(logoPath, ImageScaling.FitArea);
                        }
                        else
                        {
                            col.Item().Text("GOBIERNO DEL ESTADO DE VERACRUZ — SSP").Bold().FontSize(7.5f).FontColor(Colors.Grey.Darken2);
                        }

                        // --- 2. TABLA DE TÍTULO, FOLIO, FECHA Y OFICIO ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(0.65f); // Columna Izquierda (Título / Área)
                                c.RelativeColumn(0.12f); // Columna Etiqueta (FOLIO, FECHA, No. OFICIO)
                                c.RelativeColumn(0.23f); // Columna Valor
                            });

                            // Fila 1: Título del Dictamen (Fila de Folio)
                            table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(4).AlignCenter().AlignMiddle()
                                .Text("DICTAMEN TÉCNICO DE AUTORIZACIÓN PARA INGRESO A TALLER").Bold().FontSize(8.5f);

                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text("FOLIO:").Bold().FontSize(7f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(solicitud.Folio).Bold().FontSize(7.5f);

                            // Fila 2 y 3: Área (Abarca 2 filas hacia abajo para Fecha y No. Oficio)
                            table.Cell().RowSpan(2).Border(1).BorderColor(Colors.Black).Padding(4).AlignLeft().AlignMiddle()
                                .Text($"ÁREA: {(solicitud.AreaSolicitante?.AreaNombre ?? "N/A").ToUpper()}").Bold().FontSize(7.5f);

                            // Fila 2 (Derecha): Fecha
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text("FECHA:").Bold().FontSize(7f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(fechaFormateada).FontSize(7.5f);

                            // Fila 3 (Derecha): No. Oficio
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text("No. OFICIO:").Bold().FontSize(7f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().AlignMiddle().Text(solicitud.NumeroOficio ?? "").FontSize(7f);
                        });

                        // --- 3. MATRIZ DE DATOS DE LA UNIDAD VEHICULAR ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(14f);
                                c.RelativeColumn(22f);
                                c.RelativeColumn(12f);
                                c.RelativeColumn(22f);
                                c.RelativeColumn(10f);
                                c.RelativeColumn(8f);
                                c.RelativeColumn(6f);
                                c.RelativeColumn(6f);
                            });

                            void HeaderCell(string text) => table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(text).Bold().FontSize(6.5f);
                            void DataCell(string text, bool bold = false)
                            {
                                var cell = table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter();
                                var txt = cell.Text(text).FontSize(7f);
                                if (bold) txt.Bold();
                            }

                            HeaderCell("MARCA");
                            HeaderCell("TIPO");
                            HeaderCell("PLACA");
                            HeaderCell("SERIE");
                            HeaderCell("NUM. ECON.");
                            HeaderCell("MODELO");
                            HeaderCell("CIL.");
                            HeaderCell("KM.");

                            DataCell(marca, true);
                            DataCell(modeloNombre);
                            DataCell(placa, true);
                            DataCell(numSerie, false);
                            DataCell(numEcon);
                            DataCell(anioModelo);
                            DataCell(cilindros);
                            DataCell(kilometraje);
                        });

                        // --- PRESTADOR DE SERVICIOS ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(120);
                                c.RelativeColumn();
                            });
                            table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(2.5f).Text("PRESTADOR DE SERVICIOS:").Bold().FontSize(7f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2.5f).Text(solicitud.Taller?.RazonSocial ?? "POR ASIGNAR").Bold().FontSize(7f);
                        });

                        // --- 4. DETALLE DE SERVICIOS AUTORIZADOS (Con altura flexible y expansiva) ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c => { c.RelativeColumn(); });

                            table.Cell().Border(1).BorderColor(Colors.Black).Column(cHeader =>
                            {
                                cHeader.Item().Background(Colors.Grey.Lighten3).Padding(2.5f).Text("DETALLE DE SERVICIOS AUTORIZADOS:").Bold().FontSize(7f);

                                // MinHeight asegura que si hay pocos servicios, la caja se estire verticalmente 
                                // para ocupar el espacio inferior, empujando las firmas de forma ordenada hacia abajo.
                                // Si hay muchos servicios, crecerá automáticamente de manera fluida.
                                cHeader.Item().MinHeight(200).Padding(4).Column(cList =>
                                {
                                    if (solicitud.SolicitudMantenimientoDetalles != null && solicitud.SolicitudMantenimientoDetalles.Any(d => d.Activo))
                                    {
                                        foreach (var item in solicitud.SolicitudMantenimientoDetalles.Where(d => d.Activo))
                                        {
                                            cList.Item().PaddingBottom(3).Text(text =>
                                            {
                                                text.Span($"• ({item.Cantidad}) ").Bold();
                                                text.Span(item.ConceptoServicio?.ConceptoServicioNombre ?? "SERVICIO");
                                                if (!string.IsNullOrWhiteSpace(item.Observaciones))
                                                {
                                                    text.Span($" — ({item.Observaciones})").FontColor(Colors.Grey.Darken2).FontSize(6.5f);
                                                }
                                            });
                                        }
                                    }
                                    else
                                    {
                                        cList.Item().Text(string.IsNullOrWhiteSpace(solicitud.MotivoSolicitud) ? "Según diagnóstico inicial de revisión técnica." : solicitud.MotivoSolicitud).Italic().FontSize(7f);
                                    }
                                });
                            });
                        });

                        // --- TABLA DE CASILLAS (REFACCIONES / MANO DE OBRA) ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(38f);
                                c.RelativeColumn(12f);
                                c.RelativeColumn(38f);
                                c.RelativeColumn(12f);
                            });

                            void CheckRow(string label1, bool val1, string label2, bool val2)
                            {
                                table.Cell().Background(Colors.Grey.Lighten5).Border(1).BorderColor(Colors.Black).Padding(2).Text(label1).Bold().FontSize(6.5f);
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(val1 ? "X" : "").Bold().FontSize(7.5f);
                                table.Cell().Background(Colors.Grey.Lighten5).Border(1).BorderColor(Colors.Black).Padding(2).Text(label2).Bold().FontSize(6.5f);
                                table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(val2 ? "X" : "").Bold().FontSize(7.5f);
                            }

                            CheckRow("REFACCIONES:", tieneRefacciones, "MANO DE OBRA:", tieneManoObra);
                        });

                        // --- TABLA DE TIPOS DE SERVICIO ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(25.33f);
                                c.RelativeColumn(8f);
                                c.RelativeColumn(25.33f);
                                c.RelativeColumn(8f);
                                c.RelativeColumn(25.33f);
                                c.RelativeColumn(8f);
                            });

                            table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(2).Text("SERVICIO MECÁNICO PREVENTIVO:").Bold().FontSize(6f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(esPreventivo ? "X" : "").Bold().FontSize(7.5f);
                            table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(2).Text("SERVICIO MECÁNICO CORRECTIVO:").Bold().FontSize(6f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(esCorrectivo ? "X" : "").Bold().FontSize(7.5f);
                            table.Cell().Background(Colors.Grey.Lighten3).Border(1).BorderColor(Colors.Black).Padding(2).Text("SERVICIO DE ENLLANTADO:").Bold().FontSize(6f);
                            table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(esEnLlantado ? "X" : "").Bold().FontSize(7.5f);
                        });


                        // --- BLOQUE DE FIRMAS UNIFICADO ---
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(); // Columna Izquierda
                                c.RelativeColumn(); // Columna Derecha
                            });

                            void FirmaCell(ColumnDescriptor parentCol, string titulo, string nombre, string cargo, float alturaCelda, float paddingTopContenido)
                            {
                                // Fijamos la altura total exacta de cada bloque de firma para que NUNCA varíe ni suba
                                parentCol.Item().Height(alturaCelda).Border(1).BorderColor(Colors.Black).Column(cSig =>
                                {
                                    // Título ocupando el 100% del ancho con fondo y borde inferior
                                    cSig.Item().Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Black).PaddingVertical(2).AlignCenter().Text(titulo).Bold().FontSize(5.5f);

                                    // Contenido interior estructurado
                                    cSig.Item().Padding(2f).Column(cContent =>
                                    {
                                        // Padding superior seguro que deja libre el espacio exacto para la firma autógrafa
                                        cContent.Item().PaddingTop(paddingTopContenido);

                                        // Contenedor para el nombre y cargo en la base
                                        cContent.Item().AlignCenter().Column(cTextos =>
                                        {
                                            cTextos.Item().AlignCenter().Text(nombre).FontSize(6f);
                                            cTextos.Item().AlignCenter().Text(cargo).Bold().FontSize(6f).FontColor(Colors.Grey.Darken2);
                                        });
                                    });
                                });
                            }

                            // --- COLUMNA IZQUIERDA (2 Firmas con altura fija de 110f y padding superior de 52f para dejar espacio libre arriba) ---
                            table.Cell().Column(colIzquierda =>
                            {
                                colIzquierda.Spacing(0);
                                FirmaCell(colIzquierda, "SOLICITUD DEL ÁREA USUARIA", ObtenerNombre("SOLICITANTE"), ObtenerCargo("SOLICITANTE"), 110f, 52f);
                                FirmaCell(colIzquierda, "SUPERVISA, VERIFICA Y VALIDA REPARACIÓN", ObtenerNombre("SUPERVISOR"), ObtenerCargo("SUPERVISOR"), 110f, 52f);
                            });

                            // --- COLUMNA DERECHA (3 Firmas con altura fija de 73.3f y padding superior de 22f) ---
                            table.Cell().Column(colDerecha =>
                            {
                                colDerecha.Spacing(0);
                                FirmaCell(colDerecha, "REVISIÓN DE SOLICITUD", ObtenerNombre("SUBDIRECTOR_MAQUINARIA"), ObtenerCargo("SUBDIRECTOR_MAQUINARIA"), 73.3f, 40f);
                                FirmaCell(colDerecha, "Vo. Bo.", ObtenerNombre("DIRECTOR_RECURSOS"), ObtenerCargo("DIRECTOR_RECURSOS"), 73.3f, 40f);
                                FirmaCell(colDerecha, "AUTORIZA", ObtenerNombre("AUTORIZA"), ObtenerCargo("AUTORIZA"), 73.3f, 40f);
                            });
                        });

                    });
                });
            });
            return documento.GeneratePdf();
        }









        /// <summary>
        /// Obtiene el último kilometraje registrado para una unidad vehicular en el sistema.
        /// </summary>
        /// <param name="unidadVehicularId">ID de la unidad vehicular</param>
        /// <returns>El valor máximo de kilometraje histórico o 0 si no cuenta con registros previos.</returns>
        public async Task<int> ObtenerUltimoKilometrajeUnidadAsync(int unidadVehicularId)
        {
            // Verificamos si existen solicitudes previas activas para esta unidad
            var existeHistorial = await _context.SolicitudMantenimientos
                .AnyAsync(x => x.UnidadVehicularId == unidadVehicularId && x.Activo);

            if (!existeHistorial)
            {
                return 0; // Si es la primera vez que se registra una solicitud, no hay histórico que comparar
            }

            // Obtenemos el mayor kilometraje registrado hasta el momento para la unidad
            var ultimoKilometraje = await _context.SolicitudMantenimientos
                .Where(x => x.UnidadVehicularId == unidadVehicularId && x.Activo)
                .MaxAsync(x => (int?)x.KilometrajeActual) ?? 0;

            return ultimoKilometraje;
        }







        public async Task<List<SelectListItem>> ObtenerConfiguracionesPorTaller(int tallerId)
        {
            // 1. Consultamos de manera limpia desde la base de datos incluyendo los datos relacionados necesarios
            var configuracionesRaw = await _context.TallerConfiguracions
                .AsNoTracking()
                .Where(tc => tc.TallerId == tallerId && tc.Activo && tc.TallerConfiguracionPresupuestal.Activo)
                .Select(tc => new
                {
                    Id = tc.TallerConfiguracionPresupuestalId,
                    Contrato = tc.TallerConfiguracionPresupuestal.Contrato,
                    RecursoNombre = tc.TallerConfiguracionPresupuestal.Recurso.Nombre,
                    MontoMaximo = tc.TallerConfiguracionPresupuestal.MontoMaximo
                })
                .ToListAsync();

            // 2. Limpieza forzosa en memoria: Agrupamos estrictamente por el ID (Value) para eliminar cualquier duplicado residual
            var configuracionesUnicas = configuracionesRaw
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"Contrato: {x.Contrato} | Recurso: {x.RecursoNombre} (Monto Máx: {x.MontoMaximo:C})"
                })
                .OrderBy(x => x.Text)
                .ToList();

            return configuracionesUnicas;
        }







    }
}











