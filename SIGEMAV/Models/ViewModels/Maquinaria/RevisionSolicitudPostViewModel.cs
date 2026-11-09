using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class RevisionSolicitudPostViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "La solicitud de mantenimiento no es válida.")]
        public int SolicitudMantenimientoId { get; set; }

        public string? Observaciones { get; set; }

        public int? TipoRechazoSolicitudId { get; set; }
    }
}
