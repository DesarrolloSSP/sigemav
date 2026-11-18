namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TransmisionIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.TransmisionDto> Transmisiones { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.TransmisionDto>();
    }
}
