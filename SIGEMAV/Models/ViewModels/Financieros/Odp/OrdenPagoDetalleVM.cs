using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Financieros.Odp
{
    public class OrdenPagoDetalleVM
    {
        [Required(ErrorMessage = "Ingrese el concepto.")]
        public string? Concepto { get; set; }


        [Required(ErrorMessage = "Seleccione una partida.")]
        public int? IdPartida { get; set; }

        [Required(ErrorMessage = "Ingrese el monto.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal? Monto { get; set; }

        public string? Serie { get; set; }

        public string? Folio { get; set; }

        public DateOnly? FechaFactura { get; set; }

        public string? NoPlaca { get; set; }


    }


}

