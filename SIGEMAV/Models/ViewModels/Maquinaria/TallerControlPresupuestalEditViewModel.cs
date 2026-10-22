using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerControlPresupuestalEditViewModel
    {
        public int IdControlTaller { get; set; }

        [Display(Name = "Contrato")]
        public string Contrato { get; set; } = string.Empty;

        [Display(Name = "Recurso")]
        public string RecursoNombre { get; set; } = string.Empty;

        [Display(Name = "Ejercicio")]
        public short Ejercicio { get; set; }

        [Display(Name = "Monto máximo contractual")]
        [DataType(DataType.Currency)]
        public decimal MontoMaximo { get; set; }

        [Display(Name = "Presupuesto asignado")]
        [DataType(DataType.Currency)]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage = "El presupuesto asignado debe ser mayor que cero.")]
        public decimal PresupuestoAsignado { get; set; }

        [Display(Name = "Presupuesto comprometido")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoComprometido { get; set; }

        [Display(Name = "Presupuesto ejercido")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoEjercido { get; set; }

        [Display(Name = "Presupuesto disponible")]
        [DataType(DataType.Currency)]
        public decimal PresupuestoDisponible { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateOnly FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de fin")]
        public DateOnly FechaFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
    }
}
