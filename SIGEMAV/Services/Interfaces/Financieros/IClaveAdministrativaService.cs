using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IClaveAdministrativaService
    {

        Task<List<ClaveAdministrativaCrudVM>> Obtener();

        Task<ClaveAdministrativaCrudVM?> ObtenerPorId(int id);

        Task<bool> Guardar(ClaveAdministrativaCrudVM model);


        Task Actualizar(ClaveAdministrativaCrudVM model);

        Task Activar(int id);

        Task Desactivar(int id);
    }
}
