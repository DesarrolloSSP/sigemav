namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TipoCatalogoServicioIndexViewModel
    {
        public List<TipoCatalogoServicioRowViewModel> Registros { get; set; } = new();

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }

        public int TotalRegistros { get; set; }

        public bool TieneAnterior => PaginaActual > 1;

        public bool TieneSiguiente => PaginaActual < TotalPaginas;
    }

    public class TipoCatalogoServicioRowViewModel
    {
        public int TipoCatalogoServicioId { get; set; }

        public string TipoCatalogoServicioNombre { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }

}
