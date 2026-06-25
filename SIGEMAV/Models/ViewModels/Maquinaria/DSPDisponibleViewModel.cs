namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class DSPDisponibleViewModel
    {
        public int IdDispPresupuestal { get; set; }

        public int IdDsp { get; set; }

        public int IdObjetoGasto { get; set; }

        public string ObjetoGastoDescripcion { get; set; } = string.Empty;

        public string ClaveObjetoGasto { get; set; } = string.Empty;

        public decimal PresupuestoAsignado { get; set; }

        public decimal PresupuestoComprometido { get; set; }

        public decimal PresupuestoDevengado { get; set; }

        public decimal PresupuestoDisponible { get; set; }

        public decimal ImporteSolicitado { get; set; }


        public decimal ImporteLiberado { get; set; }

        public bool FueDescomprometida { get; set; }

        public decimal comprometidoNetoSolicitud { get; set; }

        public decimal comprometidoSolicitudActual { get; set; }

        public decimal ultimoMovimiento { get; set; }



    }
}
