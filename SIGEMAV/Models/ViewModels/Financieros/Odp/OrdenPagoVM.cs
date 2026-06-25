using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Financieros.Odp
{
    public class OrdenPagoVM
    {
        public int IdOrdenPago { get; set; }

        [Required(ErrorMessage = "Ingrese el número de orden.")]
        [Display(Name = "Número Orden")]
        public string? NumeroOrden { get; set; }

        [Required(ErrorMessage = "Seleccione la fecha.")]
        public DateOnly? Fecha { get; set; }

        [Required(ErrorMessage = "Seleccione un proveedor.")]
        [Display(Name = "Proveedor")]
        public int? IdProovedor { get; set; }

        [Required(ErrorMessage = "El importe total es requerido.")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "El importe debe ser mayor a cero.")]
        public decimal? ImporteTotal { get; set; }

        public string? Estatus { get; set; }

        public string? RFC { get; set; }
        public string? NombreProveedor { get; set; }

        public List<OrdenPagoDetalleVM> Detalles { get; set; } = new();

        // =========================
        // CATÁLOGOS
        // =========================

        [ValidateNever]
        public List<SelectListItem> ListaProveedores { get; set; } = new();


        [ValidateNever]
        public List<SelectListItem> DescripPartida { get; set; } = new();

        [ValidateNever]
        public List<SelectListItem> Partidas { get; set; } = new();

    }
}
