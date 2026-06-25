namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MantenimientoDetalleViewModel
    {
        public int? MantenimientoDetalleId { get; set; }

        public int MantenimientoId { get; set; }

        public int ConceptoServicioId { get; set; }

        public string ConceptoServicio { get; set; } =
            string.Empty;

        public int Cantidad { get; set; }

        public decimal CostoUnitarioManoObra { get; set; }

        public decimal CostoUnitarioRefacciones { get; set; }

        public decimal SubtotalManoObra { get; set; }

        public decimal SubtotalRefacciones { get; set; }

        public decimal TotalLinea {  get; set; }

        public int? IdObjetoGastoManoObra { get; set; }

        public int? IdObjetoGastoRefacciones { get; set; }

        public decimal CostoEstimadoManoObra { get; set; }

        public decimal CostoEstimadoRefacciones { get; set; }

        public string? ObjetoGastoManoObraTexto { get; set; }

        public string? ObjetoGastoRefaccionesTexto { get; set; }

        public string? Observaciones { get; set; }

        public List<MantenimientoDetalleViewModel> Servicios
        {
            get;
            set;
        } = new();

        public decimal CostoTotalServicio => TotalLinea;


        public decimal CostoCalculado => Cantidad * (CostoUnitarioManoObra + CostoUnitarioRefacciones);

    }





}
