using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class OdpService : IOdpService
    {

        private readonly BdSigeMavContext _context;

        public OdpService(BdSigeMavContext context)
        {
            _context = context;
        }



        public async Task<List<OrdenPagoVM>> Obtener()
        {
            return await _context.OrdenPagos
                .Select(x => new OrdenPagoVM
                {
                    IdOrdenPago = x.IdOrdenPago,

                    NumeroOrden = x.NumeroOrden,

                    Fecha = x.Fecha,

                    ImporteTotal = x.ImporteTotal,

                    Estatus = x.Estatus,

                    IdProovedor = x.ProovedorId,

                    RFC = x.Proovedor.Rfc,

                    NombreProveedor = x.Proovedor.RazonSocial,




                })
                .ToListAsync();
        }



        public async Task<OrdenPagoVM> ObtenerPorId(int id)
        {
            var entity = await _context.OrdenPagos
                    .Include(x => x.OrdenPagoDetalles)
                    .FirstOrDefaultAsync(x =>
                        x.IdOrdenPago == id);

            if (entity == null)
            {
                return null;
            }

            OrdenPagoVM model = new()
            {
                IdOrdenPago = entity.IdOrdenPago,

                NumeroOrden = entity.NumeroOrden,

                Fecha = entity.Fecha,

                IdProovedor = entity.ProovedorId,

                ImporteTotal = entity.ImporteTotal,

                Estatus = entity.Estatus,


                Detalles = entity.OrdenPagoDetalles
        .Select(d => new OrdenPagoDetalleVM
        {
            Concepto = d.Concepto,

            IdPartida = d.IdPartida,

            Serie = d.Serie,

            Folio = d.Folio,

            FechaFactura = d.FechaFactura,

            NoPlaca = d.NoPlaca,

            Monto = d.Importe
        })
        .ToList()

            };


            // CARGAR CATALOGOS

            model = await InicializarFormulario(model);
            return model;
        }



        public async Task<OrdenPagoVM> InicializarFormulario(OrdenPagoVM? model = null)
        {
            model ??= new OrdenPagoVM
            {
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                Estatus = "ACTIVA"
            };


            // PROVEEDORES


            model.ListaProveedores = await _context.Tallers
                    .Where(x => x.Activo == true)
                    .Select(x => new SelectListItem
                    {
                        Value = x.TallerId.ToString(),
                        Text = x.RazonSocial
                    })
                    .ToListAsync();


            // PARTIDAS


            model.Partidas = await _context.Partida
                    .Where(x => x.Activo == true)
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdPartida.ToString(),
                        Text = x.ClavePartida + " - " + x.Partida
                    })
                    .ToListAsync();




            model.DescripPartida = await _context.Partida
          .Select(x => new SelectListItem
          {
              Value = x.Descripcion,
              Text = x.Descripcion
          })
          .ToListAsync();

            return model;

        }





        public async Task<bool> Guardar(OrdenPagoVM model)
        {

            if (model.Detalles == null || !model.Detalles.Any())
            {
                return false;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                OrdenPago entity = new()
                {
                    NumeroOrden = model.NumeroOrden,
                    Fecha = model.Fecha,
                    ProovedorId = model.IdProovedor,
                    ImporteTotal = model.ImporteTotal,
                    Estatus = "ACTIVA",
                    FechaCaptura = DateTime.Now,
                    UsuarioCaptura = "AdminFinancieros"
                };

                _context.OrdenPagos.Add(entity);

                await _context.SaveChangesAsync();

                //Detalle ODP

                if (model.Detalles != null &&
                    model.Detalles.Any())
                {
                    foreach (var item in model.Detalles)
                    {
                        OrdenPagoDetalle detalle = new()
                        {
                            IdOrdenPago = entity.IdOrdenPago,

                            Concepto = item.Concepto,

                            IdPartida = item.IdPartida.Value,

                            Serie = item.Serie,

                            Folio = item.Folio,

                            FechaFactura = item.FechaFactura ?? DateOnly.FromDateTime(DateTime.Now),

                            NoPlaca = item.NoPlaca,

                            Importe = item.Monto ?? 0
                        };

                        _context.OrdenPagoDetalles
                            .Add(detalle);
                    }

                    await _context.SaveChangesAsync();
                }


                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw new Exception(
                    ex.InnerException?.Message ?? ex.Message);
            }
        }





        public async Task<bool> Editar(OrdenPagoVM model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                if (model.Detalles == null ||
                    !model.Detalles.Any())
                {
                    return false;
                }


                // OBTENER ORDEN


                var entity =
                    await _context.OrdenPagos
                        .Include(x => x.OrdenPagoDetalles)
                        .FirstOrDefaultAsync(x => x.IdOrdenPago == model.IdOrdenPago);

                if (entity == null)
                {
                    return false;
                }


                entity.NumeroOrden = model.NumeroOrden;

                entity.Fecha = model.Fecha;

                entity.ProovedorId = model.IdProovedor;

                entity.ImporteTotal = model.ImporteTotal;

                entity.Estatus = model.Estatus;


                // ELIMINAR DETALLES


                if (entity.OrdenPagoDetalles.Any())
                {
                    _context.OrdenPagoDetalles
                        .RemoveRange(entity.OrdenPagoDetalles);
                }


                // AGREGAR NUEVOS DETALLES

                foreach (var item in model.Detalles)
                {
                    OrdenPagoDetalle detalle = new()
                    {
                        IdOrdenPago = entity.IdOrdenPago,
                        Concepto = item.Concepto,
                        IdPartida = item.IdPartida.Value,
                        Serie = item.Serie,
                        Folio = item.Folio,
                        FechaFactura = (DateOnly)item.FechaFactura,
                        NoPlaca = item.NoPlaca,
                        Importe = item.Monto ?? 0
                    };

                    _context.OrdenPagoDetalles.Add(detalle);
                }


                await _context.SaveChangesAsync();

                // COMMIT               

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();

                return false;
            }
        }


        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var orden = await _context.OrdenPagos
                    .FirstOrDefaultAsync(x => x.IdOrdenPago == id);

                if (orden == null)
                    return false;

                _context.OrdenPagos.Remove(orden);

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> Cancelar(int id)
        {
            try
            {
                var entity =
                    await _context.OrdenPagos
                        .FirstOrDefaultAsync(x =>
                            x.IdOrdenPago == id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estatus = "CANCELADA";

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> Reactivar(int id)
        {
            try
            {
                var entity =
                    await _context.OrdenPagos
                        .FirstOrDefaultAsync(x =>
                            x.IdOrdenPago == id);

                if (entity == null)
                {
                    return false;
                }

                entity.Estatus = "ACTIVA";

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }



        public async Task<byte[]> GenerarExcel(int id)
        {
            var entity = await _context.OrdenPagos

        .Include(x => x.Proovedor)
        .Include(x => x.OrdenPagoDetalles)
        .ThenInclude(x => x.IdPartidaNavigation)
        .FirstOrDefaultAsync(x =>
            x.IdOrdenPago == id);

            if (entity == null)
            {
                return null;
            }


            string rutaPlantilla = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Formatos", "OrdenPago.xlsx");


            // ABRIR EXCEL


            using var workbook = new XLWorkbook(rutaPlantilla);

            var ws = workbook.Worksheet(1);

            ws.Cell("J4")
                .Value = entity.NumeroOrden;

            ws.Cell("H4")
                .Value =
                    entity.Fecha?
                        .ToString("dd/MM/yyyy");

            ws.Cell("C8")
                .Value =
                    entity.Proovedor?
                        .RazonSocial;


            ws.Cell("N30")
                .Value =
                    entity.ImporteTotal;



            var partidasAgrupadas = entity.OrdenPagoDetalles
                    .GroupBy(x => new
                    {
                        x.IdPartida,

                        ClavePartida =
                            x.IdPartidaNavigation
                                .ClavePartida,

                        NombrePartida =
                            x.IdPartidaNavigation
                                .Partida
                    })

                    .Select(g => new
                    {
                        Cuenta =
                            g.Key.ClavePartida,

                        NombrePartida =
                            g.Key.NombrePartida,

                        Total =
                            g.Sum(x => x.Importe),

                        Conceptos =
                            g.ToList()
                    })

                    .ToList();

       

            int filaInicio = 21;

          
            foreach (var grupo in partidasAgrupadas)
            {
                

                ws.Row(filaInicio)
                    .Height = 26;

                // CUENTA CONTABLE

                ws.Cell($"A{filaInicio}")
                    .Value =
                        grupo.Cuenta;

                // DESCRIPCION PARTIDA

                ws.Cell($"C{filaInicio}")
                    .Value =
                        grupo.NombrePartida;

                // IMPORTE TOTAL PARTIDA

                ws.Cell($"N{filaInicio}")
                    .Value =
                        grupo.Total;

                // ESTILO

                ws.Range($"A{filaInicio}:N{filaInicio}")
                    .Style.Font.Bold = true;

                ws.Range($"A{filaInicio}:N{filaInicio}")
                    .Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                

                filaInicio++;

                
                // RECORRER DETALLES
                // =========================================

                foreach (var item in grupo.Conceptos)
                {
                    // =========================================
                    // COPIAR FORMATO DE FILA
                    // =========================================

                    ws.Row(filaInicio)
                        .Height = 22;

                    // =========================================
                    // DESCRIPCION / CONCEPTO
                    // =========================================

                    ws.Range($"C{filaInicio}:D{filaInicio}")
    .Merge();

                    ws.Cell($"C{filaInicio}")
                        .Value = item.Concepto;
                    // =========================================
                    // FACTURA
                    // =========================================

                    ws.Cell($"E{filaInicio}")
                        .Value =
                            $"{item.Serie}{item.Folio}";

                    // =========================================
                    // FECHA FACTURA
                    // =========================================

                    ws.Cell($"F{filaInicio}")
                        .Value =
                            item.FechaFactura
                                .ToString("dd/MM/yyyy");

                    // =========================================
                    // NUMERO PLACA
                    // =========================================

                    ws.Cell($"G{filaInicio}")
                        .Value =
                            item.NoPlaca;

                    // =========================================
                    // RFC
                    // =========================================

                    ws.Cell($"I{filaInicio}")
                        .Value =
                            entity.Proovedor?.Rfc;

                    // =========================================
                    // PROVEEDOR
                    // =========================================

                    ws.Cell($"K{filaInicio}")
                        .Value =
                            entity.Proovedor?
                                .RazonSocial;

                    // =========================================
                    // PARCIAL
                    // =========================================

                    ws.Cell($"M{filaInicio}")
                        .Value =
                            item.Importe;

                    // =========================================
                    // AJUSTAR TEXTO
                    // =========================================

                    ws.Range($"C{filaInicio}:F{filaInicio}").Merge();

                    ws.Cell($"C{filaInicio}")
                        .Value = grupo.NombrePartida;

                    // =========================================
                    // ALINEACION
                    // =========================================

                    ws.Range($"A{filaInicio}:N{filaInicio}")
                        .Style.Alignment.Vertical =
                            XLAlignmentVerticalValues.Center;

                    filaInicio++;
                }

                // =========================================
                // ESPACIO ENTRE PARTIDAS
                // =========================================

                filaInicio += 2;
            }

            // =========================================
            // FORMATO MONEDA
            // =========================================

            ws.Range("M1:N500")
                .Style
                .NumberFormat
                .Format = "$ #,##0.00";

            // =========================================
            // CONFIGURACION IMPRESION
            // =========================================

            ws.PageSetup.PageOrientation =
                XLPageOrientation.Portrait;

            ws.PageSetup.FitToPages(1, 0);

            ws.PageSetup.CenterHorizontally = true;

            ws.PageSetup.Margins.Top = 0.3;
            ws.PageSetup.Margins.Bottom = 0.3;
            ws.PageSetup.Margins.Left = 0.2;
            ws.PageSetup.Margins.Right = 0.2;

            // =========================================
            // EXPORTAR
            // =========================================

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }

}



