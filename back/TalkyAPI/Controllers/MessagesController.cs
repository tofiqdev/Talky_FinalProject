using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalkyAPI.DTOs.Message;
using TalkyAPI.Services.Interfaces;

namespace TalkyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMessages(int userId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out int currentUserId))
                return Unauthorized();

            var messages = await _messageService.GetMessagesBetweenUsers(currentUserId, userId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderIdClaim) || !int.TryParse(senderIdClaim, out int senderId))
                return Unauthorized();

            var message = await _messageService.SendMessage(senderId, sendMessageDto);

            if (message == null)
                return BadRequest(new { message = "Failed to send message" });

            return CreatedAtAction(nameof(GetMessages), new { userId = message.ReceiverId }, message);
        }

        [HttpPut("{messageId}/read")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var result = await _messageService.MarkAsRead(messageId, userId);

            if (!result)
                return NotFound(new { message = "Message not found or unauthorized" });

            return Ok(new { message = "Message marked as read" });
        }
    }
}
