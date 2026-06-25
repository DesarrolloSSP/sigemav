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
    }
}
