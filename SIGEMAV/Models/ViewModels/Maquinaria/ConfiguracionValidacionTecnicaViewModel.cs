using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConfiguracionValidacionTecnicaViewModel
    {
        public int? ConfiguracionValidacionTecnicaId { get; set; }

        public int? TipoCatalogoServicioId { get; set; }

        public int? ConceptoServicioId { get; set; }

        public bool ValidarSolicitudesAbiertas { get; set; }

        public bool ValidarRecurrencia { get; set; }

        public int? DiasRecurrencia { get; set; }

        public bool Activo { get; set; }

        //-----------------------------------
        // NUEVO → ESTATUS MULTISELECCIÓN
        //-----------------------------------

        public List<int> EstatusPermitidos { get; set; }
            = new();

        //-----------------------------------

        [ValidateNever]
        public IEnumerable<SelectListItem> ConceptosServicio { get; set; }
            = new List<SelectListItem>();

        [ValidateNever]
        public List<SelectListItem> EstatusDisponibles { get; set; }
            = new();

        [ValidateNever]
        public IEnumerable<SelectListItem> TiposCatalogoServicio { get; set; }
            = new List<SelectListItem>();
    }
}
