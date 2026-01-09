using TalkyAPI.DTOs.Auth;

namespace TalkyAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> Register(RegisterDto registerDto);
        Task<AuthResponseDto?> Login(LoginDto loginDto);
        Task<DTOs.User.UserDto?> GetCurrentUser(int userId);
    }
}
