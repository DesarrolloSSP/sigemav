namespace SIGEMAV.Models.ViewModels.OrdeDePago
{

    public class OrdenDePagoVM
    {
        // Datos de la solicitud
        public string NumeroOficio { get; set; } = string.Empty;

        // Clasificación presupuestal
        public string Partida { get; set; } = string.Empty;

        public int IdObjetoGasto { get; set; }
        public string ClaveObjGasto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        // Datos de la factura
        public int FacturaMantenimientoId { get; set; }
        public int FacturaDetalleId { get; set; }
        public int MantenimientoDetalleId { get; set; }

        public string? UUID { get; set; }
        public string? Serie { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; }

        // Datos del proveedor
        public string RFCEmisor { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string ClabeInterbancaria { get; set; } = string.Empty;
        
        public int Cantidad { get; set; }

        // Datos de la unidad y concepto
        public string? PlacaActual { get; set; }
        public string? ConceptoServicioNombre { get; set; }

        // Importe facturado
        public string Origen { get; set; } = string.Empty;
        public decimal Importe { get; set; }
    }

}
