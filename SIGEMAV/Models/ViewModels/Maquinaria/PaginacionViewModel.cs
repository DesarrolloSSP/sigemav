namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class PaginacionViewModel<T>
    {
        public List<T> Registros { get; set; } = new();

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }

        public int TotalRegistros { get; set; }

        public int RegistrosPorPagina { get; set; }

        public bool TieneAnterior => PaginaActual > 1;

        public bool TieneSiguiente => PaginaActual < TotalPaginas;

        public int TotalConceptos { get; set; }
    }

}
