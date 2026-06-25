using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class TipoCatalogoServicioEditViewModel
    {
        public int TipoCatalogoServicioId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150)]
        [Display(Name = "Nombre")]
        public string TipoCatalogoServicioNombre { get; set; } = string.Empty;
    }
}
