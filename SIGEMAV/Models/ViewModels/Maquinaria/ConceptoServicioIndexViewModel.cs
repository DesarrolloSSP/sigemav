namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConceptoServicioIndexViewModel
    {
        public List<ConceptoServicioRowViewModel> Registros { get; set; } = new();

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }

        public int TotalRegistros { get; set; }

        public bool TieneAnterior => PaginaActual > 1;

        public bool TieneSiguiente => PaginaActual < TotalPaginas;
    }

    public class ConceptoServicioRowViewModel
    {
        public int ConceptoServicioId { get; set; }

        public string TipoCatalogoServicioNombre { get; set; } = string.Empty;

        public string ConceptoServicioNombre { get; set; } = string.Empty;

        public string ConceptoServicioDescripcion { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
