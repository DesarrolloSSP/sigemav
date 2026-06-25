using SIGEMAV.Models.ViewModels.Financieros;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IObjetoGastoCatalogoService
    {

        Task<List<ObjetoGastoCrudVM>> Obtener();

        Task<ObjetoGastoCrudVM> ObtenerPorId(int id);

        Task Guardar(ObjetoGastoCrudVM model);

        Task Actualizar(ObjetoGastoCrudVM model);

        Task Desactivar(int id);

        Task Activar(int id);
    }
}
