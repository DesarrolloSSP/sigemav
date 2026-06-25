using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Financieros.Proyecto
{
    public class ProyectoCrudVM
    {
        public int IdProyecto { get; set; }


        [Required(ErrorMessage = "Seleccione la clave administrativa")]
        [Display(Name = "Clave administrativa")]
        public int? IdClaveAdmin { get; set; }


        [Required(ErrorMessage = "Capture la clave proyecto")]
        [StringLength(50)]
        [Display(Name = "Clave proyecto")]
        public string ClaveProyecto { get; set; } = null!;


        [Required(ErrorMessage = "Capture la descripción")]
        [StringLength(200)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = null!;


        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;


        // COMBO
        public List<SelectListItem>
            ClavesAdministrativas
        { get; set; } = new();

        public string? ClaveAdministrativa { get; set; }

    }
}
