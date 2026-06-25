using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class DSPViewModel
    {
        public int SolicitudMantenimientoId { get; set; }

        public string Folio { get; set; }

        public int AreaId { get; set; }

        public string Area { get; set; }

        public decimal ImporteSolicitud { get; set; }

        public int IdDispPresupuestal { get; set; }

        public string? Observaciones { get; set; }

        public decimal PresupuestoAsignado { get; set; }

        public decimal PresupuestoComprometido { get; set; }

        public decimal PresupuestoDevengado { get; set; }

        public decimal PresupuestoDisponible { get; set; }

        public IEnumerable<SelectListItem> Disponibilidades = new List<SelectListItem>();

        public bool TieneSuficienciaPresupuestal { get; set; }
        public List<DSPDisponibleViewModel> DSPs { get; set; } = new();

        public int IdObjetoGasto { get; set; }

        public string? ObjetoGastoDescripcion { get; set; }

        public List<DSPPartidaSolicitudViewModel> PartidasSolicitud { get; set; } = new();

        public List<DSPDisponibleViewModel> PartidasDSP { get; set; } = new();

        public int EstatusSolicitudMantenimientoId { get; set; }

        public List<DSPDisponibleViewModel> AutorizadoPorArea { get; set; } = new();

        public decimal ImporteLiberado { get; set; }

        public bool FueDescomprometida { get; set; }


        public bool EstaRechazada { get; set; }


        public bool ViolaPolitica20Porciento { get; set; }

        public int? TallerId { get; set; }

        public string? Taller { get; set; }

        public decimal TallerMinimo { get; set; }
        public decimal TallerMaximo { get; set; }


        public decimal PresupuestoTaller { get; set; }

        public decimal ComprometidoTaller { get; set; }

        public decimal EjercidoTaller { get; set; }

        public decimal DisponibleTaller { get; set; }

        public decimal MontoMinimoTaller { get; set; }

        public decimal MontoMaximoTaller { get; set; }

        public bool TieneDisponibilidadTaller { get; set; }

        public decimal TallerDisponible { get; set; }

        public decimal TallerAsignado { get; set; }

        public decimal TallerComprometido { get; set; }

        public decimal TallerEjercido { get; set; }


    }
}

