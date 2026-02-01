using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.GroupDTO;
using Entity.DataTransferObject.GroupmessageDTO;
using Entity.DataTransferObject.GroupMenmberDTO;
using Entity.TableModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IGroupMessageService _groupMessageService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GroupsController(
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IGroupMessageService groupMessageService,
            IUserService userService,
            IMapper mapper)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _groupMessageService = groupMessageService;
            _userService = userService;
            _mapper = mapper;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            return int.Parse(userIdClaim!.Value);
        }

        // GET: api/groups
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetUserId();
            
            // Get all groups where user is a member
            var allMembers = _groupMemberService.GetAll();
            if (!allMembers.IsSuccess)
                return BadRequest(new { message = allMembers.Message });

            var userGroupIds = allMembers.Data
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToList();

            var result = _groupService.GetAll();
            if (result.IsSuccess)
            {
                var userGroups = result.Data
                    .Where(g => userGroupIds.Contains(g.Id))
                    .ToList();
                
                var data = _mapper.Map<List<GroupListDTO>>(userGroups);
                
                // Get all users for member info
                var allUsers = _userService.GetAll();
                if (!allUsers.IsSuccess)
                    return BadRequest(new { message = allUsers.Message });
                
                // Set member count and members for each group
                foreach (var group in data)
                {
                    var groupMembers = allMembers.Data.Where(gm => gm.GroupId == group.Id).ToList();
                    group.MemberCount = groupMembers.Count;
                    
                    // Map members with user info
                    group.Members = groupMembers.Select(gm =>
                    {
                        var user = allUsers.Data.FirstOrDefault(u => u.Id == gm.UserId);
                        return new Entity.DataTransferObject.GroupMenmberDTO.GroupMemberListDTO
                        {
                            Id = gm.Id,
                            GroupId = gm.GroupId,
                            UserId = gm.UserId,
                            Username = user?.Username ?? "Unknown",
                            Avatar = user?.Avatar,
                            IsAdmin = gm.IsAdmin,
                            IsMuted = gm.IsMuted,
                            IsOnline = user?.IsOnline ?? false,
                            JoinedAt = gm.JoinedAt
                        };
                    }).ToList();
                }
                
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        // GET: api/groups/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var userId = GetUserId();
            
            // Check if user is a member
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var isMember = members.Data.Any(gm => gm.GroupId == id && gm.UserId == userId);
            if (!isMember)
                return Forbid();

            var result = _groupService.Get(id);
            if (result.IsSuccess)
            {
                var data = _mapper.Map<GroupListDTO>(result.Data);
                
                // Get all users for member info
                var allUsers = _userService.GetAll();
                if (!allUsers.IsSuccess)
                    return BadRequest(new { message = allUsers.Message });
                
                // Set member count and members
                var groupMembers = members.Data.Where(gm => gm.GroupId == id).ToList();
                data.MemberCount = groupMembers.Count;
                
                // Map members with user info
                data.Members = groupMembers.Select(gm =>
                {
                    var user = allUsers.Data.FirstOrDefault(u => u.Id == gm.UserId);
                    return new Entity.DataTransferObject.GroupMenmberDTO.GroupMemberListDTO
                    {
                        Id = gm.Id,
                        GroupId = gm.GroupId,
                        UserId = gm.UserId,
                        Username = user?.Username ?? "Unknown",
                        Avatar = user?.Avatar,
                        IsAdmin = gm.IsAdmin,
                        IsMuted = gm.IsMuted,
                        IsOnline = user?.IsOnline ?? false,
                        JoinedAt = gm.JoinedAt
                    };
                }).ToList();
                
                return Ok(data);
            }
            return NotFound(new { message = result.Message });
        }

        // POST: api/groups
        [HttpPost]
        public IActionResult Add([FromBody] GroupAddDTO groupAddDTO)
        {
            var userId = GetUserId();
            groupAddDTO.CreatedById = userId;

            var result = _groupService.Add(groupAddDTO);
            
            if (result.IsSuccess)
            {
                // Get the created group
                var allGroups = _groupService.GetAll();
                if (allGroups.IsSuccess)
                {
                    var createdGroup = allGroups.Data
                        .Where(g => g.CreatedById == userId)
                        .OrderByDescending(g => g.CreatedAt)
                        .FirstOrDefault();
                    
                    if (createdGroup != null)
                    {
                        // Add creator as owner/admin
                        var ownerMember = new GroupMemberAddDTO
                        {
                            GroupId = createdGroup.Id,
                            UserId = userId,
                            IsAdmin = true
                        };
                        _groupMemberService.Add(ownerMember);
                        
                        // Add selected members
                        if (groupAddDTO.MemberIds != null && groupAddDTO.MemberIds.Any())
                        {
                            foreach (var memberId in groupAddDTO.MemberIds)
                            {
                                if (memberId != userId) // Don't add creator twice
                                {
                                    var member = new GroupMemberAddDTO
                                    {
                                        GroupId = createdGroup.Id,
                                        UserId = memberId,
                                        IsAdmin = false
                                    };
                                    _groupMemberService.Add(member);
                                }
                            }
                        }
                        
                        var groupDto = _mapper.Map<GroupListDTO>(createdGroup);
                        
                        // Set member count
                        var allMembers = _groupMemberService.GetAll();
                        if (allMembers.IsSuccess)
                        {
                            groupDto.MemberCount = allMembers.Data.Count(gm => gm.GroupId == createdGroup.Id);
                        }
                        
                        return Ok(groupDto);
                    }
                }
                
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        // PUT: api/groups/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] GroupUpdateDTO groupUpdateDTO)
        {
            if (id != groupUpdateDTO.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = _groupService.Update(groupUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        // DELETE: api/groups/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            // Only owner can delete
            if (groupResult.Data.CreatedById != userId)
                return Forbid();

            var result = _groupService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = "Group deleted successfully" });
            }
            return NotFound(new { message = result.Message });
        }

        // POST: api/groups/{id}/leave
        [HttpPost("{id}/leave")]
        public IActionResult LeaveGroup(int id)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            // Owner cannot leave
            if (groupResult.Data.CreatedById == userId)
                return BadRequest(new { message = "Group owner cannot leave. Delete the group instead." });

            // Find and remove member
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var member = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (member == null)
                return NotFound(new { message = "You are not a member of this group" });

            var deleteResult = _groupMemberService.Delete(member.Id);
            if (deleteResult.IsSuccess)
            {
                return Ok(new { message = "Left group successfully" });
            }
            return BadRequest(new { message = deleteResult.Message });
        }

        // GET: api/groups/{id}/messages
        [HttpGet("{id}/messages")]
        public IActionResult GetGroupMessages(int id)
        {
            var userId = GetUserId();
            
            // Check if user is a member
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var isMember = members.Data.Any(gm => gm.GroupId == id && gm.UserId == userId);
            if (!isMember)
                return Forbid();

            var result = _groupMessageService.GetAll();
            if (result.IsSuccess)
            {
                var groupMessages = result.Data
                    .Where(gm => gm.GroupId == id)
                    .OrderBy(gm => gm.SentAt)
                    .ToList();
                
                var data = _mapper.Map<List<GroupmessageListDTO>>(groupMessages);
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/groups/{id}/messages
        [HttpPost("{id}/messages")]
        public IActionResult SendGroupMessage(int id, [FromBody] string content)
        {
            var userId = GetUserId();
            
            // Check if user is a member
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var member = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (member == null)
                return Forbid();

            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            bool isOwner = group.CreatedById == userId;
            bool isAdmin = member.IsAdmin;
            bool canModerate = isOwner || isAdmin;

            // Check for /muteall command
            if (content.Trim().Equals("/muteall", StringComparison.OrdinalIgnoreCase))
            {
                if (!canModerate)
                    return BadRequest(new { message = "Only admins can use this command" });

                if (group.IsMutedForAll)
                    return BadRequest(new { message = "Group is already muted for all members" });

                // Update group directly
                group.IsMutedForAll = true;
                var updateDto = new GroupUpdateDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Avatar = group.Avatar,
                    IsMutedForAll = true
                };
                _groupService.Update(updateDto);

                // Create system message
                var userResult = _userService.Get(userId);
                if (!userResult.IsSuccess)
                    return BadRequest(new { message = "User not found" });
                    
                var currentUser = userResult.Data;
                
                var systemMessageDto = new GroupmessageAddDTO
                {
                    GroupId = id,
                    SenderId = userId,
                    Content = $"Doktor bütün şəhərə narkoz vurdu... Hamı dərin yuxuya gedir. By @{currentUser.Username}",
                    IsSystemMessage = true
                };

                var msgResult = _groupMessageService.Add(systemMessageDto);
                if (!msgResult.IsSuccess)
                    return BadRequest(new { message = msgResult.Message });
                    
                return Ok(msgResult.Data);
            }

            // Check for /unmuteall command
            if (content.Trim().Equals("/unmuteall", StringComparison.OrdinalIgnoreCase))
            {
                if (!canModerate)
                    return BadRequest(new { message = "Only admins can use this command" });

                if (!group.IsMutedForAll)
                    return BadRequest(new { message = "Group is not muted for all members" });

                // Update group directly
                group.IsMutedForAll = false;
                var updateDto = new GroupUpdateDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Avatar = group.Avatar,
                    IsMutedForAll = false
                };
                _groupService.Update(updateDto);

                // Create system message
                var userResult = _userService.Get(userId);
                if (!userResult.IsSuccess)
                    return BadRequest(new { message = "User not found" });
                    
                var currentUser = userResult.Data;
                
                var systemMessageDto = new GroupmessageAddDTO
                {
                    GroupId = id,
                    SenderId = userId,
                    Content = $"Narkozun təsiri keçdi! Gözünüzü açın və danışın, kim sağ qalıb? By @{currentUser.Username}",
                    IsSystemMessage = true
                };

                var msgResult = _groupMessageService.Add(systemMessageDto);
                if (!msgResult.IsSuccess)
                    return BadRequest(new { message = msgResult.Message });
                    
                return Ok(msgResult.Data);
            }

            // Check if user is muted
            if (member.IsMuted)
                return BadRequest(new { message = "You are muted in this group and cannot send messages" });

            // Check if group is muted for all
            if (group.IsMutedForAll && !isOwner && !isAdmin)
                return BadRequest(new { message = "Group is muted. Only admins can send messages" });

            // Check for @username /mute command
            var muteMatch = System.Text.RegularExpressions.Regex.Match(content, @"@(\w+)\s+/mute", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (muteMatch.Success)
            {
                if (!canModerate)
                    return BadRequest(new { message = "Only admins can mute members" });

                var targetUsername = muteMatch.Groups[1].Value;
                var allUsers = _userService.GetAll();
                var targetUser = allUsers.Data.FirstOrDefault(u => u.Username == targetUsername);
                
                if (targetUser == null)
                    return BadRequest(new { message = $"User @{targetUsername} not found" });

                var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == targetUser.Id);
                if (targetMember == null)
                    return BadRequest(new { message = $"@{targetUsername} is not a member of this group" });

                if (targetUser.Id == group.CreatedById)
                    return BadRequest(new { message = "Cannot mute group owner" });

                if (targetMember.IsMuted)
                    return BadRequest(new { message = $"@{targetUsername} is already muted" });

                // Update member directly
                var memberUpdateDto = new GroupMemberUpdateDTO
                {
                    Id = targetMember.Id,
                    IsAdmin = targetMember.IsAdmin,
                    IsMuted = true
                };
                _groupMemberService.Update(memberUpdateDto);

                // Create system message
                var userResult = _userService.Get(userId);
                if (!userResult.IsSuccess)
                    return BadRequest(new { message = "User not found" });
                    
                var currentUser = userResult.Data;
                
                var systemMessageDto = new GroupmessageAddDTO
                {
                    GroupId = id,
                    SenderId = userId,
                    Content = $"Şəhər yatır, Mafiya oyaqdır. @{targetUsername} isə artıq danışmır. By @{currentUser.Username}",
                    IsSystemMessage = true
                };

                var msgResult = _groupMessageService.Add(systemMessageDto);
                if (!msgResult.IsSuccess)
                    return BadRequest(new { message = msgResult.Message });
                    
                return Ok(msgResult.Data);
            }

            // Check for @username /unmute command
            var unmuteMatch = System.Text.RegularExpressions.Regex.Match(content, @"@(\w+)\s+/unmute", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (unmuteMatch.Success)
            {
                if (!canModerate)
                    return BadRequest(new { message = "Only admins can unmute members" });

                var targetUsername = unmuteMatch.Groups[1].Value;
                var allUsers = _userService.GetAll();
                var targetUser = allUsers.Data.FirstOrDefault(u => u.Username == targetUsername);
                
                if (targetUser == null)
                    return BadRequest(new { message = $"User @{targetUsername} not found" });

                var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == targetUser.Id);
                if (targetMember == null)
                    return BadRequest(new { message = $"@{targetUsername} is not a member of this group" });

                if (!targetMember.IsMuted)
                    return BadRequest(new { message = $"@{targetUsername} is not muted" });

                // Update member directly
                var memberUpdateDto = new GroupMemberUpdateDTO
                {
                    Id = targetMember.Id,
                    IsAdmin = targetMember.IsAdmin,
                    IsMuted = false
                };
                _groupMemberService.Update(memberUpdateDto);

                // Create system message
                var userResult = _userService.Get(userId);
                if (!userResult.IsSuccess)
                    return BadRequest(new { message = "User not found" });
                    
                var currentUser = userResult.Data;
                
                var systemMessageDto = new GroupmessageAddDTO
                {
                    GroupId = id,
                    SenderId = userId,
                    Content = $"@{targetUsername} artık konuşabilir. Unmuted by @{currentUser.Username}",
                    IsSystemMessage = true
                };

                var msgResult = _groupMessageService.Add(systemMessageDto);
                if (!msgResult.IsSuccess)
                    return BadRequest(new { message = msgResult.Message });
                    
                return Ok(msgResult.Data);
            }

            // Send normal message
            var messageDto = new GroupmessageAddDTO
            {
                GroupId = id,
                SenderId = userId,
                Content = content,
                IsSystemMessage = false
            };

            var result = _groupMessageService.Add(messageDto);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/groups/{id}/members
        [HttpPost("{id}/members")]
        public IActionResult AddMember(int id, [FromBody] int newMemberId)
        {
            var userId = GetUserId();
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            // Check if user exists
            var userResult = _userService.Get(newMemberId);
            if (!userResult.IsSuccess)
                return NotFound(new { message = "User not found" });

            // Check if already a member
            var existingMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == newMemberId);
            if (existingMember != null)
                return BadRequest(new { message = "User is already a member" });

            var newMemberDto = new GroupMemberAddDTO
            {
                GroupId = id,
                UserId = newMemberId,
                IsAdmin = false
            };

            var result = _groupMemberService.Add(newMemberDto);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Member added to group" });
            }
            return BadRequest(new { message = result.Message });
        }

        // DELETE: api/groups/{id}/members/{memberId}
        [HttpDelete("{id}/members/{memberId}")]
        public IActionResult RemoveMember(int id, int memberId)
        {
            var userId = GetUserId();
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            // Cannot remove owner
            if (memberId == group.CreatedById)
                return BadRequest(new { message = "Cannot remove group owner" });

            var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == memberId);
            if (targetMember == null)
                return NotFound(new { message = "Member not found" });

            var result = _groupMemberService.Delete(targetMember.Id);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Member removed from group" });
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/groups/{id}/members/{memberId}/promote
        [HttpPost("{id}/members/{memberId}/promote")]
        public IActionResult PromoteToAdmin(int id, int memberId)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == memberId);
            if (targetMember == null)
                return NotFound(new { message = "Member not found" });

            // Update member directly
            var updateDto = new GroupMemberUpdateDTO
            {
                Id = targetMember.Id,
                IsAdmin = true,
                IsMuted = targetMember.IsMuted
            };
            
            var result = _groupMemberService.Update(updateDto);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Member promoted to admin" });
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/groups/{id}/members/{memberId}/demote
        [HttpPost("{id}/members/{memberId}/demote")]
        public IActionResult DemoteFromAdmin(int id, int memberId)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Only owner can demote
            if (group.CreatedById != userId)
                return Forbid();

            // Cannot demote owner
            if (memberId == group.CreatedById)
                return BadRequest(new { message = "Cannot demote group owner" });

            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == memberId);
            if (targetMember == null)
                return NotFound(new { message = "Member not found" });

            // Update member directly
            var updateDto = new GroupMemberUpdateDTO
            {
                Id = targetMember.Id,
                IsAdmin = false,
                IsMuted = targetMember.IsMuted
            };
            
            var result = _groupMemberService.Update(updateDto);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Admin privileges removed" });
            }
            return BadRequest(new { message = result.Message });
        }

        // POST: api/groups/{id}/members/{memberId}/mute
        [HttpPost("{id}/members/{memberId}/mute")]
        public IActionResult MuteMember(int id, int memberId)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == memberId);
            if (targetMember == null)
                return NotFound(new { message = "Member not found" });

            // Cannot mute owner
            if (memberId == group.CreatedById)
                return BadRequest(new { message = "Cannot mute group owner" });

            if (targetMember.IsMuted)
                return BadRequest(new { message = "Member is already muted" });

            // Update member directly
            var updateDto = new GroupMemberUpdateDTO
            {
                Id = targetMember.Id,
                IsAdmin = targetMember.IsAdmin,
                IsMuted = true
            };
            _groupMemberService.Update(updateDto);

            // Create system message
            var userResult = _userService.Get(userId);
            var targetUserResult = _userService.Get(memberId);
            if (!userResult.IsSuccess || !targetUserResult.IsSuccess)
                return BadRequest(new { message = "User not found" });
                
            var currentUser = userResult.Data;
            var targetUser = targetUserResult.Data;
            
            var systemMessageDto = new GroupmessageAddDTO
            {
                GroupId = id,
                SenderId = userId,
                Content = $"Şəhər yatır, Mafiya oyaqdır. @{targetUser.Username} isə artıq danışmır. By @{currentUser.Username}",
                IsSystemMessage = true
            };

            var msgResult = _groupMessageService.Add(systemMessageDto);
            if (!msgResult.IsSuccess)
                return BadRequest(new { message = msgResult.Message });
                
            return Ok(new { 
                message = "Member muted successfully", 
                systemMessage = msgResult.Data 
            });
        }

        // POST: api/groups/{id}/members/{memberId}/unmute
        [HttpPost("{id}/members/{memberId}/unmute")]
        public IActionResult UnmuteMember(int id, int memberId)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            var targetMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == memberId);
            if (targetMember == null)
                return NotFound(new { message = "Member not found" });

            if (!targetMember.IsMuted)
                return BadRequest(new { message = "Member is not muted" });

            // Update member directly
            var updateDto = new GroupMemberUpdateDTO
            {
                Id = targetMember.Id,
                IsAdmin = targetMember.IsAdmin,
                IsMuted = false
            };
            _groupMemberService.Update(updateDto);

            // Create system message
            var userResult = _userService.Get(userId);
            var targetUserResult = _userService.Get(memberId);
            var currentUser = userResult.Data;
            var targetUser = targetUserResult.Data;
            
            var systemMessageDto = new GroupmessageAddDTO
            {
                GroupId = id,
                SenderId = userId,
                Content = $"@{targetUser.Username} artık konuşabilir. Unmuted by @{currentUser.Username}",
                IsSystemMessage = true
            };

            var msgResult = _groupMessageService.Add(systemMessageDto);
            return Ok(new { 
                message = "Member unmuted successfully", 
                systemMessage = _mapper.Map<GroupmessageListDTO>(msgResult.Data) 
            });
        }

        // POST: api/groups/{id}/mute-all
        [HttpPost("{id}/mute-all")]
        public IActionResult MuteAll(int id)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            if (group.IsMutedForAll)
                return BadRequest(new { message = "Group is already muted for all members" });

            var updateDto = _mapper.Map<GroupUpdateDTO>(group);
            updateDto.IsMutedForAll = true;
            _groupService.Update(updateDto);

            // Create system message
            var userResult = _userService.Get(userId);
            var currentUser = userResult.Data;
            
            var systemMessageDto = new GroupmessageAddDTO
            {
                GroupId = id,
                SenderId = userId,
                Content = $"Doktor bütün şəhərə narkoz vurdu... Hamı dərin yuxuya gedir. By @{currentUser.Username}",
                IsSystemMessage = true
            };

            var msgResult = _groupMessageService.Add(systemMessageDto);
            return Ok(new { 
                message = "Group muted for all members", 
                systemMessage = _mapper.Map<GroupmessageListDTO>(msgResult.Data) 
            });
        }

        // POST: api/groups/{id}/unmute-all
        [HttpPost("{id}/unmute-all")]
        public IActionResult UnmuteAll(int id)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            if (group.CreatedById != userId && !currentMember.IsAdmin)
                return Forbid();

            if (!group.IsMutedForAll)
                return BadRequest(new { message = "Group is not muted for all members" });

            var updateDto = _mapper.Map<GroupUpdateDTO>(group);
            updateDto.IsMutedForAll = false;
            _groupService.Update(updateDto);

            // Create system message
            var userResult = _userService.Get(userId);
            var currentUser = userResult.Data;
            
            var systemMessageDto = new GroupmessageAddDTO
            {
                GroupId = id,
                SenderId = userId,
                Content = $"Narkozun təsiri keçdi! Gözünüzü açın və danışın, kim sağ qalıb? By @{currentUser.Username}",
                IsSystemMessage = true
            };

            var msgResult = _groupMessageService.Add(systemMessageDto);
            return Ok(new { 
                message = "Group unmuted for all members", 
                systemMessage = _mapper.Map<GroupmessageListDTO>(msgResult.Data) 
            });
        }

        // PUT: api/groups/{id}/avatar
        [HttpPut("{id}/avatar")]
        public IActionResult UpdateGroupAvatar(int id, [FromBody] string avatar)
        {
            var userId = GetUserId();
            
            var groupResult = _groupService.Get(id);
            if (!groupResult.IsSuccess)
                return NotFound(new { message = "Group not found" });

            var group = groupResult.Data;
            
            // Check if user is owner or admin
            var members = _groupMemberService.GetAll();
            if (!members.IsSuccess)
                return BadRequest(new { message = members.Message });

            var currentMember = members.Data.FirstOrDefault(gm => gm.GroupId == id && gm.UserId == userId);
            if (currentMember == null)
                return Forbid();

            bool isOwner = group.CreatedById == userId;
            bool isAdmin = currentMember.IsAdmin;

            if (!isOwner && !isAdmin)
                return Forbid();

            // Validate base64 format
            if (!string.IsNullOrWhiteSpace(avatar) && !avatar.StartsWith("data:image/"))
                return BadRequest(new { message = "Invalid image format. Must be base64 encoded image." });

            var updateDto = _mapper.Map<GroupUpdateDTO>(group);
            updateDto.Avatar = avatar;
            
            var result = _groupService.Update(updateDto);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Group avatar updated successfully" });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}
