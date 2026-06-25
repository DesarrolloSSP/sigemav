using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConfiguracionValidacionTecnicaCreateViewModel
    {
        public int? ConfiguracionValidacionTecnicaId { get; set; }

        public int? TipoCatalogoServicioId { get; set; }

        public int? ConceptoServicioId { get; set; }

        public bool ValidarSolicitudesAbiertas { get; set; }

        public List<int> EstatusPermitidos { get; set; } = new();

        public bool ValidarRecurrencia { get; set; }

        public int? DiasRecurrencia { get; set; }

        public bool Activo { get; set; } = true;

        // combos
        public IEnumerable<SelectListItem> TiposCatalogoServicio { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> ConceptosServicio { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> EstatusDisponibles { get; set; } = new List<SelectListItem>();
    }
}
