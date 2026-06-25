using Microsoft.AspNetCore.Mvc.Rendering;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    //public class RevisionSolicitudViewModel
    //{
    //    public int SolicitudMantenimientoId { get; set; }

    //    public string Folio { get; set; }

    //    public string AreaSolicitante { get; set; }

    //    public string Vehiculo { get; set; }

    //    public string Taller { get; set; }

    //    public decimal ImporteCotizacion { get; set; }

    //    public List<SolicitudMantenimientoDetalleViewModel>
    //        Conceptos
    //    { get; set; } = new();

    //    public string Observaciones { get; set; }
    //}



    public class RevisionSolicitudViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        public string Folio { get; set; }

        public string AreaSolicitante { get; set; }

        public string Vehiculo { get; set; }

        public string Taller { get; set; }

        public decimal ImporteCotizacion { get; set; }

        public string Observaciones { get; set; }

        public int? TipoRechazoSolicitudId { get; set; }

        public bool PermiteCorreccion { get; set; }

        public List<SelectListItem> TiposRechazo { get; set; } = new();

        public List<SolicitudMantenimientoDetalleViewModel>Conceptos{ get; set; }= new();

        public bool YaProcesada { get; set; }
        public bool yaAprobadaDSP { get; set; }
        public string? MensajeEstatus { get; set; }

        public int? EstatusSolicitudMantenimientoId { get; set; }


        public List<RevisionHistoricoDto>  Historico { get; set; } = new();
        public RevisionResumenHistoricoDto ResumenHistorico { get; set; }=new();

        public List<HistorialTecnicoUnidadViewModel> HistorialTecnicoUnidad { get; set; } = new();

        public int Kilometraje { get; set; }

    }

}
