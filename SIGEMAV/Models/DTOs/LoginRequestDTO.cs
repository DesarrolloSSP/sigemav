namespace SIGEMAV.Models.DTOs
{

    public class LoginRequestDTO
    {

        public int UserId { get; set; }
        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string Sistema { get; set; } = string.Empty;

    }


}
