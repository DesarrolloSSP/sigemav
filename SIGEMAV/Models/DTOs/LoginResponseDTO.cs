namespace SIGEMAV.Models.DTOs
{
    public class LoginResponseDTO
    {

        public bool Success { get; set; }

        public string? Mensaje { get; set; }

        public UsuarioAccesoDTO? Usuario { get; set; }


    }
}
