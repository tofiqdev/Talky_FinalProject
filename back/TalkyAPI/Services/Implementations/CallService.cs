using Microsoft.EntityFrameworkCore;
using TalkyAPI.Data;
using TalkyAPI.DTOs.Call;
using TalkyAPI.Models;
using TalkyAPI.Services.Interfaces;

namespace TalkyAPI.Services.Implementations
{
    public class CallService : ICallService
    {
        private readonly AppDbContext _context;

        public CallService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CallDto>> GetUserCalls(int userId)
        {
            var calls = await _context.Calls
                .Include(c => c.Caller)
                .Include(c => c.Receiver)
                .Where(c => c.CallerId == userId || c.ReceiverId == userId)
                .OrderByDescending(c => c.StartedAt)
                .Select(c => new CallDto
                {
                    Id = c.Id,
                    CallerId = c.CallerId,
                    ReceiverId = c.ReceiverId,
                    CallerUsername = c.Caller.Username,
                    ReceiverUsername = c.Receiver.Username,
                    CallType = c.CallType,
                    Status = c.Status,
                    StartedAt = c.StartedAt,
                    EndedAt = c.EndedAt,
                    Duration = c.Duration
                })
                .ToListAsync();

            return calls;
        }

        public async Task<CallDto?> CreateCall(int callerId, int receiverId, string callType, string status, int? duration)
        {
            // Verify caller and receiver exist
            var caller = await _context.Users.FindAsync(callerId);
            var receiver = await _context.Users.FindAsync(receiverId);

            if (caller == null || receiver == null)
                return null;

            // Create call
            var call = new Call
            {
                CallerId = callerId,
                ReceiverId = receiverId,
                CallType = callType,
                Status = status,
                StartedAt = DateTime.UtcNow,
                EndedAt = duration.HasValue ? DateTime.UtcNow.AddSeconds(duration.Value) : null,
                Duration = duration
            };

            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            return new CallDto
            {
                Id = call.Id,
                CallerId = call.CallerId,
                ReceiverId = call.ReceiverId,
                CallerUsername = caller.Username,
                ReceiverUsername = receiver.Username,
                CallType = call.CallType,
                Status = call.Status,
                StartedAt = call.StartedAt,
                EndedAt = call.EndedAt,
                Duration = call.Duration
            };
        }
    }
}
