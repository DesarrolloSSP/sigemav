namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MarcaIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.MarcaDto> Marcas { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.MarcaDto>();
    }
}
