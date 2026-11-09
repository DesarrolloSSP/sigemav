using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIGEMAV.Areas.Maquinaria.DTOs;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class FirmanteIndexViewModel
    {
        public List<FirmanteDto> Firmantes { get; set; } = new List<FirmanteDto>();
        public int? AreaFiltroId { get; set; }

        [ValidateNever]
        public SelectList? Areas { get; set; }
    }

    public class FirmanteFormViewModel
    {
        public FirmanteDto Firmante { get; set; } = new FirmanteDto();

        [ValidateNever]
        public SelectList? Areas { get; set; }

        [ValidateNever]
        public SelectList? ClavesPuesto { get; set; }
    }
}
