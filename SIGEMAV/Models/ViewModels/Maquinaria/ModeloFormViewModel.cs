namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class ModeloFormViewModel
    {
        public SIGEMAV.Areas.Maquinaria.DTOs.ModeloCreateUpdateDto Modelo { get; set; } = new();
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> MarcasSelectList { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
        public bool IsEdit => Modelo.ModeloId > 0;
    }
}
