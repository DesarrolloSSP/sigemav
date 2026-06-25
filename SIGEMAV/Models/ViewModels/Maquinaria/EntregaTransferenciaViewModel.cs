using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class EntregaTransferenciaViewModel
    {
        public int TransferenciaUnidadId { get; set; }

        public string Folio { get; set; }

        public string Unidad { get; set; }

        public string AreaOrigen { get; set; }

        public string AreaDestino { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public string MotivoTransferencia { get; set; }

        [Required]
        public string ObservacionesEntrega { get; set; }

        public IEnumerable<IFormFile>? Archivos { get; set; }
    }
}
