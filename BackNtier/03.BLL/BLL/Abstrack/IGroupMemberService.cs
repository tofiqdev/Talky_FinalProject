using Core.Result.Abstrack;
using Entity.DataTransferObject.GroupMenmberDTO;
using Entity.DataTransferObject.GroupmessageDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IGroupMemberService
    {
        IResult Add(GroupMemberAddDTO groupMemberAddDTO);
        IResult Update(GroupMemberUpdateDTO groupMemberUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<GroupMemberListDTO>> GetAll();
        IDataResult<GroupMemberListDTO> Get(int id);
    }
}
