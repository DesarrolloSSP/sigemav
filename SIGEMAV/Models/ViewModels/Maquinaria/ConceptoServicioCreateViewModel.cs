using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConceptoServicioCreateViewModel
    {
        [Required(ErrorMessage = "Seleccione un tipo de catálogo.")]
        [Display(Name = "Tipo Catálogo")]
        public int? TipoCatalogoServicioId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string ConceptoServicioNombre { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string? ConceptoServicioDescripcion { get; set; }

        public List<SelectListItem> TiposCatalogo { get; set; }
            = new();
    }
}
