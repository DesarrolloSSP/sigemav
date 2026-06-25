namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularEliminarViewModel
    {
        public int UnidadVehicularId { get; set; }

        public string NumeroEconomico { get; set; } = string.Empty;

        public string PlacaActual { get; set; } = string.Empty;

        public string NumeroSerie { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public int Anio { get; set; }
    }
}
