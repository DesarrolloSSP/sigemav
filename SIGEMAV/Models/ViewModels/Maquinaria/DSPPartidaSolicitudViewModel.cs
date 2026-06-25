using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class DSPPartidaSolicitudViewModel
    {
        public int IdObjetoGasto { get; set; }

        public string ClaveObjetoGasto { get; set; } = string.Empty;

        public string ObjetoGastoDescripcion { get; set; } = string.Empty;

        public decimal ImporteSolicitado { get; set; }

        public decimal DisponibleDSP { get; set; }

        public bool TieneSuficiencia { get; set; }

        public List<SelectListItem> DSPDisponibles { get; set; } = new();

        public int? IdDispPresupuestalSeleccionado { get; set; }

        public decimal AutorizadoPorArea { get; set; }


        public decimal ImporteLiberado { get; set; }

        public bool FueDescomprometida { get; set; }


        public bool EstaRechazada { get; set; }

        public string HistorialPresupuestal { get; set; } = string.Empty;


        public decimal DisponiblePosterior { get; set; }

        public decimal LimiteMinimo20Porciento { get; set; }

        public decimal PorcentajeDisponiblePosterior { get; set; }

        public bool ViolaPolitica20Porciento { get; set; }

    }
}
