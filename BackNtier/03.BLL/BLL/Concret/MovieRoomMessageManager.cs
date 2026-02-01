using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.DataTransferObject.MovieRoomDTO;
using Entity.TableModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Concret
{
    public class MovieRoomMessageManager : IMovieRoomMessageService
    {
        private readonly IMovieRoomMessageDAL _messageDAL;
        private readonly IMovieRoomDAL _roomDAL;
        private readonly IMovieRoomParticipantDAL _participantDAL;
        private readonly IUserDAL _userDAL;
        private readonly IMapper _mapper;

        public MovieRoomMessageManager(
            IMovieRoomMessageDAL messageDAL,
            IMovieRoomDAL roomDAL,
            IMovieRoomParticipantDAL participantDAL,
            IUserDAL userDAL,
            IMapper mapper)
        {
            _messageDAL = messageDAL;
            _roomDAL = roomDAL;
            _participantDAL = participantDAL;
            _userDAL = userDAL;
            _mapper = mapper;
        }

        public IResult Add(MovieRoomMessageAddDTO dto, int senderId)
        {
            // Check if room exists
            var room = _roomDAL.Get(x => x.Id == dto.MovieRoomId && x.Deleted == 0);
            if (room == null)
                return new ErrorResult("Movie room not found");

            // Check if user is a participant
            var participant = _participantDAL.Get(x => 
                x.MovieRoomId == dto.MovieRoomId && x.UserId == senderId && x.Deleted == 0);
            if (participant == null)
                return new ErrorResult("You must join the room to send messages");

            var message = new MovieRoomMessage
            {
                MovieRoomId = dto.MovieRoomId,
                SenderId = senderId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            var validator = new MovieRoomMessageValidator();
            var validationResult = validator.Validate(message);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            _messageDAL.Add(message);
            return new SuccesResult("Message sent successfully");
        }

        public IDataResult<List<MovieRoomMessageListDTO>> GetRoomMessages(int roomId)
        {
            var messages = _messageDAL.GetAll(x => x.MovieRoomId == roomId && x.Deleted == 0)
                .OrderBy(m => m.SentAt)
                .ToList();

            var messageDTOs = messages.Select(m =>
            {
                var sender = _userDAL.Get(u => u.Id == m.SenderId && u.Deleted == 0);
                return new MovieRoomMessageListDTO
                {
                    Id = m.Id,
                    MovieRoomId = m.MovieRoomId,
                    SenderId = m.SenderId,
                    SenderUsername = sender?.Username ?? "Unknown",
                    SenderAvatar = sender?.Avatar,
                    Content = m.Content,
                    SentAt = m.SentAt
                };
            }).ToList();

            return new SuccessDataResult<List<MovieRoomMessageListDTO>>(messageDTOs);
        }
    }
}
