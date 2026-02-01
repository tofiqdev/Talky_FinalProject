using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Talky_API.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<int, string> _userConnections = new();

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

            var message = new
            {
                id = 0, // Will be set by database
                senderId = senderId.Value,
                receiverId = receiverId,
                content = content,
                isRead = false,
                sentAt = DateTime.UtcNow,
                readAt = (DateTime?)null
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
