using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class UnidadVehicularCreateViewModel
    {
        public int? IdUnidadVehicular { get; set; } = null;

        [Required(ErrorMessage = "El número de serie es obligatorio.")]
        [StringLength(30)]
        public string NumeroSerie { get; set; } = null!;

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(10)]
        public string PlacaActual { get; set; } = null!;

        [Required(ErrorMessage = "El número económico es obligatorio.")]
        [StringLength(15)]
        public string NumeroEconomico { get; set; } = null!;

        [Required(ErrorMessage = "Seleccione una marca.")]
        public int? IdMarca { get; set; }

        [Required(ErrorMessage = "Seleccione un modelo.")]
        public int? IdModelo { get; set; }

        [Required(ErrorMessage = "Seleccione un año.")]
        public int? IdAnio { get; set; }

        [Required(ErrorMessage = "Seleccione un color.")]
        public int? IdColor { get; set; }

        [Required(ErrorMessage = "Seleccione un municipio.")]
        public int? IdMunicipio { get; set; }


        [Required(ErrorMessage = "Seleccione una área.")]
        public int? IdArea { get; set; }

        [Required(ErrorMessage = "Seleccione un cilindro.")]
        public int? IdCilindro { get; set; }

        [Required(ErrorMessage = "Seleccione una transmisión.")]
        public int? IdTransmision { get; set; }


        // COMBOS

        public List<SelectListItem> Marcas { get; set; } = new();

        public List<SelectListItem> Modelos { get; set; } = new();

        public List<SelectListItem> Anios { get; set; } = new();

        public List<SelectListItem> Colores { get; set; } = new();

        public List<SelectListItem> Municipios { get; set; } = new();

        public List<SelectListItem> Areas { get; set; } = new();

        public List<SelectListItem> Cilindros { get; set; } = new();

        public List<SelectListItem> Transmisiones { get; set; } = new();
    }
}
