namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    //public class FacturaMantenimientoViewModel
    //{
    //    public int SolicitudMantenimientoId { get; set; }
    //    public int MantenimientoId { get; set; }

    //    public int TallerId { get; set; }
    //    public int UnidadVehicularId { get; set; }

    //    public DateTime FechaFactura { get; set; }

    //    public decimal TotalFactura { get; set; }

    //    public IFormFile Xml { get; set; }
    //    public IFormFile Pdf { get; set; }

    //    public List<string> Mensajes { get; set; } = new();
    //    public string Error { get; set; }
    //}



    public class FacturaMantenimientoViewModel
    {
        public int MantenimientoId { get; set; }
        public int TallerId { get; set; }

        public DateTime FechaFactura { get; set; }
        public DateTime FechaRecepcion { get; set; }

        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public List<FacturaDetalleViewModel> Detalles { get; set; }

        public string UUID { get; set; }

        public string Serie { get; set; }

        public string Folio { get; set; }

        public string RFCEmisor { get; set; }

        public string RFCReceptor { get; set; }

        public string RazonSocial { get; set; }

        public string MetodoPago { get; set; }

        public string FormaPago { get; set; }

        public string UsoCFDI { get; set; }

        public string Moneda { get; set; }

        public string TipoComprobante { get; set; }

        public decimal Retencion { get; set; }
        public string? Observaciones { get; set; }
    }


}
