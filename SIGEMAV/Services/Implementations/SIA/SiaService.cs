using SIGEMAV.Models.DTOs;
using SIGEMAV.Services.Interfaces.SIA;

namespace SIGEMAV.Services.Implementations.SIA
{
    public class SiaService : ISiaService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SiaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _httpClient.BaseAddress = new Uri(_configuration["SiaApi:BaseUrl"]!);
        }



        public async Task<LoginResponseDTO?> ValidarUsuarioAsync(string userName, string password)
        {
            var request = new LoginRequestDTO
            {
                UserName = userName,
                Password = password,
                Sistema = _configuration["SistemaActual"]!
            };

            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/acceso/validar",
                    request);

            var resultado =
                await response.Content
                    .ReadFromJsonAsync<LoginResponseDTO>();

            return resultado;
        }

    }


}
