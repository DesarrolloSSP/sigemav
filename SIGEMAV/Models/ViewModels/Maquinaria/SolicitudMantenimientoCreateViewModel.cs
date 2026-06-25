using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoCreateViewModel
    {
        public string Folio { get; set; } = string.Empty;

        public string NumeroOficio { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        public int? AreaSolicitanteId { get; set; }

        public int UnidadVehicularId { get; set; }

        public int? TallerId { get; set; }

        public int TipoMantenimientoId { get; set; }

        public int KilometrajeActual { get; set; }

        public string? MotivoSolicitud { get; set; }

        public string? DiagnosticoInicial { get; set; }

        public string? Observaciones { get; set; }

        public decimal? ImporteCotizacion { get; set; }

        public int EstatusSolicitudMantenimientoId { get; set; }

        public string? UnidadVehicularTexto { get; set; }

        public List<SelectListItem> Areas { get; set; }
            = new();

        //public List<SelectListItem> UnidadesVehiculares { get; set; }
        //    = new();

        public List<SelectListItem> TiposMantenimiento { get; set; }
            = new();

        public List<SelectListItem> Talleres { get; set; }
            = new();

        public List<SelectListItem> Estatuses { get; set; }
            = new();

        public List<IFormFile>? Archivos { get; set; }


        public List<SolicitudMantenimientoServicioViewModel>Servicios{ get; set; } = new();

        //public List<SolicitudMantenimientoDetalleViewModel>    Detalles{ get; set; } = new();

        public List<SelectListItem>ConceptosServicio{ get; set; } = new();


        public IEnumerable<SelectListItem> ObjetosGasto { get; set; } = new List<SelectListItem>();


    }
}
