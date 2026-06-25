using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MantenimientoEditViewModel
    {

        public int UnidadVehicularId { get; set; }

        public int MantenimientoId { get; set; }

        public int TallerId { get; set; }

        public int TipoMantenimientoId { get; set; }


        public DateTime FechaSalida { get; set; }

        public string MotivoMantenimiento { get; set; }

        public int SolicitudMantenimientoId { get; set; }

        public decimal TotalLinea { get; set; }

        public string Folio { get; set; } = string.Empty;

        public string NumeroOficio { get; set; } = string.Empty;

        public string UnidadVehicularTexto { get; set; } = string.Empty;

        public string TallerTexto { get; set; } = string.Empty;

        public string TipoMantenimientoTexto { get; set; } = string.Empty;

        public int KilometrajeEntrada { get; set; }

        public int? KilometrajeSalida { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaInicioTrabajo { get; set; }

        public DateTime? FechaFinTrabajo { get; set; }

        public string? Diagnostico { get; set; }

        public string? TrabajoRealizado { get; set; }

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


    }
}
