namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class FacturaIndexViewModel
    {
        public List<FacturaPendienteViewModel>
            Pendientes
            = new();

        public List<FacturaListadoViewModel>
            Facturas
            = new();
    }

    public class FacturaPendienteViewModel
    {
        public int MantenimientoId { get; set; }

        public string Unidad { get; set; }

        public string Placa { get; set; }

        public string Taller { get; set; }

        public DateTime? FechaFinalizacion { get; set; }

        public decimal TotalEstimado { get; set; }

        public string Folio { get; set; } = null!;

        public int DiasTranscurridos { get; set; }
        public int CantidadConceptos { get; set; }

        public string Area { get; set; }

    }

    public class FacturaListadoViewModel
    {
        public int FacturaId { get; set; }

        public string UUID { get; set; }

        public string Taller { get; set; }

        public DateOnly Fecha { get; set; }

        public decimal Total { get; set; }
    }
}
