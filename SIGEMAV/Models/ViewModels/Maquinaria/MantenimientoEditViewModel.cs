using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MantenimientoEditViewModel
    {

        public int UnidadVehicularId { get; set; }

        public int MantenimientoId { get; set; }

        public int TallerId { get; set; }

        public int TipoMantenimientoId { get; set; }


        public DateTime? FechaSalida { get; set; }

        public string MotivoMantenimiento { get; set; }

        public int SolicitudMantenimientoId { get; set; }

        public decimal TotalLinea { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string NumeroOficio { get; set; } = string.Empty;

        public string UnidadVehicularTexto { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string NumeroEconomico { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;
        public string PlacaActual { get; set; } = string.Empty;

        public string TallerTexto { get; set; } = string.Empty;

        public string TipoMantenimientoTexto { get; set; } = string.Empty;

        public int KilometrajeEntrada { get; set; }

        public DateTime FechaIngreso { get; set; }

        

        public string? Diagnostico { get; set; }

        public string? TrabajoRealizado { get; set; }

        [Required(ErrorMessage = "Los comentarios finales / observaciones son obligatorios.")]
        public string? Observaciones { get; set; }

        public decimal? CostoRealTotal { get; set; }

        public int EstatusMantenimientoId { get; set; }



        public IEnumerable<SelectListItem> ConceptosServicio { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> ObjetosGasto { get; set; }
            = new List<SelectListItem>();

        public List<MantenimientoDetalleViewModel> Servicios
        {
            get;
            set;
        } = new();


        [Required (ErrorMessage = "La fecha de inicio de trabajo es obligatoria.")]
        public DateTime? FechaInicioTrabajo { get; set; }

        [Required(ErrorMessage = "La fecha de fin de trabajo es obligatoria.")]
        public DateTime? FechaFinTrabajo { get; set; }

        [Required(ErrorMessage = "El kilometraje de salida es obligatorio.")]
        public int? KilometrajeSalida { get; set; }



    }
}
