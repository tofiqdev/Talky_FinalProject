using AutoMapper;
using BLL.Abstrack;
using BLL.Extension;
using BLL.Validation;
using Core.Result.Abstrack;
using Core.Result.Concret;
using DAL.Abstrack;
using Entity.DataTransferObject.GroupMenmberDTO;
using Entity.TableModel;
using System.Collections.Generic;

namespace BLL.Concret
{
    public class GroupMemberManager : IGroupMemberService
    {
        private readonly IGroupmemberDAL _groupMemberDAL;
        private readonly IMapper _mapper;

        public GroupMemberManager(IGroupmemberDAL groupMemberDAL, IMapper mapper)
        {
            _groupMemberDAL = groupMemberDAL;
            _mapper = mapper;
        }

        public IResult Add(GroupMemberAddDTO dto)
        {
            var entity = _mapper.Map<GroupMember>(dto);
            _groupMemberDAL.Add(entity);
            return new SuccesResult("Group member added successfully");
        }

        public IResult Update(GroupMemberUpdateDTO dto)
        {
            var entity = _mapper.Map<GroupMember>(dto);
            _groupMemberDAL.Update(entity);
            return new SuccesResult("Group member updated successfully");
        }

        public IResult Delete(int id)
        {
            var entity = _groupMemberDAL.Get(x => x.Id == id);
            if (entity == null)
                return new ErrorResult("Group member not found");

            entity.Deleted = 1;
            _groupMemberDAL.Update(entity);
            return new SuccesResult("Group member deleted successfully");
        }

        public IDataResult<GroupMemberListDTO> Get(int id)
        {
            var entity = _groupMemberDAL.Get(x => x.Id == id && x.Deleted == 0);
            if (entity == null)
                return new ErrorDataResult<GroupMemberListDTO>("Group member not found");

            return new SuccessDataResult<GroupMemberListDTO>(_mapper.Map<GroupMemberListDTO>(entity));
        }

        public IDataResult<List<GroupMemberListDTO>> GetAll()
        {
            var entities = _groupMemberDAL.GetAll(x => x.Deleted == 0);
            return new SuccessDataResult<List<GroupMemberListDTO>>(_mapper.Map<List<GroupMemberListDTO>>(entities));
        }
    }
}
