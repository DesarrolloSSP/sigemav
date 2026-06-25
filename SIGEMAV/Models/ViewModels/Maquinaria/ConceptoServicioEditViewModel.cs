using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class ConceptoServicioEditViewModel
    {
        public int ConceptoServicioId { get; set; }

        [Required(ErrorMessage = "Seleccione un tipo de catálogo.")]
        public int TipoCatalogoServicioId { get; set; }

        [Required(ErrorMessage = "Ingrese el nombre del concepto.")]
        [StringLength(200)]
        public string ConceptoServicioNombre { get; set; }

        [StringLength(500)]
        public string? ConceptoServicioDescripcion { get; set; }

        public List<SelectListItem> TiposCatalogoServicio= new();
    }
}
