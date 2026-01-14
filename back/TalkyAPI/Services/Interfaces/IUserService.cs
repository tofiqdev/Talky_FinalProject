using TalkyAPI.DTOs.User;

namespace TalkyAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers(int currentUserId);
        Task<UserDto?> GetUserById(int userId);
        Task<UserDto?> GetUserByUsername(string username);
        Task<List<UserDto>> SearchUsers(string searchTerm, int currentUserId);
        Task<bool> UpdateUserStatus(int userId, bool isOnline);
        Task<UserDto?> UpdateProfilePicture(int userId, string profilePictureBase64);
        Task<UserDto?> UpdateProfile(int userId, string username, string email);
    }
}
