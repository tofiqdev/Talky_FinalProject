using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TalkyAPI.Data;
using TalkyAPI.DTOs.Group;
using TalkyAPI.Models;

namespace TalkyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        // GET: api/groups
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetMyGroups()
        {
            var userId = GetUserId();

            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.CreatedBy)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Members)
                        .ThenInclude(m => m.User)
                .Select(gm => gm.Group)
                .ToListAsync();

            var groupDtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Avatar = g.Avatar,
                CreatedById = g.CreatedById,
                CreatedByUsername = g.CreatedBy.Username,
                CreatedAt = g.CreatedAt,
                MemberCount = g.Members.Count,
                Members = g.Members.Select(m => new GroupMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    Avatar = m.User.Avatar,
                    IsAdmin = m.IsAdmin,
                    IsMuted = m.IsMuted,
                    IsOnline = m.User.IsOnline,
                    JoinedAt = m.JoinedAt
                }).ToList()
            }).ToList();

            return Ok(groupDtos);
        }

        // GET: api/groups/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var userId = GetUserId();

            var group = await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return NotFound("Group not found");

            // Check if user is a member
            if (!group.Members.Any(m => m.UserId == userId))
                return Forbid();

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Avatar = group.Avatar,
                CreatedById = group.CreatedById,
                CreatedByUsername = group.CreatedBy.Username,
                CreatedAt = group.CreatedAt,
                MemberCount = group.Members.Count,
                Members = group.Members.Select(m => new GroupMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    Avatar = m.User.Avatar,
                    IsAdmin = m.IsAdmin,
                    IsMuted = m.IsMuted,
                    IsOnline = m.User.IsOnline,
                    JoinedAt = m.JoinedAt
                }).ToList()
            };

            return Ok(groupDto);
        }

        // POST: api/groups
        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupDto dto)
        {
            var userId = GetUserId();

            // Validate member IDs
            var validMembers = await _context.Users
                .Where(u => dto.MemberIds.Contains(u.Id))
                .ToListAsync();

            if (validMembers.Count != dto.MemberIds.Count)
                return BadRequest("Some user IDs are invalid");

            // Create group
            var group = new Group
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add creator as admin
            var creatorMember = new GroupMember
            {
                GroupId = group.Id,
                UserId = userId,
                IsAdmin = true,
                JoinedAt = DateTime.UtcNow
            };
            _context.GroupMembers.Add(creatorMember);

            // Add other members
            foreach (var memberId in dto.MemberIds.Where(id => id != userId))
            {
                var member = new GroupMember
                {
                    GroupId = group.Id,
                    UserId = memberId,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                };
                _context.GroupMembers.Add(member);
            }

            await _context.SaveChangesAsync();

            // Reload group with members
            group = await _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstAsync(g => g.Id == group.Id);

            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Avatar = group.Avatar,
                CreatedById = group.CreatedById,
                CreatedByUsername = group.CreatedBy.Username,
                CreatedAt = group.CreatedAt,
                MemberCount = group.Members.Count,
                Members = group.Members.Select(m => new GroupMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    Username = m.User.Username,
                    Avatar = m.User.Avatar,
                    IsAdmin = m.IsAdmin,
                    IsMuted = m.IsMuted,
                    IsOnline = m.User.IsOnline,
                    JoinedAt = m.JoinedAt
                }).ToList()
            };

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, groupDto);
        }

        // GET: api/groups/{id}/messages
        [HttpGet("{id}/messages")]
        public async Task<ActionResult<List<GroupMessageDto>>> GetGroupMessages(int id)
        {
            var userId = GetUserId();

            // Check if user is a member
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (!isMember)
                return Forbid();

            var messages = await _context.GroupMessages
                .Where(gm => gm.GroupId == id)
                .Include(gm => gm.Sender)
                .OrderBy(gm => gm.SentAt)
                .Select(gm => new GroupMessageDto
                {
                    Id = gm.Id,
                    GroupId = gm.GroupId,
                    SenderId = gm.SenderId,
                    SenderUsername = gm.Sender.Username,
                    SenderAvatar = gm.Sender.Avatar,
                    Content = gm.Content,
                    IsSystemMessage = gm.IsSystemMessage,
                    SentAt = gm.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        // POST: api/groups/{id}/messages
        [HttpPost("{id}/messages")]
        public async Task<ActionResult<GroupMessageDto>> SendGroupMessage(int id, [FromBody] string content)
        {
            var userId = GetUserId();

            // Check if user is a member
            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (member == null)
                return Forbid();

            // Check if user is muted
            if (member.IsMuted)
                return BadRequest("You are muted in this group and cannot send messages");

            var message = new GroupMessage
            {
                GroupId = id,
                SenderId = userId,
                Content = content,
                IsSystemMessage = false,
                SentAt = DateTime.UtcNow
            };

            _context.GroupMessages.Add(message);
            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(userId);

            var messageDto = new GroupMessageDto
            {
                Id = message.Id,
                GroupId = message.GroupId,
                SenderId = message.SenderId,
                SenderUsername = sender!.Username,
                SenderAvatar = sender.Avatar,
                Content = message.Content,
                IsSystemMessage = false,
                SentAt = message.SentAt
            };

            return Ok(messageDto);
        }

        // POST: api/groups/{id}/members/{memberId}/promote
        [HttpPost("{id}/members/{memberId}/promote")]
        public async Task<ActionResult> PromoteToAdmin(int id, int memberId)
        {
            var userId = GetUserId();

            // Check if user is owner or admin
            var currentMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (currentMember == null)
                return Forbid();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found");

            // Only owner or admin can promote
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == memberId);

            if (targetMember == null)
                return NotFound("Member not found");

            targetMember.IsAdmin = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Member promoted to admin" });
        }

        // POST: api/groups/{id}/members/{memberId}/demote
        [HttpPost("{id}/members/{memberId}/demote")]
        public async Task<ActionResult> DemoteFromAdmin(int id, int memberId)
        {
            var userId = GetUserId();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found");

            // Only owner can demote
            if (group.CreatedById != userId)
                return Forbid();

            var targetMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == memberId);

            if (targetMember == null)
                return NotFound("Member not found");

            // Cannot demote owner
            if (memberId == group.CreatedById)
                return BadRequest("Cannot demote group owner");

            targetMember.IsAdmin = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin privileges removed" });
        }

        // DELETE: api/groups/{id}/members/{memberId}
        [HttpDelete("{id}/members/{memberId}")]
        public async Task<ActionResult> RemoveMember(int id, int memberId)
        {
            var userId = GetUserId();

            // Check if user is owner or admin
            var currentMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (currentMember == null)
                return Forbid();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found");

            // Only owner or admin can remove members
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            // Cannot remove owner
            if (memberId == group.CreatedById)
                return BadRequest("Cannot remove group owner");

            var targetMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == memberId);

            if (targetMember == null)
                return NotFound("Member not found");

            _context.GroupMembers.Remove(targetMember);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Member removed from group" });
        }

        // POST: api/groups/{id}/members
        [HttpPost("{id}/members")]
        public async Task<ActionResult> AddMember(int id, [FromBody] int newMemberId)
        {
            var userId = GetUserId();

            // Check if user is owner or admin
            var currentMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (currentMember == null)
                return Forbid();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found");

            // Only owner or admin can add members
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            // Check if user exists
            var newUser = await _context.Users.FindAsync(newMemberId);
            if (newUser == null)
                return NotFound("User not found");

            // Check if already a member
            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == newMemberId);

            if (existingMember != null)
                return BadRequest("User is already a member");

            var newMember = new GroupMember
            {
                GroupId = id,
                UserId = newMemberId,
                IsAdmin = false,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(newMember);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Member added to group" });
        }

        // DELETE: api/groups/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteGroup(int id)
        {
            var userId = GetUserId();
            Console.WriteLine($"Delete group request: GroupId={id}, UserId={userId}");

            var group = await _context.Groups
                .Include(g => g.Members)
                .Include(g => g.Messages)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                Console.WriteLine($"Group not found: {id}");
                return NotFound("Group not found");
            }

            Console.WriteLine($"Group found: {group.Name}, CreatedBy={group.CreatedById}");

            // Only owner can delete group
            if (group.CreatedById != userId)
            {
                Console.WriteLine($"Forbidden: User {userId} is not owner (owner is {group.CreatedById})");
                return Forbid();
            }

            try
            {
                // Delete all related data (cascade will handle GroupMembers and GroupMessages)
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Group deleted successfully: {id}");

                return Ok(new { message = "Group deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting group: {ex.Message}");
                return StatusCode(500, new { message = $"Error deleting group: {ex.Message}" });
            }
        }

        // POST: api/groups/{id}/leave
        [HttpPost("{id}/leave")]
        public async Task<ActionResult> LeaveGroup(int id)
        {
            var userId = GetUserId();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound("Group not found");

            // Owner cannot leave, must delete group or transfer ownership
            if (group.CreatedById == userId)
                return BadRequest("Group owner cannot leave. Delete the group instead.");

            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (member == null)
                return NotFound("You are not a member of this group");

            _context.GroupMembers.Remove(member);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Left group successfully" });
        }

        // POST: api/groups/{id}/members/{memberId}/mute
        [HttpPost("{id}/members/{memberId}/mute")]
        public async Task<ActionResult> MuteMember(int id, int memberId)
        {
            var userId = GetUserId();

            // Check if user is owner or admin
            var currentMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (currentMember == null)
                return Forbid();

            var group = await _context.Groups
                .Include(g => g.CreatedBy)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return NotFound("Group not found");

            // Only owner or admin can mute
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = await _context.GroupMembers
                .Include(gm => gm.User)
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == memberId);

            if (targetMember == null)
                return NotFound("Member not found");

            // Cannot mute owner
            if (memberId == group.CreatedById)
                return BadRequest("Cannot mute group owner");

            // Already muted
            if (targetMember.IsMuted)
                return BadRequest("Member is already muted");

            targetMember.IsMuted = true;
            await _context.SaveChangesAsync();

            // Create system message
            var currentUser = await _context.Users.FindAsync(userId);
            var systemMessage = new GroupMessage
            {
                GroupId = id,
                SenderId = userId,
                Content = $"Şəhər yatır, Mafiya oyaqdır. @{targetMember.User.Username} isə artıq danışmır. By @{currentUser!.Username}",
                IsSystemMessage = true,
                SentAt = DateTime.UtcNow
            };

            _context.GroupMessages.Add(systemMessage);
            await _context.SaveChangesAsync();

            var messageDto = new GroupMessageDto
            {
                Id = systemMessage.Id,
                GroupId = systemMessage.GroupId,
                SenderId = systemMessage.SenderId,
                SenderUsername = currentUser.Username,
                SenderAvatar = currentUser.Avatar,
                Content = systemMessage.Content,
                IsSystemMessage = true,
                SentAt = systemMessage.SentAt
            };

            return Ok(new { message = "Member muted successfully", systemMessage = messageDto });
        }

        // POST: api/groups/{id}/members/{memberId}/unmute
        [HttpPost("{id}/members/{memberId}/unmute")]
        public async Task<ActionResult> UnmuteMember(int id, int memberId)
        {
            var userId = GetUserId();

            // Check if user is owner or admin
            var currentMember = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (currentMember == null)
                return Forbid();

            var group = await _context.Groups
                .Include(g => g.CreatedBy)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
                return NotFound("Group not found");

            // Only owner or admin can unmute
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = await _context.GroupMembers
                .Include(gm => gm.User)
                .FirstOrDefaultAsync(gm => gm.GroupId == id && gm.UserId == memberId);

            if (targetMember == null)
                return NotFound("Member not found");

            // Not muted
            if (!targetMember.IsMuted)
                return BadRequest("Member is not muted");

            targetMember.IsMuted = false;
            await _context.SaveChangesAsync();

            // Create system message
            var currentUser = await _context.Users.FindAsync(userId);
            var systemMessage = new GroupMessage
            {
                GroupId = id,
                SenderId = userId,
                Content = $"@{targetMember.User.Username} artık konuşabilir. Unmuted by @{currentUser!.Username}",
                IsSystemMessage = true,
                SentAt = DateTime.UtcNow
            };

            _context.GroupMessages.Add(systemMessage);
            await _context.SaveChangesAsync();

            var messageDto = new GroupMessageDto
            {
                Id = systemMessage.Id,
                GroupId = systemMessage.GroupId,
                SenderId = systemMessage.SenderId,
                SenderUsername = currentUser.Username,
                SenderAvatar = currentUser.Avatar,
                Content = systemMessage.Content,
                IsSystemMessage = true,
                SentAt = systemMessage.SentAt
            };

            return Ok(new { message = "Member unmuted successfully", systemMessage = messageDto });
        }
    }
}
