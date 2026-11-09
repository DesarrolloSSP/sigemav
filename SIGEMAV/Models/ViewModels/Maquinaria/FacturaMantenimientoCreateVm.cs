namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class FacturaMantenimientoCreateVm
    {
        public int SolicitudMantenimientoId { get; set; }

        public int TallerId { get; set; }

        public string FolioFactura { get; set; }

        public string UUIDFiscal { get; set; }

        public DateTime FechaFactura { get; set; }

        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public List<FacturaDetalleViewModel> Detalles { get; set; }

        public IFormFile Xml { get; set; }
        public IFormFile Pdf { get; set; }
    }
}
