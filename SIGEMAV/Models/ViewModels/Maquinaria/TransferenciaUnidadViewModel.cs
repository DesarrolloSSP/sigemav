namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadViewModel
    {
        public int TransferenciaUnidadId { get; set; }

        public string Folio { get; set; } = null!;

        // =========================================
        // UNIDAD
        // =========================================

        public int UnidadVehicularId { get; set; }

        public string UnidadVehicularTexto { get; set; } = null!;

        // =========================================
        // ÁREAS
        // =========================================

        public int AreaOrigenId { get; set; }

        public string AreaOrigenTexto { get; set; } = null!;

        public int AreaDestinoId { get; set; }

        public string AreaDestinoTexto { get; set; } = null!;

        // =========================================
        // ESTATUS
        // =========================================

        public int EstatusTransferenciaUnidadId { get; set; }

        public string EstatusTexto { get; set; } = null!;

        // =========================================
        // FECHAS
        // =========================================

        public DateTime FechaSolicitud { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaTransferencia { get; set; }

        public string MotivoTransferencia { get; set; } = null!;

        // =========================================
        // DATOS OPERATIVOS
        // =========================================

        public string? Motivo { get; set; }

        public string? Observaciones { get; set; }

        // =========================================
        // PROPIEDADES CALCULADAS
        // =========================================

        public string UnidadVehicularDisplay =>
            $"{UnidadVehicularTexto} ({Folio})";
    }
}
