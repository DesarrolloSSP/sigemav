using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerControlPresupuestalCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar una configuración presupuestal.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar una configuración presupuestal válida.")]
        [Display(Name = "Configuración presupuestal")]
        public int TallerConfiguracionPresupuestalId { get; set; }

        [Required(ErrorMessage = "El ejercicio es obligatorio.")]
        [Range(
            2000,
            2100,
            ErrorMessage = "El ejercicio debe estar entre 2000 y 2100.")]
        [Display(Name = "Ejercicio")]
        public short Ejercicio { get; set; }

        [Required(ErrorMessage = "El presupuesto asignado es obligatorio.")]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "El presupuesto asignado debe ser mayor que cero.")]
        [Display(Name = "Presupuesto asignado")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoAsignado { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de fin")]
        public DateTime FechaFin { get; set; }

        public IEnumerable<SelectListItem> ConfiguracionesPresupuestales { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
