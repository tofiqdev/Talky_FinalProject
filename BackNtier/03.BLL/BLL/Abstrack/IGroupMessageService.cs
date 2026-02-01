using Core.Result.Abstrack;
using Entity.DataTransferObject.GroupDTO;
using Entity.DataTransferObject.GroupmessageDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstrack
{
    public interface IGroupMessageService
    {
        IDataResult<GroupmessageListDTO> Add(GroupmessageAddDTO groupMessageAddDTO);
        IResult Update(GroupmessageUpdateDTO groupMessageUpdateDTO);
        IResult Delete(int id);
        IDataResult<List<GroupmessageListDTO>> GetAll();
        IDataResult<GroupmessageListDTO> Get(int id);
    }
}
