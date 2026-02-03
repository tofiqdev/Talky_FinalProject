using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using DAL.Concret;
using Entity.TableModel;
using Entity.DataTransferObject.MessageDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concret
{
    public class MessageManager : IMessageService
    {
        private readonly IMessageDal _messageDal;
        private readonly IMapper _mapper;
        public MessageManager(IMessageDal messageDal, IMapper mapper)
        {
            _messageDal = messageDal;
            _mapper = mapper;
        }
        public IResult Add(MessageAddDTO messageAddDTO)
        {
            try
            {
                var message = _mapper.Map<Message>(messageAddDTO);

                var validateValidator = new MessageValidator();
                var validationResult = validateValidator.Validate(message);

                if (!validationResult.IsValid)
                    return new ErrorResult(validationResult.Errors.FluentErrorString());

                // Set default values to avoid database conflicts
                message.Deleted = 0;
                message.SentAt = DateTime.UtcNow;
                message.IsRead = false;

                _messageDal.Add(message);
                return new SuccesResult("Added Successfully");
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Log the detailed error for debugging
                var innerException = ex.InnerException?.Message ?? ex.Message;
                
                // Return a user-friendly error message
                if (innerException.Contains("duplicate key") || innerException.Contains("unique index"))
                {
                    return new ErrorResult("Message could not be saved due to a database constraint. Please try again.");
                }
                
                return new ErrorResult($"Database error occurred: {innerException}");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"An error occurred while saving the message: {ex.Message}");
            }
        }

        public IResult Delete(int id)
        {
            var messageGet = _messageDal.Get(x => x.Id == id);

            if (messageGet is not null)
            {
                messageGet.Deleted = id;
                _messageDal.Update(messageGet);
                return new SuccesResult("Deleted Succesfully");
            }

            return new ErrorResult("Message Not found");


        }

        public IDataResult<MessageListDTO> Get(int id)
        {
            var messages = _messageDal.Get(x => x.Deleted == 0 && x.Id == id);
            return new SuccessDataResult<MessageListDTO>(_mapper.Map<MessageListDTO>(messages));
        }

        public IDataResult<List<MessageListDTO>> GetAll()
        {
            var messages = _messageDal.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<MessageListDTO>>(_mapper.Map<List<MessageListDTO>>(messages));
        }

        public IResult Update(MessageUpdateDTO messageUpdateDTO)
        {
            var messageMapper = _mapper.Map<Message>(messageUpdateDTO);

            var validateValidator = new MessageValidator();
            var validationResult = validateValidator.Validate(messageMapper);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());


            // Duplicate Check Removed

            _messageDal.Update(messageMapper);
            return new SuccesResult("Updated Succesfully");
        }



        // DuplicateMessageName method removed
    }
}