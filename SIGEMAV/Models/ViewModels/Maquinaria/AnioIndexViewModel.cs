namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class AnioIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.AnioDto> Anios { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.AnioDto>();
    }
}
