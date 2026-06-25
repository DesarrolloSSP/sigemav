using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.ViewModels.Financieros.Dsp;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;

namespace SIGEMAV.Services.Interfaces.Financieros
{
    public interface IDspService
    {
        Task<List<DspVM>> Obtener(int estado);

        Task<DspEditarVM?> ObtenerPorId(int id);

        Task<bool> Guardar(DspVM model, string rutaArchivo);

        Task<bool> Editar(DspEditarVM model, string? rutaArchivo);        

        Task Activar(int id);

        Task Desactivar(int id);

        Task<string?> VerPdf(int id);

    }
}
