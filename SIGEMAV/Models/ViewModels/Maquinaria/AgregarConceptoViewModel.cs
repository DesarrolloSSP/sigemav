using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class AgregarConceptoViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        [Required]
        public int ConceptoServicioId { get; set; }

        [Required]
        public decimal Cantidad { get; set; } = 1;

        public decimal CostoEstimadoManoObra { get; set; }

        public decimal CostoEstimadoRefacciones { get; set; }

        public string? Observaciones { get; set; }

        public List<SelectListItem> ConceptosServicio= new();

        public List<int> ConceptosYaRegistrados { get; set; }= new();

        public int TipoCatalogoServicioId { get; set; }

    }

}
