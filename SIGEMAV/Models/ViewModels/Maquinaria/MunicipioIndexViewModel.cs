namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MunicipioIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.MunicipioDto> Municipios { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.MunicipioDto>();
    }
}
