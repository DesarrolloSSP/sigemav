namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadDetalleViewModel
    {
        // =========================================
        // TRANSFERENCIA
        // =========================================

        public int TransferenciaUnidadId { get; set; }

        public string Folio { get; set; } = null!;

        // =========================================
        // UNIDAD
        // =========================================

        public int UnidadVehicularId { get; set; }

        public string NumeroSerie { get; set; } = null!;

        public string PlacaActual { get; set; } = null!;

        public string NumeroEconomico { get; set; } = null!;

        public string MarcaModelo { get; set; } = null!;

        public string Color { get; set; } = null!;

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

        public DateTime? FechaTransferencia { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public DateTime? FechaRecepcion { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        // =========================================
        // OPERACIÓN
        // =========================================

        public string? MotivoTransferencia { get; set; }

        public string? Observaciones { get; set; }

        public string? ComentariosAutorizacion { get; set; }

        public string? ObservacionesEntrega { get; set; }

        public string? ObservacionesRecepcion { get; set; }

        // =========================================
        // DOCUMENTO
        // =========================================

        public string? DocumentoRuta { get; set; }

        public string? DocumentoNombreOriginal { get; set; }

        // =========================================
        // USUARIOS
        // =========================================

        public int? UsuarioSolicitaId { get; set; }

        public int? UsuarioAutorizaId { get; set; }

        public int? UsuarioEntregaId { get; set; }

        public int? UsuarioRecibeId { get; set; }

        // =========================================
        // SEGUIMIENTO
        // =========================================

        public List<TransferenciaUnidadSeguimientoViewModel>
            Seguimientos
        { get; set; }
                = new();
    }
}
