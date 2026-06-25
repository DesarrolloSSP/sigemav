using AspNetCoreGeneratedDocument;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Areas.Maquinaria.DTOs;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;
using static SIGEMAV.Services.Implementations.Maquinaria.MaquinariaService;


namespace SIGEMAV.Services.Implementations.Maquinaria
{
    public class MaquinariaService : IMaquinariaService
    {
        private readonly BdSigeMavContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaquinariaService(BdSigeMavContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginacionViewModel<UnidadVehicularViewModel>> ObtenerVehiculos(int pagina = 1, int registrosPorPagina = 10,
          string? busqueda = null,
          int? marcaId = null,
          int? modeloId = null,
          int? areaId = null,
          int? municipioId = null
      )
        {
            var query =
                _context.UnidadVehiculars
                    .AsNoTracking()
                    .Where(x => x.Activo);

            // =========================================
            // FILTRO GENERAL
            // =========================================

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                query =
                    query.Where(x =>

                        x.NumeroSerie.Contains(busqueda)

                        ||

                        x.NumeroEconomico.Contains(busqueda)

                        ||

                        x.PlacaActual.Contains(busqueda)

                        ||

                        x.Marca.MarcaNombre.Contains(busqueda)

                        ||

                        x.Modelo.ModeloNombre.Contains(busqueda)
                    );
            }

            // =========================================
            // FILTROS
            // =========================================

            if (marcaId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.MarcaId == marcaId.Value);
            }

            if (modeloId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.ModeloId == modeloId.Value);
            }

