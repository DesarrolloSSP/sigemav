using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TipoCatalogoServicioCreateViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150)]
        [Display(Name = "Nombre")]
        public string TipoCatalogoServicioNombre { get; set; } = string.Empty;
    }
}
