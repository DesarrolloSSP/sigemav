using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoCreateViewModel
    {
        public string Folio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de oficio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El número de oficio no puede exceder los 100 caracteres.")]
        public string? NumeroOficio { get; set; } = null!;

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        public int? AreaSolicitanteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la unidad vehicular.")]
        public int? UnidadVehicularId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el taller.")]
        public int? TallerId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de mantenimiento.")]
        public int? TipoMantenimientoId { get; set; }


        [Required(ErrorMessage = "Capture el kilometraje.")]
        public int? KilometrajeActual { get; set; }

        //public string? MotivoSolicitud { get; set; }

        //public string? DiagnosticoInicial { get; set; }

        public string? Observaciones { get; set; }

        public decimal? ImporteCotizacion { get; set; }

        public int? EstatusSolicitudMantenimientoId { get; set; }

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

        [Required(ErrorMessage = "Debe seleccionar el estado del odómetro.")]
        public byte? EstadoOdometroId { get; set; }

        public IEnumerable<SelectListItem> EstadosOdometro { get; set; }
            = new List<SelectListItem>();

        public List<SolicitudMantenimientoServicioViewModel>Servicios{ get; set; } = new();

        //public List<SolicitudMantenimientoDetalleViewModel>    Detalles{ get; set; } = new();

        public List<SelectListItem>ConceptosServicio{ get; set; } = new();


        public IEnumerable<SelectListItem> ObjetosGasto { get; set; } = new List<SelectListItem>();


        [Required(ErrorMessage = "Debe seleccionar el contrato/configuración presupuestal del taller.")]
        public int? TallerConfiguracionPresupuestalId { get; set; }
        public List<SelectListItem> ConfiguracionesTaller { get; set; } = new();


    }
}
