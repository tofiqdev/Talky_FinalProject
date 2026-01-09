using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TalkyAPI.Data;
using TalkyAPI.DTOs.Message;
using TalkyAPI.Models;

namespace TalkyAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<int, string> _userConnections = new();

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                // Store connection
                _userConnections[userId.Value] = Context.ConnectionId;

                // Update user status
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    user.IsOnline = true;
                    user.LastSeen = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Notify others
                await Clients.Others.SendAsync("UserOnline", userId.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                // Remove connection
                _userConnections.Remove(userId.Value);

                // Update user status
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    user.IsOnline = false;
                    user.LastSeen = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Notify others
                await Clients.Others.SendAsync("UserOffline", userId.Value);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int receiverId, string content)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue)
                return;

            // Get sender and receiver
            var sender = await _context.Users.FindAsync(senderId.Value);
            var receiver = await _context.Users.FindAsync(receiverId);

            if (sender == null || receiver == null)
                return;

            // Create message
            var message = new Message
            {
                SenderId = senderId.Value,
                ReceiverId = receiverId,
                Content = content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Create message DTO
            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                SenderUsername = sender.Username,
                ReceiverUsername = receiver.Username,
                Content = message.Content,
                IsRead = message.IsRead,
                SentAt = message.SentAt,
                ReadAt = message.ReadAt
            };

            // Send to sender (confirmation)
            await Clients.Caller.SendAsync("ReceiveMessage", messageDto);

            // Send to receiver if online
            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", messageDto);
            }
        }

        public async Task TypingIndicator(int receiverId, bool isTyping)
        {
            var senderId = GetUserId();
            if (!senderId.HasValue)
                return;

            // Send to receiver if online
            if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("TypingIndicator", senderId.Value, isTyping);
            }
        }

        public async Task MarkAsRead(int messageId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return;

            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || message.ReceiverId != userId.Value)
                return;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Notify sender if online
            if (_userConnections.TryGetValue(message.SenderId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendAsync("MessageRead", messageId);
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}
