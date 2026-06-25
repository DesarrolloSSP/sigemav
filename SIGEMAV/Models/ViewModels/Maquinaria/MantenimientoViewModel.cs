namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MantenimientoViewModel
    {
        public int MantenimientoId { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string NumeroEconomico { get; set; } = string.Empty;

        public string PlacaActual { get; set; } = string.Empty;

        public string MarcaNombre { get; set; } = string.Empty;

        public string ModeloNombre { get; set; } = string.Empty;

        public string TipoMantenimiento { get; set; } = string.Empty;

        public string TallerNombre { get; set; } = string.Empty;

        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaSalida { get; set; }

        public int KilometrajeEntrada { get; set; }

        public bool Activo { get; set; }
    }
}
