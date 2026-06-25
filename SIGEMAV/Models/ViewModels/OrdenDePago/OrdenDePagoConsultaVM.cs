using SIGEMAV.Models.ViewModels.Financieros.Odp;

namespace SIGEMAV.Models.ViewModels.OrdeDePago
{
    public class OrdenDePagoConsultaVM
    {
        public string Folio { get; set; }

        public List<OrdenDePagoVM> Detalles { get; set; } = new();

    }
}
