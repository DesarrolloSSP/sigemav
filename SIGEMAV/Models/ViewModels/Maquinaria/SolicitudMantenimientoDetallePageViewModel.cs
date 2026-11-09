namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoDetallePageViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        public string Folio { get; set; }
            = string.Empty;

        public string Vehiculo { get; set; }
            = string.Empty;

        public string Taller { get; set; }
            = string.Empty;

        public string Estatus { get; set; } = string.Empty;
        public List<SolicitudMantenimientoDetalleViewModel>Detalles{ get; set; } = new();
        public decimal TotalManoObra => Detalles.Sum(x => x.CostoEstimadoManoObra * x.Cantidad);
        public decimal TotalRefacciones => Detalles.Sum(x => x.CostoEstimadoRefacciones * x.Cantidad);
        public decimal TotalCotizacion => TotalManoObra + TotalRefacciones;
        //public int TotalConceptos => Detalles.Count;
        public int TotalConceptos { get; set; }
        // Nueva propiedad para los documentos adjuntos
        public List<SolicitudMantenimientoDocumentoViewModel> Documentos { get; set; } = new();


        // =========================================================================
        // PROPIEDADES FINANCIERAS SEGURAS CON IVA (Calculadas en el backend)
        // =========================================================================
        public decimal IvaCotizacion => TotalCotizacion * 0.16m;
        public decimal TotalConIvaCotizacion => TotalCotizacion * 1.16m;

        

        



    }
}
