using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerConfiguracionEditViewModel
    {
        // =====================================================
        // TALLER
        // =====================================================

        public int TallerId { get; set; }

        [Display(Name = "Taller")]
        public string TallerNombre { get; set; } = string.Empty;

        [Display(Name = "RFC")]
        public string RFC { get; set; } = string.Empty;

        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;


        // =====================================================
        // CONFIGURACIÓN ORIGINAL
        // =====================================================

        /// <summary>
        /// Configuración actualmente asociada al taller.
        /// Se utiliza para identificar la asociación original
        /// que se está editando.
        /// </summary>
        public int ConfiguracionPresupuestalOriginalId { get; set; }


        // =====================================================
        // CONFIGURACIÓN DESTINO
        // =====================================================

        [Required(
            ErrorMessage =
                "Debe seleccionar una configuración presupuestal.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar una configuración presupuestal válida.")]
        [Display(Name = "Configuración presupuestal")]
        public int TallerConfiguracionPresupuestalId { get; set; }


        // =====================================================
        // INFORMACIÓN DE LA CONFIGURACIÓN ACTUAL
        // =====================================================

        [Display(Name = "Contrato actual")]
        public string ContratoActual { get; set; } = string.Empty;

        [Display(Name = "Recurso actual")]
        public string RecursoActual { get; set; } = string.Empty;

        public string RecursoNombre { get; set; } = string.Empty;


        // =====================================================
        // ESTADO
        // =====================================================

        [Display(Name = "Activo")]
        public bool Activo { get; set; }


        // =====================================================
        // AUDITORÍA
        // =====================================================

        public DateTime FechaCreacion { get; set; }


        // =====================================================
        // CATÁLOGO
        // =====================================================

        public IEnumerable<SelectListItem> ConfiguracionesPresupuestales
        {
            get;
            set;
        } = Enumerable.Empty<SelectListItem>();
    }
}
