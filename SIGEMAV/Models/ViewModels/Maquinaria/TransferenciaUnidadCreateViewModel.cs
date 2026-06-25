using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadCreateViewModel
    {
        [Required]
        public int UnidadVehicularId { get; set; }

        [Required]
        public int AreaOrigenId { get; set; }

        [Required]
        public int AreaDestinoId { get; set; }

        [Required]
        [StringLength(500)]
        public string MotivoTransferencia { get; set; } = null!;

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        [Required]
        public DateTime FechaTransferencia { get; set; }

        public string? UnidadVehicularTexto { get; set; }

        public IEnumerable<SelectListItem> Areas { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> UnidadesVehiculares { get; set; }
            = new List<SelectListItem>();

        public IFormFile? Documento { get; set; }
    }
}
