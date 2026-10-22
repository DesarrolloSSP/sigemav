using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class SolicitudMantenimientoEditViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        [Required(ErrorMessage = "El folio es obligatorio.")]
        [StringLength(50, ErrorMessage = "El folio no puede exceder los 50 caracteres.")]
        public string Folio { get; set; } = null!;


        [Required(ErrorMessage = "El número de oficio es obligatorio.")]
        [StringLength(100, ErrorMessage = "El número de oficio no puede exceder los 100 caracteres.")]
        public string? NumeroOficio { get; set; } = null!;

        [Required(ErrorMessage = "Debe indicar la fecha de solicitud.")]
        public DateTime FechaSolicitud { get; set; }


        public DateTime FechaRegistro { get; set; }


        [Required]
        public int? AreaSolicitanteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la unidad vehicular.")]
        public int? UnidadVehicularId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el taller.")]
        public int? TallerId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el tipo de mantenimiento.")]
        public int? TipoMantenimientoId { get; set; }

        [Required(ErrorMessage = "Debe indicar el kilometraje actual. ")]
        [Range(0, 999999, ErrorMessage = "El kilometraje debe estar entre 0 y 999999.")]
        public int? KilometrajeActual { get; set; }

        //[StringLength(2000)]
        //public string? MotivoSolicitud { get; set; }

        //public string? DiagnosticoInicial { get; set; }

        public string? Observaciones { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? ImporteCotizacion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el estatus de la solicitud.")]
        public int? EstatusSolicitudMantenimientoId { get; set; }

        public List<SelectListItem> Areas { get; set; }
            = new();

        public List<SelectListItem> UnidadesVehiculares { get; set; }
            = new();

        public List<SelectListItem> TiposMantenimiento { get; set; }
            = new();

        public List<SelectListItem> Talleres { get; set; }= new();

        public List<SelectListItem> Estatuses { get; set; }
            = new();

        public List<IFormFile>? Archivos { get; set; }


        //public List<SolicitudMantenimientoDocumentoViewModel> Documentos= new();


        [Required(ErrorMessage = "Debe seleccionar el estado del odómetro.")]
        public byte? EstadoOdometroId { get; set; }
        public IEnumerable<SelectListItem> EstadosOdometro { get; set; } = new List<SelectListItem>();

        public bool TieneDocumentos { get; set; }

        public int? TotalDocumentos { get; set; }

        public List<SolicitudMantenimientoDocumentoViewModel> Documentos { get; set; } = new();





        public List<SolicitudMantenimientoServicioViewModel> Servicios { get; set; }= new();

        public List<SelectListItem> ConceptosServicio { get; set; }= new();





        public IEnumerable<SelectListItem> ObjetosGasto { get; set; } = new List<SelectListItem>();



        public int? IdObjetoGastoManoObra { get; set; }

        public int? IdObjetoGastoRefacciones { get; set; }


        public int TipoCatalogoServicioId { get; set; }




        [Required(ErrorMessage = "Debe seleccionar el contrato/configuración presupuestal del taller.")]
        public int? TallerConfiguracionPresupuestalId { get; set; }

        public List<SelectListItem> ConfiguracionesTaller { get; set; } = new();


    }
}
