using SIGEMAV.Models.ViewModels.Financieros.Proyecto;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IProyectoAreaService
    {

        Task<List<ProyectoAreaCrudVM>> Obtener();

        Task<ProyectoAreaCrudVM?> ObtenerPorId(int id);

        Task Guardar(ProyectoAreaCrudVM model);

        Task Actualizar(ProyectoAreaCrudVM model);

        Task Activar(int id);

        Task Desactivar(int id);

        Task<List<ProyectoComboVM>> ObtenerProyectos();

        Task<List<AreaComboVM>> ObtenerAreas();

    }
}
