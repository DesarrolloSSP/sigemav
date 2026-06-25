using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IProyectoService
    {
        Task<List<ProyectoCrudVM>> Obtener();

        Task<ProyectoCrudVM?> ObtenerPorId(int id);

        Task Guardar(ProyectoCrudVM model);

        Task Actualizar(ProyectoCrudVM model);

        Task Activar(int id);

        Task Desactivar(int id);

        Task<List<ClaveAdministrativaComboVM>> ObtenerClavesAdministrativas();

    }

}
