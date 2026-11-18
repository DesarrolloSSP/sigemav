namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class MarcaFormViewModel
    {
        public SIGEMAV.Areas.Maquinaria.DTOs.MarcaCreateUpdateDto Marca { get; set; } = new();
        public bool IsEdit => Marca.MarcaId > 0;

    }
}
