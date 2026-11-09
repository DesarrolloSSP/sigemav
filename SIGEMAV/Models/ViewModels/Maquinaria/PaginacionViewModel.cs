using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Models.ViewModels.Maquinaria;

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

        public string? Buscar { get; set; }

        public int? EstatusSolicitudMantenimientoId { get; set; }

        public int? AreaSolicitanteId { get; set; }

        public int? TallerId { get; set; }

        public int? TipoMantenimientoId { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public List<SelectListItem> Estatus = new();

        public List<SelectListItem> Areas = new();

        public List<SelectListItem> Talleres = new();

        public List<SelectListItem> TiposMantenimiento = new();

        public SolicitudMantenimientoFiltroViewModel Filtro { get; set; } = new();

        public SolicitudMantenimientoKpiViewModel Kpi { get; set; } = new();


        public int TotalPendienteDSP { get; set; }
        public int TotalEnRevision { get; set; }

        public int TotalEnMantenimiento { get; set; }

        public int TotalFinalizadas { get; set; }

        public int ImporteTotalCotizado { get; set; }

    }

}

