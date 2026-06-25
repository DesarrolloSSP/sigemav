namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MantenimientoServicioViewModel
    {
        public int MantenimientoDetalleId { get; set; }
        public int ConceptoServicioId { get; set; }
        public string? ConceptoServicio { get; set; }

        public int Cantidad { get; set; }

        public decimal CostoEstimadoManoObra { get; set; }

        public decimal CostoEstimadoRefacciones { get; set; }

        public decimal TotalEstimado { get; set; }
        public int IdObjetoGastoManoObra { get; set; }

        public int IdObjetoGastoRefacciones { get; set; }

        public string? Observaciones { get; set; }
    }
}
