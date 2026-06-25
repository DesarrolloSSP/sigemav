using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Financieros.Proyecto
{
    public class ProyectoAreaCrudVM
    {
        public int IdProyectoArea { get; set; }


        [Required(ErrorMessage = "Seleccione proyecto")]
        [Display(Name = "Proyecto")]
        public int? IdProyecto { get; set; }


        [Required(ErrorMessage = "Seleccione área")]
        [Display(Name = "Área")]
        public int? IdArea { get; set; }


        // MOSTRAR EN GRID
        public string? Proyecto { get; set; }

        public string? Area { get; set; }


        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;


        // COMBOS
        public List<SelectListItem>
            Proyectos
        { get; set; } = new();

        public List<SelectListItem>
            Areas
        { get; set; } = new();


    }

}
