using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerConfiguracionCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un taller.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un taller válido.")]
        [Display(Name = "Taller")]
        public int TallerId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una configuración presupuestal.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una configuración presupuestal válida.")]
        [Display(Name = "Configuración presupuestal")]
        public int TallerConfiguracionPresupuestalId { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public string? Ubicacion { get; set; }

        public IEnumerable<SelectListItem> Talleres { get; set; }
            = Enumerable.Empty<SelectListItem>();

        public IEnumerable<SelectListItem> ConfiguracionesPresupuestales { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
