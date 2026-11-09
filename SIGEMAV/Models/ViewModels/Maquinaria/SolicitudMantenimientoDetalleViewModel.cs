namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoDetalleViewModel
    {
        public int SolicitudMantenimientoDetalleId { get; set; }

        public int ConceptoServicioId { get; set; }

        public string ConceptoServicio { get; set; }
            = string.Empty;

        public decimal Cantidad { get; set; }

        public decimal CostoEstimadoManoObra { get; set; }

        public decimal CostoEstimadoRefacciones { get; set; }

        public decimal Total =>
            (CostoEstimadoManoObra
            + CostoEstimadoRefacciones)
            * Cantidad;

        public string? Observaciones { get; set; }

        public string? ConceptoServicioNombre { get; set; }

        public decimal CostoManoObraConIva { get; set; }
        public decimal CostoRefaccionesConIva { get; set; }





        // Subtotal de la fila (Cantidad * (ManoObraBase + RefaccionesBase))
        public decimal SubtotalRenglon => Cantidad * (CostoEstimadoManoObra + CostoEstimadoRefacciones);

        // IVA total de la fila (16% del subtotal del renglón)
        public decimal IvaRenglon => SubtotalRenglon * 0.16m;

        // Total final de la fila (Subtotal + IVA)
        public decimal TotalRenglon => SubtotalRenglon + IvaRenglon;



    }
}
