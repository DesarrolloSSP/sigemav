using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Models.ViewModels.OrdeDePago;
using SIGEMAV.Models.ViewModels.OrdenDePago;
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



        //public async Task<List<OrdenDePagoVM>> Obtener(string numeroOficio)
        //{



        //    var resultado = await _context.OrdenDePagoResults
        //        .FromSqlInterpolated(
        //            $"EXEC dbo.usp_OrdenDePago @folio={numeroOficio}")
        //        .AsNoTracking()
        //        .ToListAsync();

        //    return resultado.Select(x => new OrdenDePagoVM
        //    {
        //        NumeroOficio = x.NumeroOficio,
        //        Partida = x.Partida,

        //        FacturaMantenimientoId = x.FacturaMantenimientoId,
        //        FacturaDetalleId = x.FacturaDetalleId,
        //        MantenimientoDetalleId = x.MantenimientoDetalleId,

        //        UUID = x.UUID,
        //        Serie = x.Serie,
        //        Folio = x.Folio,
        //        FechaFactura = x.FechaFactura,

        //        RFCEmisor = x.RFCEmisor,
        //        RazonSocial = x.RazonSocial,
        //        ClabeInterbancaria = x.ClabeInterbancaria,
        //        PlacaActual = x.PlacaActual,
        //        ConceptoServicioNombre = x.ConceptoServicioNombre,

        //        IdObjetoGasto = x.IdObjetoGasto,
        //        ClaveObjGasto = x.ClaveObjGasto,
        //        Descripcion = x.Descripcion,

        //        Origen = x.Origen,
        //        Importe = x.Importe,
        //        Cantidad = x.Cantidad

        //    }).ToList();
        //}

        public async Task<List<OrdenDePagoVM>> Obtener(string numeroOficio)
        {
            var resultado = await _context.Database
                .SqlQuery<OrdenDePagoResult>(
                    $"EXEC dbo.usp_OrdenDePago @folio={numeroOficio}")
                .ToListAsync();

            return resultado.Select(x => new OrdenDePagoVM
            {
                NumeroOficio = x.NumeroOficio,
                Partida = x.Partida,

                FacturaMantenimientoId = x.FacturaMantenimientoId,
                FacturaDetalleId = x.FacturaDetalleId,
                MantenimientoDetalleId = x.MantenimientoDetalleId,

                UUID = x.UUID,
                Serie = x.Serie,
                Folio = x.Folio,
                FechaFactura = x.FechaFactura,

                RFCEmisor = x.RFCEmisor,
                RazonSocial = x.RazonSocial,
                ClabeInterbancaria = x.ClabeInterbancaria,
                PlacaActual = x.PlacaActual,
                ConceptoServicioNombre = x.ConceptoServicioNombre,

                IdObjetoGasto = x.IdObjetoGasto,
                ClaveObjGasto = x.ClaveObjGasto,
                Descripcion = x.Descripcion,

                Origen = x.Origen,
                Importe = x.Importe,
                Cantidad = x.Cantidad

            }).ToList();
        }




        public async Task<byte[]?> GenerarExcel(string folio)
        {
           
            var encabezado = await ObtenerEncabezado(folio);

            var detalles = await Obtener(folio);

            if (encabezado == null || detalles == null || !detalles.Any())
                return null;

            // =====================================================
            // CARGAR PLANTILLA
            // =====================================================

            string rutaPlantilla = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Formatos",
                "OrdenPago.xlsx");

            using var workbook = new XLWorkbook(rutaPlantilla);

            var ws = workbook.Worksheet(1);

            // =====================================================
            // ENCABEZADO GENERAL
            // =====================================================

            ws.Cell("J4").Value = folio;
            ws.Range("J4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("J4").Style.Font.FontSize = 9;

            // Fecha de factura
            var fechasFactura = detalles
                .Select(x => x.FechaFactura.Date)
                .Distinct()
                .ToList();

            if (fechasFactura.Count == 1)
            {
                ws.Cell("H4").Value = fechasFactura.First();

                ws.Cell("H4").Style.DateFormat.Format = "dd/MM/yyyy";
            }
            else
            {
                ws.Cell("H4").Value = "VARIAS";
            }

            // =====================================================
            // DATOS DEL ENCABEZADO DE LA ORDEN
            // =====================================================

            // Proveedor y CLABE
            ws.Cell("B9").Value =
                encabezado.RazonSocial +
                Environment.NewLine +
                "CLABE: " + encabezado.ClabeInterbancaria;

            ws.Range("B9:B12").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Range("B9:B12").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Range("B9:B12").Style.Alignment.WrapText = true;



            ws.Cell("A4").Value = encabezado.AreaSolicitante;

            ws.Cell("B8").Value = encabezado.ClaveProyecto;
            ws.Range("C14:K14").Style.Font.FontSize = 10;

            ws.Cell("C8").Value = encabezado.AreaSolicitante;
            ws.Range("C8:K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            ws.Cell("E30").Value = encabezado.Contrato;
            ws.Range("C14:K14").Style.Font.FontSize = 10;
            //ws.Range("I43:J43").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



            // =====================================================
            // TOTAL GENERAL DEL ENCABEZADO
            // =====================================================

            decimal totalGeneral = detalles.Sum(x => x.Importe);

            // Total numérico
            ws.Cell("B14").Value = totalGeneral;

            ws.Cell("B14").Style.NumberFormat.Format = "$#,##0.00";

            // Total en letra
            ws.Range("C14:K14").Merge();

            ws.Cell("C14").Value = ImporteEnLetras(totalGeneral);

            ws.Range("C14:K14").Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            ws.Range("C14:K14").Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            ws.Range("C14:K14").Style.Alignment.WrapText = true;

            // =====================================================
            // AGRUPAR POR OBJETO DE GASTO
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
            // FILAS 
            // =====================================================

            int filasRequeridas = 0;

            foreach (var grupo in grupos)
            {
                filasRequeridas += 1;             // Cabecera
                filasRequeridas += grupo.Count(); // Detalles
                filasRequeridas += 1;             // Subtotal
                filasRequeridas += 1;             // Espacio
            }

          
            int filasDisponibles = 7;

            int filasExtras = Math.Max(
                0,
                filasRequeridas - filasDisponibles);

            // =====================================================
            // INSERTAR FILAS ANTES DE DSP
            // =====================================================

            if (filasExtras > 0)
            {
                ws.Row(29).InsertRowsAbove(filasExtras);

                for (int i = 0; i < filasExtras; i++)
                {
                    int nuevaFila = 29 + i;

                    // Copiar formato de una fila de detalle
                    ws.Row(28).CopyTo(ws.Row(nuevaFila));

                    ws.Row(nuevaFila).Clear(XLClearOptions.Contents);

                   
                    ws.Range($"A{nuevaFila}:B{nuevaFila}").Merge();
                    ws.Range($"C{nuevaFila}:D{nuevaFila}").Merge();
                    ws.Range($"I{nuevaFila}:J{nuevaFila}").Merge();
                    ws.Range($"K{nuevaFila}:O{nuevaFila}").Merge();
                    ws.Range($"Q{nuevaFila}:R{nuevaFila}").Merge();
                }
            }

            // =====================================================
            // GENERAR DETALLE
            // =====================================================

            int fila = 22;

            foreach (var grupo in grupos)
            {
                // ---------------------------------------
                // CABECERA OBJETO DE GASTO
                // ---------------------------------------

                ws.Range($"A{fila}:B{fila}").Merge();

                ws.Cell($"A{fila}").Value = grupo.Key.ClaveObjGasto;

                // Ajustar combinación para la descripción
                ws.Range($"C{fila}:D{fila}").Unmerge();

                ws.Range($"C{fila}:H{fila}").Merge();

                ws.Cell($"C{fila}").Value = grupo.Key.Descripcion;

                ws.Range($"A{fila}:H{fila}").Style.Font.Bold = true;

                ws.Range($"C{fila}:H{fila}").Style.Alignment.WrapText = true;

                ws.Row(fila).Height = 35;

                fila++;

                // ---------------------------------------
                // DETALLES DE FACTURAS
                // ---------------------------------------

                foreach (var item in grupo)
                {
                    // Categoría
                    ws.Cell($"C{fila}").Value = item.Partida;

                    // SERIE
                    ws.Cell($"E{fila}").Value = item.Serie;

                    // FOLIO
                    ws.Cell($"F{fila}").Value = item.Folio;

                    // FECHA DE FACTURA
                    ws.Cell($"G{fila}").Value = item.FechaFactura;

                    ws.Cell($"G{fila}").Style.DateFormat.Format =
                        "dd/MM/yyyy";

                    // PLACA
                    ws.Cell($"H{fila}").Value = item.PlacaActual;

                    // RFC
                    ws.Cell($"I{fila}").Value = item.RFCEmisor;

                    // PROVEEDOR
                    ws.Cell($"K{fila}").Value = item.RazonSocial;

                    ws.Range($"K{fila}:O{fila}")
                        .Style.Alignment.Horizontal =
                            XLAlignmentHorizontalValues.Center;

                    ws.Range($"K{fila}:O{fila}")
                        .Style.Alignment.Vertical =
                            XLAlignmentVerticalValues.Center;

                    // IMPORTE
                    ws.Cell($"Q{fila}").Value = item.Importe;

                    fila++;
                }

                // ---------------------------------------
                // SUBTOTAL
                // ---------------------------------------

                decimal subtotalGrupo = grupo.Sum(x => x.Importe);

                ws.Cell($"P{fila}").Value = "SUBTOTAL";

                ws.Cell($"Q{fila}").Value = subtotalGrupo;

                ws.Range($"P{fila}:Q{fila}")
                    .Style.Font.Bold = true;

                fila++;
               
                fila++;
            }

            // =====================================================
            // TOTAL GENERAL DEL DETALLE
            // =====================================================

            ws.Cell($"P{fila}").Value = "TOTAL";

            ws.Cell($"Q{fila}").Value = totalGeneral;

            ws.Range($"P{fila}:Q{fila}")
                .Style.Font.Bold = true;

            // =====================================================
            // FORMATO MONEDA
            // =====================================================

            ws.Range($"Q22:Q{fila}")
                .Style.NumberFormat.Format = "$ #,##0.00";

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




        private static string ImporteEnLetras(decimal importe)
        {
            if (importe < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(importe), "El importe no puede ser negativo.");

            importe = Math.Round(importe, 2, MidpointRounding.AwayFromZero);

            long parteEntera = (long)Math.Floor(importe);
            int centavos = (int)Math.Round(
                (importe - parteEntera) * 100,
                MidpointRounding.AwayFromZero);

            string letras = NumeroALetras(parteEntera);

            string moneda = parteEntera == 1 ? "PESO" : "PESOS";

            return $"{letras} {moneda} {centavos:00}/100 M.N.";
        }

        private static string NumeroALetras(long numero)
        {
            if (numero == 0)
                return "CERO";

            if (numero < 0)
                throw new ArgumentOutOfRangeException(nameof(numero));

            if (numero < 30)
                return Unidades(numero);

            if (numero < 100)
            {
                long decena = numero / 10;
                long unidad = numero % 10;

                string[] decenas =
                {
            "", "", "VEINTE", "TREINTA", "CUARENTA",
            "CINCUENTA", "SESENTA", "SETENTA",
            "OCHENTA", "NOVENTA"
        };

                if (numero < 40 && numero >= 21)
                {
                    return unidad == 0
                        ? "VEINTE"
                        : "VEINTI" + Unidades(unidad).ToLower();
                }

                return unidad == 0
                    ? decenas[decena]
                    : decenas[decena] + " Y " + Unidades(unidad);
            }

            if (numero < 1000)
            {
                long centena = numero / 100;
                long resto = numero % 100;

                string[] centenas =
                {
            "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS",
            "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS",
            "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS"
        };

                if (numero == 100)
                    return "CIEN";

                return resto == 0
                    ? centenas[centena]
                    : centenas[centena] + " " + NumeroALetras(resto);
            }

            if (numero < 1_000_000)
            {
                long miles = numero / 1000;
                long resto = numero % 1000;

                string textoMiles = miles == 1
                    ? "MIL"
                    : NumeroALetras(miles) + " MIL";

                return resto == 0
                    ? textoMiles
                    : textoMiles + " " + NumeroALetras(resto);
            }

            if (numero < 1_000_000_000)
            {
                long millones = numero / 1_000_000;
                long resto = numero % 1_000_000;

                string textoMillones = millones == 1
                    ? "UN MILLON"
                    : NumeroALetras(millones) + " MILLONES";

                return resto == 0
                    ? textoMillones
                    : textoMillones + " " + NumeroALetras(resto);
            }

            long milesMillones = numero / 1_000_000_000;
            long restoFinal = numero % 1_000_000_000;

            string textoMilesMillones = milesMillones == 1
                ? "MIL MILLONES"
                : NumeroALetras(milesMillones) + " MIL MILLONES";

            return restoFinal == 0
                ? textoMilesMillones
                : textoMilesMillones + " " + NumeroALetras(restoFinal);
        }

        private static string Unidades(long numero)
        {
            string[] unidades =
            {
        "CERO", "UNO", "DOS", "TRES", "CUATRO",
        "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE",
        "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE",
        "QUINCE", "DIECISEIS", "DIECISIETE",
        "DIECIOCHO", "DIECINUEVE", "VEINTE",
        "VEINTIUNO", "VEINTIDOS", "VEINTITRES",
        "VEINTICUATRO", "VEINTICINCO", "VEINTISEIS",
        "VEINTISIETE", "VEINTIOCHO", "VEINTINUEVE"
    };

            return unidades[numero];
        }

        public async Task<OrdenDePagoEncabezadoVM?> ObtenerEncabezado(string numeroOficio)
        {
            var resultado = await _context.Database
                .SqlQuery<OrdenDePagoEncabezadoResult>(
                    $"EXEC dbo.usp_OrdenDePagoEncabezado @folio={numeroOficio}")
                .ToListAsync();

            var encabezado = resultado.FirstOrDefault();

            if (encabezado == null)
                return null;

            return new OrdenDePagoEncabezadoVM
            {
                NumeroOficio = encabezado.NumeroOficio,
                RazonSocial = encabezado.RazonSocial,
                ClabeInterbancaria = encabezado.Clabe,
                NoDsp = encabezado.NoDsp,
                ClaveProyecto = encabezado.ClaveProyecto,
                AreaSolicitante = encabezado.AreaNombre,
                Contrato = encabezado.Contrato
            };
        }



    }
}
