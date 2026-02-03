using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using BLL.Abstrack;
using Entity.DataTransferObject.MessageDTO;

namespace Talky_API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<int, string> _userConnections = new();
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public ChatHub(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _userConnections[userId.Value] = Context.ConnectionId;
                
                // Notify all clients that user is online
                await Clients.All.SendAsync("UserOnline", userId.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _userConnections.Remove(userId.Value);
                
                // Notify all clients that user is offline
                await Clients.All.SendAsync("UserOffline", userId.Value);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int receiverId, string content)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue)
            {
                throw new HubException("Unauthorized");
            }

            // Save message to database
            var messageAddDto = new MessageAddDTO
            {
                SenderId = senderId.Value,
                ReceiverId = receiverId,
                Content = content
            };

            var result = _messageService.Add(messageAddDto);
            if (!result.IsSuccess)
            {
                throw new HubException($"Failed to save message: {result.Message}");
            }

            // Get the saved message with ID and user details
            var messages = _messageService.GetAll();
            var savedMessage = messages.Data?
                .Where(m => m.SenderId == senderId.Value && m.ReceiverId == receiverId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefault();

            if (savedMessage == null)
            {
                throw new HubException("Failed to retrieve saved message");
            }

            // Get user details for sender and receiver names
            var senderResult = _userService.Get(senderId.Value);
            var receiverResult = _userService.Get(receiverId);
            
            var senderName = senderResult.IsSuccess ? senderResult.Data.Username : "Unknown";
            var receiverName = receiverResult.IsSuccess ? receiverResult.Data.Username : "Unknown";

            var message = new
            {
                id = savedMessage.Id,
                senderId = savedMessage.SenderId,
                receiverId = savedMessage.ReceiverId,
                senderUsername = senderName,
                receiverUsername = receiverName,
                content = savedMessage.Content,
                isRead = savedMessage.IsRead,
                sentAt = savedMessage.SentAt,
                readAt = savedMessage.ReadAt
            };

            // Send to receiver if online
            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
            }

            // Send back to sender (for confirmation)
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        public async Task SendGroupMessage(int groupId, string content)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue)
            {
                throw new HubException("Unauthorized");
            }

            var message = new
            {
                id = 0,
                groupId = groupId,
                senderId = senderId.Value,
                content = content,
                isSystemMessage = false,
                sentAt = DateTime.UtcNow
            };

            // Send to all group members (implement group logic as needed)
            await Clients.Group($"Group_{groupId}").SendAsync("ReceiveGroupMessage", message);
        }

        public async Task JoinGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{groupId}");
        }

        public async Task LeaveGroup(int groupId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Group_{groupId}");
        }

        public async Task TypingIndicator(int receiverId, bool isTyping)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue) return;

            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("TypingIndicator", senderId.Value, isTyping);
            }
        }

        public async Task MarkAsRead(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return;

            // Update message in database
            var messageResult = _messageService.Get(messageId);
            if (messageResult.IsSuccess)
            {
                var message = messageResult.Data;
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
                
                _messageService.Update(updateDto);
            }

            await Clients.All.SendAsync("MessageRead", messageId);
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? Context.User?.FindFirst("sub");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
