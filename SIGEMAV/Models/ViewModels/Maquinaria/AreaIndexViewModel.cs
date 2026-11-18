namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class AreaIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.AreaDto> Areas { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.AreaDto>();
    }
}
