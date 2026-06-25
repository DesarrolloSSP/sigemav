using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{

    public class SolicitudMantenimientoEditViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Folio { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string NumeroOficio { get; set; } = null!;

        [Required]
        public DateTime FechaSolicitud { get; set; }


        public DateTime FechaRegistro { get; set; }


        [Required]
        public int AreaSolicitanteId { get; set; }

        [Required]
        public int UnidadVehicularId { get; set; }

        [Required]
        public int TallerId { get; set; }

        [Required]
        public int TipoMantenimientoId { get; set; }

        [Required]
        public decimal KilometrajeActual { get; set; }

        [StringLength(2000)]
        public string? MotivoSolicitud { get; set; }

        public string? DiagnosticoInicial { get; set; }

        public string? Observaciones { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? ImporteCotizacion { get; set; }

        [Required]
        public int EstatusSolicitudMantenimientoId { get; set; }

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



        public bool TieneDocumentos { get; set; }

        public int TotalDocumentos { get; set; }

        public List<SolicitudMantenimientoDocumentoViewModel> Documentos { get; set; } = new();





        public List<SolicitudMantenimientoServicioViewModel> Servicios { get; set; }= new();

        public List<SelectListItem> ConceptosServicio { get; set; }= new();





        public IEnumerable<SelectListItem> ObjetosGasto { get; set; } = new List<SelectListItem>();



        public int? IdObjetoGastoManoObra { get; set; }

        public int? IdObjetoGastoRefacciones { get; set; }


        public int TipoCatalogoServicioId { get; set; }

    }
}
