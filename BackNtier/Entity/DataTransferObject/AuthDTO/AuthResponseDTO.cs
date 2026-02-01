using Entity.DataTransferObject.UserDTO;

namespace Entity.DataTransferObject.AuthDTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserListDTO User { get; set; } = null!;
    }
}
