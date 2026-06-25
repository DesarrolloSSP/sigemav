using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Models.ViewModels.OrdeDePago;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class OrdenDePagoService : IOrdenDePago
    {

        private readonly BdSigeMavContext _context;

        public OrdenDePagoService(BdSigeMavContext context)
        {
            _context = context;

        }

        public async Task<List<OrdenDePagoVM>> Obtener(string folio)
        {
            var resultado = await _context.OrdenDePagoResults
                .FromSqlInterpolated(
                    $"EXEC uspOrdenPago {folio}")
                .ToListAsync();

            return resultado
                .Select(x => new OrdenDePagoVM
                {
                    Folio = x.Folio,
                    MantenimientoId = x.MantenimientoId,
                    MantenimientoDetalleId = x.MantenimientoDetalleId,
                    IdObjetoGasto = x.IdObjetoGasto,
                    ClaveObjGasto= x.ClaveObjGasto,
                    Descripcion = x.Descripcion,
                    CostoUnitario = (decimal)x.CostoUnitario,
                    Subtotal = (decimal)x.Subtotal,
                    Tipo = x.Tipo,
                    RFC = x.RFC,
                    RazonSocial = x.RazonSocial,
                    FechaSolicitud = x.FechaSolicitud
                })
                .ToList();
        }







    
        public async Task<byte[]> GenerarExcel(string folio)
        {
            var detalles = await Obtener(folio);

            if (detalles == null || !detalles.Any())
                return null;

            string rutaPlantilla = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Formatos",
                "OrdenPago.xlsx");

            using var workbook = new XLWorkbook(rutaPlantilla);

            var ws = workbook.Worksheet(1);

            // =====================================================
            // ENCABEZADO
            // =====================================================

            ws.Cell("J4").Value = folio;

            if (detalles.First().FechaSolicitud != null)
            {
                ws.Cell("H4").Value = detalles.First().FechaSolicitud;
                ws.Cell("H4").Style.DateFormat.Format = "dd/MM/yyyy";
            }

            // =====================================================
            // AGRUPAR
            // =====================================================

            var grupos = detalles
                .GroupBy(x => new
                {
                    x.ClaveObjGasto,
                    x.Descripcion
                })
                .OrderBy(x => x.Key.ClaveObjGasto)
                .ToList();

            // =====================================================
            // CALCULAR FILAS NECESARIAS
            // =====================================================

            int filasRequeridas = 0;

            foreach (var grupo in grupos)
            {
                filasRequeridas += 1;              // Cabecera
                filasRequeridas += grupo.Count();  // Detalles
                filasRequeridas += 1;              // Subtotal
                filasRequeridas += 1;              // Espacio
            }

            int filasDisponibles = 7;

            int filasExtras =
                Math.Max(0, filasRequeridas - filasDisponibles);

            if (filasExtras > 0)
            {
                // Inserta filas antes de DSP No.
                ws.Row(28).InsertRowsAbove(filasExtras);

                // Copiar formato de la fila detalle
                for (int i = 0; i < filasExtras; i++)
                {
                    ws.Row(27).CopyTo(ws.Row(28 + i));
                }
            }

            // =====================================================
            // DETALLE
            // =====================================================

            int fila = 21;

            foreach (var grupo in grupos)
            {
                // ---------------------------------------
                // CABECERA OBJETO GASTO
                // ---------------------------------------

                //ws.Cell($"A{fila}")
                //    .Value = grupo.Key.ClaveObjGasto;

                //ws.Cell($"C{fila}")
                //    .Value = grupo.Key.Descripcion;

                //ws.Range($"A{fila}:B{fila}")
                //    .Style.Font.Bold = true;

                //ws.Range($"C{fila}:F{fila}")
                //    .Style.Font.Bold = true;

                //fila++;


                ws.Range($"A{fila}:B{fila}").Merge();
                ws.Cell($"A{fila}")
                    .Value = grupo.Key.ClaveObjGasto;

                ws.Range($"C{fila}:H{fila}").Merge();
                ws.Cell($"C{fila}")
                    .Value = grupo.Key.Descripcion;

                ws.Range($"A{fila}:H{fila}")
                    .Style.Font.Bold = true;

                ws.Range($"C{fila}:H{fila}")
                    .Style.Alignment.WrapText = true;

                ws.Row(fila).Height = 35;

                fila++;

                // ---------------------------------------
                // DETALLES
                // ---------------------------------------

                foreach (var item in grupo)
                {
                    ws.Cell($"C{fila}")
                        .Value = item.Tipo;

                    ws.Cell($"I{fila}")
                        .Value = item.RFC;

                    ws.Cell($"K{fila}")
                        .Value = item.RazonSocial;

                    ws.Cell($"Q{fila}")
                        .Value = item.Subtotal ?? 0;

                    fila++;
                }

                // ---------------------------------------
                // SUBTOTAL
                // ---------------------------------------

                decimal subtotalGrupo =
                    grupo.Sum(x => x.Subtotal ?? 0);

                ws.Cell($"P{fila}")
                    .Value = "SUBTOTAL";

                ws.Cell($"Q{fila}")
                    .Value = subtotalGrupo;

                ws.Range($"P{fila}:Q{fila}")
                    .Style.Font.Bold = true;

                fila++;

                // Línea en blanco entre grupos
                fila++;
            }

            // =====================================================
            // TOTAL GENERAL
            // =====================================================

            decimal totalGeneral =
                detalles.Sum(x => x.Subtotal ?? 0);

            ws.Cell($"P{fila}")
                .Value = "TOTAL";

            ws.Cell($"Q{fila}")
                .Value = totalGeneral;

            ws.Range($"P{fila}:Q{fila}")
                .Style.Font.Bold = true;

            // =====================================================
            // FORMATO MONEDA
            // =====================================================

            ws.Range($"Q21:Q{fila}")
                .Style.NumberFormat.Format =
                    "$ #,##0.00";

            // =====================================================
            // IMPRESIÓN
            // =====================================================

            ws.PageSetup.PageOrientation =
                XLPageOrientation.Landscape;

            ws.PageSetup.FitToPages(1, 0);

            ws.PageSetup.CenterHorizontally = true;

            // =====================================================
            // EXPORTAR
            // =====================================================

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }


    }
}
