using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    
    public class MantenimientoCreateViewModel
    {
        
        public int UnidadVehicularId { get; set; }

        public string? UnidadTexto { get; set; }

        public int TipoMantenimientoId { get; set; }

        public int TallerId { get; set; }

        public DateTime FechaIngreso { get; set; }
            = DateTime.Now;

        public DateTime? FechaSalida { get; set; }

        public int KilometrajeEntrada { get; set; }

        public int? KilometrajeSalida { get; set; }

        public string MotivoMantenimiento { get; set; }
            = string.Empty;

        public string? Diagnostico { get; set; }

        public string? TrabajoRealizado { get; set; }

        public string? Observaciones { get; set; }

        public List<SelectListItem> UnidadesVehiculares
            = new();

        public List<SelectListItem> TiposMantenimiento
            = new();

        public List<SelectListItem> Talleres
            = new();

        public int SolicitudMantenimientoId { get; set; }

        public string? UnidadVehicularTexto { get; set; }

        public string? Folio { get; set; }

        public string? NumeroOficio { get; set; }

        public List<MantenimientoServicioViewModel> Servicios { get; set; }
    = new();

        public string? Taller { get; set; }

    }

}
