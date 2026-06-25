using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros;
using SIGEMAV.Models.ViewModels.Financieros.Disponibilidad;
using SIGEMAV.Models.ViewModels.Financieros.Dsp;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IDispService
    {
        Task<List<AreasVM>> ObtenerAreasPorProyecto(int idProyecto);
        Task<List<ClaveAdministrativaVM>> ObtenerClaveAdministrativa();
        Task<List<ObjetoGastoVM>> ObtenerTipoGasto();

        Task<List<CatalogoDsp>> ObtenerListadoDsp();

        Task ActualizarDsp(DispEditarVM model, string? rutaArchivo);

        Task<IActionResult> Activar(int id);

        Task<IActionResult> Desactivar(int id);

        Task<IActionResult> Editar(int id);

        Task<List<ProyectoVM>> ObtenerProyecto(int idClaveAdmin);

        Task<DispEditarVM?> ObtenerPorId(int id);

        //Task GuardarDisp(DispCrearVM model);

        Task<bool> GuardarDisp(DispCrearVM model);

        Task<List<DispListadoVM>> ObtenerDsp();


        Task<IActionResult> VerPdf(int id);

        




    }
}
