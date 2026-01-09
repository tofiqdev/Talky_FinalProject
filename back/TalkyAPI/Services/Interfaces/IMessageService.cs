using TalkyAPI.DTOs.Message;

namespace TalkyAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetMessagesBetweenUsers(int userId1, int userId2);
        Task<MessageDto?> SendMessage(int senderId, SendMessageDto sendMessageDto);
        Task<bool> MarkAsRead(int messageId, int userId);
    }
}
