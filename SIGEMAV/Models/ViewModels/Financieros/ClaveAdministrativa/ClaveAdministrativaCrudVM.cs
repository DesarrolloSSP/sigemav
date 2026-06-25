using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa
{
    public class ClaveAdministrativaCrudVM
    {
        public int IdClaveAdmin { get; set; }


        [Required(ErrorMessage = "Capture la clave administrativa")]
        [StringLength(50,
            ErrorMessage = "Máximo 50 caracteres")]
        [Display(Name = "Clave administrativa")]
        public string ClaveAdmin { get; set; } = null!;


        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;


        public DateTime? FechaCreacion { get; set; }

    }
}
