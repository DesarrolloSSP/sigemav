using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Models.ViewModels.OrdeDePago;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IOrdenDePago
    {
        Task<List<OrdenDePagoVM>> Obtener(string folio);

        Task<byte[]> GenerarExcel(string id);



    }

}
