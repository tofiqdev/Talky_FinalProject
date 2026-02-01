using AutoMapper;
using BLL.Abstrack;
using Entity.DataTransferObject.MessageDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talky_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public MessageController(IMessageService messageService, IUserService userService, IMapper mapper)
        {
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _messageService.GetAll();
            if (result.IsSuccess)
            {
                var data = _mapper.Map<List<MessageListDTO>>(result.Data);
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpGet("{userId}")]
        public IActionResult GetConversation(int userId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = _messageService.GetAll();
            if (result.IsSuccess)
            {
                var conversation = result.Data
                    .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                               (m.SenderId == userId && m.ReceiverId == currentUserId))
                    .OrderBy(m => m.SentAt)
                    .ToList();
                
                // Enrich with sender/receiver usernames
                var userResult = _userService.GetAll();
                if (userResult.IsSuccess)
                {
                    foreach (var msg in conversation)
                    {
                        var sender = userResult.Data.FirstOrDefault(u => u.Id == msg.SenderId);
                        var receiver = userResult.Data.FirstOrDefault(u => u.Id == msg.ReceiverId);
                        
                        if (sender != null) msg.SenderName = sender.Username;
                        if (receiver != null) msg.ReceiverName = receiver.Username;
                    }
                }
                
                return Ok(conversation);
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPost]
        public IActionResult Add([FromBody] MessageAddDTO messageAddDTO)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Ensure sender is the authenticated user
            messageAddDTO.SenderId = currentUserId;

            var result = _messageService.Add(messageAddDTO);
            
            if (result.IsSuccess)
            {
                // Get the created message
                var messages = _messageService.GetAll();
                if (messages.IsSuccess)
                {
                    var createdMessage = messages.Data
                        .Where(m => m.SenderId == messageAddDTO.SenderId && 
                                   m.ReceiverId == messageAddDTO.ReceiverId)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault();
                    
                    if (createdMessage != null)
                    {
                        var messageDto = _mapper.Map<MessageListDTO>(createdMessage);
                        
                        // Enrich with usernames
                        var userResult = _userService.GetAll();
                        if (userResult.IsSuccess)
                        {
                            var sender = userResult.Data.FirstOrDefault(u => u.Id == createdMessage.SenderId);
                            var receiver = userResult.Data.FirstOrDefault(u => u.Id == createdMessage.ReceiverId);
                            
                            if (sender != null) messageDto.SenderName = sender.Username;
                            if (receiver != null) messageDto.ReceiverName = receiver.Username;
                        }
                        
                        return Ok(messageDto);
                    }
                }
                
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{messageId}/read")]
        public IActionResult MarkAsRead(int messageId)
        {
            var result = _messageService.Get(messageId);
            if (result.IsSuccess)
            {
                var message = result.Data;
                var updateDto = new MessageUpdateDTO
                {
                    Id = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    IsRead = true,
                    SentAt = message.SentAt,
                    ReadAt = DateTime.UtcNow
                };
                
                var updateResult = _messageService.Update(updateDto);
                
                if (updateResult.IsSuccess)
                {
                    return NoContent();
                }
                return BadRequest(new { message = updateResult.Message });
            }
            return NotFound(new { message = result.Message });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MessageUpdateDTO messageUpdateDTO)
        {
            if (id != messageUpdateDTO.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var result = _messageService.Update(messageUpdateDTO);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _messageService.Delete(id);
            
            if (result.IsSuccess)
            {
                return Ok(new { message = result.Message });
            }
            return NotFound(new { message = result.Message });
        }

        [HttpGet("unread")]
        public IActionResult GetUnreadMessages()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("sub");
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var result = _messageService.GetAll();
            if (result.IsSuccess)
            {
                var unreadMessages = result.Data
                    .Where(m => m.ReceiverId == userId && !m.IsRead)
                    .OrderByDescending(m => m.SentAt)
                    .ToList();
                
                var data = _mapper.Map<List<MessageListDTO>>(unreadMessages);
                return Ok(data);
            }
            return BadRequest(new { message = result.Message });
        }
    }
}
