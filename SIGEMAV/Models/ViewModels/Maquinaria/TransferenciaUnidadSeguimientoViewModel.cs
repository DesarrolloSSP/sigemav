namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransferenciaUnidadSeguimientoViewModel
    {
        public int TransferenciaUnidadSeguimientoId { get; set; }

        public string EstatusTexto { get; set; } = null!;

        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string? UsuarioRegistro { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public string? TipoMovimiento { get; set; }

        public string? DocumentoRuta { get; set; }


        public string? DocumentoNombreOriginal { get; set; }
    }
}
