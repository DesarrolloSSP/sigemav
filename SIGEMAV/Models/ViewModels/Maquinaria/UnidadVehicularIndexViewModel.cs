using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularIndexViewModel
    {
        public List<UnidadVehicularViewModel> Registros { get; set; }
            = new();

        public UnidadVehicularFiltroViewModel Filtros { get; set; }
            = new();

        // COMBOS

        public IEnumerable<SelectListItem> Marcas { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Areas { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> Municipios { get; set; }
            = new List<SelectListItem>();

        // PAGINACIÓN

        public int TotalRegistros { get; set; }

        public int PaginaActual { get; set; }

        public int TotalPaginas { get; set; }
    }
}
