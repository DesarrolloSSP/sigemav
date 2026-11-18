namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class CilindroIndexViewModel
    {
        public IEnumerable<SIGEMAV.Areas.Maquinaria.DTOs.CilindroDto> Cilindros { get; set; } = new List<SIGEMAV.Areas.Maquinaria.DTOs.CilindroDto>();
    }
}
