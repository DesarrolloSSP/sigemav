using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Models.ViewModels.OrdenDePago;

namespace SIGEMAV.Models.ViewModels.OrdeDePago
{
    public class OrdenDePagoConsultaVM
    {
        //public string Folio { get; set; }

        //public List<OrdenDePagoVM> Detalles { get; set; } = new();

        public string NumeroOficio { get; set; } = string.Empty;

        public OrdenDePagoEncabezadoVM? Encabezado { get; set; }

        public List<OrdenDePagoVM> Detalles { get; set; } = new();



    }


}
