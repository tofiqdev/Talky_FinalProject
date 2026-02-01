using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Business;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.TableModel;
using Entity.DataTransferObject.GroupmessageDTO;
using System;
using System.Collections.Generic;

namespace BLL.Concret
{
    public class GroupMessageManager : IGroupMessageService
    {
        private readonly IGroupMessageDAL _groupMessageDAL;
        private readonly IMapper _mapper;

        public GroupMessageManager(IGroupMessageDAL groupMessageDAL, IMapper mapper)
        {
            _groupMessageDAL = groupMessageDAL;
            _mapper = mapper;
        }

        public IDataResult<GroupmessageListDTO> Add(GroupmessageAddDTO groupMessageAddDTO)
        {
            var groupMessage = _mapper.Map<GroupMessage>(groupMessageAddDTO);
            groupMessage.SentAt = DateTime.UtcNow;

            var validator = new GroupMessageValidator();
            var validationResult = validator.Validate(groupMessage);

            if (!validationResult.IsValid)
                return new ErrorDataResult<GroupmessageListDTO>(validationResult.Errors.FluentErrorString());

            _groupMessageDAL.Add(groupMessage);
            
            // Return the created message
            var dto = _mapper.Map<GroupmessageListDTO>(groupMessage);
            return new SuccessDataResult<GroupmessageListDTO>(dto, "Group message added successfully");
        }

        public IResult Update(GroupmessageUpdateDTO groupMessageUpdateDTO)
        {
            var groupMessage = _mapper.Map<GroupMessage>(groupMessageUpdateDTO);

            var validator = new GroupMessageValidator();
            var validationResult = validator.Validate(groupMessage);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            _groupMessageDAL.Update(groupMessage);
            return new SuccesResult("Group message updated successfully");
        }

        public IResult Delete(int id)
        {
            var groupMessage = _groupMessageDAL.Get(x => x.Id == id);

            if (groupMessage is not null)
            {
                groupMessage.Deleted = 1; // soft delete
                _groupMessageDAL.Update(groupMessage);
                return new SuccesResult("Group message deleted successfully");
            }

            return new ErrorResult("Group message not found");
        }

        public IDataResult<GroupmessageListDTO> Get(int id)
        {
            var groupMessage = _groupMessageDAL.Get(x => x.Id == id && x.Deleted == 0);
            if (groupMessage is null)
                return new ErrorDataResult<GroupmessageListDTO>("Group message not found");

            return new SuccessDataResult<GroupmessageListDTO>(_mapper.Map<GroupmessageListDTO>(groupMessage));
        }

        public IDataResult<List<GroupmessageListDTO>> GetAll()
        {
            var groupMessages = _groupMessageDAL.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<GroupmessageListDTO>>(_mapper.Map<List<GroupmessageListDTO>>(groupMessages));
        }
    }
}
