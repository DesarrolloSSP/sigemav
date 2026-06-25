using SIGEMAV.Models.DTOs;

namespace SIGEMAV.Services.Interfaces.SIA
{
    public interface ISiaService
    {
        
        Task<LoginResponseDTO?> ValidarUsuarioAsync(string userName, string password);

    }

}
