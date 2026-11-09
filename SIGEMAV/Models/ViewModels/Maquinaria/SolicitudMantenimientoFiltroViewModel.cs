using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class SolicitudMantenimientoFiltroViewModel
    {
        public string? Folio { get; set; }

        public string? NumeroOficio { get; set; }

        public int? UnidadVehicularId { get; set; }

        public int? AreaSolicitanteId { get; set; }

        public int? EstatusSolicitudMantenimientoId { get; set; }

        public int? TipoMantenimientoId { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public List<SelectListItem> Unidades { get; set; } = new();

        public List<SelectListItem> Areas { get; set; } = new();

        public List<SelectListItem> Estatus { get; set; } = new();

        public List<SelectListItem> TiposMantenimiento { get; set; } = new();

        public string? Unidad { get; set; }


        public string? UnidadSeleccionada { get; set; }

        public bool? EsUsuarioDeAreaOperativa { get; set; }
    }
}
