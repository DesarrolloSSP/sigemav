using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoServicioViewModel
    {
        public int ConceptoServicioId { get; set; }

        public decimal Cantidad { get; set; }

        public decimal CostoEstimadoManoObra { get; set; }

        public decimal CostoEstimadoRefacciones { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? ImporteCotizacion { get; set; }

        public int? IdObjetoGastoManoObra { get; set; }

        public int? IdObjetoGastoRefacciones { get; set; }

        public string? Observaciones { get; set; }

        public string? ClaveObjetoGastoManoObra { get; set; }

        public string? ClaveObjetoGastoRefacciones { get; set; }


    }







}
