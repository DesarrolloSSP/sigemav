namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularDetailsViewModel
    {
        public int UnidadVehicularId { get; set; }

        public string NumeroSerie { get; set; }

        public string PlacaActual { get; set; }

        public string NumeroEconomico { get; set; }

        public string MarcaNombre { get; set; }

        public string ModeloNombre { get; set; }

        public string AnioNombre { get; set; }

        public string TransmisionNombre { get; set; }

        public string CilindroNombre { get; set; }

        public string AreaNombre { get; set; }

        public string ColorNombre { get; set; }

        public string MunicipioNombre { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; }
    }
}
