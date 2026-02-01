using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.DataTransferObject.MovieRoomDTO;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BLL.Concret
{
    public class MovieRoomManager : IMovieRoomService
    {
        private readonly IMovieRoomDAL _movieRoomDAL;
        private readonly IMovieRoomParticipantDAL _participantDAL;
        private readonly IUserDAL _userDAL;
        private readonly IMapper _mapper;

        public MovieRoomManager(
            IMovieRoomDAL movieRoomDAL,
            IMovieRoomParticipantDAL participantDAL,
            IUserDAL userDAL,
            IMapper mapper)
        {
            _movieRoomDAL = movieRoomDAL;
            _participantDAL = participantDAL;
            _userDAL = userDAL;
            _mapper = mapper;
        }

        public IResult Add(MovieRoomAddDTO dto)
        {
            var movieRoom = _mapper.Map<MovieRoom>(dto);
            
            // Extract YouTube video ID from URL
            movieRoom.YouTubeVideoId = ExtractYouTubeVideoId(dto.YouTubeUrl);
            if (string.IsNullOrEmpty(movieRoom.YouTubeVideoId))
                return new ErrorResult("Invalid YouTube URL");

            var validator = new MovieRoomValidator();
            var validationResult = validator.Validate(movieRoom);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            _movieRoomDAL.Add(movieRoom);
            return new SuccesResult("Movie room created successfully");
        }

        public IResult Update(MovieRoomUpdateDTO dto)
        {
            var existingRoom = _movieRoomDAL.Get(x => x.Id == dto.Id && x.Deleted == 0);
            if (existingRoom == null)
                return new ErrorResult("Movie room not found");

            existingRoom.Name = dto.Name;
            existingRoom.Description = dto.Description;
            existingRoom.YouTubeUrl = dto.YouTubeUrl;
            existingRoom.YouTubeVideoId = dto.YouTubeVideoId;
            existingRoom.IsActive = dto.IsActive;
            existingRoom.CurrentTime = dto.CurrentTime;
            existingRoom.IsPlaying = dto.IsPlaying;
            existingRoom.UpdatedAt = DateTime.UtcNow;

            _movieRoomDAL.Update(existingRoom);
            return new SuccesResult("Movie room updated successfully");
        }

        public IResult Delete(int id)
        {
            var room = _movieRoomDAL.Get(x => x.Id == id);
            if (room == null)
                return new ErrorResult("Movie room not found");

            room.Deleted = id;
            _movieRoomDAL.Update(room);
            return new SuccesResult("Movie room deleted successfully");
        }

        public IDataResult<List<MovieRoomListDTO>> GetAll()
        {
            var rooms = _movieRoomDAL.GetAll(x => x.Deleted == 0);
            var roomDTOs = rooms.Select(r => MapToListDTO(r)).ToList();
            return new SuccessDataResult<List<MovieRoomListDTO>>(roomDTOs);
        }

        public IDataResult<MovieRoomListDTO> Get(int id)
        {
            var room = _movieRoomDAL.Get(x => x.Id == id && x.Deleted == 0);
            if (room == null)
                return new ErrorDataResult<MovieRoomListDTO>("Movie room not found");

            return new SuccessDataResult<MovieRoomListDTO>(MapToListDTO(room));
        }

        public IDataResult<List<MovieRoomListDTO>> GetActiveRooms()
        {
            var rooms = _movieRoomDAL.GetAll(x => x.Deleted == 0 && x.IsActive);
            var roomDTOs = rooms.Select(r => MapToListDTO(r)).ToList();
            return new SuccessDataResult<List<MovieRoomListDTO>>(roomDTOs);
        }

        public IResult JoinRoom(int roomId, int userId)
        {
            var room = _movieRoomDAL.Get(x => x.Id == roomId && x.Deleted == 0);
            if (room == null)
                return new ErrorResult("Movie room not found");

            if (!room.IsActive)
                return new ErrorResult("Movie room is not active");

            var existingParticipant = _participantDAL.Get(x => 
                x.MovieRoomId == roomId && x.UserId == userId && x.Deleted == 0);

            if (existingParticipant != null)
                return new ErrorResult("Already joined this room");

            var participant = new MovieRoomParticipant
            {
                MovieRoomId = roomId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _participantDAL.Add(participant);
            return new SuccesResult("Joined room successfully");
        }

        public IResult LeaveRoom(int roomId, int userId)
        {
            var participant = _participantDAL.Get(x => 
                x.MovieRoomId == roomId && x.UserId == userId && x.Deleted == 0);

            if (participant == null)
                return new ErrorResult("Not a participant of this room");

            participant.Deleted = participant.Id;
            _participantDAL.Update(participant);
            return new SuccesResult("Left room successfully");
        }

        public IResult UpdatePlaybackState(int roomId, double currentTime, bool isPlaying)
        {
            var room = _movieRoomDAL.Get(x => x.Id == roomId && x.Deleted == 0);
            if (room == null)
                return new ErrorResult("Movie room not found");

            room.CurrentTime = currentTime;
            room.IsPlaying = isPlaying;
            room.UpdatedAt = DateTime.UtcNow;

            _movieRoomDAL.Update(room);
            return new SuccesResult("Playback state updated");
        }

        private MovieRoomListDTO MapToListDTO(MovieRoom room)
        {
            var creator = room.CreatedById > 0 ? _userDAL.Get(u => u.Id == room.CreatedById) : null;
            var participants = _participantDAL.GetAll(p => p.MovieRoomId == room.Id && p.Deleted == 0);
            
            return new MovieRoomListDTO
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                YouTubeUrl = room.YouTubeUrl,
                YouTubeVideoId = room.YouTubeVideoId,
                CreatedById = room.CreatedById,
                CreatedByUsername = creator?.Username ?? "",
                CreatedByAvatar = creator?.Avatar,
                IsActive = room.IsActive,
                CurrentTime = room.CurrentTime,
                IsPlaying = room.IsPlaying,
                ParticipantCount = participants.Count,
                CreatedAt = room.CreatedAt,
                Participants = participants.Select(p =>
                {
                    var user = _userDAL.Get(u => u.Id == p.UserId);
                    return new MovieRoomParticipantListDTO
                    {
                        Id = p.Id,
                        UserId = p.UserId,
                        Username = user?.Username ?? "",
                        Avatar = user?.Avatar,
                        IsOnline = user?.IsOnline ?? false,
                        JoinedAt = p.JoinedAt
                    };
                }).ToList()
            };
        }

        private string ExtractYouTubeVideoId(string url)
        {
            // Support various YouTube URL formats
            var patterns = new[]
            {
                @"(?:youtube\.com\/watch\?v=|youtu\.be\/)([a-zA-Z0-9_-]{11})",
                @"youtube\.com\/embed\/([a-zA-Z0-9_-]{11})",
                @"youtube\.com\/v\/([a-zA-Z0-9_-]{11})"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(url, pattern);
                if (match.Success)
                    return match.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}
