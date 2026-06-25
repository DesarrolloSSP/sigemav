using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;
using SIGEMAV.Models.ViewModels.Financieros.Odp;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IOdpService
    {

        Task<List<OrdenPagoVM>> Obtener();
        Task<OrdenPagoVM> ObtenerPorId(int id);
        Task<bool> Guardar(OrdenPagoVM model);
        Task<bool> Editar(OrdenPagoVM model);
        Task<bool> Eliminar(int id);
        Task<OrdenPagoVM> InicializarFormulario(OrdenPagoVM? model = null);
        Task<bool> Cancelar(int id);
        Task<bool> Reactivar(int id);
        Task<byte[]> GenerarExcel(int id);

    }

}
