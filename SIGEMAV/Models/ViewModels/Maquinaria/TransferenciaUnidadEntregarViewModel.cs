namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadEntregarViewModel
    {
        public int TransferenciaUnidadId { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string AreaOrigen { get; set; } = string.Empty;

        public string AreaDestino { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public string MotivoTransferencia { get; set; } = string.Empty;

        public int UsuarioEntregaId { get; set; }

        public string? ObservacionesEntrega { get; set; }
    }
}
