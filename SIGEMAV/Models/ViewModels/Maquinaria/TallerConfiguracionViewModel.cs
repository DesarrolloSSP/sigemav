using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerConfiguracionViewModel
    {
        public int TallerId { get; set; }

        public int TallerConfiguracionPresupuestalId { get; set; }

        [Display(Name = "Taller")]
        public string TallerNombre { get; set; } = string.Empty;

        [Display(Name = "RFC")]
        public string RFC { get; set; } = string.Empty;

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        [Display(Name = "Contrato")]
        public string Contrato { get; set; } = string.Empty;

        [Display(Name = "Recurso")]
        public string RecursoNombre { get; set; } = string.Empty;

        [Display(Name = "Monto máximo")]
        public decimal MontoMaximo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
