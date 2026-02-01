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
            var message = _mapper.Map<Message>(messageAddDTO);

            var validateValidator = new MessageValidator();
            var validationResult = validateValidator.Validate(message);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            // Duplicate Check Removed

            _messageDal.Add(message);
            return new SuccesResult("Added Successfuly");
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