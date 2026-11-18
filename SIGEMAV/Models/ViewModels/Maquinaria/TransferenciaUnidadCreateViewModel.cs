using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadCreateViewModel
    {
        [Required(ErrorMessage = "Seleccione la unidad vehicular.")]
        public int UnidadVehicularId { get; set; }

        [Required(ErrorMessage = "El área de origen es obligatoria.")]
        public int AreaOrigenId { get; set; }

        [Required(ErrorMessage = "Seleccione el área de destino.")]
        public int AreaDestinoId { get; set; }

        [Required(ErrorMessage = "El motivo de la transferencia es obligatorio.")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres.")]
        public string MotivoTransferencia { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder los 1000 caracteres.")]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "La fecha de transferencia es obligatoria.")]
        public DateTime FechaTransferencia { get; set; }

        public string? UnidadVehicularTexto { get; set; }

        public IEnumerable<SelectListItem> Areas { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> UnidadesVehiculares { get; set; }
            = new List<SelectListItem>();

        [Required(ErrorMessage = "Debe adjuntar el formato de resguardo en PDF.")]
        public IFormFile? Documento { get; set; }

    }
















}
