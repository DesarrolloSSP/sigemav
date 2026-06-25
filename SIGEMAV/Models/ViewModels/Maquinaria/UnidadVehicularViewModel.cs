using System.Globalization;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularViewModel
    {
        public int IdUnidadVehicular { get; set; }
        public string? PlacaActual { get; set; }
     
        public string? NumeroSerie { get; set; }
        public string? NumeroEconomico { get; set; }
        public int IdMarca { get; set; }
        public string? MarcaNombre { get; set; }
        public int IdModelo { get; set; }
        public string? ModeloNombre { get; set; }
        public int IdAnio { get; set; }
        public string? AnioNombre { get; set; }
        public int IdColor { get; set; }
        public string? ColorNombre { get; set; }
        public int IdMunicipio { get; set; }
        public string? MunicipioNombre { get; set; }
        public int IdArea { get; set; }
        public string? AreaNombre { get; set; }
        public int IdCilindro { get; set; }
        public string? CilindroNombre { get; set; }
        public int IdTransmision { get; set; }
        public string? TransmisionNombre { get; set; } 

        public bool? Activo { get; set; }
    }






}
