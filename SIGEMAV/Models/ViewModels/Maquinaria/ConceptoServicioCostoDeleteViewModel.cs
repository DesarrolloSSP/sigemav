namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConceptoServicioCostoDeleteViewModel
    {
        public int ConceptoServicioCostoId { get; set; }

        public string ConceptoServicioNombre { get; set; }
            = string.Empty;

        public string MarcaNombre { get; set; }
            = string.Empty;

        public string ModeloNombre { get; set; }
            = string.Empty;

        public string AnioNombre { get; set; }
            = string.Empty;

        public string TransmisionNombre { get; set; }
            = string.Empty;

        public string CilindroNombre { get; set; }
            = string.Empty;

        public decimal CostoManoObra { get; set; }

        public decimal CostoRefacciones { get; set; }

        public DateOnly FechaInicioVigencia { get; set; }

        public DateOnly? FechaFinVigencia { get; set; }

        public string? Observaciones { get; set; }
    }
}
