using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class ConceptoServicioCostoCreateViewModel
    {
        public int ConceptoServicioCostoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un concepto.")]
        public int ConceptoServicioId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una marca.")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un modelo.")]
        public int ModeloId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un año.")]
        public int AnioId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una transmisión.")]
        public int TransmisionId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cilindro.")]
        public int CilindroId { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha inicio.")]
        public DateOnly FechaInicioVigencia { get; set; }

        public DateOnly? FechaFinVigencia { get; set; }

        [Required(ErrorMessage = "Debe indicar el costo de mano de obra.")]
        [Range(0.01, 999999)]
        public decimal CostoManoObra { get; set; }

        [Required(ErrorMessage = "Debe indicar el costo de refacciones.")]
        [Range(0.01, 999999)]
        public decimal CostoRefacciones { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public List<SelectListItem> ConceptosServicio
            = new();

        public List<SelectListItem> Marcas
            = new();

        public List<SelectListItem> Modelos
            = new();

        public List<SelectListItem> Anios
            = new();

        public List<SelectListItem> Transmisiones
            = new();

        public List<SelectListItem> Cilindros
            = new();
    }
}
