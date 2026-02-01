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
using Entity.DataTransferObject.GroupDTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Group = Entity.TableModel.Group;

namespace BLL.Concret
{
    public class GroupManager : IGroupService
    {
        private readonly IGroupDAL _groupDAL;
        private readonly IMapper _mapper;

        public GroupManager(IGroupDAL groupDAL, IMapper mapper)
        {
            _groupDAL = groupDAL;
            _mapper = mapper;
        }
        public IResult Add(Group group)
        {
            _groupDAL.Add(group);
            return new SuccesResult("Added Successfuly");
        }

        public IResult Delete(int id)
        {
            var gropGet = _groupDAL.Get(x => x.Id == id);

            if (gropGet is not null)
            {
                gropGet.Deleted = id;
                _groupDAL.Update(gropGet);
                return new SuccesResult("Deleted Succesfully");
            }

            return new ErrorResult("Group Not found");


        }

        public IDataResult<GroupListDTO> Get(int id)
        {
            var groups = _groupDAL.Get(x => x.Deleted == 0 && x.Id == id);
            return new SuccessDataResult<GroupListDTO>(_mapper.Map<GroupListDTO>(groups));
        }

        public IDataResult<List<GroupListDTO>> GetAll()
        {
            var groups = _groupDAL.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<GroupListDTO>>(_mapper.Map<List<GroupListDTO>>(groups));
        }

        public IResult Update(GroupUpdateDTO groupUpdateDTO)
        {
            var groupMapper = _mapper.Map<Group>(groupUpdateDTO);

            var validateValidator = new GroupValidator();
            var validationResult = validateValidator.Validate(groupMapper);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            var checkData = BusinessRules.Check(DuplicateUserName(groupMapper));

            if (!checkData.IsSuccess)
                return checkData;

            _groupDAL.Update(groupMapper);
            return new SuccesResult("Updated Succesfully");
        }



        public IResult DuplicateUserName(Group group)
        {
            var groupName = _groupDAL.Get(x => x.Name == group.Name && x.Deleted == 0);

            if (groupName is not null)
            {
                return new ErrorResult("Duplicate Name");
            }

            return new SuccesResult();
        }

        public IResult Add(GroupAddDTO groupAddDTO)
        {
            var group = _mapper.Map<Group>(groupAddDTO);

            var validateValidator = new GroupValidator();
            var validationResult = validateValidator.Validate(group);

            if (!validationResult.IsValid)
                return new ErrorResult(validationResult.Errors.FluentErrorString());

            var checkData = BusinessRules.Check(DuplicateUserName(group));

            if (!checkData.IsSuccess)
                return checkData;

            _groupDAL.Add(group);
            return new SuccesResult("Added Successfuly");
        }
    }
}
