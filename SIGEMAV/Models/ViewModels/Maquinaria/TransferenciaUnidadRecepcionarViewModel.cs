namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadRecepcionarViewModel
    {
        public int TransferenciaUnidadId { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string AreaOrigen { get; set; } = string.Empty;

        public string AreaDestino { get; set; } = string.Empty;

        public DateTime? FechaEntrega { get; set; }

        public string? ObservacionesEntrega { get; set; }

        public string? ObservacionesRecepcion { get; set; }
    }
}
