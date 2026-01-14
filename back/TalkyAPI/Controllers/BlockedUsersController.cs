using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalkyAPI.Data;
using TalkyAPI.Models;

namespace TalkyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BlockedUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlockedUsersController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        // GET: api/blockedusers
        [HttpGet]
        public async Task<ActionResult<List<object>>> GetBlockedUsers()
        {
            var userId = GetUserId();

            var blockedUsers = await _context.BlockedUsers
                .Where(bu => bu.UserId == userId)
                .Include(bu => bu.BlockedUserNavigation)
                .Select(bu => new
                {
                    id = bu.Id,
                    userId = bu.BlockedUserId,
                    username = bu.BlockedUserNavigation.Username,
                    email = bu.BlockedUserNavigation.Email,
                    avatar = bu.BlockedUserNavigation.Avatar,
                    blockedAt = bu.BlockedAt
                })
                .ToListAsync();

            return Ok(blockedUsers);
        }

        // POST: api/blockedusers/{blockedUserId}
        [HttpPost("{blockedUserId}")]
        public async Task<ActionResult> BlockUser(int blockedUserId)
        {
            var userId = GetUserId();

            // Cannot block yourself
            if (userId == blockedUserId)
                return BadRequest("You cannot block yourself");

            // Check if user exists
            var userToBlock = await _context.Users.FindAsync(blockedUserId);
            if (userToBlock == null)
                return NotFound("User not found");

            // Check if already blocked
            var existingBlock = await _context.BlockedUsers
                .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BlockedUserId == blockedUserId);

            if (existingBlock != null)
                return BadRequest("User is already blocked");

            var blockedUser = new BlockedUser
            {
                UserId = userId,
                BlockedUserId = blockedUserId,
                BlockedAt = DateTime.UtcNow
            };

            _context.BlockedUsers.Add(blockedUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"User {userToBlock.Username} has been blocked" });
        }

        // DELETE: api/blockedusers/{blockedUserId}
        [HttpDelete("{blockedUserId}")]
        public async Task<ActionResult> UnblockUser(int blockedUserId)
        {
            var userId = GetUserId();

            var blockedUser = await _context.BlockedUsers
                .FirstOrDefaultAsync(bu => bu.UserId == userId && bu.BlockedUserId == blockedUserId);

            if (blockedUser == null)
                return NotFound("User is not blocked");

            _context.BlockedUsers.Remove(blockedUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User has been unblocked" });
        }

        // GET: api/blockedusers/check/{userId}
        [HttpGet("check/{userId}")]
        public async Task<ActionResult<object>> CheckIfBlocked(int userId)
        {
            var currentUserId = GetUserId();

            // Check if current user blocked the other user
            var isBlocked = await _context.BlockedUsers
                .AnyAsync(bu => bu.UserId == currentUserId && bu.BlockedUserId == userId);

            // Check if current user is blocked by the other user
            var isBlockedBy = await _context.BlockedUsers
                .AnyAsync(bu => bu.UserId == userId && bu.BlockedUserId == currentUserId);

            return Ok(new
            {
                isBlocked = isBlocked,
                isBlockedBy = isBlockedBy
            });
        }
    }
}
