namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConceptoServicioCostoIndexViewModel
    {
        public List<ConceptoServicioCostoRowViewModel> Registros { get; set; } = new();

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }

        public int TotalRegistros { get; set; }

        public bool TieneAnterior => PaginaActual > 1;

        public bool TieneSiguiente => PaginaActual < TotalPaginas;
    }

    public class ConceptoServicioCostoRowViewModel
    {
        public int ConceptoServicioCostoId { get; set; }

        public string ConceptoServicioNombre { get; set; } = string.Empty;

        public decimal CostoManoObra { get; set; }

        public decimal CostoRefacciones { get; set; }

        public DateTime FechaInicioVigencia { get; set; }

        public DateTime? FechaFinVigencia { get; set; }

        public string MarcaNombre { get; set; } = string.Empty;

        public string ModeloNombre { get; set; } = string.Empty;

        public string AnioNombre { get; set; } = string.Empty;

        public string TransmisionNombre { get; set; } = string.Empty;

        public string CilindroNombre { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public string Observaciones { get; set; } = string.Empty;
    }
}
