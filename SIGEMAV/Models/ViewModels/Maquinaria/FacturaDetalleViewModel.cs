namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class FacturaDetalleViewModel
    {
        public int ConceptoServicioId { get; set; }
        public int FacturaMantenimientoId { get; set; }

        public string ConceptoServicioNombre { get; set; }

        public string UUID { get; set; }

        public string Serie { get; set; }

        public string Folio { get; set; }

        public string Taller { get; set; }

        public string RazonSocial { get; set; }

        public DateOnly? FechaFactura { get; set; }

        public DateOnly? FechaRecepcion { get; set; }

        public string RFCEmisor { get; set; }

        public string RFCReceptor { get; set; }

        public string MetodoPago { get; set; }

        public string FormaPago { get; set; }

        public string UsoCFDI { get; set; }

        public string Moneda { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Retencion { get; set; }

        public decimal Total { get; set; }

        public string Observaciones { get; set; }


        public int Cantidad { get; set; }
        public decimal CostoManoObra {  get; set; }
        public decimal CostoRefacciones { get; set; }


        public List<FacturaDetalleConceptoVm> Conceptos { get; set; }
            = new();

        public List<FacturaDocumentoVm> Documentos { get; set; }
            = new();
    }

    public class FacturaDetalleConceptoVm
    {
        public string Concepto { get; set; }

        public int ConceptoServicioId { get; set; }

        public int Cantidad { get; set; }

        public decimal CostoManoObra { get; set; }

        public decimal CostoRefacciones { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal TotalLinea { get; set; }
    }

    public class FacturaDocumentoVm
    {
        public int FacturaDocumentoId { get; set; }

        public string TipoDocumento { get; set; }

        public string NombreArchivo { get; set; }

        public string RutaArchivo { get; set; }
    }
}
