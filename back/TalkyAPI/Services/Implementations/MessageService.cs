using Microsoft.EntityFrameworkCore;
using TalkyAPI.Data;
using TalkyAPI.DTOs.Message;
using TalkyAPI.Models;
using TalkyAPI.Services.Interfaces;

namespace TalkyAPI.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;

        public MessageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MessageDto>> GetMessagesBetweenUsers(int userId1, int userId2)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    SenderUsername = m.Sender.Username,
                    ReceiverUsername = m.Receiver.Username,
                    Content = m.Content,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt,
                    ReadAt = m.ReadAt
                })
                .ToListAsync();

            return messages;
        }

        public async Task<MessageDto?> SendMessage(int senderId, SendMessageDto sendMessageDto)
        {
            // Verify sender exists
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
                return null;

            // Verify receiver exists
            var receiver = await _context.Users.FindAsync(sendMessageDto.ReceiverId);
            if (receiver == null)
                return null;

            // Create message
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = sendMessageDto.ReceiverId,
                Content = sendMessageDto.Content,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Return message DTO
            return new MessageDto
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
        }

        public async Task<bool> MarkAsRead(int messageId, int userId)
        {
            var message = await _context.Messages.FindAsync(messageId);

            if (message == null || message.ReceiverId != userId)
                return false;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
