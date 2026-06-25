namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string NumeroOficio { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public string AreaSolicitante { get; set; } = string.Empty;

        public string NumeroEconomico { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;

        public string PlacaActual { get; set; } = string.Empty;

        public string MarcaNombre { get; set; } = string.Empty;

        public string ModeloNombre { get; set; } = string.Empty;

        public string TipoMantenimiento { get; set; } = string.Empty;

        public string TallerNombre { get; set; } = string.Empty;

        public decimal? ImporteCotizacion { get; set; }

        public string Estatus { get; set; } = string.Empty;

        public string ColorEstatus { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public int TotalConceptos { get; set; }

        public int EstatusSolicitudMantenimientoId { get; set; }
    }


}
