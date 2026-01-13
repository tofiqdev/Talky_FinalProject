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
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.GroupId == id && gm.UserId == userId);

            if (!isMember)
                return Forbid();

            var message = new GroupMessage
            {
                GroupId = id,
                SenderId = userId,
                Content = content,
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
                SentAt = message.SentAt
            };

            return Ok(messageDto);
        }
    }
}
