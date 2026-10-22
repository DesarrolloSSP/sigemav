using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerConfiguracionPresupuestalViewModel
    {
        public int TallerConfiguracionPresupuestalId { get; set; }


        // =========================================================
        // IDENTIFICACIÓN
        // =========================================================

        [Required(
            ErrorMessage = "El contrato es obligatorio.")]
        [StringLength(
            200,
            ErrorMessage =
                "El contrato no puede exceder los 200 caracteres.")]
        [Display(Name = "Contrato")]
        public string Contrato { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Debe seleccionar un recurso.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar un recurso válido.")]
        [Display(Name = "Recurso")]
        public int RecursoId { get; set; }


        // =========================================================
        // MONTOS
        // =========================================================

        [Required(
            ErrorMessage = "El monto mínimo es obligatorio.")]
        [Range(
            0,
            double.MaxValue,
            ErrorMessage =
                "El monto mínimo no puede ser negativo.")]
        [Display(Name = "Monto mínimo")]
        public decimal MontoMinimo { get; set; }


        [Required(
            ErrorMessage = "El monto máximo es obligatorio.")]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessage =
                "El monto máximo debe ser mayor que cero.")]
        [Display(Name = "Monto máximo")]
        public decimal MontoMaximo { get; set; }


        // =========================================================
        // ALERTAS
        // =========================================================

        [Range(
            0,
            100,
            ErrorMessage =
                "El porcentaje de alerta debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de alerta")]
        public decimal? PorcentajeAlerta { get; set; }


        [Display(Name = "Monto de alerta")]
        public decimal? MontoAlerta { get; set; }


        [Range(
            0,
            100,
            ErrorMessage =
                "El porcentaje crítico debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje crítico")]
        public decimal? PorcentajeCritico { get; set; }


        [Display(Name = "Monto crítico")]
        public decimal? MontoCritico { get; set; }


        // =========================================================
        // ESTADO
        // =========================================================

        [Display(Name = "Activo")]
        public bool Activo { get; set; }


        // =========================================================
        // INFORMACIÓN DE CONSULTA
        // =========================================================

        public int CantidadTalleres { get; set; }

        public int CantidadTalleresActivos { get; set; }

        public int CantidadControles { get; set; }

        public string RecursoNombre { get; set; } = string.Empty;


        // =========================================================
        // CATÁLOGOS
        // =========================================================

        public IEnumerable<SelectListItem> Recursos { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
