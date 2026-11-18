using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Areas.Maquinaria.DTOs;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ModeloIndexViewModel
    {
        public IEnumerable<ModeloDto> Modelos { get; set; } = new List<ModeloDto>();

        // Propiedad que faltaba para alimentar los select de marcas en las modales
        public SelectList MarcasSelectList { get; set; } = null!;
        //public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.ModeloDto> Modelos { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.ModeloDto>();
    }
}
