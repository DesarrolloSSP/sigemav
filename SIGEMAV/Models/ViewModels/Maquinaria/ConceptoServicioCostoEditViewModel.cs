using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ConceptoServicioCostoEditViewModel : IValidatableObject
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

        
        [Range(0, 999999)]
        public decimal CostoManoObra { get; set; }

        
        [Range(0, 999999)]
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

        public int TipoCatalogoServicioId { get; set; }











        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CostoManoObra == 0 && CostoRefacciones == 0)
            {
                yield return new ValidationResult("Debe capturar un costo de mano de obra, de refacciones o ambos.",
                    new[]
                    {
                    nameof(CostoManoObra),
                    nameof(CostoRefacciones)
                    });
            }

            if (FechaFinVigencia.HasValue && FechaFinVigencia < FechaInicioVigencia)
            {
                yield return new ValidationResult("La fecha fin de vigencia debe ser mayor o igual a la fecha de inicio.",
                    new[]
                    {
                    nameof(FechaFinVigencia)
                    });
            }
        }


    }
}
