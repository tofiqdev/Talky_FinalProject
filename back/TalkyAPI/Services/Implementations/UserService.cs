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
            // Get blocked user IDs (users blocked by current user and users who blocked current user)
            var blockedUserIds = await _context.BlockedUsers
                .Where(bu => bu.UserId == currentUserId || bu.BlockedUserId == currentUserId)
                .Select(bu => bu.UserId == currentUserId ? bu.BlockedUserId : bu.UserId)
                .ToListAsync();

            // Get contact user IDs
            var contactUserIds = await _context.Contacts
                .Where(c => c.UserId == currentUserId)
                .Select(c => c.ContactUserId)
                .ToListAsync();

            // Only return contacts (not blocked)
            var users = await _context.Users
                .Where(u => contactUserIds.Contains(u.Id) && !blockedUserIds.Contains(u.Id))
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
            // Get blocked user IDs
            var blockedUserIds = await _context.BlockedUsers
                .Where(bu => bu.UserId == currentUserId || bu.BlockedUserId == currentUserId)
                .Select(bu => bu.UserId == currentUserId ? bu.BlockedUserId : bu.UserId)
                .ToListAsync();

            var users = await _context.Users
                .Where(u => u.Id != currentUserId &&
                           !blockedUserIds.Contains(u.Id) &&
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

        public async Task<UserDto?> UpdateProfilePicture(int userId, string profilePictureBase64)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            user.Avatar = profilePictureBase64;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

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

        public async Task<UserDto?> UpdateProfile(int userId, string username, string email)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            // Check if username is already taken by another user
            var existingUserWithUsername = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.Id != userId);

            if (existingUserWithUsername != null)
                throw new InvalidOperationException("Username is already taken");

            // Check if email is already taken by another user
            var existingUserWithEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Id != userId);

            if (existingUserWithEmail != null)
                throw new InvalidOperationException("Email is already taken");

            user.Username = username;
            user.Email = email;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

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
    }
}
