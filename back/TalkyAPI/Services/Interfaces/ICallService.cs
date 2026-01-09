using TalkyAPI.DTOs.Call;

namespace TalkyAPI.Services.Interfaces
{
    public interface ICallService
    {
        Task<List<CallDto>> GetUserCalls(int userId);
        Task<CallDto?> CreateCall(int callerId, int receiverId, string callType, string status, int? duration);
    }
}
