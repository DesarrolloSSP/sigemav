using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros.Odp;
using SIGEMAV.Models.ViewModels.OrdeDePago;
using SIGEMAV.Models.ViewModels.OrdenDePago;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IOrdenDePago
    {
        Task<OrdenDePagoEncabezadoVM?> ObtenerEncabezado(string numeroOficio);

        Task<List<OrdenDePagoVM>> Obtener(string folio);

        Task<byte[]> GenerarExcel(string id);



    }

}
