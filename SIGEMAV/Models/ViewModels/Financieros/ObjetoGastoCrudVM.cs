using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Financieros
{
    public class ObjetoGastoCrudVM
    {
        public int IdObjetoGasto { get; set; }

        [Required(ErrorMessage = "Capture la clave")]
        [StringLength(10)]
        [Display(Name = "Clave objeto gasto")]
        public string ClaveObjGasto { get; set; } = null!;

        [Required(ErrorMessage = "Capture la descripción")]
        [StringLength(200)]
        public string Descripcion { get; set; } = null!;

        [Display(Name = "Activo")]
        public bool Activo { get; set; }


    }
}