            if (areaId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.AreaId == areaId.Value);
            }

            if (municipioId.HasValue)
            {
                query =
                    query.Where(x =>
                        x.MunicipioId == municipioId.Value);
            }

            // =========================================
            // TOTAL
            // =========================================

            var totalRegistros =
                await query.CountAsync();

            // =========================================
            // REGISTROS
            // =========================================

            var registros =
                await query

                    .OrderBy(x => x.UnidadVehicularId)

                    .Skip((pagina - 1) * registrosPorPagina)

                    .Take(registrosPorPagina)

                    .Select(x =>
                        new UnidadVehicularViewModel
                        {
                            IdUnidadVehicular =
                                x.UnidadVehicularId,

                            NumeroSerie =
                                x.NumeroSerie,

                            NumeroEconomico =
                                x.NumeroEconomico,

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
                                (int)x.AreaId,

                            IdCilindro =
                                (int)x.CilindroId,

                            IdTransmision =
                                (int)x.TransmisionId,

                            Activo =
                                x.Activo,

                            PlacaActual =
                                x.PlacaActual
                        })
                    .ToListAsync();

            return new PaginacionViewModel<UnidadVehicularViewModel>
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
                dto.NumeroSerie =
                    dto.NumeroSerie.Trim();

                dto.PlacaActual =
                    dto.PlacaActual.Trim();

                dto.NumeroEconomico =
                    dto.NumeroEconomico.Trim();

                // NÚMERO DE SERIE

                var existeNumeroSerie =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroSerie
                                == dto.NumeroSerie);

                if (existeNumeroSerie)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ya existe un vehículo con el mismo número de serie."
                    };
                }

                // PLACA

                //var existePlaca =
                //    await _context.UnidadVehiculars
                //        .AnyAsync(x =>
                //            x.PlacaActual
                //                == dto.PlacaActual);

                //if (existePlaca)
                //{
                //    return new ResultadoOperacionDto
                //    {
                //        Exitoso = false,
                //        Mensaje =
                //            "Ya existe un vehículo con la misma placa."
                //    };
                //}

                // NÚMERO ECONÓMICO

                //var existeEconomico =
                //    await _context.UnidadVehiculars
                //        .AnyAsync(x =>
                //            x.NumeroEconomico
                //                == dto.NumeroEconomico);

                //if (existeEconomico)
                //{
                //    return new ResultadoOperacionDto
                //    {
                //        Exitoso = false,
                //        Mensaje =
                //            "Ya existe un vehículo con el mismo número económico."
                //    };
                //}

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
                        "Vehiculo registrado correctamente."
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





        public async Task<ResultadoOperacionDto> EditarVehiculo(UnidadVehicularEditDto dto)
        {
            try
            {
                var vehiculo =
                    await _context.UnidadVehiculars
                        .FirstOrDefaultAsync(x =>
                            x.UnidadVehicularId
                                == dto.IdUnidadVehicular);

                if (vehiculo == null)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje = "Vehículo no encontrado."
                    };
                }

                dto.NumeroSerie =
                    dto.NumeroSerie.Trim();

                dto.PlacaActual =
                    dto.PlacaActual.Trim();

                dto.NumeroEconomico =
                    dto.NumeroEconomico.Trim();

                // NÚMERO DE SERIE

                var serieDuplicada =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroSerie
                                == dto.NumeroSerie
                            &&
                            x.UnidadVehicularId
                                != dto.IdUnidadVehicular);

                if (serieDuplicada)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ya existe un vehículo con el mismo número de serie."
                    };
                }

                // PLACA

                var placaDuplicada =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.PlacaActual
                                == dto.PlacaActual
                            &&
                            x.UnidadVehicularId
                                != dto.IdUnidadVehicular);

                if (placaDuplicada)
                {
                    return new ResultadoOperacionDto
                    {
                        Exitoso = false,
                        Mensaje =
                            "Ya existe un vehículo con la misma placa."
                    };
                }

                // NÚMERO ECONÓMICO

                var economicoDuplicado =
                    await _context.UnidadVehiculars
                        .AnyAsync(x =>
                            x.NumeroEconomico
                                == dto.NumeroEconomico
                            &&
                            x.UnidadVehicularId
                                != dto.IdUnidadVehicular);

                //if (economicoDuplicado)
                //{
                //    return new ResultadoOperacionDto
                //    {
                //        Exitoso = false,
                //        Mensaje =
                //            "Ya existe un vehículo con el mismo número económico."
                //    };
                //}

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

                await _context.SaveChangesAsync();

                return new ResultadoOperacionDto
                {
                    Exitoso = true,
                    Mensaje =
                        "Vehiculo actualizado correctamente."
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


        public async Task<List<SelectListItem>> ObtenerTalleres()
        {
            return await _context.Tallers
                .AsNoTracking()
                .Include(x => x.Recurso)
                .Where(x => x.Activo)
                .OrderBy(x => x.RazonSocial)
                .Select(x => new SelectListItem
                {
                    Value = x.TallerId.ToString(),

                    //Text = x.RazonSocial + " -- " + x.Ubicacion.Trim()
                    Text = x.RazonSocial + " -- " + x.Recurso.Nombre.Trim()
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> ObtenerAreas()
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



        public async Task<PaginacionViewModel<SolicitudMantenimientoViewModel>> ObtenerSolicitudesMantenimiento(int pagina = 1, int registrosPorPagina = 10)
        {
            var query = _context.SolicitudMantenimientos
                .AsNoTracking();

            var totalRegistros = await query.CountAsync();

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


            return new PaginacionViewModel<SolicitudMantenimientoViewModel>
            {
                Registros = registros,

                PaginaActual = pagina,

                RegistrosPorPagina = registrosPorPagina,

                TotalRegistros = totalRegistros,

                TotalConceptos = registros.Sum(r => r.TotalConceptos),

                TotalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)registrosPorPagina)
            };
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





        public async Task CrearSolicitudMantenimiento(SolicitudMantenimientoCreateDto dto, IEnumerable<IFormFile>? archivos)
        {


            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var archivosGuardados = new List<string>();

            try
            {
                // =====================================================
                // VALIDACIONES GENERALES
                // =====================================================

                if (dto == null)
                {
                    throw new Exception(
                        "La información de la solicitud es inválida.");
                }

                if (string.IsNullOrWhiteSpace(dto.NumeroOficio))
                {
                    throw new Exception(
                        "Debe capturar el número de oficio.");
                }



                if (dto.UnidadVehicularId <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar una unidad vehicular.");
                }

                if (dto.TipoMantenimientoId <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar un tipo de mantenimiento.");
                }

                if (dto.TallerId <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar un taller.");
                }

                if (dto.AreaSolicitanteId <= 0)
                {
                    throw new Exception(
                        "Debe seleccionar un área solicitante.");
                }

                if (dto.KilometrajeActual < 0)
                {
                    throw new Exception(
                        "El kilometraje es inválido.");
                }

                if (
                    dto.Servicios == null
                    ||
                    !dto.Servicios.Any())
                {
                    throw new Exception(
                        "Debe capturar al menos un servicio.");
                }

                // =====================================================
                // VALIDAR SERVICIOS
                // =====================================================

                foreach (var servicio in dto.Servicios)
                {
                    if (servicio.ConceptoServicioId <= 0)
                    {
                        throw new Exception(
                            "Existe un servicio inválido.");
                    }

                    if (servicio.Cantidad <= 0)
                    {
                        throw new Exception(
                            "La cantidad del servicio debe ser mayor a cero.");
                    }

                    if (servicio.CostoEstimadoManoObra < 0)
                    {
                        throw new Exception(
                            "El costo estimado de mano de obra es inválido.");
                    }

                    if (servicio.CostoEstimadoRefacciones < 0)
                    {
                        throw new Exception(
                            "El costo estimado de refacciones es inválido.");
                    }
                }

                // GENERAR FOLIO

                var fechaActual = DateTime.Now;

                var mes = fechaActual.ToString("MMM").ToUpperInvariant();

                // OBTENER DEL USUARIO AUTENTICADO

                string Usuario = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "";
                string NombreCompleto = _httpContextAccessor.HttpContext?.User?.Claims?.ToArray()?.FirstOrDefault(c => c.Type == "NombreCompleto")?.Value ?? "";
                string inicialesUsuario = string.Join("", NombreCompleto.Split(' ').Select(n => n[0])).ToUpper();


           

                var inicioMes =
                    new DateTime(
                        fechaActual.Year,
                        fechaActual.Month,
                        1);

                var finMes =
                    inicioMes.AddMonths(1);

                var ultimoFolio = await _context.SolicitudMantenimientos
                .Where(x =>
                    x.FechaCreacion >= inicioMes
                    &&
                    x.FechaCreacion < finMes)
                .OrderByDescending(x =>
                    x.SolicitudMantenimientoId)
                .Select(x => x.Folio)
                .FirstOrDefaultAsync();

                int consecutivo = 1;

                if (!string.IsNullOrWhiteSpace(ultimoFolio))
                {
                    var partes = ultimoFolio.Split('-');

                    if (partes.Length == 3)
                    {
                        int.TryParse(partes[2], out consecutivo);
                        consecutivo++;
                    }
                }

                var folio = $"{mes}-{inicialesUsuario}-{consecutivo:D4}";


                if (folio == null)
                {
                    throw new Exception("No fue posible generar un folio único.");
                }

                // =====================================================
                // VALIDAR ARCHIVOS
                // =====================================================

                const int maxArchivos = 5;

                const long tamanioMaximoArchivo =
                    2 * 1024 * 1024;

                var extensionesPermitidas =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase)
                    {
                ".pdf",
                ".jpg",
                ".jpeg",
                ".png"
                    };

                var contentTypesPermitidos =
                    new HashSet<string>(
                        StringComparer.OrdinalIgnoreCase)
                    {
                "application/pdf",
                "image/jpeg",
                "image/png"
                    };

                var provider =
                    new FileExtensionContentTypeProvider();

                var archivosValidados =
                    new List<(
                        byte[] Bytes,
                        string Extension,
                        string NombreOriginal,
                        string NombreFisico,
                        string HashArchivo
                    )>();

                if (archivos != null)
                {
                    var archivosLista =
                         archivos
                             .Where(x =>
                                 x != null
                                 &&
                                 x.Length > 0
                                 &&
                                 !string.IsNullOrWhiteSpace(x.FileName))
                             .GroupBy(x => new
                             {
                                 Nombre =
                                     x.FileName.Trim().ToUpperInvariant(),

                                 Tamanio =
                                     x.Length
                             })
                             .Select(x => x.First())
                             .ToList();

                    if (archivosLista.Count > maxArchivos)
                    {
                        throw new Exception(
                            $"Solo se permiten {maxArchivos} archivos.");
                    }

                    foreach (var archivo in archivosLista)
                    {
                        // =============================================
                        // VALIDAR TAMAÑO
                        // =============================================

                        if (archivo.Length > tamanioMaximoArchivo)
                        {
                            throw new Exception(
                                $"El archivo '{archivo.FileName}' excede el tamaño máximo permitido de 2 MB.");
                        }

                        // =============================================
                        // VALIDAR NOMBRE
                        // =============================================

                        var nombreOriginal =
                            Path.GetFileName(
                                archivo.FileName?.Trim());

                        if (string.IsNullOrWhiteSpace(nombreOriginal))
                        {
                            throw new Exception(
                                "Nombre de archivo inválido.");
                        }

                        if (nombreOriginal.Length > 150)
                        {
                            throw new Exception(
                                "Nombre de archivo demasiado largo.");
                        }

                        // =============================================
                        // VALIDAR EXTENSION
                        // =============================================

                        var extension =
                            Path.GetExtension(nombreOriginal)
                                .ToLowerInvariant();

                        if (
                            string.IsNullOrWhiteSpace(extension)
                            ||
                            !extensionesPermitidas.Contains(extension))
                        {
                            throw new Exception(
                                $"El archivo '{nombreOriginal}' tiene una extensión no permitida.");
                        }

                        // =============================================
                        // VALIDAR CONTENT TYPE
                        // =============================================

                        if (!provider.TryGetContentType(
                                nombreOriginal,
                                out _))
                        {
                            throw new Exception(
                                $"No fue posible validar el tipo del archivo '{nombreOriginal}'.");
                        }

                        var contentTypeCliente =
                            archivo.ContentType?
                                .Trim()
                                .ToLowerInvariant();

                        if (
                            string.IsNullOrWhiteSpace(contentTypeCliente)
                            ||
                            !contentTypesPermitidos.Contains(contentTypeCliente))
                        {
                            throw new Exception(
                                $"El archivo '{nombreOriginal}' tiene un tipo MIME no permitido.");
                        }

                        // =============================================
                        // LEER BYTES
                        // =============================================

                        using var memoryStream =
                            new MemoryStream();

                        await archivo.CopyToAsync(memoryStream);

                        var bytes =
                            memoryStream.ToArray();

                        if (bytes.Length == 0)
                        {
                            throw new Exception(
                                $"El archivo '{nombreOriginal}' está vacío.");
                        }

                        // =============================================
                        // VALIDAR FIRMA BINARIA
                        // =============================================

                        if (!ArchivoValidoPorFirma(extension, bytes))
                        {
                            throw new Exception(
                                $"El contenido real del archivo '{nombreOriginal}' no coincide con su extensión.");
                        }


                        // =============================================
                        // GENERAR HASH
                        // =============================================

                        string hashArchivo;

                        using (var sha256 = SHA256.Create())
                        {
                            var hashBytes =
                                sha256.ComputeHash(bytes);

                            hashArchivo =
                                Convert.ToHexString(hashBytes);
                        }

                        // =============================================
                        // NORMALIZAR NOMBRE
                        // =============================================

                        nombreOriginal =
                            nombreOriginal.Trim();

                        // VALIDAR DUPLICADOS SOLO EN ESTA CARGA
                        // Por ello se valida HASH + NOMBRE.

                        var duplicadoEnCarga =
                            archivosValidados.Any(x =>

                                x.HashArchivo == hashArchivo
                                &&
                                string.Equals(
                                    x.NombreOriginal,
                                    nombreOriginal,
                                    StringComparison.OrdinalIgnoreCase)
                            );

                        if (duplicadoEnCarga)
                        {
                            continue;
                        }

                        // =============================================
                        // GENERAR NOMBRE FÍSICO
                        // =============================================

                        var nombreFisico =
                            $"{Guid.NewGuid():N}{extension}";

                        archivosValidados.Add((
                            Bytes: bytes,
                            Extension: extension,
                            NombreOriginal: nombreOriginal,
                            NombreFisico: nombreFisico,
                            HashArchivo: hashArchivo
                        ));
                    }
                }

                // VALIDACIÓN HISTÓRICA CENTRAL
                var conceptosServicio =
                dto.Servicios
                    .Where(x => x.ConceptoServicioId > 0)
                    .Select(x => x.ConceptoServicioId)
                    .Distinct()
                    .ToList();

                if (conceptosServicio.Any())
                {
                    var resultadoHistorico =
                        await ValidarServiciosHistoricos(
                            dto.UnidadVehicularId,
                            conceptosServicio,
                            dto.TipoMantenimientoId);

                    dto.ValidacionHistorica = resultadoHistorico;


                    if (resultadoHistorico.TieneDuplicadosActivos)
                    {
                        throw new Exception(
                            string.Join(Environment.NewLine, resultadoHistorico.Mensajes));
                    }

                    // Recurrencia → advertencia únicamente
                    // NO bloquear creación
                }

                // =====================================================
                // CREAR SOLICITUD
                // =====================================================

                var entidad =
                new SolicitudMantenimiento
                {
                    Folio =
                        folio,

                    NumeroOficio =
                        dto.NumeroOficio.Trim(),

                    FechaSolicitud =
                        dto.FechaSolicitud,

                    AreaSolicitanteId =
                        dto.AreaSolicitanteId,

                    UnidadVehicularId =
                        dto.UnidadVehicularId,

                    TallerId =
                        dto.TallerId,

                    TipoMantenimientoId =
                        dto.TipoMantenimientoId,

                    KilometrajeActual =
                        dto.KilometrajeActual,

                    MotivoSolicitud = dto.MotivoSolicitud?.Trim() ?? string.Empty,

                    DiagnosticoInicial = dto.DiagnosticoInicial,

                    Observaciones = dto.Observaciones,

                    //ImporteCotizacion =
                    //    dto.ImporteCotizacion,

                    //ImporteCotizacion = dto.Servicios.Sum(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad),

                    ImporteCotizacion = dto.Servicios.Sum(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad),


                    // CAPTURADA

                    EstatusSolicitudMantenimientoId = 2,

                    Activo = true,

                    FechaCreacion =
                        fechaActual
                };

                _context
                    .SolicitudMantenimientos
                    .Add(entidad);

                await _context.SaveChangesAsync();

                // =====================================================
                // CREAR DETALLES DE SERVICIOS
                // =====================================================

                foreach (var servicio in dto.Servicios)
                {
                    if (
                      servicio.IdObjetoGastoManoObra.HasValue &&
                      servicio.IdObjetoGastoRefacciones.HasValue &&
                      servicio.IdObjetoGastoManoObra ==
                      servicio.IdObjetoGastoRefacciones
                          )
                    {
                        throw new Exception(
                            "WARNING|La partida de mano de obra y la de refacciones no pueden ser la misma.");
                    }


                    if (servicio.CostoEstimadoManoObra > 0 &&
                      !servicio.IdObjetoGastoManoObra.HasValue)
                    {
                        throw new Exception(
                            "WARNING|Debe seleccionar una partida para mano de obra.");
                    }

                    if (servicio.CostoEstimadoRefacciones > 0 &&
                        !servicio.IdObjetoGastoRefacciones.HasValue)
                    {
                        throw new Exception(
                            "WARNING|Debe seleccionar una partida para refacciones.");
                    }



                    var detalle =
                         new SolicitudMantenimientoDetalle
                         {
                             SolicitudMantenimientoId =
                                 entidad.SolicitudMantenimientoId,

                             ConceptoServicioId =
                                 servicio.ConceptoServicioId,

                             Cantidad = (int)servicio.Cantidad,

                             CostoEstimadoManoObra =
                                 servicio.CostoEstimadoManoObra,

                             CostoEstimadoRefacciones =
                                 servicio.CostoEstimadoRefacciones,

                             IdObjetoGastoManoObra = servicio.IdObjetoGastoManoObra,

                             IdObjetoGastoRefacciones = servicio.IdObjetoGastoRefacciones,

                             Observaciones =
                                 servicio.Observaciones,

                             FechaCreacion =
                                 fechaActual,

                             Activo = true
                         };

                    _context
                        .SolicitudMantenimientoDetalles
                        .Add(detalle);
                }

                await _context.SaveChangesAsync();

                // GUARDAR DOCUMENTOS

                if (archivosValidados.Any())
                {
                    var rutaBase = _configuration["Storage:DocumentosRuta"];

                    if (string.IsNullOrWhiteSpace(rutaBase))
                    {
                        throw new Exception(
                            "No se configuró la ruta de almacenamiento.");
                    }

                    var carpetaSolicitud =
                        Path.Combine(
                            rutaBase,
                            folio);

                    if (!Directory.Exists(carpetaSolicitud))
                    {
                        Directory.CreateDirectory(carpetaSolicitud);
                    }

                    foreach (var archivo in archivosValidados)
                    {
                        var rutaCompleta =
                            Path.Combine(
                                carpetaSolicitud,
                                archivo.NombreFisico);

                        await File.WriteAllBytesAsync(
                            rutaCompleta,
                            archivo.Bytes);

                        archivosGuardados.Add(rutaCompleta);

                        var documento =
                            new SolicitudMantenimientoDocumento
                            {
                                SolicitudMantenimientoId =
                                    entidad.SolicitudMantenimientoId,

                                TipoDocumento =
                                    "SOPORTE_INICIAL",

                                NombreArchivo =
                                    archivo.NombreOriginal,

                                RutaArchivo =
                                    rutaCompleta,

                                FechaCarga =
                                    DateTime.Now,

                                UsuarioCargaId = 1,

                                Activo = true,

                                HashArchivo =
                                    archivo.HashArchivo
                            };

                        _context
                            .SolicitudMantenimientoDocumentos
                            .Add(documento);
                    }

                    await _context.SaveChangesAsync();
                }

                // COMMIT

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                // =====================================================
                // LIMPIAR ARCHIVOS
                // =====================================================

                foreach (var ruta in archivosGuardados)
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

                throw;
            }
        }









        public async Task AutorizarSolicitudMantenimiento(int solicitudId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();
            try
            {
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

                        DiasFueraServicio = 0,

                        Activo = true,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId = 1,

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







        public async Task<SolicitudMantenimientoDetallePageViewModel>
       ObtenerDetalleSolicitud(int solicitudId)
        {
            var solicitud = await _context.SolicitudMantenimientos
                .AsNoTracking()
                .Where(x => x.SolicitudMantenimientoId == solicitudId)
                .Select(x => new SolicitudMantenimientoDetallePageViewModel
                {
                    SolicitudMantenimientoId =
                        x.SolicitudMantenimientoId,
                    Folio = x.Folio,

                    Vehiculo =
                        x.UnidadVehicular.Marca.MarcaNombre
                        + " "
                        + x.UnidadVehicular.Modelo.ModeloNombre
                        + " - "
                        + x.UnidadVehicular.PlacaActual
                        + " - "
                        + x.UnidadVehicular.NumeroSerie
                        + " - "
                        + x.UnidadVehicular.NumeroEconomico
                        ,

                    Taller = x.Taller != null
                        ? x.Taller.RazonSocial
                        + " - "
                        + x.Taller.Recurso.Nombre.Trim()
                        : "SIN TALLER",

                    Estatus = x.EstatusSolicitudMantenimiento.Nombre,

                    TotalConceptos = x.SolicitudMantenimientoDetalles.Count(),

                    Detalles = x
                        .SolicitudMantenimientoDetalles
                        .Select(d => new
                            SolicitudMantenimientoDetalleViewModel
                        {
                            SolicitudMantenimientoDetalleId =
                                d.SolicitudMantenimientoDetalleId,

                            ConceptoServicioId =
                                d.ConceptoServicioId,

                            ConceptoServicio =
                                d.ConceptoServicio.ConceptoServicioNombre,

                            Cantidad =
                                d.Cantidad,

                            CostoEstimadoManoObra =
                                d.CostoEstimadoManoObra,

                            CostoEstimadoRefacciones =
                                d.CostoEstimadoRefacciones,

                            Observaciones =
                                d.Observaciones
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
                        x.Activo &&
                        x.ConceptoServicio.Activo &&
                        x.MarcaId == unidad.MarcaId &&
                        x.ModeloId == unidad.ModeloId &&
                        x.AnioId == unidad.AnioId &&
                        x.TransmisionId == unidad.TransmisionId &&
                        x.CilindroId == unidad.CilindroId)
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





        public async Task<RevisionSolicitudViewModel> ObtenerRevisionSolicitud(int solicitudId)
        {

            // OBTENER SOLICITUD

            var solicitud = await _context
                .SolicitudMantenimientos
                .AsNoTracking()

                .Include(x => x.AreaSolicitante)

                .Include(x => x.UnidadVehicular)
                    .ThenInclude(x => x.Marca)

                .Include(x => x.UnidadVehicular)
                    .ThenInclude(x => x.Modelo)

                .Include(x => x.Taller)

                .FirstOrDefaultAsync(x =>
                    x.SolicitudMantenimientoId
                        == solicitudId);

            if (solicitud == null)
            {
                throw new Exception("INFO|Solicitud no encontrada.");
            }



            if (solicitud.EstatusSolicitudMantenimientoId == 2)
            {
                throw new Exception("WARNING|No ha validado DSP.");
            }


            //if (solicitud.EstatusSolicitudMantenimientoId == 6)
            //{
            //    throw new Exception(
            //        "WARNING|Solicitud rechazada. Vuelva a validar DSP.");
            //}

            if (solicitud.EstatusSolicitudMantenimientoId == 8)
            {
                throw new Exception(
                    "INFO|Solicitud finalizada.");
            }




            var historialBase = await _context.MantenimientoDetalles
             .AsNoTracking()
             .Include(x => x.Mantenimiento)
             .Include(x => x.ConceptoServicio)
             .Where(x => x.Mantenimiento.UnidadVehicularId == solicitud.UnidadVehicularId)
             .ToListAsync();


            // OBTENER DETALLES de histórico
            var now = DateTime.Now;

            var conceptoCount = historialBase
                .GroupBy(x => x.ConceptoServicioId)
                .ToDictionary(g => g.Key, g => g.Count());

            var historialTecnico = historialBase
                .OrderByDescending(x => x.Mantenimiento.FechaEjecucion)
                .Select(x =>
                {
                    var fechaBase =
                        x.Mantenimiento.FechaEjecucion ?? x.Mantenimiento.FechaCreacion;

                    return new HistorialTecnicoUnidadViewModel
                    {
                        Fecha = fechaBase,
                        Folio = x.Mantenimiento.MantenimientoId.ToString(),
                        Concepto = x.ConceptoServicio.ConceptoServicioNombre,
                        Kilometraje = x.Mantenimiento.KilometrajeSalida ?? 0,
                        Costo = (decimal)(x.TotalLinea ??
                                 (x.SubtotalManoObra + x.SubtotalRefacciones)),

                        DiasDesdeUltimo = (now - fechaBase).Days,

                        EsRecurrencia = conceptoCount[x.ConceptoServicioId] > 1
                    };
                })
                .ToList();


            var ResumenHistorico = new RevisionResumenHistoricoDto
            {
                TotalServicios = historialTecnico.Count,
                UltimaFecha = historialTecnico.FirstOrDefault()?.Fecha,
                CostoAcumulado = historialTecnico.Sum(x => x.Costo)
            };



            // OBTENER CONCEPTOS

            var conceptos = await _context
                .SolicitudMantenimientoDetalles
                .AsNoTracking()

                .Where(x =>
                    x.SolicitudMantenimientoId
                        == solicitudId)

                .Select(x =>
                    new SolicitudMantenimientoDetalleViewModel
                    {
                        SolicitudMantenimientoDetalleId =
                            x.SolicitudMantenimientoDetalleId,

                        ConceptoServicioNombre =
                            x.ConceptoServicio.ConceptoServicioNombre,

                        Cantidad =
                            x.Cantidad,

                        CostoEstimadoManoObra =
                            x.CostoEstimadoManoObra,

                        CostoEstimadoRefacciones =
                            x.CostoEstimadoRefacciones,

                        Observaciones =
                            x.Observaciones
                    })

                .ToListAsync();

            // VALIDAR CONCEPTOS

            if (!conceptos.Any())
            {
                throw new InvalidOperationException("WARNING|La solicitud no contiene servicios...");
            }

            // VALIDAR SI YA ESTÁ APROBADA

            bool yaAprobadaDSP = solicitud.EstatusSolicitudMantenimientoId == 4;

            bool yaAprobada = solicitud.EstatusSolicitudMantenimientoId == 5;

            int estatusId = solicitud.EstatusSolicitudMantenimientoId;

            string? mensajeEstatus = null;

            if (yaAprobada || yaAprobadaDSP)
            {
                mensajeEstatus = "La solicitud ya fue aprobada/validada.";
            }

            // RETORNAR VIEWMODEL

            return new RevisionSolicitudViewModel
            {
                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,
                Folio = solicitud.Folio,
                AreaSolicitante = solicitud.AreaSolicitante.AreaNombre,

                Vehiculo = $"{solicitud.UnidadVehicular.Marca.MarcaNombre} " + $"{solicitud.UnidadVehicular.Modelo.ModeloNombre}",

                Taller = solicitud.Taller.RazonSocial,

                ImporteCotizacion = (decimal)solicitud.ImporteCotizacion,

                Conceptos = conceptos,

                TiposRechazo = await ObtenerTiposRechazo(),

                YaProcesada = yaAprobada,
                yaAprobadaDSP = yaAprobadaDSP,

                EstatusSolicitudMantenimientoId = estatusId,
                MensajeEstatus = mensajeEstatus,

                HistorialTecnicoUnidad = historialTecnico ?? new List<HistorialTecnicoUnidadViewModel>(),

                ResumenHistorico = new RevisionResumenHistoricoDto
                {
                    TotalServicios = historialTecnico?.Count ?? 0,
                    UltimaFecha = historialTecnico?.FirstOrDefault()?.Fecha,
                    CostoAcumulado = historialTecnico?.Sum(x => x.Costo) ?? 0
                },

                Kilometraje = solicitud.KilometrajeActual
            };
        }




        public async Task RevisarSolicitud(RevisionSolicitudDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var solicitud =
                    await _context.SolicitudMantenimientos
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId ==
                            dto.SolicitudMantenimientoId);

                if (solicitud == null)
                {
                    throw new Exception(
                        "WARNING|Solicitud no encontrada.");
                }

                // =========================================
                // VALIDAR CONCEPTOS
                // =========================================

                var tieneConceptos =
                    await _context.SolicitudMantenimientoDetalles
                        .AnyAsync(x =>
                            x.SolicitudMantenimientoId ==
                            dto.SolicitudMantenimientoId);

                if (!tieneConceptos)
                {
                    throw new Exception(
                        "WARNING|La solicitud no tiene conceptos registrados.");
                }

                // =========================================
                // APROBAR
                // =========================================

                if (dto.Aprobada)
                {
                    if (solicitud.EstatusSolicitudMantenimientoId >= 5
                        && solicitud.EstatusSolicitudMantenimientoId != 6)
                    {
                        throw new Exception(
                            "WARNING|La solicitud ya fue autorizada.");
                    }

                    // =====================================
                    // REAPROBACIÓN DESPUÉS DE RECHAZO
                    // =====================================

                    if (solicitud.EstatusSolicitudMantenimientoId == 6)
                    {
                        var movimientosLiberados =
                            await _context.MovimientoPresupuestals
                            .Where(x =>
                                x.SolicitudMantenimientoId ==
                                solicitud.SolicitudMantenimientoId
                                &&
                                x.TipoMovimiento ==
                                "DESCOMPROMETIDO"
                                &&
                                x.Activo == true)
                            .ToListAsync();

                        if (!movimientosLiberados.Any())
                        {
                            throw new Exception(
                                "WARNING|No existen compromisos para reaprobar.");
                        }

                        decimal importeRecomprometido = 0;

                        //=====================================
                        // VALIDAR DSP NUEVAMENTE
                        //=====================================

                        foreach (var liberado in movimientosLiberados)
                        {
                            var importe =
                                liberado.Importe ?? 0;

                            importeRecomprometido += importe;

                            var dsp =
                                await _context.DispPresupuestals
                                .FirstOrDefaultAsync(x =>
                                    x.IdDispPresupuestal ==
                                    liberado.IdDispPresupuestal
                                    &&
                                    x.Activo);

                            if (dsp == null)
                            {
                                throw new Exception(
                                    "WARNING|DSP no encontrada.");
                            }

                            var movimientos =
                                await _context.MovimientoPresupuestals
                                .Where(x =>
                                    x.IdDispPresupuestal ==
                                    dsp.IdDispPresupuestal
                                    &&
                                    x.Activo == true)
                                .ToListAsync();

                            var comprometido =
                                movimientos
                                .Where(x =>
                                    x.TipoMovimiento == "COMPROMETIDO")
                                .Sum(x =>
                                    x.Importe ?? 0);

                            var descomprometido =
                                movimientos
                                .Where(x =>
                                    x.TipoMovimiento == "DESCOMPROMETIDO")
                                .Sum(x =>
                                    x.Importe ?? 0);

                            var devengado =
                                movimientos
                                .Where(x =>
                                    x.TipoMovimiento == "DEVENGADO")
                                .Sum(x =>
                                    x.Importe ?? 0);

                            var disponible =
                                Math.Max(
                                    dsp.Importe
                                    -
                                    (comprometido - descomprometido)
                                    -
                                    devengado,
                                    0);

                            var disponiblePosterior =
                                disponible
                                -
                                importe;

                            var limite =
                                dsp.Importe * .20m;

                            if (disponiblePosterior <= limite)
                            {
                                throw new Exception(
                                    $"WARNING|La reaprobación dejaría una DSP por debajo del 20%.");
                            }

                            if (importe > disponible)
                            {
                                throw new Exception(
                                    $"WARNING|La DSP {dsp.IdObjetoGasto} ya no tiene disponibilidad.");
                            }
                        }

                        //=====================================
                        // VALIDAR TALLER NUEVAMENTE
                        //=====================================

                        if (solicitud.TallerId.HasValue)
                        {
                            var taller =
                                await _context.Tallers
                                .FirstOrDefaultAsync(x =>
                                    x.TallerId ==
                                    solicitud.TallerId.Value);

                            if (taller == null)
                            {
                                throw new Exception(
                                    "WARNING|Taller no encontrado.");
                            }

                            var movimientosTaller =
                                await _context.TallerMovimientoPresupuestos
                                .Where(x =>
                                    x.TallerId ==
                                    taller.TallerId
                                    &&
                                    x.Activo)
                                .ToListAsync();

                            var comprometido =
                                movimientosTaller
                                .Where(x =>
                                    x.TipoMovimiento == "COMPROMETIDO")
                                .Sum(x =>
                                    x.Importe);

                            var liberado =
                                movimientosTaller
                                .Where(x =>
                                    x.TipoMovimiento == "DESCOMPROMETIDO")
                                .Sum(x =>
                                    x.Importe);

                            var ejercido =
                                movimientosTaller
                                .Where(x =>
                                    x.TipoMovimiento == "EJERCIDO")
                                .Sum(x =>
                                    x.Importe);

                            var disponible =
                                Math.Max(
                                    (taller.MontoMaximo ?? 0)
                                    -
                                    (comprometido - liberado)
                                    -
                                    ejercido,
                                    0);

                            var disponiblePosterior =
                                disponible
                                -
                                importeRecomprometido;

                            if (disponiblePosterior < taller.MontoMinimo)
                            {
                                throw new Exception(
                                    $"WARNING|El taller quedaría debajo del mínimo permitido.");
                            }

                            if (importeRecomprometido > disponible)
                            {
                                throw new Exception(
                                    $"WARNING|El taller ya no tiene disponibilidad.");
                            }
                        }

                        //=====================================
                        // RECOMPROMETER DSP
                        //=====================================

                        foreach (var liberado in movimientosLiberados)
                        {
                            _context.MovimientoPresupuestals.Add(
                                new MovimientoPresupuestal
                                {
                                    SolicitudMantenimientoId =
                                        solicitud.SolicitudMantenimientoId,

                                    IdDispPresupuestal =
                                        liberado.IdDispPresupuestal,

                                    FechaMovimiento =
                                        DateTime.Now,

                                    TipoMovimiento =
                                        "COMPROMETIDO",

                                    Importe =
                                        liberado.Importe,

                                    Observaciones =
                                        $"Recompromiso automático por reaprobación técnica. {dto.Observaciones}",

                                    FechaCaptura =
                                        DateTime.Now,

                                    UsuarioCaptura =
                                        "ADMIN",

                                    Activo =
                                        true
                                });
                        }

                        //=====================================
                        // RECOMPROMETER TALLER
                        //=====================================

                        if (solicitud.TallerId.HasValue)
                        {
                            _context.TallerMovimientoPresupuestos.Add(
                                new TallerMovimientoPresupuesto
                                {
                                    TallerId =
                                        solicitud.TallerId.Value,

                                    SolicitudMantenimientoId =
                                        solicitud.SolicitudMantenimientoId,

                                    FechaMovimiento =
                                        DateTime.Now,

                                    TipoMovimiento =
                                        "COMPROMETIDO",

                                    Importe =
                                        importeRecomprometido,

                                    UsuarioCaptura =
                                        "ADMIN",

                                    Activo =
                                        true
                                });

                            var control =
                                await _context.TallerControlPresupuestals
                                .FirstOrDefaultAsync(x =>
                                    x.TallerId ==
                                    solicitud.TallerId.Value
                                    &&
                                    x.Ejercicio ==
                                    (short)DateTime.Now.Year
                                    &&
                                    x.Activo);

                            if (control != null)
                            {
                                control.PresupuestoComprometido +=
                                    importeRecomprometido;

                                control.PresupuestoDisponible =
                                    Math.Max(
                                        control.PresupuestoAsignado
                                        -
                                        control.PresupuestoComprometido
                                        -
                                        control.PresupuestoEjercido,
                                        0);

                                control.FechaActualizacion =
                                    DateTime.Now;
                            }
                        }
                    }

                    solicitud.EstatusSolicitudMantenimientoId = 5;
                }

                // =========================================
                // RECHAZAR
                // =========================================

                else
                {
                    if (solicitud.EstatusSolicitudMantenimientoId == 6)
                    {
                        throw new Exception(
                            "WARNING|La solicitud ya fue rechazada.");
                    }

                    if (!dto.TipoRechazoSolicitudId.HasValue)
                    {
                        throw new Exception(
                            "WARNING|Debe indicar el tipo de rechazo.");
                    }

                    // =====================================
                    // LIBERAR COMPROMISOS DSP
                    // =====================================

                    if (solicitud.EstatusSolicitudMantenimientoId >= 4)
                    {
                        var movimientos = await _context.MovimientoPresupuestals
                                .Where(x =>
                                    x.SolicitudMantenimientoId ==
                                        solicitud.SolicitudMantenimientoId
                                    &&
                                    x.Activo == true)
                                .ToListAsync();

                        var compromisosVigentes =
                            movimientos
                                .GroupBy(x => x.IdDispPresupuestal)
                                .Select(g =>
                                {
                                    var comprometido =
                                        g.Where(x =>
                                            x.TipoMovimiento == "COMPROMETIDO")
                                        .Sum(x => x.Importe ?? 0);

                                    var descomprometido =
                                        g.Where(x =>
                                            x.TipoMovimiento == "DESCOMPROMETIDO")
                                        .Sum(x => x.Importe ?? 0);

                                    return new
                                    {
                                        IdDispPresupuestal = g.Key,
                                        ImporteVigente = comprometido - descomprometido
                                    };
                                })
                                .Where(x => x.ImporteVigente > 0)
                                .ToList();

                        foreach (var compromiso in compromisosVigentes)
                        {
                            _context.MovimientoPresupuestals.Add(
                                new MovimientoPresupuestal
                                {
                                    SolicitudMantenimientoId =
                                        solicitud.SolicitudMantenimientoId,

                                    IdDispPresupuestal =
                                        compromiso.IdDispPresupuestal,

                                    FechaMovimiento =
                                        DateTime.Now,

                                    TipoMovimiento =
                                        "DESCOMPROMETIDO",

                                    Importe =
                                        compromiso.ImporteVigente,

                                    Observaciones =
                                        $"Liberación automática por rechazo técnico. {dto.Observaciones}",

                                    FechaCaptura =
                                        DateTime.Now,

                                    UsuarioCaptura =
                                        "ADMIN",

                                    Activo = true
                                });
                        }




                        // =====================================
                        // LIBERAR COMPROMISO DEL TALLER
                        // =====================================

                        if (solicitud.TallerId.HasValue)
                        {
                            var movimientosTaller =
                                await _context.TallerMovimientoPresupuestos
                                    .Where(x =>
                                        x.SolicitudMantenimientoId ==
                                            solicitud.SolicitudMantenimientoId
                                        &&
                                        x.Activo)
                                    .ToListAsync();

                            var comprometidoTaller =
                                movimientosTaller
                                    .Where(x =>
                                        x.TipoMovimiento == "COMPROMETIDO")
                                    .Sum(x => x.Importe);

                            var liberadoTaller =
                                movimientosTaller
                                    .Where(x =>
                                        x.TipoMovimiento == "DESCOMPROMETIDO")
                                    .Sum(x => x.Importe);

                            var compromisoVigenteTaller =
                                Math.Max(
                                    comprometidoTaller
                                    -
                                    liberadoTaller,
                                    0);

                            if (compromisoVigenteTaller > 0)
                            {
                                _context.TallerMovimientoPresupuestos.Add(
                                    new TallerMovimientoPresupuesto
                                    {
                                        TallerId =
                                            solicitud.TallerId.Value,

                                        SolicitudMantenimientoId =
                                            solicitud.SolicitudMantenimientoId,

                                        FechaMovimiento =
                                            DateTime.Now,

                                        TipoMovimiento =
                                            "DESCOMPROMETIDO",

                                        Importe =
                                            compromisoVigenteTaller,

                                        UsuarioCaptura =
                                            "ADMIN",

                                        Activo = true
                                    });

                                // ===============================
                                // ACTUALIZAR CONTROL PRESUPUESTAL
                                // ===============================

                                var control =
                                    await _context.TallerControlPresupuestals
                                        .FirstOrDefaultAsync(x =>
                                            x.TallerId ==
                                                solicitud.TallerId.Value
                                            &&
                                            x.Ejercicio ==
                                                (short)DateTime.Now.Year
                                            &&
                                            x.Activo);

                                if (control != null)
                                {
                                    control.PresupuestoComprometido = Math.Max(control.PresupuestoComprometido
                                            -
                                            compromisoVigenteTaller, 0);

                                    control.PresupuestoDisponible =
                                        Math.Max(control.PresupuestoAsignado
                                            -
                                            control.PresupuestoComprometido
                                            -
                                            control.PresupuestoEjercido, 0);

                                    control.FechaActualizacion = DateTime.Now;
                                }
                            }
                        }









                    }

                    solicitud.EstatusSolicitudMantenimientoId = 6;
                }

                // =========================================
                // SEGUIMIENTO
                // =========================================

                _context.SolicitudMantenimientoSeguimientos.Add(
                    new SolicitudMantenimientoSeguimiento
                    {
                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId,

                        EstatusSolicitudMantenimientoId =
                            solicitud.EstatusSolicitudMantenimientoId,

                        Observaciones =
                            dto.Observaciones,

                        TipoRechazoSolicitudId =
                            dto.TipoRechazoSolicitudId,

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
                throw new Exception("WARNING|Tipo de catálogo no encontrado.");
            }

            // VALIDAR RELACIONES

            var tieneConceptos = await _context.ConceptoServicios
                    .AnyAsync(x =>
                        x.TipoCatalogoServicioId == id
                        &&
                        x.Activo);

            if (tieneConceptos)
            {
                throw new Exception("WARNING|No es posible eliminar el tipo de catálogo porque tiene conceptos asociados.");
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
                        Mensaje =
                            "Concepto de servicio no encontrado."
                    };
                }

                // VALIDAR USO EN MATRIZ

                var existeEnMatriz =
                    await _context.ConceptoServicioCostos
                        .AnyAsync(x =>
                            x.ConceptoServicioId == id);

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
                throw new Exception("WARNING|La solicitud no fue encontrada.");
            }

            // =========================================
            // OBTENER DOCUMENTOS
            // =========================================

            var documentos = await _context.SolicitudMantenimientoDocumentos
                    .AsNoTracking()
                    .Where(x =>
                        x.SolicitudMantenimientoId ==
                            entidad.SolicitudMantenimientoId
                        &&
                        x.Activo)
                    .OrderByDescending(x => x.FechaCarga)
                    .Select(x =>
                        new SolicitudMantenimientoDocumentoViewModel
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





            // =========================================
            // OBTENER SERVICIOS
            // =========================================

            var servicios =
             await _context.SolicitudMantenimientoDetalles
                 .AsNoTracking()
                 .Include(x => x.IdObjetoGastoManoObraNavigation)
                 .Include(x => x.IdObjetoGastoRefaccionesNavigation)
                 .Where(x => x.SolicitudMantenimientoId == entidad.SolicitudMantenimientoId)
                 .Select(x =>
             new SolicitudMantenimientoServicioViewModel
             {
                 ConceptoServicioId = x.ConceptoServicioId,

                 Cantidad = x.Cantidad,

                 CostoEstimadoManoObra = x.CostoEstimadoManoObra,

                 CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,

                 IdObjetoGastoManoObra = x.IdObjetoGastoManoObra,

                 IdObjetoGastoRefacciones = x.IdObjetoGastoRefacciones,

                 ImporteCotizacion = (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad,

                 ClaveObjetoGastoManoObra = x.IdObjetoGastoManoObraNavigation != null ? x.IdObjetoGastoManoObraNavigation.ClaveObjGasto
                         : "",

                 ClaveObjetoGastoRefacciones = x.IdObjetoGastoRefaccionesNavigation != null ? x.IdObjetoGastoRefaccionesNavigation.ClaveObjGasto
                         : "",

                 Observaciones =
                     x.Observaciones
             })
         .ToListAsync();


            return new SolicitudMantenimientoEditViewModel
            {
                SolicitudMantenimientoId =
                    entidad.SolicitudMantenimientoId,

                Folio =
                    entidad.Folio,

                NumeroOficio =
                    entidad.NumeroOficio,

                FechaSolicitud =
                    entidad.FechaSolicitud,

                FechaRegistro =
                    entidad.FechaCreacion,

                AreaSolicitanteId =
                    entidad.AreaSolicitanteId,

                UnidadVehicularId =
                    entidad.UnidadVehicularId,

                TallerId =
                    (int)entidad.TallerId,

                TipoMantenimientoId =
                    entidad.TipoMantenimientoId,

                KilometrajeActual =
                    entidad.KilometrajeActual,

                MotivoSolicitud =
                    entidad.MotivoSolicitud,

                DiagnosticoInicial =
                    entidad.DiagnosticoInicial,

                Observaciones =
                    entidad.Observaciones,

                ImporteCotizacion = entidad.ImporteCotizacion,

                EstatusSolicitudMantenimientoId =
                    entidad.EstatusSolicitudMantenimientoId,




                // =========================================
                // SERVICIOS
                // =========================================

                Servicios = servicios,

                ConceptosServicio = await ObtenerConceptosServicioFiltrados(entidad.UnidadVehicularId),

                ObjetosGasto = await ObtenerObjetosGasto(),



                // =========================================
                // DOCUMENTOS
                // =========================================

                Documentos =
                    documentos,

                // OPCIONAL:
                // PARA MOSTRAR SI EXISTEN ARCHIVOS

                TieneDocumentos = documentos.Any(),

                TotalDocumentos = documentos.Count,

                // =========================================
                // CATALOGOS
                // =========================================

                Areas =
                    await ObtenerAreas(),

                // SOLO LA UNIDAD SELECCIONADA

                UnidadesVehiculares =
                    new List<SelectListItem>
                    {
                new SelectListItem
                {
                    Value =
                        entidad.UnidadVehicularId.ToString(),

                    Text =
                        entidad.UnidadVehicular != null
                        ? $"{entidad.UnidadVehicular.NumeroSerie} - " +
                          $"{entidad.UnidadVehicular.NumeroEconomico} - " +
                          $"{entidad.UnidadVehicular.PlacaActual}"
                        : "Unidad no encontrada",

                    Selected = true
                }
                    },

                TiposMantenimiento =
                    await ObtenerTiposMantenimiento(),

                Talleres =
                    await ObtenerTalleres(),

                Estatuses =
                    await ObtenerEstatusSolicitudMantenimiento()
            };
        }





        public async Task EditarSolicitudMantenimiento(SolicitudMantenimientoEditDto dto, IEnumerable<IFormFile>? archivos)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var archivosGuardados =
                new List<string>();

            try
            {
                // OBTENER ENTIDAD

                var entidad =
                    await _context.SolicitudMantenimientos
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId ==
                                dto.SolicitudMantenimientoId
                            &&
                            x.Activo);

                if (entidad == null)
                {
                    throw new Exception("WARNING|La solicitud no fue encontrada.");
                }

                // VALIDAR FOLIO DUPLICADO

                var existeFolio =
                    await _context.SolicitudMantenimientos
                        .AnyAsync(x =>

                            x.SolicitudMantenimientoId !=
                                dto.SolicitudMantenimientoId

                            &&

                            x.Folio == dto.Folio

                            &&

                            x.Activo);

                if (existeFolio)
                {
                    throw new Exception("WARNING|El folio ya se encuentra registrado.");
                }

                // ACTUALIZAR SOLICITUD

                entidad.Folio =
                    dto.Folio.Trim();

                entidad.NumeroOficio =
                    dto.NumeroOficio?.Trim();

                entidad.FechaSolicitud =
                    dto.FechaSolicitud;

                entidad.AreaSolicitanteId =
                    dto.AreaSolicitanteId;

                entidad.UnidadVehicularId =
                    dto.UnidadVehicularId;

                entidad.TallerId =
                    dto.TallerId;

                entidad.TipoMantenimientoId =
                    dto.TipoMantenimientoId;

                entidad.KilometrajeActual =
                    (int)dto.KilometrajeActual;

                entidad.MotivoSolicitud =
                    dto.MotivoSolicitud?.Trim();

                entidad.DiagnosticoInicial =
                    dto.DiagnosticoInicial?.Trim();

                entidad.Observaciones =
                    dto.Observaciones?.Trim();

                //dto.ImporteCotizacion = dto.Servicios.Sum(x => (x.CostoEstimadoManoObra + x.CostoEstimadoRefacciones) * x.Cantidad);

                //entidad.ImporteCotizacion = dto.ImporteCotizacion;

                // GUARDAR CAMBIOS PRINCIPALES

                await _context.SaveChangesAsync();



                var serviciosBloqueados = entidad.EstatusSolicitudMantenimientoId >= 5 && entidad.EstatusSolicitudMantenimientoId != 6;




                // ACTUALIZAR SERVICIOS
                if (serviciosBloqueados)
                {
                    await transaction.CommitAsync();
                    return;
                }


                if (!serviciosBloqueados)
                {
                    dto.ImporteCotizacion =
                        dto.Servicios.Sum(x =>
                            (x.CostoEstimadoManoObra +
                             x.CostoEstimadoRefacciones)
                             * x.Cantidad);

                    entidad.ImporteCotizacion =
                        dto.ImporteCotizacion;
                }


                // VALIDACION HISTORICA EDIT
                // EXCLUYE LA SOLICITUD ACTUAL

                var conceptosServicio =
                    dto.Servicios
                        .Where(x => x.ConceptoServicioId > 0)
                        .Select(x => x.ConceptoServicioId)
                        .Distinct()
                        .ToList();

                if (conceptosServicio.Any())
                {
                    var resultadoHistorico =
                        await ValidarServiciosHistoricos(
                            dto.UnidadVehicularId,
                            conceptosServicio,
                            dto.TipoMantenimientoId);

                    dto.ValidacionHistorica = resultadoHistorico;

                    // EXCLUIR ESTA MISMA SOLICITUD
                    var mensajesDuplicados =
                        new List<string>();

                    if (resultadoHistorico.TieneDuplicadosActivos)
                    {
                        var solicitudesActivas =
                            await (
                                from s in _context.SolicitudMantenimientos

                                join d in _context.SolicitudMantenimientoDetalles
                                    on s.SolicitudMantenimientoId
                                    equals d.SolicitudMantenimientoId

                                join c in _context.ConceptoServicios
                                    on d.ConceptoServicioId
                                    equals c.ConceptoServicioId

                                where
                                    s.Activo
                                    &&
                                    d.Activo
                                    &&
                                    s.SolicitudMantenimientoId
                                        != dto.SolicitudMantenimientoId
                                    &&
                                    s.UnidadVehicularId
                                        == dto.UnidadVehicularId
                                    &&
                                    conceptosServicio.Contains(
                                        d.ConceptoServicioId)

                                select new
                                {
                                    s.Folio,
                                    c.ConceptoServicioNombre
                                }
                            )
                            .AsNoTracking()
                            .Distinct()
                            .ToListAsync();

                        mensajesDuplicados =
                            solicitudesActivas
                                .Select(x =>
                                    $"El servicio '{x.ConceptoServicioNombre}' ya existe en la solicitud {x.Folio}.")
                                .ToList();

                        if (mensajesDuplicados.Any())
                        {

                            throw new Exception(
                                string.Join(
                                    Environment.NewLine,
                                    mensajesDuplicados));





                        }
                    }

                    // RECURRENCIA = advertencia únicamente
                }








                var serviciosActuales =
                    await _context.SolicitudMantenimientoDetalles
                        .Where(x =>
                            x.SolicitudMantenimientoId ==
                                entidad.SolicitudMantenimientoId)
                        .ToListAsync();

                // ELIMINAR SERVICIOS ACTUALES
                if (serviciosActuales.Any())
                {
                    _context
                        .SolicitudMantenimientoDetalles
                        .RemoveRange(serviciosActuales);
                }

                // INSERTAR NUEVOS SERVICIOS
                foreach (var servicio in dto.Servicios)
                {


                    //System.Diagnostics.Debug.WriteLine( 
                    //    $"Concepto={servicio.ConceptoServicioId} " +
                    //    $"MO={servicio.IdObjetoGastoManoObra} " +
                    //    $"REF={servicio.IdObjetoGastoRefacciones}");

                    if (
                           servicio.IdObjetoGastoManoObra.HasValue &&
                           servicio.IdObjetoGastoRefacciones.HasValue &&
                           servicio.IdObjetoGastoManoObra ==
                           servicio.IdObjetoGastoRefacciones
                      )
                    {
                        throw new Exception(
                            "WARNING|La partida de mano de obra y la de refacciones no pueden ser la misma.");
                    }


                    if (servicio.CostoEstimadoManoObra > 0 &&
                        !servicio.IdObjetoGastoManoObra.HasValue)
                    {
                        throw new Exception(
                            "WARNING|Debe seleccionar una partida para mano de obra.");
                    }

                    if (servicio.CostoEstimadoRefacciones > 0 &&
                        !servicio.IdObjetoGastoRefacciones.HasValue)
                    {
                        throw new Exception(
                            "WARNING|Debe seleccionar una partida para refacciones.");
                    }
                }


                if (dto.Servicios != null && dto.Servicios.Any())
                {
                    var nuevosServicios = dto.Servicios
                         .Select(x =>
                             new SolicitudMantenimientoDetalle
                             {
                                 SolicitudMantenimientoId =
                                     entidad.SolicitudMantenimientoId,

                                 ConceptoServicioId =
                                     x.ConceptoServicioId,

                                 Cantidad = (int)x.Cantidad,

                                 CostoEstimadoManoObra =
                                     x.CostoEstimadoManoObra,

                                 CostoEstimadoRefacciones =
                                     x.CostoEstimadoRefacciones,

                                 IdObjetoGastoManoObra =
                                     x.IdObjetoGastoManoObra,

                                 IdObjetoGastoRefacciones =
                                     x.IdObjetoGastoRefacciones,

                                 Observaciones =
                                     x.Observaciones,

                                 FechaCreacion =
                                     DateTime.Now,

                                 Activo = true
                             })
                         .ToList();

                    await _context
                        .SolicitudMantenimientoDetalles
                        .AddRangeAsync(nuevosServicios);
                }

                await _context.SaveChangesAsync();

                // =========================================
                // VALIDAR Y GUARDAR DOCUMENTOS
                // =========================================

                if (archivos != null && archivos.Any())
                {
                    var rutaBase =
                        _configuration["Storage:DocumentosRuta"];

                    if (string.IsNullOrWhiteSpace(rutaBase))
                    {
                        throw new Exception("WARNING|No se configuró la ruta de almacenamiento.");
                    }

                    var carpetaSolicitud =
                        Path.Combine(
                            rutaBase,
                            entidad.Folio);

                    if (!Directory.Exists(carpetaSolicitud))
                    {
                        Directory.CreateDirectory(carpetaSolicitud);
                    }

                    const long tamanioMaximo =
                        2 * 1024 * 1024;

                    const int maxArchivos = 5;

                    if (archivos.Count() > maxArchivos)
                    {
                        throw new Exception($"WARNING|Solo se permiten {maxArchivos} archivos.");
                    }

                    var extensionesPermitidas =
                        new HashSet<string>(
                            StringComparer.OrdinalIgnoreCase)
                        {
                    ".pdf",
                    ".jpg",
                    ".jpeg",
                    ".png"
                        };

                    var contentTypesPermitidos =
                        new HashSet<string>(
                            StringComparer.OrdinalIgnoreCase)
                        {
                    "application/pdf",
                    "image/jpeg",
                    "image/png"
                        };

                    var provider =
                        new FileExtensionContentTypeProvider();

                    foreach (var archivo in archivos)
                    {
                        // =====================================
                        // VALIDAR ARCHIVO VACÍO
                        // =====================================

                        if (archivo == null || archivo.Length <= 0)
                        {
                            continue;
                        }

                        // =====================================
                        // VALIDAR TAMAÑO
                        // =====================================

                        if (archivo.Length > tamanioMaximo)
                        {
                            throw new Exception($"WARNING|El archivo '{archivo.FileName}' excede 2 MB.");
                        }

                        // =====================================
                        // VALIDAR NOMBRE
                        // =====================================

                        var nombreOriginal =
                            Path.GetFileName(archivo.FileName);

                        if (string.IsNullOrWhiteSpace(nombreOriginal))
                        {
                            throw new Exception("WARNING|Nombre de archivo inválido.");
                        }

                        if (nombreOriginal.Length > 150)
                        {
                            throw new Exception($"WARNING|El nombre '{nombreOriginal}' es demasiado largo.");
                        }

                        var nombreLower =
                            nombreOriginal.ToLowerInvariant();

                        // =====================================
                        // VALIDAR EXTENSIONES PELIGROSAS
                        // =====================================

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
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' contiene extensiones no permitidas.");
                        }

                        // =====================================
                        // VALIDAR EXTENSIÓN
                        // =====================================

                        var extension =
                            Path.GetExtension(nombreOriginal)
                                .ToLowerInvariant();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' tiene una extensión no permitida.");
                        }

                        // =====================================
                        // VALIDAR MIME
                        // =====================================

                        if (!provider.TryGetContentType(
                                nombreOriginal,
                                out _))
                        {
                            throw new Exception($"WARNING|No fue posible validar '{nombreOriginal}'.");
                        }

                        var contentType =
                            archivo.ContentType
                                .Trim()
                                .ToLowerInvariant();

                        if (!contentTypesPermitidos.Contains(contentType))
                        {
                            throw new Exception($"WARNING|El archivo '{nombreOriginal}' tiene un MIME inválido.");
                        }

                        // =====================================
                        // VALIDAR FIRMA BINARIA
                        // =====================================

                        using var memoryStream =
                            new MemoryStream();

                        await archivo.CopyToAsync(memoryStream);

                        var bytes =
                            memoryStream.ToArray();

                        if (!ArchivoValidoPorFirma(extension, bytes))
                        {
                            throw new Exception($"WARNING|El contenido real de '{nombreOriginal}' no coincide con la extensión.");
                        }

                        // =====================================
                        // GENERAR HASH
                        // =====================================

                        string hashArchivo;

                        using (var sha256 = SHA256.Create())
                        {
                            var hashBytes =
                                sha256.ComputeHash(bytes);

                            hashArchivo =
                                Convert.ToHexString(hashBytes);
                        }

                        // =====================================
                        // GENERAR NOMBRE FÍSICO
                        // =====================================

                        var nombreFisico =
                            $"{Guid.NewGuid()}{extension}";

                        var rutaCompleta =
                            Path.Combine(
                                carpetaSolicitud,
                                nombreFisico);

                        // =====================================
                        // GUARDAR ARCHIVO
                        // =====================================

                        await File.WriteAllBytesAsync(
                            rutaCompleta,
                            bytes);

                        archivosGuardados.Add(rutaCompleta);

                        // =====================================
                        // REGISTRAR DOCUMENTO
                        // =====================================

                        var documento =
                            new SolicitudMantenimientoDocumento
                            {
                                SolicitudMantenimientoId =
                                    entidad.SolicitudMantenimientoId,

                                TipoDocumento =
                                    "DOCUMENTO_ADICIONAL",

                                NombreArchivo =
                                    nombreOriginal,

                                RutaArchivo =
                                    rutaCompleta,

                                FechaCarga =
                                    DateTime.Now,

                                UsuarioCargaId =
                                    1,

                                Activo = true,

                                HashArchivo =
                                    hashArchivo
                            };

                        _context
                            .SolicitudMantenimientoDocumentos
                            .Add(documento);
                    }

                    await _context.SaveChangesAsync();
                }

                // =========================================
                // COMMIT
                // =========================================

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                // =========================================
                // LIMPIAR ARCHIVOS FÍSICOS
                // =========================================

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
            var query = _context.UnidadVehiculars
                    .AsNoTracking()
                    .Where(x => x.Activo);

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
                    throw new Exception("WARNING|La unidad vehicular no fue encontrada.");
                }

                // =========================================
                // VALIDAR ÁREAS
                // =========================================

                if (dto.AreaOrigenId == dto.AreaDestinoId)
                {
                    throw new Exception("WARNING|El área destino debe ser diferente al área origen.");
                }

                if (unidad.AreaId != dto.AreaOrigenId)
                {
                    throw new Exception("WARNING|La unidad no pertenece al área origen seleccionada.");
                }

                // =========================================
                // VALIDAR DOCUMENTO
                // =========================================

                if (documento == null || documento.Length <= 0)
                {
                    throw new Exception("WARNING|Debe adjuntar el documento de transferencia en PDF.");
                }

                const long tamanioMaximo =
                    2 * 1024 * 1024;

                if (documento.Length > tamanioMaximo)
                {
                    throw new Exception("WARNING|El documento excede el tamaño máximo permitido de 2 MB.");
                }

                var nombreOriginal =
                    Path.GetFileName(documento.FileName);

                if (string.IsNullOrWhiteSpace(nombreOriginal))
                {
                    throw new Exception("WARNING|Nombre de archivo inválido.");
                }

                if (nombreOriginal.Length > 150)
                {
                    throw new Exception("WARNING|El nombre del archivo es demasiado largo.");
                }

                var extension =
                    Path.GetExtension(nombreOriginal)
                        .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    throw new Exception("WARNING|Solo se permiten documentos PDF.");
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

                        UsuarioSolicitaId = 1,

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

                        UsuarioId = 1,

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

                entidad.EstatusTransferenciaUnidadId = 2;

                entidad.UsuarioAutorizaId =
                    dto.UsuarioAutorizaId;

                entidad.ComentariosAutorizacion =
                    dto.ComentariosAutorizacion;

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

                        UsuarioId =
                            dto.UsuarioAutorizaId,

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

                transferencia.UsuarioRecibeId =
                    dto.UsuarioRecibeId;

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

                entidad.UsuarioEntregaId =
                    dto.UsuarioEntregaId;

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

                        UsuarioId =
                            dto.UsuarioEntregaId,

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






        public async Task<DSPViewModel> ObtenerDSP(int solicitudId)
        {
            var solicitud = await _context.SolicitudMantenimientos
                    .Include(x => x.AreaSolicitante)
                    .Include(x => x.SolicitudMantenimientoDetalles)
                    .ThenInclude(x => x.IdObjetoGastoManoObraNavigation)
                    .Include(x => x.SolicitudMantenimientoDetalles)
                    .ThenInclude(x => x.IdObjetoGastoRefaccionesNavigation)
                    .Include(x => x.Taller)
                    .FirstOrDefaultAsync(x => x.SolicitudMantenimientoId == solicitudId);

            if (solicitud == null)
            {
                throw new Exception(
                    "WARNING|Solicitud no encontrada.");
            }

            if (solicitud.EstatusSolicitudMantenimientoId == 9)
            {
                throw new Exception(
                    "WARNING|Solicitud cancelada.");
            }





            //=====================================================
            // VALIDACIÓN PRESUPUESTAL DEL TALLER
            //=====================================================

            var tallerId =
                solicitud.TallerId;

            var controlTaller =
                await _context.TallerControlPresupuestals
                    .FirstOrDefaultAsync(x =>
                        x.TallerId == tallerId
                        &&
                        x.Activo);

            decimal presupuestoTaller = 0;
            decimal comprometidoTaller = 0;
            decimal ejercidoTaller = 0;
            decimal disponibleTaller = 0;

            bool tallerTieneDisponibilidad = true;

            if (controlTaller != null)
            {
                presupuestoTaller =
                    controlTaller.PresupuestoAsignado;

                comprometidoTaller =
                    controlTaller.PresupuestoComprometido;

                ejercidoTaller =
                    controlTaller.PresupuestoEjercido;

                disponibleTaller = Math.Max((decimal)controlTaller.PresupuestoDisponible, 0);

            }
            else
            {
                // solicitud nueva sin historial

                presupuestoTaller = solicitud.Taller?.MontoMaximo ?? 0;

                disponibleTaller = presupuestoTaller;

                comprometidoTaller = 0;

                ejercidoTaller = 0;
            }




            // dsp validado si estatus es mayor o igual a 4, se consideran los movimientos presupuestales
            // históricos de la solicitud para calcular el disponible en DSP, ya que estos movimientos ya
            // fueron aplicados al presupuesto asignado del área, por lo tanto, no se deben considerar los movimientos
            // actuales del área para calcular el disponible en DSP, ya que estos movimientos ya están reflejados en el presupuesto asignado del área.

            //var solicitudYaValidada = solicitud.EstatusSolicitudMantenimientoId >= 4;

            var solicitudDSPAutorizada = solicitud.EstatusSolicitudMantenimientoId >= 4 && solicitud.EstatusSolicitudMantenimientoId != 6;

            var estaRechazada = solicitud.EstatusSolicitudMantenimientoId == 6;


            List<MovimientoPresupuestal> movimientosHistoricosSolicitud =
                new();

            if (solicitud.EstatusSolicitudMantenimientoId >= 4)
            {
                movimientosHistoricosSolicitud =
                    await _context.MovimientoPresupuestals
                        .Include(x => x.IdDispPresupuestalNavigation)
                        .Where(x =>
                            x.SolicitudMantenimientoId ==
                            solicitud.SolicitudMantenimientoId
                            &&
                            x.Activo == true)
                        .ToListAsync();
            }

            // =====================================================
            // DSP DEL ÁREA
            // =====================================================

            var dsps = await _context.DispPresupuestals
                    .Include(x => x.IdObjetoGastoNavigation)
                    .Where(x =>
                        x.IdArea == solicitud.AreaSolicitanteId
                        &&
                        x.Activo)
                    .ToListAsync();

            var idsDSP = dsps.Select(x => x.IdDispPresupuestal)
                    .ToList();

            var movimientos = await _context.MovimientoPresupuestals
                .Where(x =>
                    x.Activo == true
                    &&
                    idsDSP.Contains(
                        x.IdDispPresupuestal ?? 0))
                .ToListAsync();






            // =====================================================
            // RESUMEN GENERAL
            // =====================================================

            var presupuestoAsignado = dsps.Sum(x => x.Importe);

            decimal comprometido;

            if (solicitud.EstatusSolicitudMantenimientoId >= 4)
            {
                var comprometidoSolicitud =
                    await _context.MovimientoPresupuestals
                        .Where(x =>
                            x.SolicitudMantenimientoId == solicitud.SolicitudMantenimientoId
                            &&
                            x.TipoMovimiento == "COMPROMETIDO"
                            &&
                            x.Activo == true)
                        .SumAsync(x =>
                            x.Importe ?? 0);

                var descomprometidoSolicitud =
                    await _context.MovimientoPresupuestals
                        .Where(x =>
                            x.SolicitudMantenimientoId == solicitud.SolicitudMantenimientoId
                            &&
                            x.TipoMovimiento == "DESCOMPROMETIDO"
                            &&
                            x.Activo == true)
                        .SumAsync(x =>
                            x.Importe ?? 0);

                comprometido =
                    comprometidoSolicitud -
                    descomprometidoSolicitud;
            }
            else
            {

                var comprometidoGeneral =
                    movimientos
                        .Where(x =>
                            x.TipoMovimiento == "COMPROMETIDO")
                        .Sum(x =>
                            x.Importe ?? 0);

                var descomprometidoGeneral =
                    movimientos
                        .Where(x =>
                            x.TipoMovimiento == "DESCOMPROMETIDO")
                        .Sum(x =>
                            x.Importe ?? 0);

                comprometido = Math.Max(comprometidoGeneral - descomprometidoGeneral, 0);






            }

            var devengado =
                movimientos
                    .Where(x =>
                        x.TipoMovimiento ==
                        "DEVENGADO")
                    .Sum(x => x.Importe);

            var disponible =
                presupuestoAsignado
                - comprometido
                - devengado;

            // =====================================================
            // PARTIDAS DE LA SOLICITUD
            // =====================================================

            var partidas = new List<DSPPartidaSolicitudViewModel>();

            foreach (var detalle in solicitud.SolicitudMantenimientoDetalles)
            {
                if (detalle.CostoEstimadoManoObra > 0 &&
                    detalle.IdObjetoGastoManoObra.HasValue)
                {
                    var idObjeto = detalle.IdObjetoGastoManoObra.Value;

                    var partida = partidas.FirstOrDefault(x => x.IdObjetoGasto == idObjeto);

                    if (partida == null)
                    {
                        partida =
                            new DSPPartidaSolicitudViewModel
                            {
                                IdObjetoGasto = idObjeto,

                                ClaveObjetoGasto = detalle.IdObjetoGastoManoObraNavigation?.ClaveObjGasto
                                    ?? string.Empty,

                                ObjetoGastoDescripcion = detalle.IdObjetoGastoManoObraNavigation?.Descripcion
                                    ?? string.Empty
                            };

                        partidas.Add(partida);
                    }

                    partida.ImporteSolicitado += detalle.CostoEstimadoManoObra;
                }

                if (detalle.CostoEstimadoRefacciones > 0 &&
                    detalle.IdObjetoGastoRefacciones.HasValue)
                {
                    var idObjeto =
                        detalle.IdObjetoGastoRefacciones.Value;

                    var partida =
                        partidas.FirstOrDefault(x =>
                            x.IdObjetoGasto ==
                            idObjeto);

                    if (partida == null)
                    {
                        partida =
                            new DSPPartidaSolicitudViewModel
                            {
                                IdObjetoGasto = idObjeto,

                                ClaveObjetoGasto =
                                    detalle.IdObjetoGastoRefaccionesNavigation?.ClaveObjGasto
                                    ?? string.Empty,

                                ObjetoGastoDescripcion =
                                    detalle.IdObjetoGastoRefaccionesNavigation?.Descripcion
                                    ?? string.Empty
                            };

                        partidas.Add(partida);
                    }

                    partida.ImporteSolicitado += detalle.CostoEstimadoRefacciones;
                }
            }





            //=====================================================
            // IMPORTE TOTAL SOLICITUD
            //=====================================================

            var importeSolicitud =
                partidas.Sum(x =>
                    x.ImporteSolicitado);

            //=====================================================
            // DISPONIBILIDAD TALLER
            //=====================================================

            if (controlTaller != null)
            {
                tallerTieneDisponibilidad =
                    disponibleTaller
                    >=
                    importeSolicitud;
            }










            var movimientosDSPHistoricos = await _context.MovimientoPresupuestals
                    .Where(x =>
                    x.Activo == true
                    &&
                    idsDSP.Contains(
                    x.IdDispPresupuestal ?? 0))
                   .ToListAsync();



            // =====================================================
            // DSP DISPONIBLE POR PARTIDA
            // =====================================================

            foreach (var partida in partidas)
            {


                var dspObjeto =
                    dsps.Where(x =>
                        x.IdObjetoGasto ==
                        partida.IdObjetoGasto)
                    .ToList();

                decimal autorizadoPorArea = 0;
                decimal disponibleObjeto = 0;




                if (estaRechazada)
                {
                    var autorizadoPorArea1 = dspObjeto.Sum(x => x.Importe);

                    var idsDSPPartida = dspObjeto
                        .Select(x => x.IdDispPresupuestal)
                        .ToList();




                    var comprometidoSolicitudOriginal =
                        movimientosHistoricosSolicitud
                            .Where(x =>
                                x.TipoMovimiento == "COMPROMETIDO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);

                    var descomprometidoSolicitudActual =
                        movimientosHistoricosSolicitud
                            .Where(x =>
                                x.TipoMovimiento == "DESCOMPROMETIDO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);

                    var comprometidoHistorico =
                        movimientosDSPHistoricos
                            .Where(x =>
                                x.TipoMovimiento == "COMPROMETIDO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);

                    var descomprometidoHistorico =
                        movimientosDSPHistoricos
                            .Where(x =>
                                x.TipoMovimiento == "DESCOMPROMETIDO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);

                    var comprometidoHistoricoNeto =
                        Math.Max(
                            comprometidoHistorico -
                            descomprometidoHistorico,
                            0);

                    var devengadoHistorico =
                        movimientosDSPHistoricos
                            .Where(x =>
                                x.TipoMovimiento == "DEVENGADO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);


                    //////////////////////////////////////////////////
                    partida.AutorizadoPorArea = autorizadoPorArea1;

                    partida.ImporteSolicitado = comprometidoSolicitudOriginal;

                    partida.ImporteLiberado = descomprometidoSolicitudActual;

                    partida.FueDescomprometida = descomprometidoSolicitudActual > 0;

                    partida.DisponibleDSP = autorizadoPorArea1 - comprometidoHistoricoNeto - devengadoHistorico;

                    partida.TieneSuficiencia = true;


                    partida.EstaRechazada = estaRechazada;


                    partida.DSPDisponibles =
                        dspObjeto.Select(x =>
                            new SelectListItem
                            {
                                Value = x.IdDispPresupuestal.ToString(),
                                Text = $"{x.IdDsp} - {x.IdObjetoGastoNavigation.Descripcion}"
                            })
                        .ToList();

                    continue;

                }

                // =====================================
                // SOLICITUD YA VALIDADA DSP
                // =====================================

                if (solicitudDSPAutorizada)
                {
                    var autorizadoPorArea1 =
                        dspObjeto.Sum(x => x.Importe);

                    var idsDSPPartida =
                        dspObjeto
                            .Select(x => x.IdDispPresupuestal)
                            .ToList();

                    var comprometidoHistorico =
                     movimientosDSPHistoricos
                         .Where(x =>
                             x.TipoMovimiento == "COMPROMETIDO"
                             &&
                             idsDSPPartida.Contains(
                                 x.IdDispPresupuestal ?? 0))
                         .Sum(x => x.Importe ?? 0);

                    var descomprometidoHistorico =
                        movimientosDSPHistoricos
                            .Where(x =>
                                x.TipoMovimiento == "DESCOMPROMETIDO"
                                &&
                                idsDSPPartida.Contains(
                                    x.IdDispPresupuestal ?? 0))
                            .Sum(x => x.Importe ?? 0);

                    var comprometidoHistoricoNeto =
                        Math.Max(
                            comprometidoHistorico -
                            descomprometidoHistorico,
                            0);

                    var devengadoHistorico =
                    movimientosDSPHistoricos
                        .Where(x =>
                            x.TipoMovimiento == "DEVENGADO"
                            &&
                            idsDSPPartida.Contains(
                                x.IdDispPresupuestal ?? 0))
                        .Sum(x =>
                            x.Importe ?? 0);



                    var comprometidoSolicitudActual =
                    movimientosHistoricosSolicitud
                        .Where(x =>
                            x.IdDispPresupuestalNavigation != null
                            &&
                            x.IdDispPresupuestalNavigation.IdObjetoGasto ==
                                partida.IdObjetoGasto
                            &&
                            x.TipoMovimiento == "COMPROMETIDO")
                        .Sum(x =>
                            x.Importe ?? 0);


                    var descomprometidoSolicitudActual =
                        movimientosHistoricosSolicitud
                            .Where(x =>
                                x.IdDispPresupuestalNavigation != null
                                &&
                                x.IdDispPresupuestalNavigation.IdObjetoGasto ==
                                    partida.IdObjetoGasto
                                &&
                                x.TipoMovimiento == "DESCOMPROMETIDO")
                            .Sum(x =>
                                x.Importe ?? 0);






                    if (descomprometidoSolicitudActual > 0)
                    {
                        partida.HistorialPresupuestal =
                            "Liberado y recomprometido";
                    }
                    else
                    {
                        partida.HistorialPresupuestal =
                            "Sin incidencias";
                    }


                    var comprometidoNetoSolicitud = Math.Max(comprometidoSolicitudActual - descomprometidoSolicitudActual, 0);


                    var ultimoMovimiento = movimientosHistoricosSolicitud
                        .Where(x =>
                            x.IdDispPresupuestalNavigation != null
                            &&
                            x.IdDispPresupuestalNavigation.IdObjetoGasto ==
                                partida.IdObjetoGasto)
                        .OrderByDescending(x => x.FechaMovimiento)
                        .FirstOrDefault();






                    var tieneComprometido = comprometidoSolicitudActual > 0;

                    var tieneDescomprometido = descomprometidoSolicitudActual > 0;

                    if (tieneDescomprometido && !tieneComprometido)
                    {
                        partida.HistorialPresupuestal =
                            "Rechazada";
                    }
                    else if (tieneDescomprometido && tieneComprometido)
                    {
                        partida.HistorialPresupuestal =
                            "Rechazada y posteriormente reaprobada";
                    }
                    else
                    {
                        partida.HistorialPresupuestal =
                            "Sin incidencias";
                    }







                    var estaLiberadaActualmente =
                        ultimoMovimiento?.TipoMovimiento == "DESCOMPROMETIDO";

                    partida.AutorizadoPorArea =
                        autorizadoPorArea1;

                    partida.ImporteSolicitado =
                        comprometidoNetoSolicitud;

                    partida.FueDescomprometida =
                        estaLiberadaActualmente;

                    partida.ImporteLiberado =
                        estaLiberadaActualmente
                            ? descomprometidoSolicitudActual
                            : 0;

                    partida.DisponibleDSP =
                        autorizadoPorArea1 -
                        comprometidoHistoricoNeto -
                        devengadoHistorico;


                    partida.TieneSuficiencia = true;

                    partida.DSPDisponibles =
                        dspObjeto.Select(x =>
                            new SelectListItem
                            {
                                Value =
                                    x.IdDispPresupuestal.ToString(),

                                Text =
                                    $"{x.IdDsp} - {x.IdObjetoGastoNavigation.Descripcion}"
                            })
                        .ToList();

                    continue;
                }

                // =====================================
                // SOLICITUD NUEVA
                // =====================================

                foreach (var dsp in dspObjeto)
                {
                    var comprometidoDSP =
                     movimientos
                         .Where(x =>
                             x.IdDispPresupuestal ==
                             dsp.IdDispPresupuestal
                             &&
                             x.TipoMovimiento ==
                             "COMPROMETIDO"
                             &&
                             x.Activo == true
                             )
                         .Sum(x =>
                             x.Importe ?? 0);

                    var descomprometidoDSP =
                        movimientos
                            .Where(x =>
                                x.IdDispPresupuestal ==
                                dsp.IdDispPresupuestal
                                &&
                                x.TipoMovimiento ==
                                "DESCOMPROMETIDO"
                                &&
                                x.Activo == true
                                )
                            .Sum(x =>
                                x.Importe ?? 0);

                    var comprometidoNetoDSP = Math.Max(comprometidoDSP - descomprometidoDSP, 0);

                    var devengadoDSP =
                        movimientos
                            .Where(x =>
                                x.IdDispPresupuestal ==
                                dsp.IdDispPresupuestal
                                &&
                                x.TipoMovimiento ==
                                "DEVENGADO")
                            .Sum(x =>
                                x.Importe);

                    autorizadoPorArea += dsp.Importe;

                    disponibleObjeto += Math.Max((decimal)(dsp.Importe - comprometidoNetoDSP - devengadoDSP), 0);


                }

                partida.AutorizadoPorArea = autorizadoPorArea;

                partida.DisponibleDSP = disponibleObjeto;

                partida.TieneSuficiencia = disponibleObjeto >= partida.ImporteSolicitado;





                ////// si es nueva validar el 20% de la partida
                var disponiblePosterior = disponibleObjeto - partida.ImporteSolicitado;

                var limite20 =
                    autorizadoPorArea * 0.20m;

                var porcentajePosterior =
                    autorizadoPorArea == 0
                        ? 0
                        : (disponiblePosterior / autorizadoPorArea) * 100m;

                partida.DisponiblePosterior = disponiblePosterior;

                partida.LimiteMinimo20Porciento = limite20;

                partida.PorcentajeDisponiblePosterior = porcentajePosterior;

                partida.ViolaPolitica20Porciento = disponiblePosterior <= limite20;

                // =====================================




                partida.DSPDisponibles =
                    dspObjeto
                        .Select(x =>
                            new SelectListItem
                            {
                                Value =
                                    x.IdDispPresupuestal.ToString(),

                                Text =
                                    $"{x.IdDsp} - {x.IdObjetoGastoNavigation.Descripcion}"
                            })
                        .ToList();
            }


            var violaPolitica20 = partidas.Any(x => x.ViolaPolitica20Porciento);

            var partidasDSP = dsps
              .GroupBy(x => new
              {
                  x.IdObjetoGasto,
                  x.IdObjetoGastoNavigation.ClaveObjGasto,
                  x.IdObjetoGastoNavigation.Descripcion
              })
              .Select(g =>
              {
                  var asignado =
                      g.Sum(x => x.Importe);

                  var idsDSPGrupo =
                      g.Select(x => x.IdDispPresupuestal)
                       .ToList();

                  var comprometidoGrupo = movimientos
                  .Where(x =>
                      x.Activo == true
                      &&
                      x.TipoMovimiento == "COMPROMETIDO"
                      &&
                      idsDSPGrupo.Contains(
                          x.IdDispPresupuestal ?? 0))
                  .Sum(x =>
                      x.Importe ?? 0);

                  var descomprometidoGrupo =
                      movimientos
                          .Where(x =>
                              x.Activo == true
                              &&
                              x.TipoMovimiento == "DESCOMPROMETIDO"
                              &&
                              idsDSPGrupo.Contains(
                                  x.IdDispPresupuestal ?? 0))
                          .Sum(x =>
                              x.Importe ?? 0);

                  var comprometidoNeto = Math.Max(comprometidoGrupo - descomprometidoGrupo, 0);





                  var devengadoGrupo = movimientos
                    .Where(x =>
                        x.Activo == true
                        &&
                        x.TipoMovimiento == "DEVENGADO"
                        &&
                        idsDSPGrupo.Contains(
                            x.IdDispPresupuestal ?? 0))
                    .Sum(x =>
                        x.Importe ?? 0);



                  var solicitadoSolicitudActual = movimientos
                    .Where(x =>
                        x.Activo == true
                        &&
                        x.TipoMovimiento == "COMPROMETIDO"
                        &&
                        x.SolicitudMantenimientoId ==
                            solicitud.SolicitudMantenimientoId
                        &&
                        idsDSPGrupo.Contains(
                            x.IdDispPresupuestal ?? 0))
                    .Sum(x =>
                        x.Importe ?? 0);




                  var descomprometidoSolicitudActual =
                    movimientos
                        .Where(x =>
                            x.Activo == true
                            &&
                            x.TipoMovimiento == "DESCOMPROMETIDO"
                            &&
                            x.SolicitudMantenimientoId ==
                                solicitud.SolicitudMantenimientoId
                            &&
                            idsDSPGrupo.Contains(
                                x.IdDispPresupuestal ?? 0))
                        .Sum(x =>
                            x.Importe ?? 0);

                  solicitadoSolicitudActual -= descomprometidoSolicitudActual;

                  if (solicitadoSolicitudActual < 0)
                  {
                      solicitadoSolicitudActual = 0;
                  }





                  var ultimoMovimiento = movimientos
                    .Where(x =>
                        x.Activo == true &&
                        x.SolicitudMantenimientoId ==
                            solicitud.SolicitudMantenimientoId &&
                        idsDSPGrupo.Contains(
                            x.IdDispPresupuestal ?? 0))
                    .OrderByDescending(x => x.FechaMovimiento)
                    .FirstOrDefault();

                  var sigueLiberada =
                      ultimoMovimiento?.TipoMovimiento ==
                      "DESCOMPROMETIDO";



                  return new DSPDisponibleViewModel
                  {
                      IdObjetoGasto = g.Key.IdObjetoGasto,

                      ClaveObjetoGasto = g.Key.ClaveObjGasto,

                      ObjetoGastoDescripcion = g.Key.Descripcion,

                      //ImporteSolicitado = solicitadoSolicitudActual,

                      PresupuestoAsignado = asignado,

                      PresupuestoComprometido = comprometidoNeto,

                      PresupuestoDevengado = devengadoGrupo,

                      PresupuestoDisponible = asignado - comprometidoNeto - devengadoGrupo,

                      ImporteSolicitado = Math.Max(solicitadoSolicitudActual, 0),

                      ImporteLiberado = sigueLiberada ? descomprometidoSolicitudActual : 0,

                      FueDescomprometida = sigueLiberada




                      //ultimoMovimiento = movimientosHistoricosSolicitud.Where(x => x.IdDispPresupuestalNavigation != null
                      //  &&
                      //x.IdDispPresupuestalNavigation.IdObjetoGasto == partida.IdObjetoGasto).OrderByDescending(x => x.FechaMovimiento).FirstOrDefault()



                  };
              })
              .ToList();


            return new DSPViewModel
            {
                SolicitudMantenimientoId = solicitud.SolicitudMantenimientoId,

                Folio = solicitud.Folio,

                AreaId = solicitud.AreaSolicitanteId,

                Area = solicitud.AreaSolicitante.AreaNombre,

                ImporteSolicitud = (decimal)solicitud.ImporteCotizacion,

                PresupuestoAsignado = presupuestoAsignado,

                PresupuestoComprometido = (decimal)comprometido,

                PresupuestoDevengado = (decimal)devengado,

                PresupuestoDisponible = (decimal)disponible,

                TieneSuficienciaPresupuestal = partidas.All(x => x.TieneSuficiencia),

                PartidasSolicitud = partidas,

                PartidasDSP = partidasDSP,

                EstatusSolicitudMantenimientoId = solicitud.EstatusSolicitudMantenimientoId,

                ViolaPolitica20Porciento = violaPolitica20,

                TallerId = solicitud.TallerId,
                Taller = solicitud.Taller.RazonSocial.Trim(),
                TallerMinimo = solicitud.Taller.MontoMinimo,
                TallerMaximo = (decimal)solicitud.Taller.MontoMaximo,

                PresupuestoTaller = presupuestoTaller,

                ComprometidoTaller = comprometidoTaller,

                EjercidoTaller = ejercidoTaller,

                DisponibleTaller = disponibleTaller,

                TieneDisponibilidadTaller = tallerTieneDisponibilidad,

                MontoMinimoTaller = solicitud.Taller?.MontoMinimo ?? 0,

                MontoMaximoTaller = solicitud.Taller?.MontoMaximo ?? 0,

            };
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





        public async Task ProcesarDSP(DSPDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            //foreach (var partida in dto.Partidas)
            //{
            //    Debug.WriteLine(
            //        $"Partida={partida.IdObjetoGasto}");

            //    Debug.WriteLine(
            //        $"ImporteSolicitado={partida.ImporteSolicitado}");
            //}

            try
            {
                var solicitud =
                    await _context.SolicitudMantenimientos
                        .Include(x => x.AreaSolicitante)
                        .FirstOrDefaultAsync(x =>
                            x.SolicitudMantenimientoId ==
                            dto.SolicitudMantenimientoId);

                if (solicitud == null)
                {
                    throw new Exception("INFO|Solicitud no encontrada.");
                }


                // VALIDAR ESTATUS

                if (solicitud.EstatusSolicitudMantenimientoId >= 4)
                {
                    throw new Exception(
                        "WARNING|La solicitud ya cuenta con validación presupuestal.");
                }




                //=====================================
                // VALIDAR TALLER
                //=====================================

                var taller =
                await _context.Tallers
                .FirstOrDefaultAsync(x =>
                    x.TallerId == solicitud.TallerId
                    &&
                    x.Activo);

                if (taller == null)
                {
                    throw new Exception(
                        "WARNING|El taller no existe.");
                }


                //=====================================
                // OBTENER MOVIMIENTOS DEL TALLER
                //=====================================

                var movimientosTaller = await _context.TallerMovimientoPresupuestos
                .Where(x =>
                    x.TallerId == taller.TallerId
                    &&
                    x.Activo)
                .ToListAsync();

                var reservado = movimientosTaller
                .Where(x =>
                    x.TipoMovimiento == "COMPROMETIDO")
                .Sum(x =>
                    x.Importe);

                var liberado =
                movimientosTaller
                .Where(x =>
                    x.TipoMovimiento == "LIBERACION")
                .Sum(x =>
                    x.Importe);

                var ejercido =
                movimientosTaller
                .Where(x =>
                    x.TipoMovimiento == "EJERCIDO")
                .Sum(x =>
                    x.Importe);

                var reservadoNeto = Math.Max(reservado - liberado, 0);


                //=====================================
                // DISPONIBLE REAL DEL TALLER
                //=====================================

                var disponibleTaller = (taller.MontoMaximo ?? 0) - reservadoNeto - ejercido;

                var importeTotalSolicitud =
                dto.Partidas.Sum(x =>
                    x.ImporteSolicitado);


                //=====================================
                // VALIDAR MÍNIMO
                //=====================================



                if ((taller.MontoMaximo ?? 0) <= 0)
                {
                    throw new Exception("WARNING|El taller no tiene presupuesto máximo configurado.");
                }

                //var disponiblePosterior =
                //disponibleTaller
                //-
                //importeTotalSolicitud;
                var disponiblePosterior = Math.Max(disponibleTaller - importeTotalSolicitud, 0);



                if (disponiblePosterior < taller.MontoMinimo)
                {
                    throw new Exception(
                    $"WARNING|El taller quedaría por debajo del mínimo permitido. " +
                    $"Disponible posterior: {disponiblePosterior:C}. " +
                    $"Mínimo requerido: {taller.MontoMinimo:C}");
                }


                //=====================================
                // VALIDAR DISPONIBLE
                //=====================================

                if (
                importeTotalSolicitud >
                disponibleTaller)
                {
                    throw new Exception(
                    $"WARNING|El taller no tiene disponibilidad. " +
                    $"Disponible: {disponibleTaller:C}");
                }


                var movimientosDSPGlobal =
                await _context.MovimientoPresupuestals
                    .AsNoTracking()
                    .Where(x => x.Activo == true)
                    .Select(x => new
                    {
                        x.IdDispPresupuestal,
                        x.TipoMovimiento,
                        x.Importe
                    })
                    .ToListAsync();


                // VALIDAR PARTIDAS

                foreach (var partida in dto.Partidas)
                {
                    var dsp =
                        await _context.DispPresupuestals
                            .FirstOrDefaultAsync(x =>
                                x.IdArea ==
                                    solicitud.AreaSolicitanteId
                                &&
                                x.IdObjetoGasto ==
                                    partida.IdObjetoGasto
                                &&
                                x.Activo);

                    if (dsp == null)
                    {
                        throw new Exception(
                            $"WARNING|No existe DSP para el objeto de gasto {partida.IdObjetoGasto}.");
                    }

                    // MOVIMIENTOS REALES DEL DSP

                    var movimientosDSP = movimientosDSPGlobal
                        .Where(x =>
                    x.IdDispPresupuestal ==
                    dsp.IdDispPresupuestal)
                    .ToList();



                    var comprometido =
                          movimientosDSP
                              .Where(x =>
                                  x.TipoMovimiento ==
                                  "COMPROMETIDO")
                              .Sum(x => x.Importe);

                    var descomprometido =
                        movimientosDSP
                            .Where(x =>
                                x.TipoMovimiento ==
                                "DESCOMPROMETIDO")
                            .Sum(x => x.Importe);

                    //var comprometidoNeto =
                    //    comprometido
                    //    - descomprometido;
                    //var comprometidoNeto = Math.Max((decimal)(comprometido - descomprometido), 0);

                    var comprometidoNeto = Math.Max((comprometido ?? 0) - (descomprometido ?? 0), 0);


                    var devengado =
                        movimientosDSP
                            .Where(x =>
                                x.TipoMovimiento == "DEVENGADO")
                            .Sum(x => x.Importe);


                    //var disponible = dsp.Importe - comprometidoNeto - devengado;

                    var disponible = Math.Max((decimal)(dsp.Importe - comprometidoNeto - (devengado ?? 0)), 0);



                    // VALIDAR POLÍTICA DEL 20%
                    var disponiblePosterior20 = disponible - partida.ImporteSolicitado;

                    var limiteMinimo = dsp.Importe * 0.20m;

                    if (disponiblePosterior20 <= limiteMinimo)
                    {
                        var porcentaje = dsp.Importe == 0 ? 0 : Math.Round((decimal)((disponiblePosterior20 / dsp.Importe) * 100m), 2);

                        throw new Exception(
                            $"WARNING|La partida {partida.ClaveObjetoGasto ?? partida.IdObjetoGasto.ToString()} no puede autorizarse. " +
                            $"Despues del compromiso quedarian {disponiblePosterior20:C} disponibles ({porcentaje}%), " +
                            $"lo cual es igual o menor al limite permitido del 20% ({limiteMinimo:C}).");
                    }


                    if (partida.ImporteSolicitado > disponible)
                    {
                        throw new Exception(
                            $"WARNING|La partida {partida.ClaveObjetoGasto ?? partida.IdObjetoGasto.ToString()} no tiene suficiencia presupuestal.");
                    }

                    // =====================================
                    // REGISTRAR COMPROMISO
                    // =====================================

                    _context.MovimientoPresupuestals.Add(
                        new MovimientoPresupuestal
                        {
                            SolicitudMantenimientoId =
                                solicitud.SolicitudMantenimientoId,

                            IdDispPresupuestal =
                                dsp.IdDispPresupuestal,

                            FechaMovimiento =
                                DateTime.Now,

                            TipoMovimiento =
                                "COMPROMETIDO",

                            Importe =
                                partida.ImporteSolicitado,

                            Observaciones =
                                dto.Observaciones,

                            FechaCaptura =
                                DateTime.Now,

                            UsuarioCaptura =
                                "ADMIN",

                            Activo = true
                        });
                }



                //=====================================
                // REGISTRAR COMPROMETIDO TALLER
                //=====================================

                _context.TallerMovimientoPresupuestos.Add(
                new TallerMovimientoPresupuesto
                {
                    TallerId =
                        taller.TallerId,

                    SolicitudMantenimientoId =
                        solicitud.SolicitudMantenimientoId,

                    FechaMovimiento =
                        DateTime.Now,

                    TipoMovimiento =
                        "COMPROMETIDO",

                    Importe =
                        importeTotalSolicitud,

                    UsuarioCaptura =
                        "ADMIN",

                    Activo = true
                });





                // =========================================
                // ACTUALIZAR SOLICITUD
                // =========================================

                solicitud.EstatusSolicitudMantenimientoId = 4;

                solicitud.FechaAutorizacion =
                    DateTime.Now;

                // =========================================
                // SEGUIMIENTO
                // =========================================

                _context.SolicitudMantenimientoSeguimientos.Add(
                    new SolicitudMantenimientoSeguimiento
                    {
                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId,

                        EstatusSolicitudMantenimientoId =
                            solicitud.EstatusSolicitudMantenimientoId,

                        Observaciones =
                            string.IsNullOrWhiteSpace(dto.Observaciones)
                                ? "Solicitud validada presupuestalmente."
                                : dto.Observaciones,

                        FechaMovimiento =
                            DateTime.Now
                    });






                //await _context.SaveChangesAsync();





                //=====================================
                // CONTROL PRESUPUESTAL TALLER
                //=====================================

                var controlTaller =
                await _context.TallerControlPresupuestals
                .FirstOrDefaultAsync(x =>
                    x.TallerId == taller.TallerId
                    &&
                    x.Ejercicio == (short)DateTime.Now.Year
                    &&
                    x.Activo);

                var presupuestoAsignado =
                taller.MontoMaximo ?? 0;

                if (controlTaller == null)
                {
                    decimal comprometidoInicial = importeTotalSolicitud;

                    decimal disponibleInicial = Math.Max(presupuestoAsignado - comprometidoInicial, 0m);

                    controlTaller = new TallerControlPresupuestal
                    {
                        TallerId =
                            taller.TallerId,

                        Ejercicio =
                            (short)DateTime.Now.Year,

                        PresupuestoAsignado =
                            presupuestoAsignado,

                        PresupuestoComprometido =
                            comprometidoInicial,

                        PresupuestoEjercido =
                            0m,

                        PresupuestoDisponible =
                            disponibleInicial,

                        FechaInicio =
                            DateOnly.FromDateTime(DateTime.Now),

                        FechaFin =
                            new DateOnly(
                                DateTime.Now.Year,
                                12,
                                31),

                        FechaActualizacion =
                            DateTime.Now,

                        Activo =
                            true
                    };

                    _context.TallerControlPresupuestals
                        .Add(controlTaller);
                }

                //_context.Entry(controlTaller)
                //.Property(x => x.PresupuestoDisponible)
                //.CurrentValue =
                //    controlTaller.PresupuestoDisponible;

                //_context.Entry(controlTaller)
                //    .Property(x => x.PresupuestoDisponible)
                //    .IsModified =
                //        true;

                //_context.Entry(controlTaller)
                //    .Property(x => x.PresupuestoComprometido)
                //    .IsModified =
                //        true;




                //Debug.WriteLine($"Asignado={controlTaller.PresupuestoAsignado}");

                //Debug.WriteLine(
                //$"Comprometido={controlTaller.PresupuestoComprometido}");

                //Debug.WriteLine(
                //$"Disponible={controlTaller.PresupuestoDisponible}");


                ////=====================================
                //// GUARDAR CONTROL TALLER
                ////=====================================
                //_context.ChangeTracker.DetectChanges();


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
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
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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
                    throw new Exception(
                        "WARNING|La solicitud no fue encontrada.");
                }

                // ============================================
                // VALIDAR ESTATUS AUTORIZADA
                // ============================================

                const int ESTATUS_AUTORIZADA = 5; // Ajustar

                if (
                    solicitud.EstatusSolicitudMantenimientoId !=
                    ESTATUS_AUTORIZADA)
                {
                    throw new Exception(
                        "WARNING|La solicitud debe estar autorizada para ingresar al taller.");
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
                    throw new Exception(
                        "WARNING|La solicitud ya fue ingresada al taller.");
                }

                // ============================================
                // OBTENER DETALLES
                // ============================================

                var detallesSolicitud =
                    await _context
                        .SolicitudMantenimientoDetalles
                        .Where(x =>
                            x.SolicitudMantenimientoId ==
                                solicitudMantenimientoId
                            &&
                            x.Activo)
                        .ToListAsync();

                if (!detallesSolicitud.Any())
                {
                    throw new Exception(
                        "WARNING|La solicitud no contiene servicios.");
                }

                // ============================================
                // CREAR MANTENIMIENTO
                // ============================================

                const int ESTATUS_MANT_INGRESADO = 1; // Ajustar

                var mantenimiento =
                    new Mantenimiento
                    {
                        SolicitudMantenimientoId =
                            solicitud.SolicitudMantenimientoId,

                        UnidadVehicularId =
                            solicitud.UnidadVehicularId,

                        TipoMantenimientoId =
                            solicitud.TipoMantenimientoId,

                        TallerId = (int)solicitud.TallerId,

                        FechaIngreso =
                            DateTime.Now,

                        KilometrajeEntrada =
                            solicitud.KilometrajeActual,

                        MotivoMantenimiento = string.IsNullOrWhiteSpace(solicitud.MotivoSolicitud) ? "SIN ESPECIFICAR" : solicitud.MotivoSolicitud.Trim(),

                        Diagnostico =
                            solicitud.DiagnosticoInicial,

                        Observaciones =
                            solicitud.Observaciones,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId =
                            1, // TODO Usuario autenticado

                        EstatusMantenimientoId =
                            ESTATUS_MANT_INGRESADO,

                        Activo = true
                    };


                if (string.IsNullOrWhiteSpace(solicitud.MotivoSolicitud))
                {
                    throw new Exception("WARNING|La solicitud no contiene un motivo de mantenimiento.");
                }


                _context.Mantenimientos.Add(mantenimiento);

                await _context.SaveChangesAsync();

                // ============================================
                // CREAR DETALLES DE MANTENIMIENTO
                // ============================================

                decimal costoRealTotal = 0;

                foreach (var item in detallesSolicitud)
                {
                    var subtotalManoObra =
                        item.Cantidad *
                        item.CostoEstimadoManoObra;

                    var subtotalRefacciones =
                        item.Cantidad *
                        item.CostoEstimadoRefacciones;

                    var totalLinea =
                        subtotalManoObra +
                        subtotalRefacciones;

                    costoRealTotal += totalLinea;

                    var detalle =
                        new MantenimientoDetalle
                        {
                            MantenimientoId =
                                mantenimiento.MantenimientoId,

                            ConceptoServicioId =
                                item.ConceptoServicioId,

                            Cantidad =
                                item.Cantidad,

                            CostoUnitarioManoObra =
                                item.CostoEstimadoManoObra,

                            CostoUnitarioRefacciones =
                                item.CostoEstimadoRefacciones,

                            SubtotalManoObra =
                                subtotalManoObra,

                            SubtotalRefacciones =
                                subtotalRefacciones,

                            TotalLinea =
                                totalLinea,

                            IdObjetoGastoManoObra =
                                item.IdObjetoGastoManoObra,

                            IdObjetoGastoRefacciones =
                                item.IdObjetoGastoRefacciones,

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

                // ============================================
                // ACTUALIZAR COSTO TOTAL
                // ============================================

                mantenimiento.CostoRealTotal =
                    costoRealTotal;

                await _context.SaveChangesAsync();

                // ============================================
                // CAMBIAR ESTATUS DE SOLICITUD
                // ============================================

                const int ESTATUS_EN_TALLER = 7; // Ajustar

                solicitud.EstatusSolicitudMantenimientoId =
                    ESTATUS_EN_TALLER;

                // ============================================
                // GENERAR SEGUIMIENTO
                // ============================================

                _context
                    .SolicitudMantenimientoSeguimientos
                    .Add(
                        new SolicitudMantenimientoSeguimiento
                        {
                            SolicitudMantenimientoId =
                                solicitud.SolicitudMantenimientoId,

                            EstatusSolicitudMantenimientoId =
                                ESTATUS_EN_TALLER,

                            Observaciones =
                                "La unidad fue ingresada al taller.",

                            UsuarioId = 1, // TODO Usuario autenticado

                            FechaMovimiento =
                                DateTime.Now
                        });

                await _context.SaveChangesAsync();

                // ============================================
                // COMMIT
                // ============================================

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
                        x.SolicitudMantenimientoId ==
                        solicitudId
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
                .Where(x => x.SolicitudMantenimientoId == solicitudId &&
                x.Activo)
                .ToListAsync();


            // ============================================
            // MAPEAR VIEWMODEL
            // ============================================

            return new MantenimientoCreateViewModel
            {
                SolicitudMantenimientoId =
                    solicitud.SolicitudMantenimientoId,

                Folio = solicitud.Folio,

                NumeroOficio = solicitud.NumeroOficio,

                UnidadVehicularId = solicitud.UnidadVehicularId,

                UnidadTexto = solicitud.UnidadVehicular.PlacaActual + " - " + solicitud.UnidadVehicular.NumeroSerie + " - " +
                solicitud.UnidadVehicular.NumeroEconomico,

                TipoMantenimientoId =
                    solicitud.TipoMantenimientoId,

                TallerId = (int)solicitud.TallerId,

                KilometrajeEntrada =
                    solicitud.KilometrajeActual,

                MotivoMantenimiento =
                    solicitud.MotivoSolicitud,

                Diagnostico =
                    solicitud.DiagnosticoInicial,

                Observaciones =
                    solicitud.Observaciones,

                FechaIngreso =
                    DateTime.Now,

                Taller = solicitud.Taller.RazonSocial,

                UnidadVehicularTexto = solicitud.UnidadVehicular != null
                        ? $"{solicitud.UnidadVehicular.NumeroEconomico} - {solicitud.UnidadVehicular.PlacaActual}"
                        : string.Empty,



                Servicios = detalles
                .Select(x =>
            new MantenimientoServicioViewModel
            {
                ConceptoServicio = x.ConceptoServicio.ConceptoServicioNombre,

                Cantidad = (int)x.Cantidad,
                CostoEstimadoManoObra = x.CostoEstimadoManoObra,
                CostoEstimadoRefacciones = x.CostoEstimadoRefacciones,
                TotalEstimado = x.Cantidad * (
                        x.CostoEstimadoManoObra +
                        x.CostoEstimadoRefacciones
                    ),

                Observaciones = x.Observaciones
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

            return await ObtenerMantenimientoEditarPorMantenimientoId(mantenimiento.MantenimientoId);
        }





        public async Task EditarMantenimiento(
     MantenimientoEditDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var mantenimiento =
                    await _context.Mantenimientos
                        .Include(x => x.MantenimientoDetalles)
                        .FirstOrDefaultAsync(x =>
                            x.MantenimientoId ==
                            dto.MantenimientoId);

                if (mantenimiento == null)
                {
                    throw new Exception(
                        "WARNING|El mantenimiento no existe.");
                }

                // =========================================
                // VALIDAR ESTATUS
                // =========================================

                if (mantenimiento.EstatusMantenimientoId != 1)
                {
                    throw new Exception(
                        "WARNING|El mantenimiento ya fue finalizado y no puede modificarse.");
                }

                // =========================================
                // VALIDAR SERVICIOS
                // =========================================

                if (dto.Servicios == null ||
                    !dto.Servicios.Any())
                {
                    throw new Exception(
                        "WARNING|Debe existir al menos un servicio.");
                }

                // =========================================
                // ACTUALIZAR ENCABEZADO
                // =========================================

                mantenimiento.FechaInicioTrabajo =
                    dto.FechaInicioTrabajo;

                mantenimiento.FechaFinTrabajo =
                    dto.FechaFinTrabajo;

                mantenimiento.Diagnostico =
                    dto.Diagnostico;

                mantenimiento.TrabajoRealizado =
                    dto.TrabajoRealizado;

                mantenimiento.Observaciones =
                    dto.Observaciones;

                mantenimiento.KilometrajeSalida = dto.KilometrajeSalida;

                // =========================================
                // ELIMINAR DETALLES
                // =========================================

                var idsRecibidos =
                    dto.Servicios
                        .Where(x =>
                            x.MantenimientoDetalleId.HasValue)
                        .Select(x =>
                            x.MantenimientoDetalleId!.Value)
                        .ToList();

                var detallesEliminar =
                    mantenimiento.MantenimientoDetalles
                        .Where(x =>
                            !idsRecibidos.Contains(
                                x.MantenimientoDetalleId))
                        .ToList();

                if (detallesEliminar.Any())
                {
                    _context.MantenimientoDetalles
                        .RemoveRange(detallesEliminar);
                }

                // =========================================
                // ACTUALIZAR DETALLES
                // =========================================

                foreach (var servicio in dto.Servicios)
                {
                    if (servicio.Cantidad <= 0)
                    {
                        throw new Exception(
                            "WARNING|La cantidad debe ser mayor a cero.");
                    }

                    var subtotalManoObra =
                        servicio.Cantidad *
                        servicio.CostoUnitarioManoObra;

                    var subtotalRefacciones =
                        servicio.Cantidad *
                        servicio.CostoUnitarioRefacciones;

                    var totalLinea =
                        subtotalManoObra +
                        subtotalRefacciones;

                    if (!servicio.MantenimientoDetalleId.HasValue)
                    {
                        var nuevoDetalle =
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
                                    subtotalManoObra,

                                SubtotalRefacciones =
                                    subtotalRefacciones,

                                TotalLinea =
                                    totalLinea,

                                Observaciones =
                                    servicio.Observaciones,

                                IdObjetoGastoManoObra =
                                    servicio.IdObjetoGastoManoObra,

                                IdObjetoGastoRefacciones =
                                    servicio.IdObjetoGastoRefacciones,

                                FechaCreacion =
                                    DateTime.Now
                            };

                        _context.MantenimientoDetalles
                            .Add(nuevoDetalle);

                        continue;
                    }

                    var detalleExistente =
                        mantenimiento.MantenimientoDetalles
                            .FirstOrDefault(x =>
                                x.MantenimientoDetalleId ==
                                servicio.MantenimientoDetalleId);

                    if (detalleExistente == null)
                    {
                        continue;
                    }

                    detalleExistente.Cantidad =
                        servicio.Cantidad;

                    detalleExistente.CostoUnitarioManoObra =
                        servicio.CostoUnitarioManoObra;

                    detalleExistente.CostoUnitarioRefacciones =
                        servicio.CostoUnitarioRefacciones;

                    detalleExistente.SubtotalManoObra =
                        subtotalManoObra;

                    detalleExistente.SubtotalRefacciones =
                        subtotalRefacciones;

                    detalleExistente.TotalLinea =
                        totalLinea;

                    detalleExistente.Observaciones =
                        servicio.Observaciones;

                    detalleExistente.IdObjetoGastoManoObra =
                        servicio.IdObjetoGastoManoObra;

                    detalleExistente.IdObjetoGastoRefacciones =
                        servicio.IdObjetoGastoRefacciones;
                }

                await _context.SaveChangesAsync();

                // =========================================
                // RECALCULAR TOTAL
                // =========================================

                mantenimiento.CostoRealTotal =
                    await _context.MantenimientoDetalles
                        .Where(x =>
                            x.MantenimientoId ==
                            mantenimiento.MantenimientoId)
                        .SumAsync(x =>
                            (decimal?)x.TotalLinea) ?? 0;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }




        private async Task<MantenimientoEditViewModel>
    ObtenerMantenimientoEditarPorMantenimientoId(
        int mantenimientoId)
        {
            var mantenimiento =
                await _context.Mantenimientos
                    .Include(x => x.UnidadVehicular)
                    .Include(x => x.Taller)
                    .Include(x => x.TipoMantenimiento)
                    .Include(x => x.SolicitudMantenimiento)
                    .FirstOrDefaultAsync(x =>
                        x.MantenimientoId ==
                        mantenimientoId);

            if (mantenimiento == null)
            {
                throw new Exception(
                    "Mantenimiento no encontrado.");
            }

            var detalles =
                await _context.MantenimientoDetalles
                    .Include(x => x.ConceptoServicio)
                    .Where(x =>
                        x.MantenimientoId ==
                        mantenimientoId)
                    .ToListAsync();

            if (!detalles.Any())
            {
                throw new Exception(
                    $"No existen detalles para el mantenimiento {mantenimientoId}");
            }

            // =====================================
            // OBTENER OBJETOS DE GASTO
            // =====================================

            var idsObjetoGasto =
                detalles
                    .SelectMany(x => new[]
                    {
                x.IdObjetoGastoManoObra,
                x.IdObjetoGastoRefacciones
                    })
                    .Where(x => x.HasValue)
                    .Select(x => x!.Value)
                    .Distinct()
                    .ToList();

            var objetosGasto =
                await _context.ObjetoGastos
                    .Where(x =>
                        idsObjetoGasto.Contains(
                            x.IdObjetoGasto))
                    .ToDictionaryAsync(
                        x => x.IdObjetoGasto,
                        x => $"{x.ClaveObjGasto} - {x.Descripcion}");

            return new MantenimientoEditViewModel
            {
                MantenimientoId =
                    mantenimiento.MantenimientoId,

                Folio =
                    mantenimiento.SolicitudMantenimiento?.Folio
                    ?? string.Empty,

                NumeroOficio =
                    mantenimiento.SolicitudMantenimiento?.NumeroOficio
                    ?? string.Empty,

                SolicitudMantenimientoId =
                    mantenimiento.SolicitudMantenimientoId ?? 0,

                UnidadVehicularId =
                    mantenimiento.UnidadVehicularId,

                TallerId =
                    mantenimiento.TallerId,

                TipoMantenimientoId =
                    mantenimiento.TipoMantenimientoId,

                FechaIngreso =
                    mantenimiento.FechaIngreso,

                FechaSalida =
                    mantenimiento.FechaSalida ?? DateTime.MinValue,

                FechaInicioTrabajo =
                    mantenimiento.FechaInicioTrabajo,

                FechaFinTrabajo =
                    mantenimiento.FechaFinTrabajo,

                KilometrajeEntrada =
                    mantenimiento.KilometrajeEntrada,

                KilometrajeSalida =
                    mantenimiento.KilometrajeSalida,

                MotivoMantenimiento =
                    mantenimiento.MotivoMantenimiento,

                Diagnostico =
                    mantenimiento.Diagnostico,

                TrabajoRealizado =
                    mantenimiento.TrabajoRealizado,

                Observaciones =
                    mantenimiento.Observaciones,

                EstatusMantenimientoId =
                    mantenimiento.EstatusMantenimientoId ?? 0,

                CostoRealTotal =
                    mantenimiento.CostoRealTotal,

                UnidadVehicularTexto =
                    $"{mantenimiento.UnidadVehicular.NumeroEconomico} - {mantenimiento.UnidadVehicular.PlacaActual}",

                TallerTexto =
                    mantenimiento.Taller.RazonSocial,

                TipoMantenimientoTexto =
                    mantenimiento.TipoMantenimiento.Nombre,

                Servicios =
                    detalles.Select(d =>
                        new MantenimientoDetalleViewModel
                        {
                            MantenimientoDetalleId =
                                d.MantenimientoDetalleId,

                            MantenimientoId =
                                d.MantenimientoId,

                            ConceptoServicioId =
                                d.ConceptoServicioId,

                            ConceptoServicio =
                                d.ConceptoServicio.ConceptoServicioNombre,

                            Cantidad =
                                Convert.ToInt32(d.Cantidad),

                            CostoUnitarioManoObra =
                                d.CostoUnitarioManoObra,

                            CostoUnitarioRefacciones =
                                d.CostoUnitarioRefacciones,

                            SubtotalManoObra =
                                d.SubtotalManoObra ?? 0m,

                            SubtotalRefacciones =
                                d.SubtotalRefacciones ?? 0m,

                            TotalLinea =
                                (d.SubtotalManoObra ?? 0m)
                                +
                                (d.SubtotalRefacciones ?? 0m),

                            Observaciones =
                                d.Observaciones,

                            IdObjetoGastoManoObra =
                                d.IdObjetoGastoManoObra,

                            IdObjetoGastoRefacciones =
                                d.IdObjetoGastoRefacciones,

                            ObjetoGastoManoObraTexto =
                                d.IdObjetoGastoManoObra.HasValue &&
                                objetosGasto.ContainsKey(
                                    d.IdObjetoGastoManoObra.Value)
                                ? objetosGasto[
                                    d.IdObjetoGastoManoObra.Value]
                                : string.Empty,

                            ObjetoGastoRefaccionesTexto =
                                d.IdObjetoGastoRefacciones.HasValue &&
                                objetosGasto.ContainsKey(
                                    d.IdObjetoGastoRefacciones.Value)
                                ? objetosGasto[
                                    d.IdObjetoGastoRefacciones.Value]
                                : string.Empty
                        })
                    .ToList()
            };
        }







        public async Task FinalizarMantenimiento(int mantenimientoId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var mantenimiento =
                    await _context.Mantenimientos
                        .Include(x => x.MantenimientoDetalles)
                        .FirstOrDefaultAsync(x =>
                            x.MantenimientoId == mantenimientoId);

                if (mantenimiento == null)
                {
                    throw new Exception(
                        "WARNING|El mantenimiento no existe.");
                }

                const int ESTATUS_EN_PROCESO = 1;

                if (mantenimiento.EstatusMantenimientoId !=
                    ESTATUS_EN_PROCESO)
                {
                    throw new Exception(
                        "WARNING|Solo pueden finalizarse mantenimientos en proceso.");
                }

                if (!mantenimiento.FechaInicioTrabajo.HasValue)
                {
                    throw new Exception(
                        "WARNING|Debe indicar la fecha de inicio del trabajo.");
                }

                if (!mantenimiento.FechaFinTrabajo.HasValue)
                {
                    throw new Exception(
                        "WARNING|Debe indicar la fecha de finalización.");
                }

                if (mantenimiento.FechaFinTrabajo <
                    mantenimiento.FechaInicioTrabajo)
                {
                    throw new Exception(
                        "WARNING|La fecha de fin no puede ser menor a la fecha de inicio.");
                }

                if (string.IsNullOrWhiteSpace(
                    mantenimiento.TrabajoRealizado))
                {
                    throw new Exception(
                        "WARNING|Debe capturar el trabajo realizado.");
                }

                if (!mantenimiento.MantenimientoDetalles.Any())
                {
                    throw new Exception(
                        "WARNING|El mantenimiento no contiene servicios.");
                }

                mantenimiento.CostoRealTotal =
                    mantenimiento.MantenimientoDetalles
                        .Sum(x => x.TotalLinea ?? 0);

                if (mantenimiento.CostoRealTotal <= 0)
                {
                    throw new Exception(
                        "WARNING|El costo total debe ser mayor a cero.");
                }

                const int ESTATUS_FINALIZADO = 2;

                mantenimiento.EstatusMantenimientoId =
                    ESTATUS_FINALIZADO;

                mantenimiento.FechaSalida ??=
                    DateTime.Now;

                await _context.SaveChangesAsync();

                // =====================================
                // ACTUALIZAR SOLICITUD
                // =====================================

                if (mantenimiento.SolicitudMantenimientoId.HasValue)
                {
                    var solicitud =
                        await _context.SolicitudMantenimientos
                            .FirstOrDefaultAsync(x =>
                                x.SolicitudMantenimientoId ==
                                mantenimiento.SolicitudMantenimientoId);

                    if (solicitud != null)
                    {
                        const int ESTATUS_SOLICITUD_FINALIZADA = 8;

                        solicitud.EstatusSolicitudMantenimientoId =
                            ESTATUS_SOLICITUD_FINALIZADA;

                        _context
                            .SolicitudMantenimientoSeguimientos
                            .Add(
                                new SolicitudMantenimientoSeguimiento
                                {
                                    SolicitudMantenimientoId =
                                        solicitud.SolicitudMantenimientoId,

                                    EstatusSolicitudMantenimientoId =
                                        ESTATUS_SOLICITUD_FINALIZADA,

                                    Observaciones =
                                        "Mantenimiento finalizado.",

                                    UsuarioId = 1,

                                    FechaMovimiento =
                                        DateTime.Now
                                });
                    }
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

                            UsuarioCaptura =
                                "ADMIN",

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

                        Observaciones =
                            observaciones,

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



        public async Task<ResultadoValidacionHistoricaDto> ValidarServiciosHistoricos(int unidadVehicularId, List<int> conceptos, int tipoMantenimientoId)
        {
            var resultado = new ResultadoValidacionHistoricaDto();

            var fechaActual = DateTime.Now;

            // =====================================================
            // CONFIGURACIÓN
            // =====================================================

            var configuraciones =
                await ObtenerConfiguracionValidacion(conceptos);

            if (!configuraciones.Any())
                return resultado;

            var configIds =
                configuraciones
                    .Select(x => x.ConfiguracionValidacionTecnicaId)
                    .Distinct()
                    .ToList();

            var estatusPermitidos =
                await _context.ConfiguracionValidacionEstatuses
                    .AsNoTracking()
                    .Where(x =>
                        x.Activo &&
                        configIds.Contains(x.ConfiguracionValidacionTecnicaId))
                    .ToListAsync();

            // SOLICITUDES ABIERTAS (BLOQUEANTE)

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
                    select new
                    {
                        s.SolicitudMantenimientoId,
                        s.Folio,
                        s.EstatusSolicitudMantenimientoId,
                        d.ConceptoServicioId,
                        c.TipoCatalogoServicioId,
                        c.ConceptoServicioNombre
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            var duplicados =
                solicitudesActivas
                    .Where(x =>
                    {
                        var regla =
                            configuraciones.FirstOrDefault(r =>
                                r.ConceptoServicioId == x.ConceptoServicioId &&
                                r.TipoCatalogoServicioId == x.TipoCatalogoServicioId &&
                                r.ValidarSolicitudesAbiertas);

                        if (regla == null)
                            return false;

                        var estatusAplicables =
                            estatusPermitidos
                                .Where(e =>
                                    e.ConfiguracionValidacionTecnicaId ==
                                    regla.ConfiguracionValidacionTecnicaId)
                                .Select(e => e.EstatusSolicitudMantenimientoId)
                                .ToList();

                        return estatusAplicables.Contains(x.EstatusSolicitudMantenimientoId);
                    })
                    .ToList();

            if (duplicados.Any())
            {
                resultado.TieneDuplicadosActivos = true;

                foreach (var x in duplicados)
                {
                    resultado.Mensajes.Add(
                        $"El servicio '{x.ConceptoServicioNombre}' ya existe en la solicitud {x.Folio}.");
                }
            }

            // HISTORIAL (RECURRENCIA - SOLO ALERTA)

            var historial =
                await (
                    from m in _context.Mantenimientos
                    join md in _context.MantenimientoDetalles
                        on m.MantenimientoId equals md.MantenimientoId
                    join c in _context.ConceptoServicios
                        on md.ConceptoServicioId equals c.ConceptoServicioId
                    where m.Activo
                        && m.FechaEjecucion != null
                        && m.UnidadVehicularId == unidadVehicularId
                        && conceptos.Contains(md.ConceptoServicioId)
                    select new
                    {
                        md.ConceptoServicioId,
                        c.TipoCatalogoServicioId,
                        c.ConceptoServicioNombre,
                        Fecha = m.FechaEjecucion.Value
                    }
                )
                .AsNoTracking()
                .ToListAsync();

            var historialReciente =
                      historial
                          .Where(x =>
                          {
                              var regla =
                                  configuraciones.FirstOrDefault(r =>
                                      r.ConceptoServicioId == x.ConceptoServicioId &&
                                      r.TipoCatalogoServicioId == x.TipoCatalogoServicioId &&
                                      r.ValidarRecurrencia);

                              if (regla == null)
                                  return false;

                              if (regla.DiasRecurrencia == null || regla.DiasRecurrencia <= 0)
                                  return false;

                              var fechaMinima = fechaActual.AddDays(-regla.DiasRecurrencia);

                              return x.Fecha >= fechaMinima;
                          })
                          .ToList();




            if (historialReciente.Any())
            {
                resultado.TieneRecurrencia = true;

                foreach (var x in historialReciente)
                {
                    var regla =
                        configuraciones.First(r =>
                            r.ConceptoServicioId == x.ConceptoServicioId &&
                            r.TipoCatalogoServicioId == x.TipoCatalogoServicioId);

                    resultado.Mensajes.Add(
                        $"El servicio '{x.ConceptoServicioNombre}' ya fue realizado el {x.Fecha:dd/MM/yyyy}. Deben transcurrir al menos {regla.DiasRecurrencia} días.");
                }
            }

            // =====================================================
            // RESULTADO FINAL
            // =====================================================

            resultado.EsValido =
                !resultado.TieneDuplicadosActivos &&
                !resultado.TieneRecurrencia;

            return resultado;
        }






    }
}











