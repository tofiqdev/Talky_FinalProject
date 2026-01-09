using Microsoft.EntityFrameworkCore;
using TalkyAPI.Data;
using TalkyAPI.DTOs.User;
using TalkyAPI.Services.Interfaces;

namespace TalkyAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetAllUsers(int currentUserId)
        {
            var users = await _context.Users
                .Where(u => u.Id != currentUserId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    Bio = u.Bio,
                    IsOnline = u.IsOnline,
                    LastSeen = u.LastSeen,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserDto?> GetUserById(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                Bio = user.Bio,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto?> GetUserByUsername(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                Bio = user.Bio,
                IsOnline = user.IsOnline,
                LastSeen = user.LastSeen,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<List<UserDto>> SearchUsers(string searchTerm, int currentUserId)
        {
            var users = await _context.Users
                .Where(u => u.Id != currentUserId &&
                           (u.Username.ToLower().Contains(searchTerm.ToLower()) ||
                            u.Email.ToLower().Contains(searchTerm.ToLower())))
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Avatar = u.Avatar,
                    Bio = u.Bio,
                    IsOnline = u.IsOnline,
                    LastSeen = u.LastSeen,
                    CreatedAt = u.CreatedAt
                })
                .Take(10) // Limit results
                .ToListAsync();

            return users;
        }

        public async Task<bool> UpdateUserStatus(int userId, bool isOnline)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            user.IsOnline = isOnline;
            user.LastSeen = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
