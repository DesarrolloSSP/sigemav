namespace SIGEMAV.Models.ViewModels.OrdeDePago
{
    public class OrdenDePagoVM
    {
        public string Folio { get; set; }

        public int MantenimientoId { get; set; }

        public int MantenimientoDetalleId { get; set; }

        public int? IdObjetoGasto { get; set; }

        public string ClaveObjGasto { get; set; }

        public string Descripcion { get; set; }



        public decimal? CostoUnitario { get; set; }

        public decimal? Subtotal { get; set; }

        public string Tipo { get; set; }



        public string RFC { get; set; }

        public string RazonSocial { get; set; }

        public DateTime? FechaSolicitud { get; set; }
    }
}
