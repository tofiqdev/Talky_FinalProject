using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using BLL.Abstrack;
using Microsoft.Extensions.Logging;

namespace Talky_API.Hubs
{
    [Authorize]
    public class MovieRoomHub : Hub
    {
        private readonly IMovieRoomService _movieRoomService;
        private readonly ILogger<MovieRoomHub> _logger;

        public MovieRoomHub(IMovieRoomService movieRoomService, ILogger<MovieRoomHub> logger)
        {
            _movieRoomService = movieRoomService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier) ?? Context.User?.FindFirst("sub");
            return int.Parse(userIdClaim!.Value);
        }

        public async Task JoinMovieRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"MovieRoom_{roomId}");
            
            var userId = GetUserId();
            _logger.LogInformation($"User {userId} joined movie room {roomId}");
            
            // Notify others that someone joined
            await Clients.OthersInGroup($"MovieRoom_{roomId}").SendAsync("UserJoined", userId);
        }

        public async Task LeaveMovieRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"MovieRoom_{roomId}");
            
            var userId = GetUserId();
            _logger.LogInformation($"User {userId} left movie room {roomId}");
            
            // Notify others that someone left
            await Clients.OthersInGroup($"MovieRoom_{roomId}").SendAsync("UserLeft", userId);
        }

        // Only room owner can control playback
        public async Task SyncPlayback(int roomId, double currentTime, bool isPlaying)
        {
            var userId = GetUserId();
            _logger.LogInformation($"SyncPlayback called by user {userId} for room {roomId}: time={currentTime}, playing={isPlaying}");
            
            var room = _movieRoomService.Get(roomId);
            
            if (!room.IsSuccess || room.Data == null)
            {
                _logger.LogWarning($"Room {roomId} not found");
                return;
            }

            // Only owner can control playback
            if (room.Data.CreatedById != userId)
            {
                _logger.LogWarning($"User {userId} is not the owner of room {roomId}. Owner is {room.Data.CreatedById}");
                return;
            }

            _logger.LogInformation($"Broadcasting playback sync to room {roomId}");

            // Update room state
            _movieRoomService.UpdatePlaybackState(roomId, currentTime, isPlaying);

            // Broadcast to all participants in the room (including sender)
            await Clients.Group($"MovieRoom_{roomId}").SendAsync("PlaybackSync", new
            {
                currentTime,
                isPlaying,
                timestamp = DateTime.UtcNow
            });
            
            _logger.LogInformation($"Playback sync broadcasted successfully");
        }

        // Send message to room
        public async Task SendRoomMessage(int roomId, string content)
        {
            var userId = GetUserId();
            
            // Message will be saved via REST API, this is just for real-time notification
            await Clients.Group($"MovieRoom_{roomId}").SendAsync("ReceiveRoomMessage", new
            {
                senderId = userId,
                content,
                sentAt = DateTime.UtcNow
            });
        }
    }
}
